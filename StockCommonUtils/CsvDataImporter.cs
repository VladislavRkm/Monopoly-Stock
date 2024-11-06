using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using StockDataAccess;
using StockCore.Models;

namespace StockCommonUtils;

public class CsvDataImporter
{
    private readonly DatabaseContext _context;

    public CsvDataImporter(DatabaseContext context)
    {
        _context = context;
    }

    public void ImportBoxes(string csvFilePath)
    {
        ImportCsvData<Box>(csvFilePath);
    }

    public void ImportPallets(string csvFilePath)
    {
        ImportCsvData<Pallet>(csvFilePath);
    }

    private void ImportCsvData<T>(string csvFilePath) where T : class
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ",",
            HasHeaderRecord = true,
            MissingFieldFound = null,
            BadDataFound = null, 
            HeaderValidated = null, 

        };

        using (var reader = new StreamReader(csvFilePath))
        using (var csv = new CsvReader(reader, config))
        {
            var records = csv.GetRecords<T>();

            foreach (var record in records)
            {
              
                _context.Set<T>().Add(record);
            }
            _context.SaveChanges();
        }

        Console.WriteLine("Данные успешно загружены");
    }
}
