using Moq;

namespace PromoCodes.UnitTests.Hubs;

[TestFixture]
public partial class DiscountHubTests
{
    [Test]
    public async Task OnConnectedAsync_ShouldLogMessage_AndLoadCodes()
    {
        // Arrange
        _contextMock.Setup(ctx => ctx.ConnectionId).Returns("123");

        // Act
        await _hub.OnConnectedAsync();

        // Assert
        _loggerMock.Verify(logger => logger.Log("User 123 connected."), Times.Once);
    }
}