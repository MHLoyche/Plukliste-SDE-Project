using ClassLibrary;
using System.Drawing;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Serialization;

namespace Temp
{
    public class FileConverter : IScanner
    {
        private Dictionary<string, int> dictionary;

        public FileConverter()
        {
            dictionary = new Dictionary<string, int>();
        }

        public void convert(Dictionary<string, int> content)
        {
            // TODO: Implement the conversion logic here
            using (FileStream fs = new FileStream(@"C:\Users\AweSa\source\repos\Plukliste-master\Plukliste\Test filer 2\", FileMode.Open))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(FileConverter[]));
                var data = (FileConverter[])serializer.Deserialize(fs);
                List<string> list = new List<string>();
                foreach (var item in data)
                {
                    List<string> ss = new List<string>();
// foreach (var point in item.SourcePath) ss.Add(point.X + ";" + point.Y);
                    list.Add(string.Join(",", ss));
                }
                File.WriteAllLines("D:\\csvFile.csv", list);
            }
        }
        public void read(Pluklist content)
        {
            foreach (Item line in content.Lines)
            {
                if (line.Type == ItemType.Fysisk)
                {
                    if (dictionary.ContainsKey(line.Title))
                    {
                        dictionary[line.Title] += line.Amount;
                    }
                    else
                    {
                        dictionary[line.Title] = line.Amount;
                    }
                }

            }

            foreach (var item in dictionary)
            {
                Console.WriteLine($"\"{item.Key}\": {item.Value}");

            }
        }
    }
}


//{"NETGEAR": 10, "TP-LINK": 5, "D-LINK": 8}