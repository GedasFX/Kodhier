using Kodhier.Areas.Admin.Controllers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Kodhier.Tests
{
    public class AdminGalleryControllerTests
    {
        GalleryController _controller;

        public AdminGalleryControllerTests()
        {
            var mockEnvironment = new Mock<IHostingEnvironment>();
            mockEnvironment.Setup(m => m.EnvironmentName).Returns("Hosting:UnitTestEnvironment");

            _controller = new GalleryController(mockEnvironment.Object);
        }
        
        [Fact(DisplayName = "Create method add order to db with correct data")]
        public async Task Upload_Something_Ok()
        {
            // arrange
            var testFile = new Mock<IFormFile>();
            var sourceFile = File.OpenRead(@"source image path");

            var memStream = new MemoryStream();
            var writer = new StreamWriter(memStream);
            writer.Write(sourceFile);
            writer.Flush();
            memStream.Position = 0;

            var fileName = "QQ.png"; // sourceImg.Name
            var fileType = "image/png";
            long fileSize = 100;
            testFile.Setup(f => f.Length).Returns(fileSize).Verifiable();
            testFile.Setup(f => f.ContentType).Returns(fileType).Verifiable();
            testFile.Setup(f => f.FileName).Returns(fileName).Verifiable();
            testFile.Setup(_ => _.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                .Returns((Stream stream, CancellationToken token) => memStream.CopyToAsync(stream))
                .Verifiable();

            // act
            var inputFile = testFile.Object;
            var result = await _controller.Upload(inputFile);

            // assert
            testFile.Verify();
			
            //Assert.IsType<RedirectToActionResult>(result);
            //Assert.Equal("Index", ((RedirectToActionResult)result).ActionName);
			
			var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal("Index", redirectToActionResult.ActionName);
        }
    }
}
