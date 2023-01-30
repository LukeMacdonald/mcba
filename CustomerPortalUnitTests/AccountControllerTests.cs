using CustomerPortal.Controllers;
using CustomerPortal.Models;
using CustomerPortal.Models.Enum;
using CustomerPortal.Models.ViewModels;
using CustomerPortal.Models.Extensions;
using CustomerPortalUnitTests.Base;
using CustomerPortalUnitTests.Utils;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;

namespace CustomerPortalUnitTests;

public class AccountControllerTests : BackendTest
{
    private readonly AccountController _controller;
    
    public AccountControllerTests()
    {
        _controller = Container.ResolveControllerWithSeededData<AccountController>();
    }
    
    
    [Theory]
    [InlineData(0)]
    [InlineData(4100)]
    public async Task Deposit(int accountNumber)
    {
        // Act
        var result = _controller.Deposit();
    
        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        
        // Assert
        Assert.Null(redirectToActionResult.ControllerName);
        
        Assert.Equal("Transaction", redirectToActionResult.ActionName);
    }
    
    //
    [Theory]
    [InlineData(0)]
    [InlineData(4100)]
    public async Task Withdraw(int accountNumber)
    {
        // Act
        var result = _controller.Withdraw();
        
        // Assert
        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        
        Assert.Null(redirectToActionResult.ControllerName);
        
        Assert.Equal("Transaction", redirectToActionResult.ActionName);
    }
    
    
    [Theory]
    [InlineData(0)]
    [InlineData(4100)]
    public async Task Transfer(int accountNumber)
    {
        // Act
        var result = _controller.Deposit();
        
        // Assert
        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        
        Assert.Null(redirectToActionResult.ControllerName);
        
        Assert.Equal("Transaction", redirectToActionResult.ActionName);
    }
    
    [Theory]
    [InlineData(TransactionType.Deposit, 4100, 2100)]
    [InlineData(TransactionType.Withdraw, 4100, 2100)]
    [InlineData(TransactionType.Transfer, 4100, 2100)]
    public async Task Transaction(TransactionType type, int accountNumber, int customerID)
    {
        
        
        // Arrange
        _controller.HttpContext.Session.SetInt32(nameof(Customer.CustomerID),customerID);

        var customer = await _controller.FindCustomer(customerID);
        
        var account = await _controller.FindAccount(accountNumber);

        var count = await _controller.TotalPayments(account.AccountNumber);
        
        var expected = new TransactionViewModel()
        {
            TransactionType = type,
            AccountNumber = accountNumber,
            CustomerAccounts = customer.Accounts,
            AccountBalance = account.Balance,
            AccountType = account.AccountType,
            TotalPayments = count,
        };
        
        // Act.
        var result = await _controller.Transaction(type,accountNumber);
        
        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<TransactionViewModel>(viewResult.ViewData.Model);
        Assert.Equal(expected,model);
    }
    
