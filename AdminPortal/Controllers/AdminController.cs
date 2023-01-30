using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using AdminPortal.Models;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdminPortal.Controllers;

public class AdminController : Controller
{
    private readonly IHttpClientFactory _clientFactory;
    private HttpClient Client => _clientFactory.CreateClient("api");

    public AdminController(IHttpClientFactory clientFactory) => _clientFactory = clientFactory;

    // GET: admin/Index/SearchCustomers
    public async Task<IActionResult> Index(string? search)
    {
        /*
         * If search string is empty then return all customers in list
         */
        if (search.IsNullOrEmpty())
        {
            var customers = await getCustomers();

            foreach (var customer in customers)
            {
                var login = await getLogin(customer.CustomerID);
                customer.Locked = login.Disabled;
            }

            return View(customers);
        }

        /*
         * If search string is not empty, find customers with names that contain
         * the search string
         */
        var response = await Client.GetAsync($"api/customer/search/{search}");

        if (!response.IsSuccessStatusCode)
            throw new Exception();

        var result = await response.Content.ReadAsStringAsync();
        // Deserializing the response received from web api and storing into a list.
        var customersSearch = JsonConvert.DeserializeObject<List<CustomerDto>>(result);

        var query = customersSearch.FindAll(e => e.Name.Contains(search));

        // Setting the locked state for each customer by getting corresponding login details
        foreach (var customer in query)
        {
            var login = await getLogin(customer.CustomerID);
            customer.Locked = login.Disabled;
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            // Storing the search into ViewBag to populate the textbox.
            ViewBag.CustomerName = search;
        }

        // Adding an order by to the query for the customers name.
        query = query.OrderBy(c => c.Name).ToList();

        return View(query);
    }
    

    // GET: api/customer/1
    public async Task<IActionResult> EditCustomer(int? id)
    {
        if (id == null)
            return NotFound();

        // get customer from api with customer ID
        var customer = await getCustomer(id);

        // return edit view of customer
        return View(customer);
    }

    // POST: api/customer
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult EditCustomer(int id, CustomerDto customer)
    {
        if (id != customer.CustomerID)
            return NotFound();

        // If the model state is invalid, don't redirect
        if (!ModelState.IsValid)
        {
            return View(customer);
        }

        // Serialise customer and updated details
        var content = new StringContent(JsonConvert.SerializeObject(customer), Encoding.UTF8, "application/json");

        // Api call to update customer
        var response = Client.PutAsync($"api/customer", content).Result;

        if (response.IsSuccessStatusCode)
            return RedirectToAction("Index");

        return RedirectToAction("Index");
    }

    // GET: api/login/1
    public async Task<IActionResult> EditLock(int? id)
    {
        if (id == null)
            return NotFound();

        // Get customer from api
        var customer = await getCustomer(id);

        // Get corresponding login from api
        var login = await getLogin(id);

        // Change the 'locked' status on the login
        if (login.Disabled)
        {
            login.Disabled = false;
        }
        else
        {
            login.Disabled = true;
        }

        login.Customer = customer;

        // Serialise login with updated 'locked' value
        var content = new StringContent(JsonConvert.SerializeObject(login), Encoding.UTF8, "application/json");

        // Update call to api for login object
        var response = Client.PutAsync($"api/login", content).Result;

        if (response.IsSuccessStatusCode)
            return RedirectToAction("Index");

        return RedirectToAction("Index");
    }


    public async Task<IActionResult> CustomerBillPay(int? accountNumber)
    {
        if (accountNumber == null)
            return NotFound();

        // Get account
        var account = await getAccount(accountNumber);

        // Get BillPays corresponding to account
        var billPays = await getBillPays(accountNumber);

        // Add the BillPays to the account's list of BillPays
        foreach (var billPay in billPays)
        {
            account.BillPay.Add(billPay);
        }

        return View(account);
    }

    public async Task<IActionResult> ViewCustomerAccounts(int? id)
    {
        if (id == null)
            return NotFound();

        // Get customer
        var customer = await getCustomer(id);

        // Get customers accounts
        var accounts = await getAccounts(id);

        // Get all bill pays
        var responseBillPay = await Client.GetAsync($"api/billpay");

        if (!responseBillPay.IsSuccessStatusCode)
            throw new Exception();

        var resultBillPay = await responseBillPay.Content.ReadAsStringAsync();

        foreach (var account in accounts)
        {
            // Add account to customers accounts
            customer.Accounts.Add(account);
            
            var allBillPays = JsonConvert.DeserializeObject<List<BillPayDto>>(resultBillPay);

            // Find BillPays that correspond with account
            var billPays = allBillPays.FindAll(e => e.AccountNumber.Equals(account.AccountNumber));

            // Add BillPays to list of account's BillPays
            foreach (var billPay in billPays)
            {
                account.BillPay.Add(billPay);
            }
        }

        return View(customer);
    }

