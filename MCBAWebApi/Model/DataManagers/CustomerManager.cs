using Microsoft.EntityFrameworkCore;

namespace MCBAWebApi.Models.DataManager;

using MCBAWebApi.Data;
using MCBAWebApi.Models.Repository;
using MCBAWebApi.Models;

public class CustomerManager : IDataRepository<Customer, int>
{
    private readonly MCBAContext _context;

    public CustomerManager(MCBAContext context)
    {
        _context = context;
    }
    
    // Get customer from the databse
    public Customer Get(int id)
    {
        return _context.Customer.Find(id);
    }
    
    // Get all customers from the database
    public IEnumerable<Customer> GetAll()
    {
        return _context.Customer.ToList();
    }

    // Add customer to the database
    public int Add(Customer customer)
    {
        _context.Customer.Add(customer);
        _context.SaveChanges();

        return customer.CustomerID;
    }

    // Delete customer from the database
    public int Delete(int id)
    {
        _context.Customer.Remove(_context.Customer.Find(id));
        _context.SaveChanges();

        return id;
    }

    // Get customers with name containing specified string
    public IEnumerable<Customer> Search(string name)
    {
        return _context.Customer.Where(x => x.Name.Contains(name));
    } 

    // Update customer in the databse
    public int Update(int id, Customer customer)
    {
        _context.Update(customer);
        _context.SaveChanges();
            
        return id;
    }
}