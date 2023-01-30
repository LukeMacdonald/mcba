using CustomerPortal.Controllers;
using CustomerPortal.Models;
using CustomerPortal.Models.ViewModels;
using CustomerPortalUnitTests.Base;
using CustomerPortalUnitTests.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;

namespace CustomerPortalUnitTests;

public class BillPayControllerTests : BackendTest
{
    
    private readonly BillPayController _controller;
    
    public BillPayControllerTests()
    {
        _controller = Container.ResolveControllerWithSeededData<BillPayController>();
    }
    
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public async Task PayBill_Payment_Successful(int billID)
    {
        // Act
        var result = await _controller.PayBill(billID);
        
        // Assert
        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("ViewBills", redirectToActionResult.ActionName);

    }
    
    [Theory]
    [InlineData(4)]
    [InlineData(6)]
    [InlineData(8)]
    public async Task PayBill_Payment_Fail(int billID)
    {
        // Act
        var result = await _controller.PayBill(billID);
        
        // Assert
        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        
        Assert.Equal(nameof(_controller.UnpaidBills), redirectToActionResult.ActionName);
    }
    
    [Theory]
    [InlineData(20)]
    [InlineData(-1)]
    [InlineData(0)]
    public async Task PayBill_Payment_Null_BillID(int billID)
    {
        // Act
        var result = await _controller.PayBill(billID);
        
        // Assert
        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        
        Assert.Equal("Error", redirectToActionResult.ActionName);
        Assert.Equal("Home",redirectToActionResult.ControllerName);
    }
    
    
    [Theory]
    [InlineData(2200,1)]
    [InlineData(2300,2)]
    [InlineData(2300,3)]
    [InlineData(2100,6)]
    public async Task EditBill(int customerID,int billID)
    {
        // Arrange
        _controller.HttpContext.Session.SetInt32(nameof(Customer.CustomerID),customerID);
        
        var customer = await _controller.FindCustomer(customerID);
        
        var bill = await _controller.FindBillPay(billID);
        
        var viewModel = new BillViewModel()
        {
            BillPayID = billID,
            ScheduledTime = bill.ScheduleTimeUtc.ToLocalTime(),
            Amount = bill.Amount,
            Accounts = customer.Accounts,
            AccountNumber = bill.AccountNumber
        };
        
        // Act
        var result = await _controller.EditBill(billID);
        
        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<BillViewModel>(viewResult.ViewData.Model);
        Assert.NotNull(model);
        Assert.Equal(viewModel,model);
        
    }
    
    [Theory]
    [InlineData(2300,1)]
    [InlineData(2200,2)]
    [InlineData(2100,3)]
    public async Task EditBill_IncorrectAccount_To_BillID(int customerID,int billID)
    {
        // Arrange
        _controller.HttpContext.Session.SetInt32(nameof(Customer.CustomerID),customerID);
        
        // Act
        var result = await _controller.PayBill(billID);
        
        // Assert
        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        
        Assert.Equal(nameof(_controller.ViewBills), redirectToActionResult.ActionName);
    }
    
    [Theory]
    [InlineData(4200,2)]
    [InlineData(4101,1)]
    [InlineData(4300,0)]
    public async Task UnpaidBill(int accountNumber,int expected)
    {
        // Arrange
        _controller.HttpContext.Session.SetInt32("ViewBills",accountNumber);
        
        // Act
        var result = await _controller.UnpaidBills();
        
        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        
        var model = Assert.IsAssignableFrom<IPagedList<BillPay>>(viewResult.ViewData.Model);
        
        Assert.Equal(expected,model.TotalItemCount);
        
    }
    [Theory]
    [InlineData(4200,2200,0)]
    [InlineData(4101,2100,0)]
    [InlineData(4300,2300,3)]
    [InlineData(4100,2100,4)]
    public async Task ViewBill(int accountNumber,int customerID, int expected)
    {
        // Arrange
        _controller.HttpContext.Session.SetInt32(nameof(Customer.CustomerID),customerID);
        _controller.HttpContext.Session.SetInt32("ViewBills",accountNumber);
        
        // Act
        var result = await _controller.ViewBills();
        
        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        
        var model = Assert.IsAssignableFrom<IPagedList<BillPay>>(viewResult.ViewData.Model);
        
        Assert.Equal(expected,model.TotalItemCount);
        
    }
    [Theory]
    [InlineData(4100,2200,0)]
    [InlineData(4200,2100,0)]
    [InlineData(4101,2300,3)]
    [InlineData(4200,2300,4)]
    public async Task ViewBill_Invalid_Account_Number(int accountNumber,int customerID, int expected)
    {
        // Arrange
        _controller.HttpContext.Session.SetInt32(nameof(Customer.CustomerID),customerID);
        
        _controller.HttpContext.Session.SetInt32("ViewBills",accountNumber);
        
        // Act
        var result = await _controller.ViewBills();

        // Assert
        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);

        Assert.Equal("Customer", redirectToActionResult.ControllerName);

        Assert.Equal("Index", redirectToActionResult.ActionName);
        
    }
    
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public async Task CancelBill_Payment_Fail(int billID)
    {
        // Act
        var result = await _controller.CancelBill(billID);
        
        //Assert
        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        
        Assert.Equal(nameof(_controller.ViewBills), redirectToActionResult.ActionName);
    }
    
    [Theory]
    [InlineData(20)]
    [InlineData(-1)]
    [InlineData(0)]
    public async Task CancelBill_Payment_Null_BillID(int billID)
    {
        // Act
        var result = await _controller.CancelBill(billID);
        
        // Assert
        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        
        Assert.Equal("Error", redirectToActionResult.ActionName);
        Assert.Equal("Home",redirectToActionResult.ControllerName);
    }
}