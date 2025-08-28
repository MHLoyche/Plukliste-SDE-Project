using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.HelperClasses
{
    public static class EnumUtil
    {
        public static IEnumerable<T> GetValues<T>() // Generic method to get all values of an enum type T where T : Enum
        {
            return Enum.GetValues(typeof(T)).Cast<T>(); // Retrieves and casts the enum values to the specified type T
        }
    }
}
