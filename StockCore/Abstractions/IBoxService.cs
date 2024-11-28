using StockCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockCore.Abstractions;

public interface IBoxService
{
    Task<IEnumerable<Box>> GetAllBoxesAsync();
    Task<Box> GetBoxesByIdAsync(Guid id);
    Task AddBoxAsync(Box box);
    Task UpdateBoxAsync(Box box);
    Task DeleteBoxAsync(Guid id);
}