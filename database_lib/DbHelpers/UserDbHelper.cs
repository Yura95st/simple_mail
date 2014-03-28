using data_models.Exceptions;
using data_models.Models;
using data_models.Validations;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace database_lib.DbHelpers
{
    public class UserDbHelper : BaseDbHelper
    {
        private string selectUsersCommand = @"
            SELECT u.user_id, u.first_name, u.email, u.password, u.state
            FROM users u ";

        public UserDbHelper()
        {
        }

        // gets all users
        public List<User> GetAllUsers()
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = selectUsersCommand;

            return ExecuteSelectUsersCommand(cmd);
        }

        // gets user by it's id number
        public User GetUserById(int userId)
        {
            if (userId <= 0)
            {
                throw new ArgumentOutOfRangeException("userId");
            }

            List<int> usersIdList = new List<int>();
            usersIdList.Add(userId);

            User user = null;

            try
            {
                user = GetUsersById(usersIdList)[0];
            }
            catch
            {
                return null;
            }

            return user;
        }

        // gets users with id numbers from "userIdsList" list
        public List<User> GetUsersById(List<int> usersIdList)
        {
            if (usersIdList == null || usersIdList.Count == 0) {
                return null;
            }

            //convert User id list to string
            string idString = String.Join(", ", usersIdList.FindAll(id => id > 0));

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = selectUsersCommand +
                @" WHERE u.user_id IN (" + idString + ")";

            return ExecuteSelectUsersCommand(cmd);
        }

        // gets user by email
        public User GetUserByEmail(string email)
        {
            if (!MyValidation.IsValidEmail(email))
            {
                throw new InvalidEmailException("email");
            }

            User user = null;

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = selectUsersCommand +
                @" WHERE u.email = @email";

            cmd.Parameters.Add(new SqlParameter
            {
                ParameterName = "@email",
                Value = email,
                SqlDbType = SqlDbType.VarChar
            });

            try
            {
                user = ExecuteSelectUsersCommand(cmd)[0];
            }
            catch
            {
                return null;
            }

            return user;
        }   

        // gets users by searching parameters
        public List<User> SearchForUsers(string title, int type = 0, int publisherId = 0, int authorId = 0)
        {
            throw new NotImplementedException();
        }

        // adds new user to the library
        // returns id of the newly created user or 0 - if it wasn't created
        public int AddNewUser(User user)
        {
            try
            {
                MyValidation.CheckValidUserFields(user);
            }
            catch
            {
                throw;
            }

            if (this.GetUserByEmail(user.Email) != null)
            {
                throw new UserAlreadyExistsException(user.Email);
            }

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = @"
                    INSERT INTO users(first_name, email, password, state) 
                    VALUES(@first_name, @email, @password, @state);
                    SELECT SCOPE_IDENTITY();";

            cmd.Parameters.Add(new SqlParameter
            {
                ParameterName = "@first_name",
                Value = user.FirstName,
                SqlDbType = SqlDbType.VarChar
            });

            cmd.Parameters.Add(new SqlParameter
            {
                ParameterName = "@email",
                Value = user.Email,
                SqlDbType = SqlDbType.VarChar
            });

            cmd.Parameters.Add(new SqlParameter
            {
                ParameterName = "@password",
                Value = MyValidation.Hash(user.Password, user.Email),
                SqlDbType = SqlDbType.VarChar
            });

            cmd.Parameters.Add(new SqlParameter
            {
                ParameterName = "@state",
                Value = user.State,
                SqlDbType = SqlDbType.Int
            });

            int userId;

            try
            {
                userId = Convert.ToInt32(ExecuteScalarCommand(cmd).ToString());
            }
            catch
            {
                return 0;
            }

            return userId;
        }

        // deletes user from the library
        public void DeleteUser(int userId)
        {
            throw new NotImplementedException();
        }

        // modifies user's info
        public void ModifyUserInfo(User user)
        {
            throw new NotImplementedException();
        }

        public int LogInUser(User user)
        {
            User existingUser = null;

            try
            {
                existingUser = this.GetUserByEmail(user.Email);
            }
            catch
            {
                throw;
            }

            if (existingUser == null) 
            {
                throw new UserDoesNotExistException();
            }

            if (!MyValidation.AreTwoPasswordsEqual(existingUser.Password, MyValidation.Hash(user.Password, existingUser.Email)))
            {
                throw new InvalidPasswordException(user.Password);
            }

            try
            {
                SetUserOnline(existingUser.Id);
            }
            catch
            {
                throw;
            }

            return existingUser.Id;
        }

        public void LogOutUser(int userId)
        {
            try
            {
                SetUserOffline(userId);
            }
            catch
            {
                throw;
            }
        }

        // returns user object from the query result - dataReader
        public static User GetUserFromQueryResult(DbDataReader dataReader)
        {
            User user = new User();

            try
            {
                user.Id = Convert.ToInt32(dataReader["user_id"]);
                user.FirstName = (string)dataReader["first_name"];
                user.Email = (string)dataReader["email"];
                user.Password = (string)dataReader["password"];
                user.State = Convert.ToInt32(dataReader["state"]);
            }
            catch
            {
                return null;
            }

            return user;
        }

        public void SetUserOnline(int userId)
        {
            SetUserState(userId, 1);
        }

        public void SetUserOffline(int userId)
        {
            SetUserState(userId, 0);
        }

        // executes sql command to get users
        private List<User> ExecuteSelectUsersCommand(SqlCommand command)
        {
            List<User> users = new List<User>();

            try
            {
                connection.Open();

                //bing opened connection to the command
                command.Connection = connection;

                using (DbDataReader dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        users.Add(GetUserFromQueryResult(dataReader));
                    }
                }
            }
            catch (SqlException ex)
            { }
            finally
            {
                connection.Close();
            }

            return users;
        }

        // changes user state: 0 - offline, 1 - online
        private void SetUserState(int userId, int state)
        {
            if (userId <= 0)
            {
                throw new ArgumentOutOfRangeException("userId");
            }

            if (state < 0)
            {
                throw new ArgumentOutOfRangeException("state");
            }

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = @"
                    UPDATE users SET state = @state
                    WHERE user_id = @user_id";

            cmd.Parameters.Add(new SqlParameter
            {
                ParameterName = "@user_id",
                Value = userId,
                SqlDbType = SqlDbType.Int
            });

            cmd.Parameters.Add(new SqlParameter
            {
                ParameterName = "@state",
                Value = state,
                SqlDbType = SqlDbType.Int
            });

            ExecuteNonQueryCommand(cmd);
        }
    }
}
