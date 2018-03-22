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
        String testRootDir = "../../../uploadTest";

        public AdminGalleryControllerTests()
        {
            var mockEnvironment = new Mock<IHostingEnvironment>();
            mockEnvironment.Setup(m => m.WebRootPath).Returns(testRootDir);

            _controller = new GalleryController(mockEnvironment.Object);
		}

		[Fact(DisplayName = "Gallery upload normal PNG")]
		public async Task Upload_Something_Ok()
		{
			// arrange
			var testFile = new Mock<IFormFile>();
			var sourceFile = File.OpenRead(testRootDir + "/test.png");

			var memStream = new MemoryStream();
			var writer = new StreamWriter(memStream);
			writer.Write(sourceFile);
			writer.Flush();
			memStream.Position = 0;

			var fileName = sourceFile.Name;
			var fileType = "image/png";
			long fileSize = 500_000;

			testFile.Setup(f => f.Length).Returns(fileSize);
			testFile.Setup(f => f.ContentType).Returns(fileType);
			testFile.Setup(f => f.FileName).Returns(fileName);
			testFile.Setup(_ => _.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
				.Returns((Stream stream, CancellationToken token) => memStream.CopyToAsync(stream));

			// act
			var inputFile = testFile.Object;
			var result = await _controller.Upload(inputFile);

			// assert
			var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal("Index", redirectToActionResult.ActionName);
		}

		[Fact(DisplayName = "Gallery upload too large")]
		public async Task Upload_Too_Large()
		{
			// arrange
			var testFile = new Mock<IFormFile>();
			var sourceFile = File.OpenRead(testRootDir + "/test.png");

			var memStream = new MemoryStream();
			var writer = new StreamWriter(memStream);
			writer.Write(sourceFile);
			writer.Flush();
			memStream.Position = 0;

			var fileName = sourceFile.Name;
			var fileType = "image/png";
			long fileSize = 520_000;

			testFile.Setup(f => f.Length).Returns(fileSize);
			testFile.Setup(f => f.ContentType).Returns(fileType);
			testFile.Setup(f => f.FileName).Returns(fileName);
			testFile.Setup(_ => _.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
				.Returns((Stream stream, CancellationToken token) => memStream.CopyToAsync(stream));

			// act
			var inputFile = testFile.Object;
			var result = await _controller.Upload(inputFile);

			// assert
			var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal("Index", redirectToActionResult.ActionName);
		}

		[Fact(DisplayName = "Gallery upload not an image")]
		public async Task Upload_Not_Image()
		{
			// arrange
			var testFile = new Mock<IFormFile>();
			var sourceFile = File.OpenRead(testRootDir + "/test.txt");

			var memStream = new MemoryStream();
			var writer = new StreamWriter(memStream);
			writer.Write(sourceFile);
			writer.Flush();
			memStream.Position = 0;

			var fileName = sourceFile.Name;
			var fileType = "text/plain";
			long fileSize = 5_000;

			testFile.Setup(f => f.Length).Returns(fileSize);
			testFile.Setup(f => f.ContentType).Returns(fileType);
			testFile.Setup(f => f.FileName).Returns(fileName);
			testFile.Setup(_ => _.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
				.Returns((Stream stream, CancellationToken token) => memStream.CopyToAsync(stream));

			// act
			var inputFile = testFile.Object;
			var result = await _controller.Upload(inputFile);

			// assert
			var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal("Index", redirectToActionResult.ActionName);
		}
	}
}
