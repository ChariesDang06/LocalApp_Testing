using AutoMapper;
using LocalEdu_App.Controllers;
using LocalEdu_App.Dto;
using LocalEdu_App.Interfaces;
using LocalEdu_App.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Moq;
using Xunit;


namespace LocalEdu_App.UnitTests.Controllers
{
    public class UserProgressControllerFixture
    {
        public Mock<IUserProgressRepository> MockUserProgressRepository { get; }
        public Mock<IMapper> MockMapper { get; }
        public Mock<IAppUserRopsitory> MockAppUserRepository { get; }
        public Mock<ITopicRepository> MockTopicRepository { get; }
        public UserProgressController Controller { get; }

        public UserProgressControllerFixture()
        {
            MockUserProgressRepository = new Mock<IUserProgressRepository>();
            MockMapper = new Mock<IMapper>();
            MockAppUserRepository = new Mock<IAppUserRopsitory>();
            MockTopicRepository = new Mock<ITopicRepository>();
            Controller = new UserProgressController(MockUserProgressRepository.Object, MockMapper.Object, MockAppUserRepository.Object, MockTopicRepository.Object);
        }
    }
    public class UserProgressControllerTests
    {
        private readonly UserProgressControllerFixture _fixture;

        public UserProgressControllerTests()
        {
            _fixture = new UserProgressControllerFixture();
        }

        [Fact]
        public void GetUserProgress_ShouldReturnOkWithListOfUserProgressDto()
        {
            // Arrange
            var expectedUserProgresses = new List<UserProgress>() { new UserProgress() { UserId = "1", Score = 50 } };
            _fixture.MockUserProgressRepository.Setup(repo => repo.GetUserProgresses()).Returns(expectedUserProgresses);
            var expectedDto = new List<UserProgressDto>() { new UserProgressDto() { UserId = "1", Score = 50 } };
            _fixture.MockMapper.Setup(m => m.Map<List<UserProgressDto>>(It.IsAny<List<UserProgress>>())).Returns(expectedDto);

            // Act
            var result = _fixture.Controller.GetUserProgress();

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var objectResult = (OkObjectResult)result;
            Assert.Equal(expectedDto, objectResult.Value);
        }

        [Fact]
        public void GetUserProgressOfUser_ShouldReturnOkWithListOfUserProgressDto()
        {
            // Arrange
            var userId = "1";
            var expectedUserProgresses = new List<UserProgress>() { new UserProgress() { UserId = userId, Score = 50 } };
            _fixture.MockUserProgressRepository.Setup(repo => repo.GetUserProgressOfUser(userId)).Returns(expectedUserProgresses);
            var expectedDto = new List<UserProgressDto>() { new UserProgressDto() { UserId = userId, Score = 50 } };
            _fixture.MockMapper.Setup(m => m.Map<List<UserProgressDto>>(It.IsAny<List<UserProgress>>())).Returns(expectedDto);

            // Act
            var result = _fixture.Controller.GetUserProgressOfUser(userId);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var objectResult = (OkObjectResult)result;
            Assert.Equal(expectedDto, objectResult.Value);
        }
        [Fact]
        public void CreateUserProgress_ShouldReturnNoContent_WhenUserProgressCreatedSuccessfully()
        {
            // Arrange
            var userId = "1";
            var topicId = "topic1";
            var userProgressCreate = new UserProgressDto { UserId = userId, TopicId = topicId, Score = 50 };
            _fixture.MockAppUserRepository.Setup(repo => repo.AppUserExist(userId)).Returns(true);
            _fixture.MockTopicRepository.Setup(repo => repo.TopicExist(topicId)).Returns(true);
            _fixture.MockUserProgressRepository.Setup(repo => repo.GetUserProgresses())
                                               .Returns(new List<UserProgress>());
            var userProgressMap = new UserProgress { UserId = userId, TopicId = topicId, Score = 50 };
            _fixture.MockMapper.Setup(m => m.Map<UserProgress>(It.IsAny<UserProgressDto>())).Returns(userProgressMap);
            _fixture.MockUserProgressRepository.Setup(repo => repo.CreateUserProgress(userProgressMap)).Returns(true);

            // Act
            var result = _fixture.Controller.CreateUserProgress(userId, topicId, userProgressCreate);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var objectResult = (OkObjectResult)result;
            Assert.Equal("Successfully created", objectResult.Value);
        }

