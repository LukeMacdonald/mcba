using CustomerPortal.DTO;
using CustomerPortal.Models;
using CustomerPortal.Models.Enum;
using Newtonsoft.Json;
namespace CustomerPortal.Data;

public static class SeedData
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<MCBAContext>();

        // Look for customers.
        if(context.Customer.Any())
            return; // DB has already been seeded.
        
        const string Url = "https://coreteaching01.csit.rmit.edu.au/~e103884/wdt/services/customers/";
        
        // Initiales the object used to contact the server to retrieve the json
        using var client = new HttpClient();
        // Gets Json for Server
        var json = client.GetStringAsync(Url).Result;

        // Turns the Json into defined objects (Customer,Login,Account and Transaction)
        var customers = JsonConvert.DeserializeObject<List<CustomerDTO>>(json, new JsonSerializerSettings
        {
            // See here for DateTime format string documentation:
            // https://docs.microsoft.com/en-au/dotnet/standard/base-types/custom-date-and-time-format-strings
            DateFormatString = "dd/MM/yyyy"
        });

        context.Payee.AddRange(
            new Payee()
            {
                Name = "Telstra",
                Postcode = "3000",
                Mobile = "0412 123 567",
                Address = "156 Elizabeth St",
                City = "Melbourne",
                State = AustralianState.Victoria,
            },
            new Payee()
            {
                Name = "Optus",
                Postcode = "5000",
                Mobile = "0410 012 389",
                Address = "354 King William St",
                City = "Adelaide",
                State = AustralianState.SouthAustralia,
            },
            new Payee()
            {
                Name = "Apple",
                Postcode = "4000",
                Mobile = "0498 763 567",
                Address = "233 Queen St",
                City = "Brisbane",
                State = AustralianState.Queensland,
            },
            new Payee()
            {
                Name = "AGL Energy",
                Postcode = "2000",
                Mobile = "0412 123 567",
                Address = "200 George St",
                City = "Sydney",
                State = AustralianState.NewSouthWales,
            },
            new Payee()
            {
                Name = "Real Estate of Melbourne",
                Postcode = "3000",
                Mobile = "0412 123 567",
                Address = "U 4104/560 Lonsdale St",
                City = "Melbourne",
                State = AustralianState.Victoria,
            });

        decimal balance;
        
        customers.ForEach(customer =>
        {
            context.Customer.Add(new Customer()
            {
                Address = customer.Address,
                City = customer.City,
                CustomerID = customer.CustomerID,
                Name = customer.Name,
                Postcode = customer.Postcode,
                // DisplayPicture = ""
            });
            
            context.Login.Add(new Login()
            {
                LoginID = customer.Login.LoginID.ToString(),
                PasswordHash = customer.Login.PasswordHash,
                CustomerID = customer.CustomerID,
                Disabled = false
            });
            
            customer.Accounts.ForEach(account =>
            {
                balance = 0;
                account.CustomerID = customer.CustomerID;
                account.Transactions.ForEach(transaction =>
                {
                    context.Transaction.Add(new Transaction()
                    {
                        AccountNumber = account.AccountNumber,
                        TransactionType = TransactionType.Deposit,
                        Amount = transaction.Amount,
                        DestinationAccountNumber = transaction.DestinationAccountNumber,
                        Comment = transaction.Comment,
                        TransactionTimeUtc = transaction.TransactionTimeUTC
                    });
                    balance += transaction.Amount;
                });
                context.Account.Add(new Account()
                {
                    AccountNumber = account.AccountNumber,
                    AccountType = EnumUtils.ConvertAccountType(account.AccountType),
                    Balance = balance,
                    CustomerID = customer.CustomerID
                });

            });
        });
        context.SaveChanges();
    }
      public static void Initialize(MCBAContext context)
    {
        
        // Look for customers.
        if(context.Customer.Any())
            return; // DB has already been seeded.

        context.Customer.AddRange(
            new Customer
            {
                CustomerID = 2100,
                Name = "Matthew Bolger",
                Address = "123 Fake Street",
                City = "Melbourne",
                Postcode = "3000"
            },
            new Customer
            {
                CustomerID = 2200,
                Name = "Rodney Cocker",
                Address = "456 Real Road",
                City = "Melbourne",
                Postcode = "3005"
            },
            new Customer
            {
                CustomerID = 2300,
                Name = "Shekhar Kalra"
            });

        context.Login.AddRange(
            new Login
            {
                LoginID = "12345678",
                CustomerID = 2100,
                PasswordHash =
                    "Rfc2898DeriveBytes$50000$MrW2CQoJvjPMlynGLkGFrg==$x8iV0TiDbEXndl0Fg8V3Rw91j5f5nztWK1zu7eQa0EE="
            },
            new Login
            {
                LoginID = "38074569",
                CustomerID = 2200,
                PasswordHash =
                    "Rfc2898DeriveBytes$50000$fB5lteA+LLB0mKVz9EBA7A==$Tx0nXJ8aJjBU/mS2ssFIMs3m7vaiyitRmBRvBAYWauw="
            },
            new Login
            {
                LoginID = "17963428",
                CustomerID = 2300,
                PasswordHash =
                    "Rfc2898DeriveBytes$50000$jDBijGZNWLh+0MOXnp68Yw==$4bQ9SJGtRQJolToCjFTPsVzRtH8QQUpEsioJ6Y3ewN4="
            });

        context.Account.AddRange(
            new Account
            {
                AccountNumber = 4100,
                AccountType = AccountType.Saving,
                CustomerID = 2100,
                Balance = 100
            },
            new Account
            {
                AccountNumber = 4101,
                AccountType = AccountType.Checking,
                CustomerID = 2100,
                Balance = 900
            },
            new Account
            {
                AccountNumber = 4200,
                AccountType = AccountType.Saving,
                CustomerID = 2200,
                Balance = 500.95m
            },
            new Account
            {
                AccountNumber = 4300,
                AccountType = AccountType.Checking,
                CustomerID = 2300,
                Balance = 1250.50m
            });
            
        const string format = "dd/MM/yyyy hh:mm:ss tt";

        context.Transaction.AddRange(
            new Transaction
            {
                TransactionType = TransactionType.Deposit,
                AccountNumber = 4100,
                Amount = 100,
                Comment = "Opening balance",
                TransactionTimeUtc = DateTime.ParseExact("02/01/2023 08:00:00 PM", format, null)
            },
            new Transaction
            {
                TransactionType = TransactionType.Deposit,
                AccountNumber = 4101,
                Amount = 600,
                Comment = "First deposit",
                TransactionTimeUtc = DateTime.ParseExact("02/01/2023 08:30:00 PM", format, null)
            },
            new Transaction
            {
                TransactionType = TransactionType.Deposit,
                AccountNumber = 4101,
                Amount = 300,
                Comment = "Second deposit",
                TransactionTimeUtc = DateTime.ParseExact("02/01/2023 08:45:00 PM", format, null)
            },
            new Transaction
            {
                TransactionType = TransactionType.Deposit,
                AccountNumber = 4200,
                Amount = 500,
                Comment = "Deposited $500",
                TransactionTimeUtc = DateTime.ParseExact("02/01/2023 09:00:00 PM", format, null)
            },
            new Transaction
            {
                TransactionType = TransactionType.Deposit,
                AccountNumber = 4200,
                Amount = 0.95m,
                Comment = "Deposited $0.95",
                TransactionTimeUtc = DateTime.ParseExact("02/01/2023 09:15:00 PM", format, null)
            },
            new Transaction
            {
                TransactionType = TransactionType.Deposit,
                AccountNumber = 4300,
                Amount = 1250.50m,
                Comment = null,
                TransactionTimeUtc = DateTime.ParseExact("02/01/2023 10:00:00 PM", format, null)
            },
            new Transaction
            {
                TransactionType = TransactionType.Transfer,
                AccountNumber = 4100,
                Amount = 1250.50m,
                DestinationAccountNumber = 4101,
                Comment = null,
                TransactionTimeUtc = DateTime.ParseExact("02/01/2023 10:00:00 PM", format, null)
            },
            new Transaction
            {
                TransactionType = TransactionType.Withdraw,
                AccountNumber = 4100,
                Amount = 120.50m,
                Comment = "Withdraw",
                TransactionTimeUtc = DateTime.ParseExact("02/01/2023 10:00:00 PM", format, null)
            }
            );
        
        context.Payee.AddRange(
            new Payee()
            {
                Name = "Telstra",
                Postcode = "3000",
                Mobile = "0412 123 567",
                Address = "156 Elizabeth St",
                City = "Melbourne",
                State = AustralianState.Victoria,
            },
            new Payee()
            {
                Name = "Optus",
                Postcode = "5000",
                Mobile = "0410 012 389",
                Address = "354 King William St",
                City = "Adelaide",
                State = AustralianState.SouthAustralia,
            },
            new Payee()
            {
                Name = "Apple",
                Postcode = "4000",
                Mobile = "0498 763 567",
                Address = "233 Queen St",
                City = "Brisbane",
                State = AustralianState.Queensland,
            },
            new Payee()
            {
                Name = "AGL Energy",
                Postcode = "2000",
                Mobile = "0412 123 567",
                Address = "200 George St",
                City = "Sydney",
                State = AustralianState.NewSouthWales,
            },
            new Payee()
            {
                Name = "Real Estate of Melbourne",
                Postcode = "3000",
                Mobile = "0412 123 567",
                Address = "U 4104/560 Lonsdale St",
                City = "Melbourne",
                State = AustralianState.Victoria,
            });
        context.BillPay.AddRange(
            new BillPay()
            {
                AccountNumber = 4200,
                Amount = 100.00m,
                PayID = 1,
                Active = true,
                Period = PeriodType.Monthly,
                Failed = true,
                ScheduleTimeUtc = DateTime.ParseExact("10/03/2023 10:00:00 PM", format, null)
            },
            new BillPay()
            {
                AccountNumber = 4300,
                Amount = 20.00m,
                PayID = 2,
                Active = true,
                Period = PeriodType.OneOff,
                Failed = false,
                ScheduleTimeUtc = DateTime.ParseExact("02/02/2023 12:30:00 PM", format, null)
            },
            new BillPay()
            {
                AccountNumber = 4300,
                Amount = 10.00m,
                PayID = 2,
                Active = true,
                Period = PeriodType.OneOff,
                Failed = false,
                ScheduleTimeUtc = DateTime.ParseExact("02/04/2023 12:30:00 PM", format, null)
            },
            new BillPay()
            {
                AccountNumber = 4300,
                Amount = 1000.00m,
                PayID = 2,
                Active = true,
                Period = PeriodType.OneOff,
                Failed = false,
                ScheduleTimeUtc = DateTime.ParseExact("02/03/2023 12:30:00 PM", format, null)
            },
            new BillPay()
            {
                AccountNumber = 4200,
                Amount = 10.50m,
                PayID = 3,
                Active = true,
                Period = PeriodType.OneOff,
                Failed = true,
                ScheduleTimeUtc = DateTime.ParseExact("09/01/2023 10:20:00 PM", format, null)
            },new BillPay()
            {
                AccountNumber = 4101,
                Amount = 1000.00m,
                PayID = 3,
                Active = true,
                Period = PeriodType.Monthly,
                Failed = true,
                ScheduleTimeUtc = DateTime.ParseExact("03/01/2023 10:00:00 PM", format, null)
            },new BillPay()
            {
                AccountNumber = 4100,
                Amount = 100.00m,
                PayID = 1,
                Active = true,
                Period = PeriodType.OneOff,
                Failed = false,
                ScheduleTimeUtc = DateTime.ParseExact("02/07/2023 10:00:00 PM", format, null)
            },new BillPay()
            {
                AccountNumber = 4100,
                Amount = 900.00m,
                PayID = 1,
                Active = true,
                Period = PeriodType.OneOff,
                Failed = false,
                ScheduleTimeUtc = DateTime.ParseExact("02/06/2023 10:00:00 PM", format, null)
            },new BillPay()
            {
                AccountNumber = 4100,
                Amount = 100.00m,
                PayID = 1,
                Active = true,
                Period = PeriodType.OneOff,
                Failed = false,
                ScheduleTimeUtc = DateTime.ParseExact("02/05/2023 10:00:00 PM", format, null)
            },new BillPay()
            {
                AccountNumber = 4100,
                Amount = 100.00m,
                PayID = 1,
                Active = true,
                Period = PeriodType.OneOff,
                Failed = false,
                ScheduleTimeUtc = DateTime.ParseExact("02/04/2023 10:00:00 PM", format, null)
            });
        

        context.SaveChanges();
    }
}