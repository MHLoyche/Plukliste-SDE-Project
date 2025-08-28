using ClassLibrary.Enums;

namespace ClassLibrary.Model
{
    public class Order
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public Delivery Delivery { get; set; }

        public List<Item> Lines { get; set; } = new();
    }
}
