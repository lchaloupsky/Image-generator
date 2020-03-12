using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Image_Generator.Models.Interfaces
{
    interface IPreprocessor
    {
        string Preprocess(string text);
    }
}
