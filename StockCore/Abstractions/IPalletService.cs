using StockCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockCore.Abstractions;

public interface IPalletService
{
    Task<IEnumerable<Pallet>> GetAllPalletsAsync();
    Task<Pallet> GetPalletByIdAsync(Guid id);
    Task AddPalletAsync(Pallet pallet);
    Task UpdatePalletAsync(Pallet pallet);
    Task DeletePalletAsync(Guid id);

}