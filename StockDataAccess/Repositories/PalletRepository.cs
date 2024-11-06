using Microsoft.EntityFrameworkCore;
using StockCore.Abstractions;
using StockCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockDataAccess.Repositories;



public class PalletRepository : IPalletRepository
{
    private readonly DatabaseContext _context;

    public PalletRepository(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Pallet>> GetAllAsync()
    {
        return await _context.Pallets.ToListAsync();
    }

    public async Task<Pallet> GetByIdAsync(Guid id)
    {
        return await _context.Pallets.FindAsync(id);
    }

    public async Task AddAsync(Pallet pallet)
    {
        await _context.Pallets.AddAsync(pallet);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Pallet pallet)
    {
        _context.Pallets.Update(pallet);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var student = await _context.Pallets.FindAsync(id);
        if (student != null)
        {
            _context.Pallets.Remove(student);
            await _context.SaveChangesAsync();
        }
    }
}
