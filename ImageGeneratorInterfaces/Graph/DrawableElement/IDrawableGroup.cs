using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageGeneratorInterfaces.Graph.DrawableElement
{
    /// <summary>
    /// Wrapper for IDrawable
    /// </summary>
    public interface IDrawableGroup : IDrawable
    {
        // Indicator if group is still valid
        bool IsValid { get; set; }
    }
}
