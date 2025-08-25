using ClassLibrary;
using System.Drawing;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Serialization;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

namespace Temp
{
    public class XMLToCSVConverter : IScanner
    {
        private Dictionary<string, int> tempDic = new Dictionary<string, int>();
        List<string> myList = new List<string>();
        List<string> list = new List<string>();
        int ID = 63834656;
        string DeliveryMan = "Kej Nielsen";

        public void convert()
        {
            using (var writer = new StreamWriter("export//" + ID + "_" + DeliveryMan + ".csv"))
            using (var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture) { 
                Delimiter = ";" }
            ))

            {
                csv.WriteField("ProductID");
                csv.WriteField("Type");
                csv.WriteField("Description");
                csv.WriteField("Amount");
                csv.NextRecord();

                int counter = 0;
                foreach (var item in tempDic)
                {
                    csv.WriteField(list[counter]);
                    csv.WriteField("Fysisk");
                    csv.WriteField(item.Key);
                    csv.WriteField(item.Value);
                    csv.NextRecord();
                    counter++;
                }
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