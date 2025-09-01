using ClassLibrary.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.Interfaces
{
    internal interface IScanner
    {
        // Methods that will be injected in the class that implements this interface
        void convert(); // Method to convert the picking list data
        void read(Pluklist content); // Method to read and process the picking list
    }
}
