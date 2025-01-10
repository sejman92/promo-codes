using FluentValidation;
using Microsoft.AspNetCore.SignalR;
using Moq;
using PromoCodes.Clients;
using PromoCodes.Hubs;
using PromoCodes.Models;
using PromoCodes.Utils;

namespace PromoCodes.UnitTests.Hubs;

[TestFixture]
public partial class DiscountHubTests
{
    private Mock<IValidator<GenerateRequest>> _generateRequestValidatorMock;
    private Mock<IValidator<UseCodeRequest>> _useCodeRequestValidatorMock;
    private Mock<ILogger> _loggerMock;
    private Mock<IDiscountClient> _discountClientMock;
    private Mock<IHubCallerClients<IDiscountClient>> _clientsMock;
    private Mock<HubCallerContext> _contextMock;
    private DiscountHub _hub;

    [SetUp]
    public void Setup()
    {
        _loggerMock = new Mock<ILogger>();
        _generateRequestValidatorMock = new Mock<IValidator<GenerateRequest>>();
        _useCodeRequestValidatorMock = new Mock<IValidator<UseCodeRequest>>();
        _discountClientMock = new Mock<IDiscountClient>();
        _clientsMock = new Mock<IHubCallerClients<IDiscountClient>>();
        _contextMock = new Mock<HubCallerContext>();

        _clientsMock.Setup(clients => clients.Client(It.IsAny<string>())).Returns(_discountClientMock.Object);

        _hub = new DiscountHub(
            _loggerMock.Object,
            _generateRequestValidatorMock.Object,
            _useCodeRequestValidatorMock.Object)
        {
            Clients = _clientsMock.Object,
            Context = _contextMock.Object
        };
    }

    [TearDown]
    public void TearDown()
    {
        _hub?.Dispose();
    }
}