        [Fact]
        public void CreateUserProgress_ShouldReturnBadRequest_WhenUserProgressDtoIsNull()
        {
            // Arrange
            string userId = "1";
            string topicId = "topic1";

            // Act
            var result = _fixture.Controller.CreateUserProgress(userId, topicId, null);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            var badRequestObjectResult = (BadRequestObjectResult)result;
            Assert.Empty((Dictionary<string, object>)badRequestObjectResult.Value);

        }




        [Fact]
        public void CreateUserProgress_ShouldReturnStatusCode500_WhenUserProgressCreationFails()
        {
            // Arrange
            var userId = "1";
            var topicId = "topic1";
            var userProgressCreate = new UserProgressDto { UserId = userId, TopicId = topicId, Score = 50 };
            _fixture.MockAppUserRepository.Setup(repo => repo.AppUserExist(userId)).Returns(true);
            _fixture.MockTopicRepository.Setup(repo => repo.TopicExist(topicId)).Returns(true);
            _fixture.MockUserProgressRepository.Setup(repo => repo.GetUserProgresses())
                                               .Returns(new List<UserProgress>());
            var userProgressMap = new UserProgress { UserId = userId, TopicId = topicId, Score = 50 };
            _fixture.MockMapper.Setup(m => m.Map<UserProgress>(It.IsAny<UserProgressDto>())).Returns(userProgressMap);
            _fixture.MockUserProgressRepository.Setup(repo => repo.CreateUserProgress(userProgressMap)).Returns(false);

            // Act
            var result = _fixture.Controller.CreateUserProgress(userId, topicId, userProgressCreate);

            // Assert
            Assert.IsType<ObjectResult>(result);
            var objectResult = (ObjectResult)result;
            Assert.Equal(500, objectResult.StatusCode);

        }
        [Fact]
        public void UpdateUserProgress_ShouldReturnNoContent_WhenUserProgressUpdatedSuccessfully()
        {
            // Arrange
            var userId = "1";
            var topicId = "topic1";
            var userProgressUpdated = new UserProgressDto { UserId = userId, TopicId = topicId, Score = 75 };
            _fixture.MockAppUserRepository.Setup(repo => repo.AppUserExist(userId)).Returns(true);
            _fixture.MockTopicRepository.Setup(repo => repo.TopicExist(topicId)).Returns(true);
            var userProgressMap = new UserProgress { UserId = userId, TopicId = topicId, Score = 75 };
            _fixture.MockMapper.Setup(m => m.Map<UserProgress>(It.IsAny<UserProgressDto>())).Returns(userProgressMap);
            _fixture.MockUserProgressRepository.Setup(repo => repo.UpdateUserProgress(userProgressMap)).Returns(true);

            // Act
            var result = _fixture.Controller.UpdateUserProgress(userId, topicId, userProgressUpdated);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var objectResult = (OkObjectResult)result;
            Assert.Equal("Successfully updated", objectResult.Value);
        }

        [Fact]
        public void UpdateUserProgress_ShouldReturnBadRequest_WhenUserProgressDtoIsNull()
        {
            // Arrange
            var userId = "1";
            var topicId = "topic1";

            // Act
            var result = _fixture.Controller.UpdateUserProgress(userId, topicId, null);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);

        }

