using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Image_Generator.Models.Interfaces
{
    /// <summary>
    /// Preprocessor interface
    /// </summary>
    interface IPreprocessor
    {
        string Preprocess(string text);
    }
}
