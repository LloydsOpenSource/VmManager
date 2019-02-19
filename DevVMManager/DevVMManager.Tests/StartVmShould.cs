namespace DevVMManager.Tests
{
    using Management;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.Management.Compute.Fluent;
    using Microsoft.Azure.Management.Fluent;
    using Microsoft.Extensions.Logging;
    using Moq;
    using System.IO;
    using Xunit;

    public class StartVmShould
    {
        private readonly Mock<IAzure> _mockAzure = new Mock<IAzure>();
        private readonly Mock<IVirtualMachines> _mockVms = new Mock<IVirtualMachines>();
        private readonly Mock<IVirtualMachine> _mockVm = new Mock<IVirtualMachine>();
        private readonly Mock<ILogger> _logMock = new Mock<ILogger>();
        private readonly Mock<HttpRequest> _httpRequestMock = new Mock<HttpRequest>();

        [Fact]
        public void ReturnBadRequestIfNoVmNameIsProvided()
        {
            // Arrange
            _httpRequestMock.Setup(x => x.Body).Returns(new MemoryStream());
            _httpRequestMock.Setup(x => x.Query["groupName"]).Returns("TestGroup");

            _mockVms.Setup(x => x.GetByResourceGroup(It.IsAny<string>(), It.IsAny<string>())).Returns(_mockVm.Object);
            _mockAzure.Setup(x => x.VirtualMachines).Returns(_mockVms.Object);
            var sut = new VmManager(_mockAzure.Object);

            // Act
            var expected = new BadRequestObjectResult("Please pass a vmName and groupName on the query string or in the request body");
            var expectedStatus = expected.StatusCode;

            var actual = sut.StartVm(_httpRequestMock.Object, _logMock.Object).Result as BadRequestObjectResult;
            Assert.NotNull(actual);
            var actualStatus = actual.StatusCode;

            // Assert
            Assert.Equal(expectedStatus, actualStatus);
        }

        [Fact]
        public void ReturnBadRequestIfNoGroupNameIsProvided()
        {
            // Arrange
            _httpRequestMock.Setup(x => x.Body).Returns(new MemoryStream());
            _httpRequestMock.Setup(x => x.Query["vmName"]).Returns("TestVm");

            _mockVms.Setup(x => x.GetByResourceGroup(It.IsAny<string>(), It.IsAny<string>())).Returns(_mockVm.Object);
            _mockAzure.Setup(x => x.VirtualMachines).Returns(_mockVms.Object);
            var sut = new VmManager(_mockAzure.Object);

            // Act
            var expected = new BadRequestObjectResult("Please pass a vmName and groupName on the query string or in the request body");
            var expectedStatus = expected.StatusCode;

            var actual = sut.StartVm(_httpRequestMock.Object, _logMock.Object).Result as BadRequestObjectResult;
            Assert.NotNull(actual);
            var actualStatus = actual.StatusCode;

            // Assert
            Assert.Equal(expectedStatus, actualStatus);
        }
    }
}
