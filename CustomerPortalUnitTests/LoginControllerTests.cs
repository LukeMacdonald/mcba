using A2Practice.Controllers;
using CustomerPortalUnitTests.Base;
using CustomerPortalUnitTests.Utils;
using Microsoft.AspNetCore.Mvc;

namespace CustomerPortalUnitTests;

public class LoginControllerTests : BackendTest
{
    private readonly LoginController _controller;
    
    public LoginControllerTests()
    {
        _controller = Container.ResolveControllerWithSeededData<LoginController>();
    }

    [Fact]
    public async Task Index()
    {
        // Act
        var result = _controller.Index() as ViewResult;

        // Assert
        Assert.NotNull(result);
    }

    [Theory]
    [InlineData("abc123", "12345678")]
    public async Task Index_With_Parameters_Successful_Login(string password, string loginId)
    {
        // Act
        var result = await _controller.Index(loginId, password);

        // Assert
        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);

        Assert.Equal("Customer", redirectToActionResult.ControllerName);

        Assert.Equal("Index", redirectToActionResult.ActionName);
    }

    [Theory]
    [InlineData("abc123", "1234567")]
    public async Task Index_With_Parameters_Unsuccessful_Login(string password, string loginId)
    {
        // Act
        var result = await _controller.Index(loginId, password);
        
        // Assert
        Assert.NotNull(result);

    }

    [Fact]
    public async Task Logout()
    {
        // Act
        var result = _controller.Logout();

        // Assert
        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);

        Assert.Equal("Home", redirectToActionResult.ControllerName);

        Assert.Equal("Index", redirectToActionResult.ActionName);
    }
}