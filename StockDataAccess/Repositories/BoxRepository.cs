using Microsoft.EntityFrameworkCore;
using StockCore.Abstractions;
using StockCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockDataAccess.Repositories;



public class BoxRepository : IBoxRepository
{
    private readonly DatabaseContext _context;

    public BoxRepository(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Box>> GetAllAsync()
    {
        return await _context.Boxes.ToListAsync();
    }

    public async Task<Box> GetByIdAsync(Guid id)
    {
        return await _context.Boxes.FindAsync(id);
    }

    public async Task AddAsync(Box box)
    {
        await _context.Boxes.AddAsync(box);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Box box)
    {
        _context.Boxes.Update(box);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var box = await _context.Boxes.FindAsync(id);
        if (box != null)
        {
            _context.Boxes.Remove(box);
            await _context.SaveChangesAsync();
        }
    }
}