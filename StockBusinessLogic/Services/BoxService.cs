using StockCore.Abstractions;
using StockCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockBusinessLogic.Services;

public class BoxService : IBoxService
{
    private readonly IBoxRepository _boxRepository;

    public BoxService(IBoxRepository boxRepository)
    {
        _boxRepository = boxRepository;
    }

    public async Task<IEnumerable<Box>> GetAllBoxesAsync()
    {
        return await _boxRepository.GetAllAsync();
    }

    public async Task<Box> GetBoxesByIdAsync(Guid id)
    {
        return await _boxRepository.GetByIdAsync(id);
    }

    public async Task AddBoxAsync(Box box)
    {
        await _boxRepository.AddAsync(box);
    }

    public async Task UpdateBoxAsync(Box box)
    {
        await _boxRepository.UpdateAsync(box);
    }

    public async Task DeleteBoxAsync(Guid id)
    {
        await _boxRepository.DeleteAsync(id);
    }
}