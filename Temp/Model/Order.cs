using ClassLibrary.Enums;

namespace ClassLibrary.Model
{
    public class Order
    {
        public string Name { get; set; }
        public string Adresse { get; set; }
        public Forsendelse Forsendelse { get; set; }

        public List<Item> Lines { get; set; }
    }
}
