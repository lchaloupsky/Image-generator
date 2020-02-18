using Image_Generator.Models.Interfaces;
using Image_Generator.Models.Text_elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Image_Generator.Models.Edges
{
    /// <summary>
    /// Default edge for connecting Drawable elements in a sentence
    /// </summary>
    class DefaultEdge : Edge
    {
        protected override void PositionateRight(int maxWidth, int maxHeight)
        {
            this.Right.Position = this.Left.Position + new Vector2(this.Left.Width, 0);
        }

        protected override void PositionateLeft(int maxWidth, int maxHeight)
        {
            this.Left.Position = this.Right.Position - new Vector2(this.Right.Width, 0);
        }

        protected override bool CheckConcretePosition()
        {
            return true;
        }
    }
}
