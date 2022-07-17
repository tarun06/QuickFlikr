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
            var flikrFeedService = new Mock<IFlikrFeedService>() { DefaultValue = DefaultValue.Mock };
            flikrFeedService.SetupAllProperties();
            flikrFeedService.Setup(x => x.GetFlikrFeedAsync(It.IsAny<string>(), cts.Token)).ReturnsAsync(Array.Empty<FeedInfo>());

            // Act
            var vm = new QuickFlikrViewModel(flikrFeedService.Object);
            cts.Cancel();

            // Asserts
            Assert.ThrowsAsync<OperationCanceledException>(() =>
            {
                vm.SearchCommand.Execute(null);
                return Task.CompletedTask;
            });
        }

        [Theory]
        [InlineData("cat", 2)]
        [InlineData("doll", 1)]
        public async void Should_Get_EqualFeed(string searchText, int count)
        {
            var feeds = DummyFeedGenerator.GeneratorFeedInfo(searchText).ToList();

            // Arrange
            var flikrFeedService = new Mock<IFlikrFeedService>() { DefaultValue = DefaultValue.Mock };
            flikrFeedService.SetupAllProperties();
            flikrFeedService.Setup(x => x.GetFlikrFeedAsync(It.IsAny<string>(), CancellationToken.None)).ReturnsAsync(feeds);

            // Act
            var result = await flikrFeedService.Object.GetFlikrFeedAsync(searchText);

            // Asserts
            Assert.Equal(feeds.Count, count);
            Assert.Equal(feeds, result);
        }

        [Fact]
        public void Should_GetFlikrFeed()
        {
            var feeds = DummyFeedGenerator.GeneratorFeedInfo().ToList();

            // Arrange
            var flikrFeedService = new Mock<IFlikrFeedService>() { DefaultValue = DefaultValue.Mock };
            flikrFeedService.SetupAllProperties();
            flikrFeedService.Setup(x => x.GetFlikrFeedAsync(It.IsAny<string>(), CancellationToken.None)).ReturnsAsync(feeds);

            // Act
            var vm = new QuickFlikrViewModel(flikrFeedService.Object);
            vm.SearchCommand.Execute("cat");

            // Asserts
            Assert.Equal(feeds.Count, vm.Photos.Count());
            var isEqual = vm.Photos.ToList().SequenceEqual(feeds.Select(x => x.Media.Path));
            Assert.True(isEqual);
        }

        [Fact]
        public void Should_Throw_ArgumentNullException()
        {
            // Arrange
            var flikrFeedService = new Mock<IFlikrFeedService>() { DefaultValue = DefaultValue.Mock };
            flikrFeedService.SetupAllProperties();
            flikrFeedService.Setup(x => x.GetFlikrFeedAsync(It.IsAny<string>(), CancellationToken.None)).ReturnsAsync(Array.Empty<FeedInfo>());

            // Act
            var vm = new QuickFlikrViewModel(flikrFeedService.Object);

            // Asserts
            Assert.ThrowsAsync<ArgumentNullException>(() =>
            {
                vm.SearchCommand.Execute(null);
                return Task.CompletedTask;
            });
        }

        [Fact]
        public void Should_ThrowException_WhenServiceNull()
        {
            // Asserts
            Assert.Throws<ArgumentNullException>(() =>
            {
                var vm = new QuickFlikrViewModel(null);
                return vm;
            });
        }
    }
}