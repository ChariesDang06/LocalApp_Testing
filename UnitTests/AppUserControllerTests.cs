using AutoMapper;
using LocalEdu_App.Controllers;
using LocalEdu_App.Dto;
using LocalEdu_App.Interfaces;
using LocalEdu_App.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace LocalEdu_App.Tests.Controllers
{
    public class AppUserControllerTests
    {
        [Fact]
        public void GetAppUser_ReturnsOkResult_WithMappedUsers()
        {
            // Arrange
            var mockAppUsers = new List<AppUser>
            {
                new AppUser { Id = "1", Email = "User 1" },
                new AppUser { Id = "2", Email = "User 2" }
            };

            var mockAppUserRepository = new Mock<IAppUserRopsitory>();
            mockAppUserRepository.Setup(repo => repo.GetAppUsers()).Returns(mockAppUsers);

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(mapper => mapper.Map<List<AppUserDto>>(mockAppUsers))
                      .Returns(new List<AppUserDto>
                      {
                          new AppUserDto { Id = "1", Email = "User 1" },
                          new AppUserDto { Id = "2", Email = "User 3" }
                      });

            var controller = new AppUserController(mockAppUserRepository.Object, mockMapper.Object);

            // Act
            var result = controller.GetAppUser();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var users = Assert.IsAssignableFrom<IEnumerable<AppUserDto>>(okResult.Value);
            Assert.Equal(2, users.Count()); // Ensure two users are returned
        }

        [Fact]
        public void GetAppUserReturnsBadRequestWhenModelStateIsInvalid()
        {
            // Arrange
            var mockAppUserRepository = new Mock<IAppUserRopsitory>();
            var mockMapper = new Mock<IMapper>();

            var controller = new AppUserController(mockAppUserRepository.Object, mockMapper.Object);
            controller.ModelState.AddModelError("Key", "Error message");

            // Act
            var result = controller.GetAppUser();

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsAssignableFrom<SerializableError>(badRequestResult.Value);
        }

        [Fact]
        public void GetAppUser_ReturnsOkResult_WithEmptyList_WhenRepositoryReturnsEmpty()
        {
            // Arrange
            var mockAppUsers = new List<AppUser>();

            var mockAppUserRepository = new Mock<IAppUserRopsitory>();
            mockAppUserRepository.Setup(repo => repo.GetAppUsers()).Returns(mockAppUsers);

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(mapper => mapper.Map<List<AppUserDto>>(mockAppUsers))
                      .Returns(new List<AppUserDto>());

            var controller = new AppUserController(mockAppUserRepository.Object, mockMapper.Object);

            // Act
            var result = controller.GetAppUser();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var users = Assert.IsAssignableFrom<IEnumerable<AppUserDto>>(okResult.Value);
            Assert.Empty(users); // Ensure no users are returned
        }
        [Fact]
        public void GetAppUser_ReturnsOkResult_WhenUserExists()
        {
            // Arrange
            var mockAppUserId = "1";
            var mockAppUserDto = new AppUserDto { Id = mockAppUserId, Email = "Test User" };
            var mockAppUser = new AppUser { Id = mockAppUserId, Email = "Test User" };

            var mockAppUserRepository = new Mock<IAppUserRopsitory>();
            mockAppUserRepository.Setup(repo => repo.AppUserExist(mockAppUserId)).Returns(true);
            mockAppUserRepository.Setup(repo => repo.GetAppUserById(mockAppUserId)).Returns(mockAppUser);

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(mapper => mapper.Map<AppUserDto>(mockAppUser)).Returns(mockAppUserDto);

            var controller = new AppUserController(mockAppUserRepository.Object, mockMapper.Object);

            // Act
            var result = controller.GetAppUser(mockAppUserId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var appUser = Assert.IsAssignableFrom<AppUserDto>(okResult.Value);
            Assert.Equal(mockAppUserId, appUser.Id);
        }

        [Fact]
        public void GetAppUser_ReturnsNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var mockAppUserId = "1";

            var mockAppUserRepository = new Mock<IAppUserRopsitory>();
            mockAppUserRepository.Setup(repo => repo.AppUserExist(mockAppUserId)).Returns(false);

            var mockMapper = new Mock<IMapper>();

            var controller = new AppUserController(mockAppUserRepository.Object, mockMapper.Object);

            // Act
            var result = controller.GetAppUser(mockAppUserId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void GetAppUser_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var mockAppUserId = "1";
            var mockAppUserDto = new AppUserDto { Id = mockAppUserId, Email = "TestUser@gmail.com" };
            var mockAppUser = new AppUser { Id = mockAppUserId, Email = "TestUser@gmail.com" };

            var mockAppUserRepository = new Mock<IAppUserRopsitory>();
            mockAppUserRepository.Setup(repo => repo.AppUserExist(mockAppUserId)).Returns(true);
            mockAppUserRepository.Setup(repo => repo.GetAppUserById(mockAppUserId)).Returns(mockAppUser);

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(mapper => mapper.Map<AppUserDto>(mockAppUser)).Returns(mockAppUserDto);

            var controller = new AppUserController(mockAppUserRepository.Object, mockMapper.Object);
            controller.ModelState.AddModelError("Key", "Error message");

            // Act
            var result = controller.GetAppUser(mockAppUserId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsAssignableFrom<SerializableError>(badRequestResult.Value);
        }
    }
}
