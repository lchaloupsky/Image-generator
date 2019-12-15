using Image_Generator.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Image_Generator.Models.Text_elements
{
    class Root : Element
    {
        public Root() : base(0) { }

        public override IProcessable Process(IProcessable element)
        {
            return element;
        }
    }
}
