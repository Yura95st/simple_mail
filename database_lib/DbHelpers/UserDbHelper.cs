using data_models.Models;
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
                //TODO: throw InvalidArgument exception
            }

            List<int> usersIdList = new List<int>();
            usersIdList.Add(userId);

            User user = null;

            try
            {
                user = GetUsersById(usersIdList)[0];
            }
            catch(Exception e)
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
            User user = null;

            //TODO: email validation

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
            catch (Exception e)
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
        public int AddNewUser(User user)
        {
            if (!CheckValidUserFields(user))
            {
                //TODO: throw UserInvalidFields exeption;
                return -1;
            }

            int userId = -1;

            if (this.GetUserByEmail(user.Email) != null)
            {
                //TODO: throw UserAlreadyExists exeption;
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
                Value = user.Password,
                SqlDbType = SqlDbType.VarChar
            });

            cmd.Parameters.Add(new SqlParameter
            {
                ParameterName = "@state",
                Value = user.State,
                SqlDbType = SqlDbType.Int
            });

            userId = Convert.ToInt32(ExecuteScalarCommand(cmd).ToString());

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

        // returns user object from the query result - dataReader
        public static User GetUserFromQueryResult(DbDataReader dataReader)
        {
            User user = new User();

            user.Id = Convert.ToInt32(dataReader["user_id"]);
            user.FirstName = (string) dataReader["first_name"];
            user.Email = (string) dataReader["email"];
            user.Password = (string) dataReader["password"];
            user.State = Convert.ToInt32(dataReader["state"]);

            return user;
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

        private bool CheckValidUserFields(User user)
        {
            //TODO: create UserValidation class
            if (user == null) {
                return false;
            }

            if (string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.Password) || string.IsNullOrEmpty(user.FirstName)) {
                return false;
            }

            return true;
        }
    }
}
