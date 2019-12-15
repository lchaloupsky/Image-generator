using Image_Generator.Models.Text_elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Image_Generator.Models.Interfaces
{
    interface IProcessable
    {
        int Id { get; }

        IProcessable Process(IProcessable element);
    }
}
