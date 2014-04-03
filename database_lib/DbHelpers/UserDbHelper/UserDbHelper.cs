using data_models.Exceptions;
using data_models.Models;
using data_models.Validations;
using database_lib.Validation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace database_lib.DbHelpers
{
    public class UserDbHelper : BaseUserDbHelper
    {
        private static UserDbHelper _instance;

        //SELECT u.user_id, u.first_name, u.email, u.password, u.state
        //FROM users u
        private string selectUsersCommand = String.Format(@"
            SELECT {0}.{1} AS {2}, {0}.{3} AS {4}, {0}.{5} AS {6}, {0}.{7} AS {8}, {0}.{9} AS {10}
            FROM {11} {0}",            
            DbValues.TABLE_CAPTION_USERS, 
            DbValues.USERS_COLUMN_ID, DbValues.USERS_CAPTION_COLUMN_ID,
            DbValues.USERS_COLUMN_FIRST_NAME, DbValues.USERS_CAPTION_COLUMN_FIRST_NAME,
            DbValues.USERS_COLUMN_EMAIL, DbValues.USERS_CAPTION_COLUMN_EMAIL,
            DbValues.USERS_COLUMN_PASSWORD, DbValues.USERS_CAPTION_COLUMN_PASSWORD,
            DbValues.USERS_COLUMN_STATE, DbValues.USERS_CAPTION_COLUMN_STATE,
            DbValues.TABLE_USERS
        );        

        private UserDbHelper() { }

        //singleton
        public static UserDbHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new UserDbHelper();
                }
                return _instance;
            }
        }

        public override List<User> GetAllUsers()
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = selectUsersCommand;

            return ExecuteSelectUsersCommand(cmd);
        }

        public override List<User> GetUsersById(List<int> usersIdList)
        {
            if (usersIdList == null || usersIdList.Count == 0) {
                return null;
            }

            //convert User id list to string
            string idString = String.Join(", ", usersIdList.FindAll(id => id > 0));

            SqlCommand cmd = new SqlCommand();

            //WHERE u.user_id IN (" + idString + ");
            cmd.CommandText = String.Format(@"
                {0} WHERE {1}.{2} IN ({3})",
                selectUsersCommand, DbValues.TABLE_CAPTION_USERS, 
                DbValues.USERS_COLUMN_ID, idString
            );

            return ExecuteSelectUsersCommand(cmd);
        }

        public override User GetUserByEmail(string email)
        {
            if (!MyValidation.IsValidEmail(email))
            {
                throw new InvalidEmailException("email");
            }

            User user = null;

            SqlCommand cmd = new SqlCommand();

            //WHERE u.email = @email
            cmd.CommandText = String.Format(@"
                {0} WHERE {1}.{2} = @email",
                selectUsersCommand, DbValues.TABLE_CAPTION_USERS,
                DbValues.USERS_COLUMN_EMAIL
            );            

            cmd.Parameters.Add(new SqlParameter
            {
                ParameterName = "@email",
                Value = email.ToLower(),
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

        public override List<User> SearchForUsers()
        {
            throw new NotImplementedException();
        }

        public override int AddNewUser(User user)
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

            // INSERT INTO users(first_name, email, password, state) 
            // VALUES(@first_name, @email, @password, @state);
            cmd.CommandText = String.Format(@"
                INSERT INTO {0}({1}, {2}, {3}, {4}) 
                VALUES(@first_name, @email, @password, @state); SELECT SCOPE_IDENTITY();",
                DbValues.TABLE_USERS,
                DbValues.USERS_COLUMN_FIRST_NAME, DbValues.USERS_COLUMN_EMAIL,
                DbValues.USERS_COLUMN_PASSWORD, DbValues.USERS_COLUMN_STATE 
            );

            cmd.Parameters.Add(new SqlParameter
            {
                ParameterName = "@first_name",
                Value = user.FirstName,
                SqlDbType = SqlDbType.VarChar
            });

            cmd.Parameters.Add(new SqlParameter
            {
                ParameterName = "@email",
                Value = user.Email.ToLower(),
                SqlDbType = SqlDbType.VarChar
            });

            cmd.Parameters.Add(new SqlParameter
            {
                ParameterName = "@password",
                Value = MyValidation.Hash(user.Password, user.Email.ToLower()),
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

        public override void DeleteUser(int userId)
        {
            throw new NotImplementedException();
        }

        public override void ModifyUserInfo(User user)
        {
            throw new NotImplementedException();
        }

        public override void ChangePassword(User user)
        {
            if (!MyValidation.IsValidPassword(user.Password))
            {
                throw new InvalidPasswordException();
            }

            SqlCommand cmd = new SqlCommand();

            // UPDATE users SET password = @password
            // WHERE user_id = @user_id
            cmd.CommandText = String.Format(@"
                UPDATE {0} SET {1} = @password
                WHERE {2} = @user_id",
                DbValues.TABLE_USERS,
                DbValues.USERS_COLUMN_PASSWORD, DbValues.USERS_COLUMN_ID
            );

            cmd.Parameters.Add(new SqlParameter
            {
                ParameterName = "@user_id",
                Value = user.Id,
                SqlDbType = SqlDbType.VarChar
            });

            cmd.Parameters.Add(new SqlParameter
            {
                ParameterName = "@password",
                Value = MyValidation.Hash(user.Password, user.Email.ToLower()),
                SqlDbType = SqlDbType.VarChar
            });

            ExecuteNonQueryCommand(cmd);
        }

        // changes user state: 0 - offline, 1 - online
        protected override void SetUserState(int userId, int state)
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

            // UPDATE users SET state = @state
            // WHERE user_id = @user_id";
            cmd.CommandText = String.Format(@"
                UPDATE {0} SET {1} = @state
                WHERE {2} = @user_id",
                DbValues.TABLE_USERS, DbValues.USERS_COLUMN_STATE,
                DbValues.USERS_COLUMN_ID
            );

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

        // returns user object from the query result - dataReader
        public static User GetUserFromQueryResult(DbDataReader dataReader)
        {
            User user = null;

            if (dataReader != null)
            {
                user = new User();

                if (DbValidation.ColumnExists(dataReader, DbValues.USERS_CAPTION_COLUMN_ID))
                {
                    user.Id = Convert.ToInt32(dataReader[DbValues.USERS_CAPTION_COLUMN_ID]);
                }

                if (DbValidation.ColumnExists(dataReader, DbValues.USERS_CAPTION_COLUMN_FIRST_NAME))
                {
                    user.FirstName = dataReader[DbValues.USERS_CAPTION_COLUMN_FIRST_NAME].ToString();
                }

                if (DbValidation.ColumnExists(dataReader, DbValues.USERS_CAPTION_COLUMN_EMAIL))
                {
                    user.Email = dataReader[DbValues.USERS_CAPTION_COLUMN_EMAIL].ToString();
                }

                if (DbValidation.ColumnExists(dataReader, DbValues.USERS_CAPTION_COLUMN_PASSWORD))
                {
                    user.Password = dataReader[DbValues.USERS_CAPTION_COLUMN_PASSWORD].ToString();
                }

                if (DbValidation.ColumnExists(dataReader, DbValues.USERS_CAPTION_COLUMN_STATE))
                {
                    user.State = Convert.ToInt32(dataReader[DbValues.USERS_CAPTION_COLUMN_STATE]);
                }
            }

            return user;
        }
        
        // executes sql command to get users
        private List<User> ExecuteSelectUsersCommand(SqlCommand command)
        {
            List<User> users = new List<User>();

            try
            {
                connection.Open();

                //bind opened connection to the command
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
    }
}
