using Moq;
using QuickFlikr.Model;
using QuickFlikr.Service;
using QuickFlikr.WinApp;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace QuickFlikr.Tests
{
    public class QuickFlikrServiceTests
    {
        [Fact]
        public void Should_CancelledTask()
        {
            // Arrange
            var cts = new CancellationTokenSource();
            var flickrFeedService = new Mock<IFlickrFeedService>() { DefaultValue = DefaultValue.Mock };
            flickrFeedService.SetupAllProperties();
            flickrFeedService.Setup(x => x.GetFlickrFeedAsync(It.IsAny<string>(), cts.Token)).ReturnsAsync(Array.Empty<FeedInfo>());

            // Act
            var vm = new QuickFlikrViewModel(flickrFeedService.Object);
            cts.Cancel();

            // Asserts
            Assert.ThrowsAsync<OperationCanceledException>(() =>
            {
                vm.SearchCommand.Execute(null);
                return Task.CompletedTask;
            });
        }

        [Fact]
        public async void Should_Get_EqualFeed()
        {
            var feeds = DummyFeedGenerator.GeneratorFeedInfo().ToList();

            // Arrange
            var flickrFeedService = new Mock<IFlickrFeedService>() { DefaultValue = DefaultValue.Mock };
            flickrFeedService.SetupAllProperties();
            flickrFeedService.Setup(x => x.GetFlickrFeedAsync(It.IsAny<string>(), CancellationToken.None)).ReturnsAsync(feeds);

            // Act
            var result = await flickrFeedService.Object.GetFlickrFeedAsync("cat");

            // Asserts
            Assert.Equal(3, result.Count());
            Assert.Equal(feeds, result);
        }

        [Fact]
        public void Should_GetFlikrFeed()
        {
            var feeds = DummyFeedGenerator.GeneratorFeedInfo().ToList();

            // Arrange
            var flickrFeedService = new Mock<IFlickrFeedService>() { DefaultValue = DefaultValue.Mock };
            flickrFeedService.SetupAllProperties();
            flickrFeedService.Setup(x => x.GetFlickrFeedAsync(It.IsAny<string>(), CancellationToken.None)).ReturnsAsync(feeds);

            // Act
            var vm = new QuickFlikrViewModel(flickrFeedService.Object);
            vm.SearchCommand.Execute("cat");

            // Asserts
            Assert.Equal(3, vm.Photos.Count());
            var isEqual = vm.Photos.ToList().SequenceEqual(feeds.Select(x => x.Media.Path));
            Assert.True(isEqual);
        }

        [Fact]
        public void Should_Throw_ArgumentNullException()
        {
            // Arrange
            var flickrFeedService = new Mock<IFlickrFeedService>() { DefaultValue = DefaultValue.Mock };
            flickrFeedService.SetupAllProperties();
            flickrFeedService.Setup(x => x.GetFlickrFeedAsync(It.IsAny<string>(), CancellationToken.None)).ReturnsAsync(Array.Empty<FeedInfo>());

            // Act
            var vm = new QuickFlikrViewModel(flickrFeedService.Object);

            // Asserts
            Assert.ThrowsAsync<ArgumentNullException>(() =>
            {
                vm.SearchCommand.Execute(null);
                return Task.CompletedTask;
            });
        }
    }
}