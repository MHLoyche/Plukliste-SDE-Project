using System.Drawing;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Serialization;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.Net.Http.Json;
using ClassLibrary.Model;
using ClassLibrary.Enums;
using ClassLibrary.Interfaces;

namespace Temp
{
    // Class to convert XML picking list data to CSV format with Injected IScanner interface
    public class XMLToCSVConverter : IScanner
    {
        private Dictionary<string, int> tempDic = new (); // Initially empty dictionary to hold item titles and their ordered amounts
        List<string> myList = new List<string>();         // List to hold product IDs
        List<string> list = new List<string>();           // Distinct list of product IDs
        int ID = 63834656;
        string DeliveryMan = "Kej Nielsen";

        // Method to convert the collected data into a CSV file (Later converted to save it to the DB)
        public void convert()
        {
            // fetch storage data from API endpoint
            var http = new HttpClient();
            var storage = http.GetFromJsonAsync<Dictionary<string, StorageItem>>( // fetch data from API
                              "https://localhost:7148/storage/items") // URL for the API endpoint
                          .GetAwaiter().GetResult() // synchronous wait for the async call to complete
                       ?? new();

            // Import is the save location and export is the read location
            using var writer = new StreamWriter($"import/{ID}_{DeliveryMan}.csv"); // create a new CSV file for writing using StreamWriter
            using var csv = new CsvWriter(writer,
                new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = ";" }); // configure CsvWriter with semicolon delimiter

            csv.WriteField("ProductID"); // CSV header fields
            csv.WriteField("Type");
            csv.WriteField("Description");
            csv.WriteField("Amount");      // ordered (your existing value)
            csv.WriteField("CurrentStock"); // ← add this column
            csv.NextRecord(); // move to the next record in the CSV

            int counter = 0; // counter to track the index in the product ID list
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
                counter++; // Increments the counter to move to the next product ID in the list
            }
        }
        // Method to read and process the picking list
        public void read(Pluklist content)
        {
            foreach (Item line in content.Lines) // Loop through each item in the picking list
            {
                if (line.Type == ItemType.Fysisk) // Only process physical items
                {
                    if (tempDic.ContainsKey(line.Title)) // If the item title already exists in the dictionary, increment its amount
                    {
                        tempDic[line.Title] += line.Amount;
                    }
                    else                               // If the item title does not exist, add it to the dictionary with its amount
                    {
                        tempDic[line.Title] = line.Amount;
                    }
                    myList.Add(line.ProductID);   // Add the product ID to the list
                    list = myList.Distinct().ToList(); // Create a distinct list of product IDs to avoid duplicates
                }
            }
        }
    }
}


