using data_models.Exceptions;
using data_models.Models;
using data_models.Validations;
using database_lib.EntityDbDataModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace database_lib.DbHelpers
{
    public class UserDbHelper_EF : IUserDbHelper
    {
        private static UserDbHelper_EF _instance;

        private UserDbHelper_EF() { }

        // singleton
        public static UserDbHelper_EF Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new UserDbHelper_EF();
                }
                return _instance;
            }
        }

        public List<User> GetAllUsers()
        {
            List<User> listUser = new List<User>();

            using (var db = new simple_mailEntities())
            {
                try
                {
                    var users = (from u in db.users
                                 select u).ToList();

                    foreach (var user in users)
                    {
                        listUser.Add(GetUserFromDataEntity(user));
                    }
                }
                catch (ArgumentNullException e)
                {
                    return listUser;
                }
            }

            return listUser;
        }

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

        public List<User> GetUsersById(List<int> usersIdList)
        {
            if (usersIdList == null || usersIdList.Count == 0)
            {
                return null;
            }

            List<User> userList = new List<User>();

            using (var db = new simple_mailEntities())
            {
                try
                {
                    var userEntityList = (from u in db.users
                                          where usersIdList.Contains(u.user_id)
                                          select u).ToList();

                    foreach (var userEntity in userEntityList)
                    {
                        userList.Add(GetUserFromDataEntity(userEntity));
                    }
                }
                catch (ArgumentNullException)
                {
                    return null;
                }
            }

            return userList;
        }

        public User GetUserByEmail(string email)
        {
            if (!MyValidation.IsValidEmail(email))
            {
                throw new InvalidEmailException("email");
            }

            User user = null;

            using (var db = new simple_mailEntities())
            {
                try
                {
                    var userEntity = (from u in db.users
                                      where u.email == email
                                      select u).SingleOrDefault();

                    user = GetUserFromDataEntity(userEntity);
                }
                catch (ArgumentNullException)
                {
                    return null;
                }
                catch (InvalidOperationException)
                {
                    return null;
                }
            }

            return user;
        }

        public List<User> SearchForUsers()
        {
            throw new System.NotImplementedException();
        }

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

            int userId;

            using (var db = new simple_mailEntities())
            {
                user userEntity = new user();

                userEntity.email = user.Email;
                userEntity.first_name = user.FirstName;
                userEntity.password = MyValidation.Hash(user.Password, user.Email.ToLower());
                userEntity.state = user.State;

                db.users.Add(userEntity);

                try
                {
                    userId = db.SaveChanges();
                }
                catch
                {
                    return 0;
                }
            }

            return userId;
        }

        public void DeleteUser(int userId)
        {
            throw new System.NotImplementedException();
        }

        public void ModifyUserInfo(User user)
        {
            throw new System.NotImplementedException();
        }

        public void ChangePassword(User user)
        {
            if (!MyValidation.IsValidPassword(user.Password))
            {
                throw new InvalidPasswordException();
            }

            using (var db = new simple_mailEntities())
            {
                try
                {
                    var userEntity = (from u in db.users
                                      where u.user_id == user.Id
                                      select u).SingleOrDefault();

                    if (userEntity == null)
                    {
                        return;
                    }

                    userEntity.password = MyValidation.Hash(user.Password, user.Email.ToLower());

                    db.SaveChanges();
                }
                catch (ArgumentNullException)
                {
                    return;
                }
                catch (InvalidOperationException)
                {
                    return;
                }
            }
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

        public void SetUserOnline(int userId)
        {
            SetUserState(userId, (int)User.States.Online);
        }

        public void SetUserOffline(int userId)
        {
            SetUserState(userId, (int)User.States.Offline);
        }

        // returns user object from data
        public static User GetUserFromDataEntity(user data)
        {            
            if (data == null)
            {
                return null;
            }

            User userObject = new User();

            if (data.user_id != null)
            {
                userObject.Id = data.user_id;
            }

            if (data.first_name != null)
            {
                userObject.FirstName = data.first_name;
            }

            if (data.email != null)
            {
                userObject.Email = data.email;
            }

            if (data.password != null)
            {
                userObject.Password = data.password;
            }

            if (data.state != null)
            {
                userObject.State = data.state;
            }

            return userObject;
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

            using (var db = new simple_mailEntities())
            {
                try
                {
                    var userEntity = (from u in db.users
                                      where u.user_id == userId
                                      select u).SingleOrDefault();

                    if (userEntity == null)
                    {
                        return;
                    }

                    userEntity.state = state;

                    db.SaveChanges();
                }
                catch (ArgumentNullException)
                {
                    return;
                }
                catch (InvalidOperationException)
                {
                    return;
                }
            }
        }
    }
}