    public async Task<IActionResult> BlockBillPay(int? payID, int? accountNumber)
    {
        if (payID == null)
            return NotFound();

        // Get BillPay
        var billPay = await getBillPay(payID);

        // Change BillPay's active value
        if (billPay.Active)
        {
            billPay.Active = false;
        }
        else
        {
            billPay.Active = true;
        }
        
        // Serialise BillPay with updated 'active' value
        var content = new StringContent(JsonConvert.SerializeObject(billPay), Encoding.UTF8, "application/json");

        // Upate BillPay in api
        var response = Client.PutAsync($"api/billpay", content).Result;

        if (response.IsSuccessStatusCode)
            return RedirectToAction("Index");

        return RedirectToAction("Index");
    }

    public async Task<CustomerLogin> getLogin(int? customerID)
    {
        var response2 = await Client.GetAsync($"api/login");

        if (!response2.IsSuccessStatusCode)
            throw new Exception();

        var result2 = await response2.Content.ReadAsStringAsync();
        // Deserializing the response received from web api and storing into a list.
        var logins = JsonConvert.DeserializeObject<List<CustomerLogin>>(result2);

        // Eager loading the Product table - join between OwnerInventory and the Product table.
        var login = logins.Find(e => e.CustomerID.Equals(customerID));

        return login;
    }

    public async Task<IEnumerable<CustomerDto>> getCustomers()
    {
        var response = await Client.GetAsync("api/customer");
        if (!response.IsSuccessStatusCode)
            throw new Exception();

        // Storing the response details received from web api.
        var result = await response.Content.ReadAsStringAsync();

        // Deserializing the response received from web api and storing into a list.
        var customers = JsonConvert.DeserializeObject<List<CustomerDto>>(result);

        return customers;
    }

    public async Task<CustomerDto> getCustomer(int? id)
    {
        var response = await Client.GetAsync($"api/customer/{id}");
        //var response = await MovieApi.InitializeClient().GetAsync($"api/movies/{id}");

        if (!response.IsSuccessStatusCode)
            throw new Exception();

        var result = await response.Content.ReadAsStringAsync();
        var customer = JsonConvert.DeserializeObject<CustomerDto>(result);

        return customer;
    }

    public async Task<AccountDto> getAccount(int? accountNumber)
    {
        var responseAccount = await Client.GetAsync($"api/account/{accountNumber}");
        //var response = await MovieApi.InitializeClient().GetAsync($"api/movies/{id}");

        if (!responseAccount.IsSuccessStatusCode)
            throw new Exception();

        var resultAccount = await responseAccount.Content.ReadAsStringAsync();
        var account = JsonConvert.DeserializeObject<AccountDto>(resultAccount);

        return account;
    }

    public async Task<IEnumerable<AccountDto>> getAccounts(int? customerID)
    {
        var responseAccounts = await Client.GetAsync($"api/account");

        if (!responseAccounts.IsSuccessStatusCode)
            throw new Exception();

        var resultAccounts = await responseAccounts.Content.ReadAsStringAsync();
        // Deserializing the response received from web api and storing into a list.
        var allAccounts = JsonConvert.DeserializeObject<List<AccountDto>>(resultAccounts);

        // Eager loading the Product table - join between OwnerInventory and the Product table.
        var accounts = allAccounts.FindAll(e => e.CustomerID.Equals(customerID));

        return accounts;
    }

    public async Task<IEnumerable<BillPayDto>> getBillPays(int? accountNumber)
    {
        var responseBillPay = await Client.GetAsync($"api/billpay");

        if (!responseBillPay.IsSuccessStatusCode)
            throw new Exception();

        var resultBillPay = await responseBillPay.Content.ReadAsStringAsync();
        // Deserializing the response received from web api and storing into a list.
        var allBillPays = JsonConvert.DeserializeObject<List<BillPayDto>>(resultBillPay);

        // Eager loading the Product table - join between OwnerInventory and the Product table.
        var billPays = allBillPays.FindAll(e => e.AccountNumber.Equals(accountNumber));

        return billPays;
    }

    public async Task<BillPayDto> getBillPay(int? payID)
    {
        var responseBillPay = await Client.GetAsync($"api/billpay/{payID}");

        if (!responseBillPay.IsSuccessStatusCode)
            throw new Exception();

        var resultBillPay = await responseBillPay.Content.ReadAsStringAsync();
        var billPay = JsonConvert.DeserializeObject<BillPayDto>(resultBillPay);

        return billPay;
    }

}