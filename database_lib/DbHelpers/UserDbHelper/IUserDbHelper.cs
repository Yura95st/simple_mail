using data_models.Models;
using System.Collections.Generic;

namespace database_lib.DbHelpers
{
    public interface IUserDbHelper
    {
        // gets all users
        List<User> GetAllUsers();

        // gets user by it's id number
        User GetUserById(int userId);

        // gets users with id numbers from "userIdsList" list
        List<User> GetUsersById(List<int> usersIdList);

        // gets user by email
        User GetUserByEmail(string email);

        // gets users by searching parameters
        List<User> SearchForUsers();

        // adds new user
        // returns id of the newly created user or 0 - if it wasn't created
        int AddNewUser(User user);

        // deletes user
        void DeleteUser(int userId);

        // modifies user's info
        void ModifyUserInfo(User user);

        // changes user password
        void ChangePassword(User user);

        int LogInUser(User user);

        void LogOutUser(int userId);
       
        void SetUserOnline(int userId);

        void SetUserOffline(int userId);       
    }
}
