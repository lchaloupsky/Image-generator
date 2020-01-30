using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Image_Generator.Models.Interfaces
{
    interface IPositionable
    {
        int X { get; set; }
        int Y { get; set; }
        int Width { get; set; }
        int Height { get; set; }
        bool IsPositioned { get; set; }
    }
}
