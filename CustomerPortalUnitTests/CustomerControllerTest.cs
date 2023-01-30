using CustomerPortal.Controllers;
using CustomerPortal.Models;
using CustomerPortal.Models.ViewModels;
using CustomerPortalUnitTests.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CustomerPortalUnitTests.Utils;

namespace CustomerPortalUnitTests;

public class CustomerControllerTests : BackendTest
{
    
    private readonly CustomerController _controller;
    
    public CustomerControllerTests()
    {
        _controller = Container.ResolveControllerWithSeededData<CustomerController>();
    }
    
    [Theory]
    [InlineData(2100)]
    [InlineData(2200)]
    public async Task Index(int customerID)
    {
        // Arrange
        _controller.HttpContext.Session.SetInt32(nameof(Customer.CustomerID),customerID);
        
        
        // Act
        var result = await _controller.Index();
        
        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<Customer>(viewResult.ViewData.Model);
        Assert.NotNull(model);
    }
    
    [Theory]
    [InlineData(2100)]
    [InlineData(2200)]
    [InlineData(2300)]
    public async Task Profile(int customerID)
    {
        // Arrange
        _controller.HttpContext.Session.SetInt32(nameof(Customer.CustomerID),customerID);

        // Act
        var result = await _controller.Profile();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        
        var model = Assert.IsAssignableFrom<Customer>(viewResult.ViewData.Model);
        
        Assert.NotNull(model);
        
    }
    
    [Theory]
    [InlineData(2100,"abc123","abc123","abc124")]
    public async Task NewPassword_Invalid_Input(int customerID,string oldPassword,string newPassword,string confirmPassword)
    {
        // Arrange
        var customer = await _controller.FindCustomer(customerID);
        
        var login = customer.Login;

        var viewModel = new LoginViewModel()
        {
            ConfirmPassword = confirmPassword,
            LoginID = login.LoginID,
            NewPassword = newPassword,
            OldPassword = oldPassword,
            PasswordHash = login.PasswordHash
        };
        
        
        _controller.ModelState.AddModelError("fakeError", "fakeError");

        // Act
        var result = await _controller.NewPassword(viewModel);
        
        // Assert
        Assert.IsType<ViewResult>(result);
        
    }
    
    [Theory]
    [InlineData(2100,"abc123","password","password")]
    public async Task NewPassword(int customerID,string oldPassword,string newPassword,string confirmPassword)
    {
        // Arrange
        var customer = await _controller.FindCustomer(customerID);
        
        var login = customer.Login;

        var viewModel = new LoginViewModel()
        {
            ConfirmPassword = confirmPassword,
            LoginID = login.LoginID,
            NewPassword = newPassword,
            OldPassword = oldPassword,
            PasswordHash = login.PasswordHash
        };
        
        // Act
        var result = await _controller.NewPassword(viewModel);
        
        // Assert
        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        
        Assert.Null(redirectToActionResult.ControllerName);
        
        Assert.Equal("Profile", redirectToActionResult.ActionName);
        
    }
    
    [Theory]
    [InlineData(2300)]
    [InlineData(2200)]
    [InlineData(2100)]
    public async Task NewPassword_Valid_CustomerID(int customerID)
    {
        // Arrange
        _controller.HttpContext.Session.SetInt32(nameof(Customer.CustomerID),customerID);
        
        // Act
        var result = await _controller.NewPassword();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<LoginViewModel>(viewResult.ViewData.Model);
        Assert.NotNull(model);
    }
    
    [Theory]
    [InlineData(310)]
    [InlineData(0)]
    public async Task NewPasswordView_Invalid_CustomerID(int customerID)
    {
        // Arrange
        _controller.HttpContext.Session.SetInt32(nameof(Customer.CustomerID),customerID);
        
        // Act/Assert
        await Assert.ThrowsAsync<NullReferenceException>(()=> _controller.NewPassword());
        
    }
    
    [Theory]
    [InlineData(2300)]
    public async Task UpdateProfile_Valid_CustomerID(int customerID)
    {
        // Arrange
        _controller.HttpContext.Session.SetInt32(nameof(Customer.CustomerID),customerID);
        
        // Act
        var result = await _controller.UpdateProfile();
        
        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        
        var model = Assert.IsAssignableFrom<Customer>(viewResult.ViewData.Model);
        
        Assert.NotNull(model);
    }
}