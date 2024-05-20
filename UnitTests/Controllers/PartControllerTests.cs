using AutoMapper;
using LocalEdu_App.Controllers;
using LocalEdu_App.Dto;
using LocalEdu_App.Interfaces;
using LocalEdu_App.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace LocalEdu_App.UnitTests.Controllers
{
    public class PartControllerFixture
    {
        public Mock<IPartRepository> MockPartRepository { get; }
        public Mock<ITopicRepository> MockTopicRepository { get; }
        public Mock<IMapper> MockMapper { get; }
        public PartController Controller { get; }

        public PartControllerFixture()
        {
            MockPartRepository = new Mock<IPartRepository>();
            MockTopicRepository = new Mock<ITopicRepository>();
            MockMapper = new Mock<IMapper>();
            Controller = new PartController(MockPartRepository.Object, MockTopicRepository.Object, MockMapper.Object);
        }
    }

    public class PartControllerTests
    {
        private readonly PartControllerFixture _fixture;

        public PartControllerTests()
        {
            _fixture = new PartControllerFixture();
        }

        [Fact]
        public void GetParts_ShouldReturnOkWithListOfPartDto_WhenPartsExist()
        {
            // Arrange
            var expectedParts = new List<Part>() { new Part() { Id = "1", Title = "Test Part" } };
            _fixture.MockPartRepository.Setup(repo => repo.GetParts()).Returns(expectedParts);
            var expectedDto = new List<PartDto>() { new PartDto() { Id = "1", Title = "Test Part" } };
            _fixture.MockMapper.Setup(m => m.Map<List<PartDto>>(It.IsAny<List<Part>>())).Returns(expectedDto);

            // Act
            var result = _fixture.Controller.GetParts();

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var objectResult = (OkObjectResult)result;
            Assert.Equal(expectedDto, objectResult.Value);
        }
        [Fact]
        public void CreatePart_ShouldReturnOk_WhenPartIsCreatedSuccessfully()
        {
            // Arrange
            var topicId = "1";
            var newPart = new PartDto() { Id = "1", Title = "Test Part" };
            var mappedPart = new Part() { Id = "1", Title = "Test Part" };

            _fixture.MockPartRepository.Setup(repo => repo.GetParts()).Returns(new List<Part>());
            _fixture.MockPartRepository.Setup(repo => repo.CreatePart(It.IsAny<Part>())).Returns(true);
            _fixture.MockTopicRepository.Setup(repo => repo.GetTopicById(topicId)).Returns(new Topic());

            _fixture.MockMapper.Setup(m => m.Map<Part>(It.IsAny<PartDto>())).Returns(mappedPart);

            // Act
            var result = _fixture.Controller.CreatePart(topicId, newPart);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Successfully created", ((OkObjectResult)result).Value);
        }

        [Fact]
        public void CreatePart_ShouldReturnBadRequest_WhenPartDtoIsNull()
        {
            // Arrange
            var topicId = "1";
            PartDto newPart = null;

            // Act
            var result = _fixture.Controller.CreatePart(topicId, newPart);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void CreatePart_ShouldReturnStatusCode422_WhenPartAlreadyExists()
        {
            // Arrange
            var topicId = "1";
            var existingPart = new PartDto() { Id = "1", Title = "Existing Part" };
            _fixture.MockPartRepository.Setup(repo => repo.GetParts()).Returns(new List<Part> { new Part() { Id = "1", Title = "Existing Part" } });

            // Act
            var result = _fixture.Controller.CreatePart(topicId, existingPart);

            // Assert
            Assert.IsType<ObjectResult>(result);
            var objectResult = (ObjectResult)result;
            Assert.Equal(422, objectResult.StatusCode);
        }

        [Fact]
        public void UpdatePart_ShouldReturnOk_WhenPartIsUpdatedSuccessfully()
        {
            // Arrange
            var fixture = new PartControllerFixture();
            var partId = "1";
            var updatePart = new PartDto { Id = "1", Title = "Updated Part" };
            var mappedPart = new Part { Id = "1", Title = "Updated Part" };

            fixture.MockPartRepository.Setup(repo => repo.PartExist(partId)).Returns(true);
            fixture.MockPartRepository.Setup(repo => repo.UpdatePart(It.IsAny<Part>())).Returns(true);
            fixture.MockMapper.Setup(m => m.Map<Part>(It.IsAny<PartDto>())).Returns(mappedPart);

            // Act
            var result = fixture.Controller.UpdatePart(partId, updatePart);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Successfully updated", okResult.Value);
        }

        [Fact]
        public void UpdatePart_ShouldReturnBadRequest_WhenPartIdDoesNotMatch()
        {
            // Arrange
            var fixture = new PartControllerFixture();
            var partId = "1";
            var updatePart = new PartDto { Id = "2", Title = "Updated Part" };

            // Act
            var result = fixture.Controller.UpdatePart(partId, updatePart);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void UpdatePart_ShouldReturnNotFound_WhenPartDoesNotExist()
        {
            // Arrange
            var fixture = new PartControllerFixture();
            var partId = "1";
            var updatePart = new PartDto { Id = "1", Title = "Updated Part" };

            fixture.MockPartRepository.Setup(repo => repo.PartExist(partId)).Returns(false);

            // Act
            var result = fixture.Controller.UpdatePart(partId, updatePart);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void UpdatePart_ShouldReturnBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var fixture = new PartControllerFixture();
            var partId = "1";
            var updatePart = new PartDto { Id = "1", Title = "Updated Part" };

            // Mocking TopicExist to return true to ensure it passes the existence check
            fixture.MockPartRepository.Setup(repo => repo.PartExist(partId)).Returns(true);

            // Adding an error to the ModelState to make it invalid
            fixture.Controller.ModelState.AddModelError("Title", "Required");

            // Act
            var result = fixture.Controller.UpdatePart(partId, updatePart);

            // Assert
            Assert.IsType<BadRequestResult>(result); // Changed from BadRequestObjectResult to BadRequestResult
        }

        [Fact]
        public void UpdatePart_ShouldReturnBadRequest_WhenUpdatePartIsNull()
        {
            // Arrange
            var fixture = new PartControllerFixture();
            var partId = "1";

            // Act
            var result = fixture.Controller.UpdatePart(partId, null);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void UpdatePart_ShouldReturnStatusCode500_WhenUpdateFails()
        {
            // Arrange
            var fixture = new PartControllerFixture();
            var partId = "1";
            var updatePart = new PartDto { Id = "1", Title = "Updated Part" };
            var mappedPart = new Part { Id = "1", Title = "Updated Part" };

            fixture.MockPartRepository.Setup(repo => repo.PartExist(partId)).Returns(true);
            fixture.MockPartRepository.Setup(repo => repo.UpdatePart(It.IsAny<Part>())).Returns(false);
            fixture.MockMapper.Setup(m => m.Map<Part>(It.IsAny<PartDto>())).Returns(mappedPart);

            // Act
            var result = fixture.Controller.UpdatePart(partId, updatePart);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }
        [Fact]
        public void DeletePart_ShouldReturnOk_WhenPartIsDeletedSuccessfully()
        {
            // Arrange
            var fixture = new PartControllerFixture();
            var partId = "1";
            var partToDelete = new Part { Id = "1", Title = "Test Part" };

            fixture.MockPartRepository.Setup(repo => repo.PartExist(partId)).Returns(true);
            fixture.MockPartRepository.Setup(repo => repo.GetPartById(partId)).Returns(partToDelete);
            fixture.MockPartRepository.Setup(repo => repo.DeletePart(partToDelete)).Returns(true);

            // Act
            var result = fixture.Controller.DeletePart(partId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Successfully deleted", okResult.Value);
        }

        [Fact]
        public void DeletePart_ShouldReturnNotFound_WhenPartDoesNotExist()
        {
            // Arrange
            var fixture = new PartControllerFixture();
            var partId = "1";

            fixture.MockPartRepository.Setup(repo => repo.PartExist(partId)).Returns(false);

            // Act
            var result = fixture.Controller.DeletePart(partId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void DeletePart_ShouldReturnBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var fixture = new PartControllerFixture();
            var partId = "1";
            var partToDelete = new Part { Id = "1", Title = "Test Part" };

            fixture.MockPartRepository.Setup(repo => repo.PartExist(partId)).Returns(true);
            fixture.MockPartRepository.Setup(repo => repo.GetPartById(partId)).Returns(partToDelete);
            fixture.Controller.ModelState.AddModelError("Title", "Required");

            // Act
            var result = fixture.Controller.DeletePart(partId);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void DeletePart_ShouldReturnStatusCode500_WhenDeletionFails()
        {
            // Arrange
            var fixture = new PartControllerFixture();
            var partId = "1";
            var partToDelete = new Part { Id = "1", Title = "Test Part" };

            fixture.MockPartRepository.Setup(repo => repo.PartExist(partId)).Returns(true);
            fixture.MockPartRepository.Setup(repo => repo.GetPartById(partId)).Returns(partToDelete);
            fixture.MockPartRepository.Setup(repo => repo.DeletePart(partToDelete)).Returns(false);

            // Act
            var result = fixture.Controller.DeletePart(partId);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result); // Check for ObjectResult
            Assert.Equal(500, statusCodeResult.StatusCode); // Check for status code 500
        }

    }
}
