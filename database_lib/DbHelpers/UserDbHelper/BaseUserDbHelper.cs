using data_models.Exceptions;
using data_models.Models;
using data_models.Validations;
using System;
using System.Collections.Generic;

namespace database_lib.DbHelpers
{
    public abstract class BaseUserDbHelper : BaseDbHelper, IUserDbHelper
    {
        public abstract List<User> GetAllUsers();

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

        public abstract List<User> GetUsersById(List<int> usersIdList);

        public abstract User GetUserByEmail(string email);

        public abstract List<User> SearchForUsers();

        public abstract int AddNewUser(User user);

        public abstract void DeleteUser(int userId);

        public abstract void ModifyUserInfo(User user);

        public abstract void ChangePassword(User user);

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

        // changes user state: 0 - offline, 1 - online
        protected abstract void SetUserState(int userId, int state);
    }
}
