using System.Collections.Concurrent;
using System.Reflection;
using FluentValidation.Results;
using Moq;
using PromoCodes.Hubs;
using PromoCodes.Models;

namespace PromoCodes.UnitTests.Hubs;

[TestFixture]
public partial class DiscountHubTests
{
    [Test]
    public async Task UseCode_ShouldProduceSuccessResponse_WhenCodeIsFound()
    {
        // Arrange
        var request = new UseCodeRequest { Code = "VALIDCODE" };
        var discountCodesField = typeof(DiscountHub).GetField("DiscountCodes", BindingFlags.Static | BindingFlags.NonPublic);
        var discountCodes = (ConcurrentDictionary<string, Discount>)discountCodesField.GetValue(null);
        discountCodes.TryAdd("VALIDCODE", new Discount { Code = "VALIDCODE" });

        _useCodeRequestValidatorMock
            .Setup(v => v.ValidateAsync(request, default))
            .ReturnsAsync(new ValidationResult());

        // Act
        await _hub.UseCode(request);

        // Assert
        Assert.IsFalse(discountCodes.ContainsKey("VALIDCODE"));
        _discountClientMock.Verify(client => client.CodeUsed(It.Is<UseCodeResponse>(r => r.Result == (byte)UseCodeResponseType.Success)), Times.Once);
    }

    [Test]
    public async Task UseCode_ShouldProduceErrorResponse_WhenCodeIsNotFound()
    {
        // Arrange
        var request = new UseCodeRequest { Code = "INVALIDCODE" };
        _useCodeRequestValidatorMock
            .Setup(v => v.ValidateAsync(request, default))
            .ReturnsAsync(new ValidationResult());

        // Act
        await _hub.UseCode(request);

        // Assert
        _discountClientMock.Verify(client => client.CodeUsed(It.Is<UseCodeResponse>(r => r.Result == (byte)UseCodeResponseType.CodeNotFound)), Times.Once);
    }
}