using CMSTechnical.Controllers;
using CMSTechnical.Data;
using CMSTechnical.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace CMSTechnical.Test
{
    [TestClass]
    public class UserControllerTest
    {
        private static CMSTechnicalContext _context;
        private static UsersController _controller;

        [ClassInitialize]
        public static void ClassInitialize(TestContext tc)
        {
            var options = new DbContextOptionsBuilder<CMSTechnicalContext>().UseInMemoryDatabase(databaseName: "CMSTechnical").Options;
            _context = new CMSTechnicalContext(options);

            _context.User.Add(new User { Username = "Steve" });
            _context.User.Add(new User { Username = "Stephen" });
            _context.User.Add(new User { Username = "Evets" });
            _context.User.Add(new User { Username = "Nehpets" });

            _context.SaveChanges();

            _controller = new UsersController(_context);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            _context.Dispose();
        }

        [TestMethod]
        public async Task PostUser_AddNewUser()
        {
            //Arrange
            var newUser = new User { Username = "AddNewUser" };

            //Act
            var actionResult = await _controller.PostUser(newUser);
            var result = actionResult.Result as CreatedAtActionResult;
            var user = result.Value as User;

            //Assert
            Assert.AreEqual(201, result.StatusCode);
            Assert.IsNotNull(user.UserId);
        }

        [TestMethod]
        public async Task PostUser_DuplicateUsername()
        {
            //Arrange
            var newUser = new User { Username = "Steve" };

            //Act
            var actionResult = await _controller.PostUser(newUser);
            var result = actionResult.Result as ConflictObjectResult;

            //Assert
            Assert.AreEqual(409, result.StatusCode);
        }

        [TestMethod]
        public async Task PutUser_BadRequest()
        {
            //Arrange
            var newUser = new User { Username = "BadRequest" };

            //Act
            var actionResult = await _controller.PutUser(1, newUser);
            var result = actionResult as BadRequestResult;

            //Assert
            Assert.AreEqual(400, result.StatusCode);
        }

        [TestMethod]
        public async Task PutUser_UserNotFound()
        {
            //Arrange
            var newUser = new User {UserId = 100, Username = "UserNotFound" };

            //Act
            var actionResult = await _controller.PutUser(100, newUser);
            var result = actionResult as NotFoundResult;

            //Assert
            Assert.AreEqual(404, result.StatusCode);
        }

        [TestMethod]
        public async Task PutUser_UpdateUsername()
        {
            //Arrange
            var existingUser = await _context.User.FirstOrDefaultAsync();
            existingUser.Username = "UpdateUsername";

            //Act
            var actionResult = await _controller.PutUser(existingUser.UserId, existingUser);
            var result = actionResult as NoContentResult;

            //Assert
            Assert.AreEqual(204, result.StatusCode);
        }

        [TestMethod]
        public async Task DeleteUser_UserNotExist()
        {
            //Arrange

            //Act
            var actionResult = await _controller.DeleteUser(100);
            var result = actionResult.Result as NotFoundResult;

            //Assert
            Assert.AreEqual(404, result.StatusCode);
        }

        [TestMethod]
        public async Task DeleteUser_UserDeleted()
        {
            //Arrange
            var newUser = new User { Username = "DeleteUser" };
            _context.User.Add(newUser);

            //Act
            var actionResult = await _controller.DeleteUser(newUser.UserId);
            var user = actionResult.Value as User;

            //Assert
            Assert.AreEqual(user, newUser);
            Assert.IsNull(await _context.User.FindAsync(user.UserId));
        }
    }
}
