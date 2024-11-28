using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StockCore.Models;
using StockDataAccess;

namespace StockCommonUtils
{
    public static class PalletUtility
    {
        public static async Task AddAllBoxesToPallets(DatabaseContext context)
        {
            var pallets = await context.Pallets
                .Where(p => p.Weight == 0 && p.Volume == 0)
                .Include(p => p.Boxes)
                .ToListAsync();

            if (pallets.Count == 0)
            {
                Console.WriteLine("Нет паллетов с нулевыми весом и объемом.");
                return;
            }

            var allBoxes = await context.Boxes.ToListAsync();
            int palletIndex = 0;
            var currentPallet = pallets[palletIndex];

            foreach (var box in allBoxes)
            {
                if (currentPallet.Width >= box.Width &&
                    currentPallet.Height >= box.Height &&
                    currentPallet.Depth >= box.Depth)
                {
                    currentPallet.Boxes.Add(box);
                    currentPallet.Width -= box.Width;
                    currentPallet.Height -= box.Height;
                    currentPallet.Depth -= box.Depth;

                    currentPallet.Volume += box.Volume;
                    currentPallet.Weight += box.Weight;

                    await context.SaveChangesAsync();
                    Console.WriteLine($"Box {box.BoxId} added to Pallet {currentPallet.PalletId}");
                }
                else
                {
                    DisplayPalletInfo(currentPallet); // Отображение информации о текущем паллете перед переходом к следующему

                    palletIndex++;
                    if (palletIndex >= pallets.Count)
                    {
                        Console.WriteLine("Не хватает паллет для размещения всех коробок.");
                        break;
                    }
                    currentPallet = pallets[palletIndex];
                    currentPallet.Boxes.Add(box);
                    currentPallet.Width -= box.Width;
                    currentPallet.Height -= box.Height;
                    currentPallet.Depth -= box.Depth;

                    currentPallet.Volume += box.Volume;
                    currentPallet.Weight += box.Weight;

                    await context.SaveChangesAsync();
                    Console.WriteLine($"Box {box.BoxId} added to Pallet {currentPallet.PalletId}");
                }
            }

            // Отображение информации для всех оставшихся паллетов
            foreach (var pallet in pallets)
            {
                pallet.expirationDate = pallet.Boxes.Min(b => b.expirationDate);  // Наименьший срок годности
                pallet.Weight += 30;  // Вес паллеты плюс вес паллеты 30 кг
                pallet.Volume += pallet.Width * pallet.Height * pallet.Depth;  // Общий объем паллеты

                DisplayPalletInfo(pallet); // Отображение информации о паллете
            }
        }

        public static async Task<List<Guid>> GetEmptyPalletGuids(DatabaseContext context)
        {
            return await context.Pallets
                .Where(p => p.Weight == 0 && p.Volume == 0)
                .Select(p => p.PalletId)
                .ToListAsync();
        }

        private static void DisplayPalletInfo(Pallet pallet)
        {
            Console.WriteLine($"Pallet ID: {pallet.PalletId}");
            Console.WriteLine($"Dimensions: {pallet.Width} x {pallet.Height} x {pallet.Depth}");
            Console.WriteLine($"Volume: {pallet.Volume}");
            Console.WriteLine($"Weight: {pallet.Weight}");
            Console.WriteLine($"Expiration Date: {pallet.expirationDate}");  // Срок годности паллеты
            Console.WriteLine($"Number of Boxes: {pallet.Boxes.Count}");
            foreach (var box in pallet.Boxes)
            {
                Console.WriteLine($"Box ID: {box.BoxId}, Width: {box.Width}, Height: {box.Height}, Depth: {box.Depth}, Weight: {box.Weight} Volume: {box.Volume}, Expiration Date: {box.expirationDate}");
            }
            Console.WriteLine();
        }

        public static void GroupAndSortPallets(DatabaseContext context)
        {
            var pallets = context.Pallets
                .Include(p => p.Boxes)
                .ToList()
                .Where(p => p.Boxes.Any()) // Исключаем паллеты без коробок
                .GroupBy(p => p.Boxes.Min(b => b.expirationDate)) // Группировка по сроку годности
                .OrderBy(group => group.Key) // Сортировка по возрастанию срока годности
                .ToList();

            foreach (var group in pallets)
            {
                Console.WriteLine($"Group Expiration Date: {group.Key}");
                foreach (var pallet in group.OrderBy(p => p.Weight)) // Сортировка паллетов по весу в каждой группе
                {
                    DisplayPalletInfo(pallet);
                }
            }
        }

        public static void GetTopThreePalletsByExpirationDate(DatabaseContext context)
        {
            var pallets = context.Pallets
                .Include(p => p.Boxes)
                .ToList()
                .Where(p => p.Boxes.Any()) // Исключаем паллеты без коробок
                .OrderByDescending(p => p.Boxes.Min(b => b.expirationDate)) // Сортировка по убыванию срока годности
                .ThenBy(p => p.Volume) // Сортировка по возрастанию объема
                .Take(3) // Выбор трех паллетов
                .ToList();

            foreach (var pallet in pallets)
            {
                DisplayPalletInfo(pallet);
            }
        }










    }
}
