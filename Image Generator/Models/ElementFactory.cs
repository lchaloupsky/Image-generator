using Image_Generator.Models.Text_elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Image_Generator.Models
{
    /// <summary>
    /// Class for creating elements
    /// </summary>
    class ElementFactory
    {
        public Element CreateElement(string[] parts)
        {
            Element part = null;

            int pendingID = int.Parse(parts[6]);
            switch (parts[3])
            {
                case "NOUN":
                    part = new Noun(int.Parse(parts[0]), parts[2], parts[7]);
                    break;
                case "ADJ":
                    part = new Adjective(int.Parse(parts[0]), parts[2], parts[7]);
                    break;
                default:
                    break;
            }

            return part;
        }
    }
}
