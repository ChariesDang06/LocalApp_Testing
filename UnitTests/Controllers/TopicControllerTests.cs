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
    public class TopicControllerTests : IClassFixture<TopicControllerFixture>
    {
        private readonly TopicControllerFixture _fixture;

        public TopicControllerTests(TopicControllerFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void GetTopic_ShouldReturnOkWithListOfTopics()
        {
            // Arrange
            var expectedTopics = new List<Topic>() { new Topic() { Id = "1", Title = "Test Topic" } };
            _fixture.MockTopicRepository.Setup(repo => repo.GetTopics()).Returns(expectedTopics);
            var expectedDto = new List<TopicDto>() { new TopicDto() { Id = "1", Title = "Test Topic" } };
            _fixture.MockMapper.Setup(m => m.Map<List<TopicDto>>(It.IsAny<List<Topic>>())).Returns(expectedDto);

            // Act
            var result = _fixture.Controller.GetTopic();

            // Assert
            Assert.IsType<OkObjectResult>(result); // Use IsType for checking object type
            var objectResult = (OkObjectResult)result;
            Assert.Equal(expectedDto, objectResult.Value);
        }
        [Fact]
        public void GetTopicById_ShouldReturnOkWithTopicDto_WhenTopicExists()
        {
            // Arrange
            var fixture = new TopicControllerFixture();
            var expectedTopic = new Topic() { Id = "1", Title = "Test Topic" };
            var expectedDto = new TopicDto() { Id = "1", Title = "Test Topic" };

            fixture.MockTopicRepository.Setup(repo => repo.TopicExist("1")).Returns(true);
            fixture.MockTopicRepository.Setup(repo => repo.GetTopicById("1")).Returns(expectedTopic);
            fixture.MockMapper.Setup(m => m.Map<TopicDto>(It.IsAny<Topic>())).Returns(expectedDto);

            // Act
            var result = fixture.Controller.GetTopicById("1");

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var objectResult = (OkObjectResult)result;
            Assert.Equal(expectedDto, objectResult.Value);
        }



        [Fact]
        public void GetTopicById_ShouldReturnNotFound_WhenTopicDoesNotExist()
        {
            // Arrange
            var fixture = new TopicControllerFixture();
            fixture.MockTopicRepository.Setup(repo => repo.GetTopicById("1")).Returns((Topic)null);

            // Act
            var result = fixture.Controller.GetTopicById("1");

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
        [Fact]
        public void CreateTopic_ShouldReturnOk_WhenTopicIsCreatedSuccessfully()
        {
            // Arrange
            var fixture = new TopicControllerFixture();
            var newTopic = new TopicDto() { Id = "1", Title = "Test Topic" };
            var mappedTopic = new Topic() { Id = "1", Title = "Test Topic" };

            fixture.MockMapper.Setup(m => m.Map<Topic>(It.IsAny<TopicDto>())).Returns(mappedTopic);
            fixture.MockTopicRepository.Setup(repo => repo.GetTopics()).Returns(new List<Topic>());
            fixture.MockTopicRepository.Setup(repo => repo.CreateTopic(It.IsAny<Topic>())).Returns(true);

            // Act
            var result = fixture.Controller.CreateTopic(newTopic);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Successfully created", ((OkObjectResult)result).Value);
        }
        [Fact]
        public void UpdateTopic_ShouldReturnOk_WhenTopicIsUpdatedSuccessfully()
        {
            // Arrange
            var fixture = new TopicControllerFixture();
            var topicId = "1";
            var updateTopic = new TopicDto { Id = "1", Title = "Updated Topic" };
            var mappedTopic = new Topic { Id = "1", Title = "Updated Topic" };

            fixture.MockTopicRepository.Setup(repo => repo.TopicExist(topicId)).Returns(true);
            fixture.MockTopicRepository.Setup(repo => repo.UpdateTopic(It.IsAny<Topic>())).Returns(true);
            fixture.MockMapper.Setup(m => m.Map<Topic>(It.IsAny<TopicDto>())).Returns(mappedTopic);

            // Act
            var result = fixture.Controller.UpdateTopic(topicId, updateTopic);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Successfully updated", okResult.Value);
        }

        [Fact]
        public void UpdateTopic_ShouldReturnBadRequest_WhenTopicIdDoesNotMatch()
        {
            // Arrange
            var fixture = new TopicControllerFixture();
            var topicId = "1";
            var updateTopic = new TopicDto { Id = "2", Title = "Updated Topic" };

            // Act
            var result = fixture.Controller.UpdateTopic(topicId, updateTopic);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void UpdateTopic_ShouldReturnNotFound_WhenTopicDoesNotExist()
        {
            // Arrange
            var fixture = new TopicControllerFixture();
            var topicId = "1";
            var updateTopic = new TopicDto { Id = "1", Title = "Updated Topic" };

            fixture.MockTopicRepository.Setup(repo => repo.TopicExist(topicId)).Returns(false);

            // Act
            var result = fixture.Controller.UpdateTopic(topicId, updateTopic);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void UpdateTopic_ShouldReturnBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var fixture = new TopicControllerFixture();
            var topicId = "1";
            var updateTopic = new TopicDto { Id = "1", Title = "Updated Topic" };

            // Mocking TopicExist to return true to ensure it passes the existence check
            fixture.MockTopicRepository.Setup(repo => repo.TopicExist(topicId)).Returns(true);

            // Adding an error to the ModelState to make it invalid
            fixture.Controller.ModelState.AddModelError("Title", "Required");

            // Act
            var result = fixture.Controller.UpdateTopic(topicId, updateTopic);

            // Assert
            Assert.IsType<BadRequestResult>(result); // Changed from BadRequestObjectResult to BadRequestResult
        }

        [Fact]
        public void UpdateTopic_ShouldReturnBadRequest_WhenUpdateTopicIsNull()
        {
            // Arrange
            var fixture = new TopicControllerFixture();
            var topicId = "1";

            // Act
            var result = fixture.Controller.UpdateTopic(topicId, null);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void UpdateTopic_ShouldReturnStatusCode500_WhenUpdateFails()
        {
            // Arrange
            var fixture = new TopicControllerFixture();
            var topicId = "1";
            var updateTopic = new TopicDto { Id = "1", Title = "Updated Topic" };
            var mappedTopic = new Topic { Id = "1", Title = "Updated Topic" };

            fixture.MockTopicRepository.Setup(repo => repo.TopicExist(topicId)).Returns(true);
            fixture.MockTopicRepository.Setup(repo => repo.UpdateTopic(It.IsAny<Topic>())).Returns(false);
            fixture.MockMapper.Setup(m => m.Map<Topic>(It.IsAny<TopicDto>())).Returns(mappedTopic);

            // Act
            var result = fixture.Controller.UpdateTopic(topicId, updateTopic);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }
        [Fact]
        public void DeleteTopic_ShouldReturnOk_WhenTopicIsDeletedSuccessfully()
        {
            // Arrange
            var fixture = new TopicControllerFixture();
            var topicId = "1";
            var topicToDelete = new Topic { Id = "1", Title = "Test Topic" };

            fixture.MockTopicRepository.Setup(repo => repo.TopicExist(topicId)).Returns(true);
            fixture.MockTopicRepository.Setup(repo => repo.GetTopicById(topicId)).Returns(topicToDelete);
            fixture.MockTopicRepository.Setup(repo => repo.DeleteTopic(topicToDelete)).Returns(true);

            // Act
            var result = fixture.Controller.DeleteTopic(topicId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Successfully deleted", okResult.Value);
        }

        [Fact]
        public void DeleteTopic_ShouldReturnNotFound_WhenTopicDoesNotExist()
        {
            // Arrange
            var fixture = new TopicControllerFixture();
            var topicId = "1";

            fixture.MockTopicRepository.Setup(repo => repo.TopicExist(topicId)).Returns(false);

            // Act
            var result = fixture.Controller.DeleteTopic(topicId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void DeleteTopic_ShouldReturnBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var fixture = new TopicControllerFixture();
            var topicId = "1";
            var topicToDelete = new Topic { Id = "1", Title = "Test Topic" };

            fixture.MockTopicRepository.Setup(repo => repo.TopicExist(topicId)).Returns(true);
            fixture.MockTopicRepository.Setup(repo => repo.GetTopicById(topicId)).Returns(topicToDelete);
            fixture.Controller.ModelState.AddModelError("Title", "Required");

            // Act
            var result = fixture.Controller.DeleteTopic(topicId);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void DeleteTopic_ShouldReturnStatusCode500_WhenDeletionFails()
        {
            // Arrange
            var fixture = new TopicControllerFixture();
            var topicId = "1";
            var topicToDelete = new Topic { Id = "1", Title = "Test Topic" };

            fixture.MockTopicRepository.Setup(repo => repo.TopicExist(topicId)).Returns(true);
            fixture.MockTopicRepository.Setup(repo => repo.GetTopicById(topicId)).Returns(topicToDelete);
            fixture.MockTopicRepository.Setup(repo => repo.DeleteTopic(topicToDelete)).Returns(false);

            // Act
            var result = fixture.Controller.DeleteTopic(topicId);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result); // Check for ObjectResult
            Assert.Equal(500, statusCodeResult.StatusCode); // Check for status code 500
        }

    }
    public class TopicControllerFixture
    {
        public Mock<ITopicRepository> MockTopicRepository { get; }
        public Mock<IMapper> MockMapper { get; }
        public TopicController Controller { get; }

        public TopicControllerFixture()
        {
            MockTopicRepository = new Mock<ITopicRepository>();
            MockMapper = new Mock<IMapper>();
            Controller = new TopicController(MockTopicRepository.Object, MockMapper.Object);
        }
    }
}
