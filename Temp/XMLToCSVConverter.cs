using ClassLibrary;
using System.Drawing;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Serialization;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.Net.Http.Json;

namespace Temp
{
    public class XMLToCSVConverter : IScanner
    {
        private Dictionary<string, int> tempDic = new ();
        List<string> myList = new List<string>();
        List<string> list = new List<string>();
        int ID = 63834656;
        string DeliveryMan = "Kej Nielsen";

        public void convert()
        {
            // fetch storage
            var http = new HttpClient();
            var storage = http.GetFromJsonAsync<Dictionary<string, StorageItem>>(
                              "https://localhost:7148/storage/items")
                          .GetAwaiter().GetResult()
                       ?? new();

            // Import is the save location and export is the read location
            using var writer = new StreamWriter($"import/{ID}_{DeliveryMan}.csv");
            using var csv = new CsvWriter(writer,
                new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = ";" });

            csv.WriteField("ProductID");
            csv.WriteField("Type");
            csv.WriteField("Description");
            csv.WriteField("Amount");      // ordered (your existing value)
            csv.WriteField("CurrentStock"); // ← add this column
            csv.NextRecord();

            int counter = 0;
            foreach (var item in tempDic) // item.Key = Title, item.Value = ordered
            {
                var productId = list[counter]; // assumes this aligns with the same product
                var currentStock = storage.TryGetValue(productId, out var s) ? s.Amount : 0;

                csv.WriteField(productId);
                csv.WriteField("Fysisk");
                csv.WriteField(item.Key);      // Title
                csv.WriteField(item.Value);    // Ordered
                csv.WriteField(currentStock);  // ← DB amount only
                csv.NextRecord();
                counter++;
            }
        }

        public void read(Pluklist content)
        {
            foreach (Item line in content.Lines)
            {
                if (line.Type == ItemType.Fysisk)
                {
                    if (tempDic.ContainsKey(line.Title))
                    {
                        tempDic[line.Title] += line.Amount;
                    } else
                    {
                        tempDic[line.Title] = line.Amount;
                    }
                    myList.Add(line.ProductID);
                    list = myList.Distinct().ToList();
                }
            }
        }
    }
}

public class StorageItem
{
    public string Name { get; set; } = "";
    public int Amount { get; set; }
}