        [Fact]
        public void UpdateUserProgress_ShouldReturnBadRequest_WhenModelStateIsNotValid()
        {
            // Arrange
            var userId = "1";
            var topicId = "topic1";
            var userProgressUpdated = new UserProgressDto { UserId = userId, TopicId = topicId, Score = -10 };
            _fixture.Controller.ModelState.AddModelError("key", "error");

            // Act
            var result = _fixture.Controller.UpdateUserProgress(userId, topicId, userProgressUpdated);

            // Assert
            Assert.IsType<ObjectResult>(result);

        }

        [Fact]
        public void UpdateUserProgress_ShouldReturnStatusCode500_WhenUserProgressUpdateFails()
        {
            // Arrange
            var userId = "1";
            var topicId = "topic1";
            var userProgressUpdated = new UserProgressDto { UserId = userId, TopicId = topicId, Score = 75 };
            _fixture.MockAppUserRepository.Setup(repo => repo.AppUserExist(userId)).Returns(true);
            _fixture.MockTopicRepository.Setup(repo => repo.TopicExist(topicId)).Returns(true);
            var userProgressMap = new UserProgress { UserId = userId, TopicId = topicId, Score = 75 };
            _fixture.MockMapper.Setup(m => m.Map<UserProgress>(It.IsAny<UserProgressDto>())).Returns(userProgressMap);
            _fixture.MockUserProgressRepository.Setup(repo => repo.UpdateUserProgress(userProgressMap)).Returns(false);

            // Act
            var result = _fixture.Controller.UpdateUserProgress(userId, topicId, userProgressUpdated);

            // Assert
            Assert.IsType<ObjectResult>(result);
            var objectResult = (ObjectResult)result;
            Assert.Equal(500, objectResult.StatusCode);

        }
        [Fact]
        public void DeleteTopicProgressOfUser_ShouldReturnNoContent_WhenTopicProgressDeletedSuccessfully()
        {
            // Arrange
            var userId = "1";
            var topicId = "topic1";
            _fixture.MockUserProgressRepository.Setup(repo => repo.UserProgressExist(userId, topicId)).Returns(true);
            var topicProgressToDelete = new UserProgress { UserId = userId, TopicId = topicId, Score = 75 };
            _fixture.MockUserProgressRepository.Setup(repo => repo.GetUserProgressOfUser(userId)).Returns(new List<UserProgress> { topicProgressToDelete });
            _fixture.MockUserProgressRepository.Setup(repo => repo.DeleteUserProgress(topicProgressToDelete)).Returns(true);

            // Act
            var result = _fixture.Controller.DeleteTopicProgressOfUser(userId, topicId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Theory]
        [InlineData("1", "topic1")]
        public void DeleteTopicProgressOfUser_ShouldReturnBadRequest_WhenTopicProgressDoesNotExist(string userId, string topicId)
        {
            // Arrange
            _fixture.MockUserProgressRepository.Setup(repo => repo.UserProgressExist(userId, topicId)).Returns(false);

            // Act
            var result = _fixture.Controller.DeleteTopicProgressOfUser(userId, topicId);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);

        }

        [Fact]
        public void DeleteTopicProgressOfUser_ShouldReturnStatusCode500_WhenTopicProgressDeletionFails()
        {
            // Arrange
            var userId = "1";
            var topicId = "topic1";
            _fixture.MockUserProgressRepository.Setup(repo => repo.UserProgressExist(userId, topicId)).Returns(true);
            var topicProgressToDelete = new UserProgress { UserId = userId, TopicId = topicId, Score = 75 };
            _fixture.MockUserProgressRepository.Setup(repo => repo.GetUserProgressOfUser(userId)).Returns(new List<UserProgress> { topicProgressToDelete });
            _fixture.MockUserProgressRepository.Setup(repo => repo.DeleteUserProgress(topicProgressToDelete)).Returns(false);

            // Act
            var result = _fixture.Controller.DeleteTopicProgressOfUser(userId, topicId);

            // Assert
            Assert.IsType<ObjectResult>(result);

        }

    }
}
