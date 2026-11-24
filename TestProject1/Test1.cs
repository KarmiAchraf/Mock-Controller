using Microsoft.AspNetCore.Mvc;
using mock.depart.Controllers;
using mock.depart.Models;
using mock.depart.Services;
using mock.Exceptions;
using Moq;

namespace TestProject1
{
    [TestClass]
    public sealed class Test1
    {
        [TestMethod]
        public void DeleteCat_CatNotFound_ReturnsNotFound()
        {
            // Arrange
            Mock<CatsService> serviceMock = new Mock<CatsService>();
            serviceMock.Setup(s => s.Get(1)).Returns((Cat?)null);

            Mock<CatsController> controller = new Mock<CatsController>(serviceMock.Object) { CallBase = true };
            controller.Setup(c => c.UserId).Returns("user-1");

            // Act
            ActionResult<Cat> response = controller.Object.DeleteCat(1);

            // Assert
            Assert.IsInstanceOfType(response.Result, typeof(NotFoundResult));
        }

        [TestMethod]
        public void DeleteCat_NotOwner_ReturnsBadRequest()
        {
            // Arrange
            Cat cat = new Cat
            {
                Id = 1,
                Name = "Mittens",
                CuteLevel = Cuteness.BarelyOk,
                CatOwner = new CatOwner { Id = "user-2" }
            };

            Mock<CatsService> serviceMock = new Mock<CatsService>();
            serviceMock.Setup(s => s.Get(1)).Returns(cat);

            Mock<CatsController> controller = new Mock<CatsController>(serviceMock.Object) { CallBase = true };
            controller.Setup(c => c.UserId).Returns("user-1");

            // Act
            ActionResult<Cat> response = controller.Object.DeleteCat(1);

            // Assert
            Assert.IsInstanceOfType(response.Result, typeof(BadRequestObjectResult));
            BadRequestObjectResult badRequest = (BadRequestObjectResult)response.Result!;
            Assert.AreEqual("Cat is not yours", badRequest.Value);
        }

        [TestMethod]
        public void DeleteCat_CatTooCute_ReturnsBadRequest()
        {
            // Arrange
            Cat cat = new Cat
            {
                Id = 1,
                Name = "Fluffy",
                CuteLevel = Cuteness.Amazing,
                CatOwner = new CatOwner { Id = "user-1" }
            };

            Mock<CatsService> serviceMock = new Mock<CatsService>();
            serviceMock.Setup(s => s.Get(1)).Returns(cat);

            Mock<CatsController> controller = new Mock<CatsController>(serviceMock.Object) { CallBase = true };
            controller.Setup(c => c.UserId).Returns("user-1");

            // Act
            ActionResult<Cat> response = controller.Object.DeleteCat(1);

            // Assert
            Assert.IsInstanceOfType(response.Result, typeof(BadRequestObjectResult));
            BadRequestObjectResult badRequest = (BadRequestObjectResult)response.Result!;
            Assert.AreEqual("Cat is too cute", badRequest.Value);
        }

        [TestMethod]
        public void DeleteCat_ValidBarelyOkCat_ReturnsOkWithDeletedCat()
        {
            // Arrange
            Cat cat = new Cat
            {
                Id = 1,
                Name = "Garfield",
                CuteLevel = Cuteness.BarelyOk,
                CatOwner = new CatOwner { Id = "user-1" }
            };

            Mock<CatsService> serviceMock = new Mock<CatsService>();
            serviceMock.Setup(s => s.Get(1)).Returns(cat);
            serviceMock.Setup(s => s.Delete(1)).Returns(cat);

            Mock<CatsController> controller = new Mock<CatsController>(serviceMock.Object) { CallBase = true };
            controller.Setup(c => c.UserId).Returns("user-1");

            // Act
            ActionResult<Cat> response = controller.Object.DeleteCat(1);

            // Assert
            Assert.IsInstanceOfType(response.Result, typeof(OkObjectResult));
            OkObjectResult okResult = (OkObjectResult)response.Result!;
            Assert.AreEqual(cat, okResult.Value);
        }

        [TestMethod]
        public void DeleteCat_ThrowsCode18Exception_ReturnsUnauthorized()
        {
            // Arrange
            Mock<CatsService> serviceMock = new Mock<CatsService>();
            serviceMock.Setup(s => s.Get(1)).Throws(new Code18Exception());

            Mock<CatsController> controller = new Mock<CatsController>(serviceMock.Object) { CallBase = true };
            controller.Setup(c => c.UserId).Returns("user-1");

            // Act
            ActionResult<Cat> response = controller.Object.DeleteCat(1);

            // Assert
            Assert.IsInstanceOfType(response.Result, typeof(UnauthorizedResult));
        }

        [TestMethod]
        public void ChiffreChanceux_SequenceOfCalls_ReturnsExpectedValuesInOrder()
        {
            // Arrange
            Mock<CatsService> serviceMock = new Mock<CatsService>();
            serviceMock.SetupSequence(s => s.ChiffreChanceux())
                .Returns(33)
                .Returns(42)
                .Returns(67);

            Mock<CatsController> controller = new Mock<CatsController>(serviceMock.Object) { CallBase = true };

            // Act & Assert - 1er appel : 33
            ActionResult<int> response1 = controller.Object.ChiffreChanceux();
            Assert.IsInstanceOfType(response1.Result, typeof(OkObjectResult));
            OkObjectResult okResult1 = (OkObjectResult)response1.Result!;
            Assert.AreEqual(33, okResult1.Value);

            // Act & Assert - 2e appel : 42
            ActionResult<int> response2 = controller.Object.ChiffreChanceux();
            Assert.IsInstanceOfType(response2.Result, typeof(OkObjectResult));
            OkObjectResult okResult2 = (OkObjectResult)response2.Result!;
            Assert.AreEqual(42, okResult2.Value);

            // Act & Assert - 3e appel : 67
            ActionResult<int> response3 = controller.Object.ChiffreChanceux();
            Assert.IsInstanceOfType(response3.Result, typeof(OkObjectResult));
            OkObjectResult okResult3 = (OkObjectResult)response3.Result!;
            Assert.AreEqual(67, okResult3.Value);
        }
    }
}
