using ClassLibrary.Enums;
using ClassLibrary.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    public class PrintHTML
    {
        // Method to generate and print HTML files based on the picking list
        public static void Print(Pluklist plukliste)
        {

            List<string> fysiskeItems = new List<string>();
            foreach (Item item in plukliste.Lines)
            {
                if (item.Type == ItemType.Fysisk) // Making sure to only include physical items in the list
                {
                    fysiskeItems.Add($"<p>* Amount:{item.Amount}  Product ID: {item.ProductID}  Title: {item.Title} </p>"); // Formatting each item as an HTML paragraph
                }
            }

            string fysiskeItemsString = string.Join(" ", fysiskeItems); // Joining all item paragraphs into a single string

            foreach (var item in plukliste.Lines) // Looping through each item in the picking list
            {
                if (item.ProductID == "PRINT-OPGRADE" || item.ProductID == "PRINT-WELCOME") // Checking for specific product IDs to determine the type of HTML to generate based on if it's an upgrade or welcome letter
                {
                    string html = File.ReadAllText(Path.Combine("HTML-Templates", item.ProductID + ".html"));    // Reading the appropriate HTML template file for a relative path in the pc
                    html = html.Replace("[Name]", plukliste.Name);                                               // Replacing placeholders in the HTML template with actual values from the picking list
                    html = html.Replace("[Adresse]", plukliste.Adresse);                                         // Replacing the address placeholder
                    html = html.Replace("[Plukliste]", fysiskeItemsString);                                      // Inserting the list of physical items into the HTML

                    File.WriteAllLines(Path.Combine("Print", plukliste.Name + ".html"), new string[] { html });
                } else if (item.ProductID == "PRINT-OPSIGELSE") // Checking for a specific product ID to determine if it's a cancellation letter
                {
                    string html = File.ReadAllText(Path.Combine("HTML-Templates", "PRINT-OPSIGELSE.html")); // Reading the cancellation letter HTML template
                    html = html.Replace("[Name]", plukliste.Name); // Replacing the name placeholder
                    html = html.Replace("[Adresse]", plukliste.Adresse); // Replacing the address placeholder

                    File.WriteAllLines(Path.Combine("Print", plukliste.Name +".html"), new string[] { html }); // Writing the customized HTML content to a new file in the "Print" directory
                }
            }
        }
    }
}
