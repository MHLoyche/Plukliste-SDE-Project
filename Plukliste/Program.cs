using System.Net.Quic;
using ClassLibrary;
using ClassLibrary.Model;
using Temp;

namespace Plukliste;

class PluklisteProgram { 

    static void Main()
    {
        // Setup Variables
        char readKey = ' '; // Menu Input
        List<string> files; // List of files in export directory
        var index = -1; // Current file index
        var standardColor = Console.ForegroundColor; // Standard console color
        Pluklist plukliste; // Current plukliste object read from XML
        XMLToCSVConverter fileConv = new XMLToCSVConverter(); // Helper for CSV conversion

        // Ensuring import and export folders exist
        Directory.CreateDirectory("import");

        if (!Directory.Exists("export"))
        {
            Console.WriteLine("Directory \"export\" not found");
            Console.ReadLine();
            return;
        }
        // Get all files from export directory
        files = Directory.EnumerateFiles("export").ToList();

        Console.WriteLine(Directory.GetCurrentDirectory());
        
        // Main Console program that runs until user presses 'Q'
        while (readKey != 'Q')
        {
            if (files.Count == 0)
            {
                Console.WriteLine("No files found.");
                return;
            }
            else
            {
                if (index == -1) index = 0; // Reinialize the index to redetermine current file (incase of refresh)

                Console.WriteLine($"Plukliste {index + 1} af {files.Count}");
                Console.WriteLine($"\nfile: {files[index]}");

                // Deserialize current XML file to plukliste object
                FileStream file = File.OpenRead(files[index]);
                System.Xml.Serialization.XmlSerializer xmlSerializer =
                    new System.Xml.Serialization.XmlSerializer(typeof(Pluklist));
                plukliste = (Pluklist?)xmlSerializer.Deserialize(file);

                // Print plukliste
                if (plukliste != null && plukliste.Lines != null)
                {
                    Console.WriteLine("\n{0, -13}{1}", "Name:", plukliste.Name);
                    Console.WriteLine("{0, -13}{1}", "Forsendelse:", plukliste.Forsendelse);
                    Console.WriteLine("\n{0,-7}{1,-9}{2,-20}{3}", "Antal", "Type", "Produktnr.", "Navn");
                    foreach (var item in plukliste.Lines)
                    {
                        Console.WriteLine("{0,-7}{1,-9}{2,-20}{3}", item.Amount, item.Type, item.ProductID, item.Title);
                    }
                }
                file.Close();
            }

            // Menu Options
            Console.WriteLine("\n\nOptions:");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("Q");
            Console.ForegroundColor = standardColor;
            Console.WriteLine("uit"); // Quit
            if (index >= 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("A");
                Console.ForegroundColor = standardColor;
                Console.WriteLine("fslut plukseddel"); // Complete current plukliste
            }
            if (index > 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("F");
                Console.ForegroundColor = standardColor;
                Console.WriteLine("orrige plukseddel"); // Previous plukliste
            }
            if (index < files.Count - 1)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("N");
                Console.ForegroundColor = standardColor;
                Console.WriteLine("æste plukseddel"); // Next plukliste
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("G");
            Console.ForegroundColor = standardColor;
            Console.WriteLine("enindlæs pluksedler"); // Refresh

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("S");
            Console.ForegroundColor = standardColor;
            Console.WriteLine("can plukliste"); // Scan plukliste

            // Read user input
            readKey = Console.ReadKey().KeyChar;
            if (readKey >= 'a') readKey -= (char)('a' - 'A'); // to uppercase
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Red; //status message in red

            // Switch case for menu options
            switch (readKey)
            {
                case 'G':
                    files = Directory.EnumerateFiles("export").ToList();
                    index = -1;
                    Console.WriteLine("Pluklister genindlæst");
                    break;
                case 'F':
                    if (index > 0) index--;
                    break;
                case 'N':
                    if (index < files.Count - 1) index++;
                    break;
                case 'A':
                    PrintHTML.Print(plukliste);
                    // Move file to import directory
                    var filewithoutPath = files[index].Substring(files[index].LastIndexOf('\\'));
                    File.Move(files[index], string.Format(@"import\\{0}", filewithoutPath));
                    Console.WriteLine($"Plukseddel {files[index]} afsluttet.");
                    files.Remove(files[index]);
                    if (index == files.Count) index--;
                    break;
                case 'S':
                    fileConv.read(plukliste);
                    Console.WriteLine($"Plukseddel {files[index]} er læst.");
                    break;
            }
            Console.ForegroundColor = standardColor; // resets color
        }
        // After quitting, converts all read pluklister to CSV
        fileConv.convert();
    }
}
