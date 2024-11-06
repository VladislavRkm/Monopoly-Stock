using StockCore.Abstractions;
using StockCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockBusinessLogic.Services;

public class PalletService : IPalletService
{
    private readonly IPalletRepository _palletRepository;

    public PalletService(IPalletRepository palletRepository)
    {
        _palletRepository = palletRepository;
    }

    public async Task<IEnumerable<Pallet>> GetAllPalletsAsync()
    {
        return await _palletRepository.GetAllAsync();
    }

    public async Task<Pallet> GetPalletByIdAsync(Guid id)
    {
        return await _palletRepository.GetByIdAsync(id);
    }

    public async Task AddPalletAsync(Pallet pallet)
    {
        await _palletRepository.AddAsync(pallet);
    }

    public async Task UpdatePalletAsync(Pallet pallet)
    {
        await _palletRepository.UpdateAsync(pallet);
    }

    public async Task DeletePalletAsync(Guid id)
    {
        await _palletRepository.DeleteAsync(id);
    }
}