using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    internal interface IScanner
    {
        void convert(Dictionary<string, int> content);
        void read(Pluklist content);
    }
}
