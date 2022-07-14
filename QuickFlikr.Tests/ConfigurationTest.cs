using Moq;
using QuickFlikr.Service.Contract;
using System;
using Xunit;

namespace QuickFlikr.Tests
{
    public class ConfigurationTest
    {
        [Fact]
        public void Should_SetServiceUri()
        {
            // Arrange
            var uri = new Uri("https://www.flickr.com/", UriKind.Absolute);
            var appConfiguration = new Mock<IAppConfiguration>() { DefaultValue = DefaultValue.Mock };
            appConfiguration.SetupGet(x => x.FlikrServiceUrl).Returns(uri);

            // Asserts
            Assert.Equal(appConfiguration.Object.FlikrServiceUrl, uri);
        }

        [Fact]
        public void Should_ThrowException_WhenConfigurationNull()
        {
            // Arrange
            var appConfiguration = new Mock<IAppConfiguration>() { DefaultValue = DefaultValue.Mock };

            // Asserts
            Assert.Throws<ArgumentException>(() =>
            {
                return appConfiguration.Object.FlikrServiceUrl;
            });
        }
    }
}