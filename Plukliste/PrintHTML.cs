using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plukliste
{
    internal class PrintHTML
    {
        public static void Print(Pluklist plukliste)
        {

            List<string> fysiskeItems = new List<string>();
            foreach (Item item in plukliste.Lines)
            {
                if (item.Type == ItemType.Fysisk)
                {
                    fysiskeItems.Add($"<p>* Amount:{item.Amount}  Product ID: {item.ProductID}  Title: {item.Title} </p>");
                }
            }

            string fysiskeItemsString = string.Join(" ", fysiskeItems);

            foreach (var item in plukliste.Lines)
            {
                if (item.ProductID == "PRINT-OPGRADE" || item.ProductID == "PRINT-WELCOME")
                {
                    string html = File.ReadAllText("C:\\Users\\AweSa\\source\\repos\\Plukliste-master\\Plukliste\\HTML-Templates\\" + item.ProductID + ".html");
                    html = html.Replace("[Name]", plukliste.Name);
                    html = html.Replace("[Adresse]", plukliste.Adresse);
                    html = html.Replace("[Plukliste]", fysiskeItemsString);

                    File.WriteAllLines($"C:\\Users\\AweSa\\source\\repos\\Plukliste-master\\Plukliste\\Print\\{plukliste.Name}.html", new string[] { html });
                } else if (item.ProductID == "PRINT-OPSIGELSE")
                {
                    string html = File.ReadAllText("C:\\Users\\AweSa\\source\\repos\\Plukliste-master\\Plukliste\\HTML-Templates\\PRINT-OPSIGELSE.html");
                    html = html.Replace("[Name]", plukliste.Name);
                    html = html.Replace("[Adresse]", plukliste.Adresse);

                    File.WriteAllLines($"C:\\Users\\AweSa\\source\\repos\\Plukliste-master\\Plukliste\\Print\\{plukliste.Name}.html", new string[] { html });
                }
            }
        }
    }
}
