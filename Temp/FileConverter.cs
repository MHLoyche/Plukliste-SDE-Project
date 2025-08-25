using ClassLibrary;
using System.Drawing;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Serialization;

namespace Temp
{
    public class FileConverter : IScanner
    {
        public void convert(FileStream content)
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
                    foreach (var point in item.SourcePath) ss.Add(point.X + ";" + point.Y);
                    list.Add(string.Join(",", ss));
                }
                File.WriteAllLines("D:\\csvFile.csv", list);
            }
        }
    }
}
