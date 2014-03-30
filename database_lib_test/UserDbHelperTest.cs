using data_models.Models;
using database_lib.DbHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Configuration;

namespace library_database_lib_test
{
    [TestClass]
    public class UserDbHelperTest
    {
        UserDbHelper userDbHelper = UserDbHelper.Instance;

        [TestInitialize]
        public void UserDbHelperTestInit()
        {
            string connectionString = ConfigurationManager.
                ConnectionStrings["SqlConnectionStrings"].ConnectionString;

            userDbHelper.SetConnectionString(connectionString);
        }
        
        [TestMethod]
        public void GetAllUsers_Test()
        {
            List<User> users;

            users = userDbHelper.GetAllUsers();
            Assert.IsNotNull(users);
        }

        [TestMethod]
        public void GetUserById_Test()
        {
            User user = null;
            int userId = -1;

            user = userDbHelper.GetUserById(userId);
            Assert.IsNull(user);

            userId = 1;
            user = userDbHelper.GetUserById(userId);
            Assert.AreEqual(user.Id, userId);
        }

        [TestMethod]
        public void GetUsersById_Test()
        {
            List<User> users = new List<User>();
            List<int> usersIdList = new List<int>();
            usersIdList.Add(-1);

            users = userDbHelper.GetUsersById(usersIdList);
            Assert.IsTrue(users.Count == 0);
            //Assert.AreEqual(users.Count, usersIdList.FindAll(id => id != -1).Count);

            usersIdList.Add(1);
            usersIdList.Add(2);

            users = userDbHelper.GetUsersById(usersIdList);
            Assert.AreEqual(users.Count, usersIdList.FindAll(id => id != -1).Count);
        }

        [TestMethod]
        public void GetUserByEmail_Test()
        {
            User user = null;
            string userEmail = "";

            user = userDbHelper.GetUserByEmail(userEmail);
            Assert.IsNull(user);

            userEmail = "admin@gmail.com";
            user = userDbHelper.GetUserByEmail(userEmail);
            Assert.AreEqual(userEmail, user.Email);
        }

        [TestMethod]
        public void AddNewUser_Test()
        {
            User user = null;
            int userId;

            userId = userDbHelper.AddNewUser(user);
            Assert.IsTrue(userId != -1);
        }
    }
}
