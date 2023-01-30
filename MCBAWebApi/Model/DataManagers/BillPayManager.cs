using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MCBAWebApi.Models.DataManager;

using MCBAWebApi.Data;
using MCBAWebApi.Models.Repository;
using MCBAWebApi.Models;

public class BillPayManager : IDataRepository<BillPay, int>
{
    private readonly MCBAContext _context;

    public BillPayManager(MCBAContext context)
    {
        _context = context;
    }
    
    // get billpay from database by id
    public BillPay Get(int id)
    {
        return _context.BillPay.Find(id);
    }
    
    // get all billpays from the database
    public IEnumerable<BillPay> GetAll()
    {
        return _context.BillPay.ToList();
    }

    // add billpay to the database
    public int Add(BillPay billPay)
    {
        _context.BillPay.Add(billPay);
        _context.SaveChanges();

        return billPay.BillPayID;
    }

    // delete billpay from the database by id
    public int Delete(int id)
    {
        _context.BillPay.Remove(_context.BillPay.Find(id));
        _context.SaveChanges();

        return id;
    }

    // get billpays the database that have a id containing
    // payID
    public IEnumerable<BillPay> Search(int payID)
    {
        return _context.BillPay.Where(x => x.BillPayID.Equals(payID));
    } 

    // Updated billpay in the database
    public int Update(int id, BillPay billPay)
    {
        _context.Update(billPay);
        _context.SaveChanges();
            
        return id;
    }
}