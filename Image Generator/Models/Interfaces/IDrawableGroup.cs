using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Image_Generator.Models.Interfaces
{
    /// <summary>
    /// Wrapper for IDrawable
    /// </summary>
    interface IDrawableGroup : IDrawable
    {
        // Indicator if group is still valid
        bool IsValid { get; set; }
    }
}
