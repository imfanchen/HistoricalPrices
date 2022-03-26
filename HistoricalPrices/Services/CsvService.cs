using CsvHelper;
using System.Globalization;
using HistoricalPrices.Models;

namespace HistoricalPrices.Services {
    public class CsvService {
        private readonly string directory = $"{Environment.CurrentDirectory}\\HistoricalPrices.csv";
        private readonly List<Price> cachedPrices;
        public CsvService() {
            using (StreamReader? reader = new StreamReader(directory))
            using (CsvReader? csv = new CsvReader(reader, CultureInfo.InvariantCulture)) {
                cachedPrices = csv.GetRecords<Price>().Reverse().ToList();
                Console.WriteLine("Data is loaded from the csv file.");
            }
        }
        public List<Price> Get() => cachedPrices;

        public Price? Get(DateTime date) => cachedPrices.Where(p => p.Date == date).FirstOrDefault();

        public void Create(Price newPrice) {
            cachedPrices.Add(newPrice);
            cachedPrices.Sort((a, b) => a.Date.CompareTo(b.Date));
            using (var writer = new StreamWriter(directory))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture)) {
                csv.WriteRecords(cachedPrices);
            }
        }

        public void Update(DateTime date, Price updatedPrice) {
            var price = cachedPrices.Where(p => p.Date == date).FirstOrDefault();
            if (price == null) return;
            price.High = updatedPrice.High;
            price.Low = updatedPrice.Low;
            price.Open = updatedPrice.Open;
            price.Close = updatedPrice.Close;
            using (var writer = new StreamWriter(directory))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture)) {
                csv.WriteRecords(cachedPrices);
            }
        }

        public void Remove(DateTime date) {
            var price = cachedPrices.Where(p => p.Date == date).FirstOrDefault();
            if (price == null) return;
            cachedPrices.Remove(price);
            using (var writer = new StreamWriter(directory))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture)) {
                csv.WriteRecords(cachedPrices);
            }
        }
    }
}
