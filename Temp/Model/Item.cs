using ClassLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.Model
{
    public class Item
    {
        // Properties of the Item class used to represent a product item
        public string ProductID { get; set; }
        public string Title { get; set; }
        public ItemType Type { get; set; } // Enum representing the type of item
        public int Amount { get; set; }
    }
}
