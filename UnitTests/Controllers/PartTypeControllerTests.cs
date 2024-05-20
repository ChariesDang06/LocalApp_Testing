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
    public class PartTypeControllerFixture
    {
        public Mock<IPartTypeRepository> MockPartTypeRepository { get; }
        public Mock<IPartRepository> MockPartRepository { get; }
        public Mock<IMapper> MockMapper { get; }
        public PartTypeController Controller { get; }

        public PartTypeControllerFixture()
        {
            MockPartTypeRepository = new Mock<IPartTypeRepository>();
            MockPartRepository = new Mock<IPartRepository>();
            MockMapper = new Mock<IMapper>();
            Controller = new PartTypeController(MockPartTypeRepository.Object, MockPartRepository.Object, MockMapper.Object);
        }
    }

    public class PartTypeControllerTests
    {
        private readonly PartTypeControllerFixture _fixture;

        public PartTypeControllerTests()
        {
            _fixture = new PartTypeControllerFixture();
        }

        [Fact]
        public void GetPartTypes_ShouldReturnOkWithListOfPartTypeDto_WhenPartTypesExist()
        {
            // Arrange
            var expectedPartTypes = new List<PartType>() { new PartType() { Id = "1", Content = "Test PartType" } };
            _fixture.MockPartTypeRepository.Setup(repo => repo.GetPartTypes()).Returns(expectedPartTypes);
            var expectedDto = new List<PartTypeDto>() { new PartTypeDto() { Id = "1", Content = "Test PartType" } };
            _fixture.MockMapper.Setup(m => m.Map<List<PartTypeDto>>(It.IsAny<List<PartType>>())).Returns(expectedDto);

            // Act
            var result = _fixture.Controller.GetPartTypes();

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var objectResult = (OkObjectResult)result;
            Assert.Equal(expectedDto, objectResult.Value);
        }

        [Fact]
        public void GetPartType_ShouldReturnOkWithPartTypeDto_WhenPartTypeExists()
        {
            // Arrange
            var partTypeId = "1";
            var expectedPartType = new PartType() { Id = "1", Content = "Test PartType" };
            _fixture.MockPartTypeRepository.Setup(repo => repo.PartTypeExist(partTypeId)).Returns(true);
            _fixture.MockPartTypeRepository.Setup(repo => repo.GetPartTypeById(partTypeId)).Returns(expectedPartType);
            var expectedDto = new PartTypeDto() { Id = "1", Content = "Test PartType" };
            _fixture.MockMapper.Setup(m => m.Map<PartTypeDto>(It.IsAny<PartType>())).Returns(expectedDto);

            // Act
            var result = _fixture.Controller.GetPartType(partTypeId);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var objectResult = (OkObjectResult)result;
            Assert.Equal(expectedDto, objectResult.Value);
        }

        [Fact]
        public void GetPartType_ShouldReturnNotFound_WhenPartTypeDoesNotExist()
        {
            // Arrange
            var partTypeId = "1";
            _fixture.MockPartTypeRepository.Setup(repo => repo.PartTypeExist(partTypeId)).Returns(false);

            // Act
            var result = _fixture.Controller.GetPartType(partTypeId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
        [Fact]
        public void CreatePartType_ShouldReturnOk_WhenPartTypeIsCreatedSuccessfully()
        {
            // Arrange
            var partId = "1";
            var newPartType = new PartTypeDto() { Id = "1", Content = "Test PartType" };
            var mappedPartType = new PartType() { Id = "1", Content = "Test PartType" };

            _fixture.MockPartTypeRepository.Setup(repo => repo.GetPartTypes()).Returns(new List<PartType>());
            _fixture.MockPartTypeRepository.Setup(repo => repo.CreatePartType(It.IsAny<PartType>())).Returns(true);
            _fixture.MockPartRepository.Setup(repo => repo.GetPartById(partId)).Returns(new Part());

            _fixture.MockMapper.Setup(m => m.Map<PartType>(It.IsAny<PartTypeDto>())).Returns(mappedPartType);

            // Act
            var result = _fixture.Controller.CreatePartType(partId, newPartType);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Successfully created", ((OkObjectResult)result).Value);
        }

        [Fact]
        public void CreatePartType_ShouldReturnBadRequest_WhenPartTypeDtoIsNull()
        {
            // Arrange
            var partId = "1";
            PartTypeDto newPartType = null;

            // Act
            var result = _fixture.Controller.CreatePartType(partId, newPartType);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            var badRequestObjectResult = (BadRequestObjectResult)result;
            Assert.Empty((Dictionary<string, object>)badRequestObjectResult.Value);

        }

        [Fact]
        public void CreatePartType_ShouldReturnStatusCode422_WhenPartTypeAlreadyExists()
        {
            // Arrange
            var partId = "1";
            var existingPartType = new PartTypeDto() { Id = "1", Content = "Existing PartType" };
            _fixture.MockPartTypeRepository.Setup(repo => repo.GetPartTypes()).Returns(new List<PartType> { new PartType() { Id = "1", Content = "Existing PartType" } });

            // Act
            var result = _fixture.Controller.CreatePartType(partId, existingPartType);

            // Assert
            Assert.IsType<ObjectResult>(result);
            var objectResult = (ObjectResult)result;
            Assert.Equal(422, objectResult.StatusCode);
        }

        [Fact]
        public void DeletePartType_ShouldReturnNoContent_WhenPartTypeExists()
        {
            // Arrange
            var partTypeId = "1";
            var expectedPartType = new PartType() { Id = "1", Content = "Test PartType" };
            _fixture.MockPartTypeRepository.Setup(repo => repo.PartTypeExist(partTypeId)).Returns(true);
            _fixture.MockPartTypeRepository.Setup(repo => repo.GetPartTypeById(partTypeId)).Returns(expectedPartType);
            _fixture.MockPartTypeRepository.Setup(repo => repo.DeletePartType(expectedPartType)).Returns(true);

            // Act
            var result = _fixture.Controller.DeletePartType(partTypeId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void DeletePartType_ShouldReturnNotFound_WhenPartTypeDoesNotExist()
        {
            // Arrange
            var partTypeId = "1";
            _fixture.MockPartTypeRepository.Setup(repo => repo.PartTypeExist(partTypeId)).Returns(false);

            // Act
            var result = _fixture.Controller.DeletePartType(partTypeId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void DeletePartType_ShouldReturnBadRequest_WhenModelStateIsNotValid()
        {
            /// Arrange
            var partTypeId = "1";
            _fixture.Controller.ModelState.AddModelError("key", "error");

            // Act
            var result = _fixture.Controller.DeletePartType(partTypeId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void DeletePartType_ShouldReturnNoContent_WhenDeletionFails()
        {
            // Arrange
            var partTypeId = "1";
            var expectedPartType = new PartType() { Id = "1", Content = "Test PartType" };
            _fixture.MockPartTypeRepository.Setup(repo => repo.PartTypeExist(partTypeId)).Returns(true);
            _fixture.MockPartTypeRepository.Setup(repo => repo.GetPartTypeById(partTypeId)).Returns(expectedPartType);
            _fixture.MockPartTypeRepository.Setup(repo => repo.DeletePartType(expectedPartType)).Returns(false);

            // Act
            var result = _fixture.Controller.DeletePartType(partTypeId);

            // Assert
            Assert.IsType<ObjectResult>(result);
            var objectResult = (ObjectResult)result;
            Assert.Equal(500, objectResult.StatusCode);
        }



    }
}
