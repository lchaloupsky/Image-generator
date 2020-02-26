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
    class OnTopEdge : Edge
    {
        protected override void PositionateRight(int maxWidth, int maxHeight)
        {
            this.Right.Position = this.Left.Position + new Vector2(this.GetShift(this.Left.Width, this.Right.Width), this.Left.Height);
        }

        protected override void PositionateLeft(int maxWidth, int maxHeight)
        {
            this.Left.Position = this.Right.Position - new Vector2(this.GetShift(this.Left.Width, this.Right.Width), this.Left.Height);
        }
    }
}
