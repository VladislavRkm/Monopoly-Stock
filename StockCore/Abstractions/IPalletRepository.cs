using StockCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockCore.Abstractions;

public interface IPalletRepository
{
    Task<IEnumerable<Pallet>> GetAllAsync();
    Task<Pallet> GetByIdAsync(Guid id);
    Task AddAsync(Pallet pallet);
    Task UpdateAsync(Pallet pallet);
    Task DeleteAsync(Guid id);
}