    [Theory]
    [InlineData(TransactionType.Deposit, 4100, -102)]
    [InlineData(TransactionType.Withdraw, 0, 2100)]
    [InlineData(TransactionType.Transfer, 0, 2100)]
    public async Task TransactionError(TransactionType type, int accountNumber, int customerID)
    {
        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _controller.Transaction(type,accountNumber));
    }
    
    [Theory]
    [InlineData(4100,2)]
    [InlineData(4101,0)]
    [InlineData(0,0)]
    [InlineData(-4101,0)]
    public async Task TotalPayments(int accountNumber,int expected)
    {
        // Act.
        var result = await _controller.TotalPayments(accountNumber);
        // Assert
        Assert.Equal(expected,result);
    }
    [Theory]
    [InlineData(4100,1250.50,false)]
    [InlineData(4100,50.50,true)]
    [InlineData(4101,250.50,true)]
    public async Task CheckRemovalOfAmount(int accountNumber,decimal amount,bool expected)
    {
        
        // Arrange
        var account = await _controller.FindAccount(accountNumber);
        
        // Act
        var result =  _controller.IsValid(account,amount);
        
        // Assert
        Assert.Equal(expected,result);
        
    }
    
    [Theory]
    [InlineData(4200,2200,2)]
    [InlineData(4101,2100,2)]
    [InlineData(4300,2300,1)]
    public async Task ViewStatement(int accountNumber,int customerID, int expected)
    {
        
        // Arrange
        _controller.HttpContext.Session.SetInt32(nameof(Customer.CustomerID),customerID);
        _controller.HttpContext.Session.SetInt32("AccountController_Account",accountNumber);
        
        // Act
        var result = await _controller.ViewStatements();
        
        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        
        var model = Assert.IsAssignableFrom<IPagedList<Transaction>>(viewResult.ViewData.Model);
        
        Assert.Equal(expected,model.TotalItemCount);
        
        
    }
    [Theory]
    // [InlineData(4100,2200,0)]
    [InlineData(4200,2100,0)]
    [InlineData(4101,2300,3)]
    [InlineData(4200,2300,4)]
    public async Task ViewStatement_Invalid_Account_Number(int accountNumber,int customerID, int expected)
    {
        // Arrange
        _controller.HttpContext.Session.SetInt32(nameof(Customer.CustomerID),customerID);
        _controller.HttpContext.Session.SetInt32("AccountController_Account",accountNumber);
        
        // Act
        var result = await _controller.ViewStatements();
    
        // Assert
        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);

        Assert.Equal("Customer", redirectToActionResult.ControllerName);
    
        Assert.Equal("Index", redirectToActionResult.ActionName);
        
    }
    
    [Theory]
    [InlineData(TransactionType.Deposit, 4100, 2100,500.50)]
    [InlineData(TransactionType.Withdraw, 4100, 2100,700.50)]
    [InlineData(TransactionType.Transfer, 4200, 2200,200.50,4101)]
    [InlineData(TransactionType.Deposit, 4300, 2300,500.50)]
    [InlineData(TransactionType.Withdraw, 4200, 2200,700.50)]
    [InlineData(TransactionType.Transfer, 4101, 2100,200.50,4100)]
    public async Task CompleteTransaction_Successful(TransactionType type, int accountNumber, int customerID,decimal amount, int destination = 0)
    {
        // Arrange
        _controller.HttpContext.Session.SetInt32(nameof(Customer.CustomerID),customerID);
        
        var account = await _controller.FindAccount(accountNumber);

        var count = await _controller.TotalPayments(accountNumber);
        
        var viewModel = new TransactionViewModel()
        {
            TransactionType = type,
            AccountNumber = accountNumber,
            AccountBalance = account.Balance,
            AccountType = account.AccountType,
            TotalPayments = count,
            PaymentAmount = amount,
            DestinationAccountNumber = destination
        };
        
        _controller.HttpContext.Session.Set(nameof(viewModel),viewModel);
    
        // Act
        var result = await _controller.CompleteTransaction();
        
        // Assert
        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);

        Assert.Equal("Customer", redirectToActionResult.ControllerName);
    
        Assert.Equal("Index", redirectToActionResult.ActionName);
        
    }
    
    
    [Theory]
    [InlineData(TransactionType.Deposit, 4100, 2200,500.50)]
    [InlineData(TransactionType.Withdraw, 4100, 2300,700.50)]
    [InlineData(TransactionType.Transfer, 4200, 2300,200.50,4101)]
    [InlineData(TransactionType.Deposit, 4300, 2200,500.50)]
    [InlineData(TransactionType.Withdraw, 4200, 2100,700.50)]
    [InlineData(TransactionType.Transfer, 4101, 2200,200.50,4100)]
    public async Task CompleteTransaction_Failure(TransactionType type, int accountNumber, int customerID,decimal amount, int destination = 0)
    {
        // Arrange
        _controller.HttpContext.Session.SetInt32(nameof(Customer.CustomerID),customerID);
        
        var account =  await _controller.FindAccount(accountNumber);
        
        var count = await _controller.TotalPayments(accountNumber);
        
        var viewModel = new TransactionViewModel()
        {
            TransactionType = type,
            AccountNumber = accountNumber,
            AccountBalance = account.Balance,
            AccountType = account.AccountType,
            TotalPayments = count,
            PaymentAmount = amount,
            DestinationAccountNumber = destination
        };
        
        _controller.HttpContext.Session.Set(nameof(viewModel),viewModel);
    
        // Act
        var result = await _controller.CompleteTransaction();
        
        // Assert
        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
    
        Assert.Equal("Home", redirectToActionResult.ControllerName);
    
        Assert.Equal("Error", redirectToActionResult.ActionName);
        
    }
    
    

}