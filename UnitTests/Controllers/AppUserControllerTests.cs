using AutoMapper;
using LocalEdu_App.Controllers;
using LocalEdu_App.Dto;
using LocalEdu_App.Interfaces;
using LocalEdu_App.Models;
using LocalEdu_App.Repository;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace LocalEdu_App.UnitTests.Controllers
{
    public class AppUserControllerFixture
    {
        public Mock<IAppUserRopsitory> MockAppUserRepository { get; }
        public Mock<IMapper> MockMapper { get; }
        public AppUserController Controller { get; }

        public AppUserControllerFixture()
        {
            MockAppUserRepository = new Mock<IAppUserRopsitory>();
            MockMapper = new Mock<IMapper>();
            Controller = new AppUserController(MockAppUserRepository.Object, MockMapper.Object);
        }
    }
    public class AppUserControllerTests : IClassFixture<AppUserControllerFixture>
    {
        private readonly AppUserControllerFixture _fixture;

        public AppUserControllerTests(AppUserControllerFixture fixture)
        {
            _fixture = fixture;
        }
        [Fact]
        public void GetAppUser_ShouldReturnOkWithListOfAppUsers()
        {
            // Arrange
            var expectedAppUsers = new List<AppUser>() { new AppUser() { Id = "1",  AppUserName = "Test AppUser" } };
            _fixture.MockAppUserRepository.Setup(repo => repo.GetAppUsers()).Returns(expectedAppUsers);
            var expectedDto = new List<AppUserDto>() { new AppUserDto() { Id = "1", AppUserName = "Test AppUser" } };
            _fixture.MockMapper.Setup(m => m.Map<List<AppUserDto>>(It.IsAny<List<AppUser>>())).Returns(expectedDto);

            // Act
            var result = _fixture.Controller.GetAppUser();

            // Assert
            Assert.IsType<OkObjectResult>(result); // Use IsType for checking object type
            var objectResult = (OkObjectResult)result;
            Assert.Equal(expectedDto, objectResult.Value);
        }
        [Fact]
        public void GetAppUserById_ShouldReturnOkWithTopicDto_WhenAppUserExists()
        {
            // Arrange
            var fixture = new AppUserControllerFixture();
            var expectedAppUser = new AppUser() { Id = "1", AppUserName = "Test AppUser" };
            var expectedDto = new AppUserDto() { Id = "1", AppUserName = "Test AppUser" };

            fixture.MockAppUserRepository.Setup(repo => repo.AppUserExist("1")).Returns(true);
            fixture.MockAppUserRepository.Setup(repo => repo.GetAppUserById("1")).Returns(expectedAppUser);
            fixture.MockMapper.Setup(m => m.Map<AppUserDto>(It.IsAny<AppUser>())).Returns(expectedDto);

            // Act
            var result = fixture.Controller.GetAppUser("1");

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var objectResult = (OkObjectResult)result;
            Assert.Equal(expectedDto, objectResult.Value);
        }



        [Fact]
        public void GetAppUserById_ShouldReturnNotFound_WhenAppUserDoesNotExist()
        {
            // Arrange
            var fixture = new AppUserControllerFixture();
            fixture.MockAppUserRepository.Setup(repo => repo.GetAppUserById("1")).Returns((AppUser)null);

            // Act
            var result = fixture.Controller.GetAppUser("1");

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
        [Fact]
        public void CreateAppUser_ShouldReturnOk_WhenAppUserIsCreatedSuccessfully()
        {
            // Arrange
            var fixture = new AppUserControllerFixture();
            var newAppUser = new AppUserDto() { Id = "1", AppUserName = "Test AppUser" };
            var mappedAppUser = new AppUser() { Id = "1", AppUserName = "Test AppUser" };

            fixture.MockMapper.Setup(m => m.Map<AppUser>(It.IsAny<AppUserDto>())).Returns(mappedAppUser);
            fixture.MockAppUserRepository.Setup(repo => repo.GetAppUsers()).Returns(new List<AppUser>());
            fixture.MockAppUserRepository.Setup(repo => repo.CreateAppUser(It.IsAny<AppUser>())).Returns(true);

            // Act
            var result = fixture.Controller.CreateAppUser(newAppUser);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Successfully created", ((OkObjectResult)result).Value);
        }
        [Fact]
        public void UpdateAppUser_ShouldReturnOk_WhenAppUserIsUpdatedSuccessfully()
        {
            // Arrange
            var fixture = new AppUserControllerFixture();
            var appUserId = "1";
            var updateAppUser = new AppUserDto { Id = "1", AppUserName = "Updated AppUser" };
            var mappedAppUser = new AppUser { Id = "1", AppUserName = "Updated AppUser" };

            fixture.MockAppUserRepository.Setup(repo => repo.AppUserExist(appUserId)).Returns(true);
            fixture.MockAppUserRepository.Setup(repo => repo.UpdateAppUser(It.IsAny<AppUser>())).Returns(true);
            fixture.MockMapper.Setup(m => m.Map<AppUser>(It.IsAny<AppUserDto>())).Returns(mappedAppUser);

            // Act
            var result = fixture.Controller.UpdateAppUser(appUserId, updateAppUser);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Successfully updated", okResult.Value);
        }

        [Fact]
        public void UpdateAppUser_ShouldReturnBadRequest_WhenAppUserIdDoesNotMatch()
        {
            // Arrange
            var fixture = new AppUserControllerFixture();
            var appUserId = "1";
            var updateAppUser = new AppUserDto { Id = "2", AppUserName = "Updated AppUser" };

            // Act
            var result = fixture.Controller.UpdateAppUser(appUserId, updateAppUser);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void UpdateAppUser_ShouldReturnNotFound_WhenAppUserDoesNotExist()
        {
            // Arrange
            var fixture = new AppUserControllerFixture();
            var appUserId = "1";
            var updateAppUser = new AppUserDto { Id = "1", AppUserName = "Updated AppUser" };

            fixture.MockAppUserRepository.Setup(repo => repo.AppUserExist(appUserId)).Returns(false);

            // Act
            var result = fixture.Controller.UpdateAppUser(appUserId, updateAppUser);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void UpdateAppUser_ShouldReturnBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var fixture = new AppUserControllerFixture();
            var appUserId = "1";
            var updateAppUser = new AppUserDto { Id = "1", AppUserName = "Updated AppUser" };

            // Mocking TopicExist to return true to ensure it passes the existence check
            fixture.MockAppUserRepository.Setup(repo => repo.AppUserExist(appUserId)).Returns(true);

            // Adding an error to the ModelState to make it invalid
            fixture.Controller.ModelState.AddModelError("AppUserName", "Required");

            // Act
            var result = fixture.Controller.UpdateAppUser(appUserId, updateAppUser);

            // Assert
            Assert.IsType<BadRequestResult>(result); // Changed from BadRequestObjectResult to BadRequestResult
        }

        [Fact]
        public void UpdateTopic_ShouldReturnBadRequest_WhenUpdateTopicIsNull()
        {
            // Arrange
            var fixture = new AppUserControllerFixture();
            var appUserId = "1";

            // Act
            var result = fixture.Controller.UpdateAppUser(appUserId, null);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void UpdateAppUser_ShouldReturnStatusCode500_WhenUpdateFails()
        {
            // Arrange
            var fixture = new AppUserControllerFixture();
            var appUserId = "1";
            var updateAppUser = new AppUserDto { Id = "1", AppUserName = "Updated AppUser" };
            var mappedAppUser = new AppUser { Id = "1", AppUserName = "Updated AppUser" };

            fixture.MockAppUserRepository.Setup(repo => repo.AppUserExist(appUserId)).Returns(true);
            fixture.MockAppUserRepository.Setup(repo => repo.UpdateAppUser(It.IsAny<AppUser>())).Returns(false);
            fixture.MockMapper.Setup(m => m.Map<AppUser>(It.IsAny<AppUserDto>())).Returns(mappedAppUser);

            // Act
            var result = fixture.Controller.UpdateAppUser(appUserId, updateAppUser);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }
        [Fact]
        public void DeleteAppUser_ShouldReturnOk_WhenAppUserIsDeletedSuccessfully()
        {
            // Arrange
            var fixture = new AppUserControllerFixture();
            var appUserId = "1";
            var appUserToDelete = new AppUser { Id = "1", AppUserName = "Test AppUser" };

            fixture.MockAppUserRepository.Setup(repo => repo.AppUserExist(appUserId)).Returns(true);
            fixture.MockAppUserRepository.Setup(repo => repo.GetAppUserById(appUserId)).Returns(appUserToDelete);
            fixture.MockAppUserRepository.Setup(repo => repo.DeleteAppUser(appUserToDelete)).Returns(true);

            // Act
            var result = fixture.Controller.DeleteAppUser(appUserId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Successfully deleted", okResult.Value);
        }

        [Fact]
        public void DeleteAppUser_ShouldReturnNotFound_WhenAppUserDoesNotExist()
        {
            // Arrange
            var fixture = new AppUserControllerFixture();
            var appUserId = "1";

            fixture.MockAppUserRepository.Setup(repo => repo.AppUserExist(appUserId)).Returns(false);

            // Act
            var result = fixture.Controller.DeleteAppUser(appUserId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void DeleteAppUser_ShouldReturnBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var fixture = new AppUserControllerFixture();
            var appUserId = "1";
            var appUserToDelete = new AppUser { Id = "1", AppUserName = "Test AppUser" };

            fixture.MockAppUserRepository.Setup(repo => repo.AppUserExist(appUserId)).Returns(true);
            fixture.MockAppUserRepository.Setup(repo => repo.GetAppUserById(appUserId)).Returns(appUserToDelete);
            fixture.Controller.ModelState.AddModelError("AppUserName", "Required");

            // Act
            var result = fixture.Controller.DeleteAppUser(appUserId);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void DeleteAppUser_ShouldReturnStatusCode500_WhenDeletionFails()
        {
            // Arrange
            var fixture = new AppUserControllerFixture();
            var appUserId = "1";
            var appUserToDelete = new AppUser { Id = "1", AppUserName = "Test AppUser" };

            fixture.MockAppUserRepository.Setup(repo => repo.AppUserExist(appUserId)).Returns(true);
            fixture.MockAppUserRepository.Setup(repo => repo.GetAppUserById(appUserId)).Returns(appUserToDelete);
            fixture.MockAppUserRepository.Setup(repo => repo.DeleteAppUser(appUserToDelete)).Returns(false);

            // Act
            var result = fixture.Controller.DeleteAppUser(appUserId);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result); // Check for ObjectResult
            Assert.Equal(500, statusCodeResult.StatusCode); // Check for status code 500
        }

    }
}