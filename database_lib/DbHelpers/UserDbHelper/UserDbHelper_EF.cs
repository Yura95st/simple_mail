using data_models.Exceptions;
using data_models.Models;
using data_models.Validations;
using database_lib.EntityDbDataModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace database_lib.DbHelpers
{
    public class UserDbHelper_EF : BaseUserDbHelper
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

        public override List<User> GetAllUsers()
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

        public override List<User> GetUsersById(List<int> usersIdList)
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

        public override User GetUserByEmail(string email)
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

            int userId;

            using (var db = new simple_mailEntities())
            {
                user userEntity = new user();

                userEntity.email = user.Email;
                userEntity.first_name = user.FirstName;
                userEntity.password = MyValidation.Hash(user.Password, user.Email.ToLower());
                userEntity.state = user.State;

                try
                {
                    db.users.Add(userEntity);
                    db.SaveChanges();

                    userId = userEntity.user_id;
                }
                catch (InvalidOperationException)
                {
                    return 0;
                }
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

        // returns user object from data
        public static User GetUserFromDataEntity(user data)
        {
            User userObject = null;

            if (data != null)
            {
                userObject = new User();

                userObject.Id = data.user_id;
                userObject.FirstName = data.first_name;
                userObject.Email = data.email;
                userObject.Password = data.password;
                userObject.State = data.state;
            }

            return userObject;
        }
    }
}
