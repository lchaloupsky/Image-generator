using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDPipeParsing.Interfaces
{
    public interface IPreprocessor
    {
        string Preprocess(string sentence);
    }
}
