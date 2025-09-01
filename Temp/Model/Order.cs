using ClassLibrary.Enums;

namespace ClassLibrary.Model
{
    public class Order
    {
        // Properties of the Order class used to represent a driver's order
        public string Name { get; set; }
        public string Address { get; set; }
        public Delivery Delivery { get; set; }

        public List<Item> Lines { get; set; } = new();
    }
}
