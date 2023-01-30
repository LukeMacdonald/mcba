using A2Practice.Models;
using Microsoft.EntityFrameworkCore;

namespace A2Practice.Data;

public class MCBAContext :DbContext
{
    public MCBAContext(DbContextOptions<MCBAContext>options) : base(options){}
    
    public DbSet<Customer> Customer { get; set; }
    public DbSet<Account> Account { get; set; }
    public DbSet<Login> Login { get; set; }
    public DbSet<Transaction> Transaction { get; set; }
    public DbSet<BillPay> BillPay { get; set; }
    public DbSet<Payee> Payee { get; set; }
    
    
    
}