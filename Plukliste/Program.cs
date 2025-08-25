//Eksempel på funktionel kodning hvor der kun bliver brugt et model lag
using System.Net.Quic;
using ClassLibrary;
using Temp;

namespace Plukliste;

class PluklisteProgram { 

    static void Main()
    {
        //Arrange
        char readKey = ' ';
        List<string> files;
        var index = -1;
        var standardColor = Console.ForegroundColor;
        Pluklist plukliste;
        XMLToCSVConverter fileConv = new XMLToCSVConverter();

        Directory.CreateDirectory("import");

        if (!Directory.Exists("export"))
        {
            Console.WriteLine("Directory \"export\" not found");
            Console.ReadLine();
            return;
        }
        files = Directory.EnumerateFiles("export").ToList();

        Console.WriteLine(Directory.GetCurrentDirectory());
        //ACT
        while (readKey != 'Q')
        {
            if (files.Count == 0)
            {
                Console.WriteLine("No files found.");
                return;
            }
            else
            {
                if (index == -1) index = 0;

                Console.WriteLine($"Plukliste {index + 1} af {files.Count}");
                Console.WriteLine($"\nfile: {files[index]}");

                //read file
                FileStream file = File.OpenRead(files[index]);
                System.Xml.Serialization.XmlSerializer xmlSerializer =
                    new System.Xml.Serialization.XmlSerializer(typeof(Pluklist));
                plukliste = (Pluklist?)xmlSerializer.Deserialize(file);

                //print plukliste
                if (plukliste != null && plukliste.Lines != null)
                {
                    Console.WriteLine("\n{0, -13}{1}", "Name:", plukliste.Name);
                    Console.WriteLine("{0, -13}{1}", "Forsendelse:", plukliste.Forsendelse);
                    //TODO: Add adresse to screen print

                    Console.WriteLine("\n{0,-7}{1,-9}{2,-20}{3}", "Antal", "Type", "Produktnr.", "Navn");
                    foreach (var item in plukliste.Lines)
                    {
                        Console.WriteLine("{0,-7}{1,-9}{2,-20}{3}", item.Amount, item.Type, item.ProductID, item.Title);
                    }
                }
                file.Close();
            }

            //Print options
            Console.WriteLine("\n\nOptions:");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("Q");
            Console.ForegroundColor = standardColor;
            Console.WriteLine("uit");
            if (index >= 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("A");
                Console.ForegroundColor = standardColor;
                Console.WriteLine("fslut plukseddel");
            }
            if (index > 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("F");
                Console.ForegroundColor = standardColor;
                Console.WriteLine("orrige plukseddel");
            }
            if (index < files.Count - 1)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("N");
                Console.ForegroundColor = standardColor;
                Console.WriteLine("æste plukseddel");
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("G");
            Console.ForegroundColor = standardColor;
            Console.WriteLine("enindlæs pluksedler");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("S");
            Console.ForegroundColor = standardColor;
            Console.WriteLine("can plukliste");

            readKey = Console.ReadKey().KeyChar;
            if (readKey >= 'a') readKey -= (char)('a' - 'A'); //HACK: To upper
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Red; //status in red
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
                    //Move files to import directory
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
            Console.ForegroundColor = standardColor; //reset color

        }
        
        fileConv.convert();
    }
}
