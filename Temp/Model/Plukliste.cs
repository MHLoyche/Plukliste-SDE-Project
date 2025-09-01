namespace ClassLibrary.Model;
public class Pluklist
{
    // Properties of the Pluklist class used to represent a picking list
    public string? Name;
    public string? Forsendelse;
    public string? Adresse;
    public List<Item> Lines = new List<Item>();
    // Method to add an item to the picking list
    public void AddItem(Item item) { Lines.Add(item); }
}







