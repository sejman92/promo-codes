using FluentValidation.Results;
using Moq;
using PromoCodes.Models;

namespace PromoCodes.UnitTests.Hubs;

[TestFixture]
public partial class DiscountHubTests
{
    [Test]
    public async Task GenerateCodes_ShouldProduceResponse_WhenRequestIsValid()
    {
        // Arrange
        var request = new GenerateRequest { Count = 2, Length = 8 };
        _generateRequestValidatorMock
            .Setup(v => v.ValidateAsync(request, default))
            .ReturnsAsync(new ValidationResult());

        // Act
        await _hub.GenerateCodes(request);

        // Assert
        _generateRequestValidatorMock.Verify(v => v.ValidateAsync(request, default), Times.Once);
        _discountClientMock.Verify(client => client.CodesGenerated(It.Is<GenerateResponse>(r => r.Result)), Times.Once);
    }
    
    [Test]
    public async Task GenerateCodes_ShouldLogAndRespondWithError_WhenRequestIsInvalid()
    {
        // Arrange
        var request = new GenerateRequest { Count = 2, Length = 8 };
        var validationResult = new ValidationResult(new[] { new ValidationFailure("Count", "Count must be positive.") });
        _generateRequestValidatorMock
            .Setup(v => v.ValidateAsync(request, default))
            .ReturnsAsync(validationResult);

        // Act
        await _hub.GenerateCodes(request);

        // Assert
        _loggerMock.Verify(logger => logger.LogErrors("GenerateCodes", validationResult.Errors), Times.Once);
        _discountClientMock.Verify(client => client.CodesGenerated(It.Is<GenerateResponse>(r => !r.Result)), Times.Once);
    }
}