namespace ClassLibrary;
public class Pluklist
{
    public string? Name;
    public string? Forsendelse;
    public string? Adresse;
    public List<Item> Lines = new List<Item>();
    public void AddItem(Item item) { Lines.Add(item); }
}

public class Item
{
    public string ProductID;
    public string Title;
    public ItemType Type;
    public int Amount;
}

/*
public class StorageItems
{
    public string Name {  get; set; }
    public int Amount { get; set; }
}

public class StorageResponse : Dictionary<string, StorageItems>
{

}*/

public enum ItemType
{
    Fysisk, Print
}



