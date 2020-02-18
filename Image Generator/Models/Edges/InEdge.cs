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
    //TODO Z-index
    class InEdge : Edge
    {
        protected override void PositionateRight(int maxWidth, int maxHeight)
        {
            this.Right.Width = this.Left.Width;
            this.Right.Height = this.Left.Height;
            this.Right.ZIndex = this.Left.ZIndex;
            this.Right.Position = this.Left.Position;

            this.Left.Width /= 2;
            this.Left.Height /= 2;
            this.Left.ZIndex++;
            this.Left.Position = this.Left.Position + new Vector2(this.Left.Width / 2, this.Left.Height);
        }

        protected override void PositionateLeft(int maxWidth, int maxHeight)
        {
            this.Left.Width = this.Right.Width / 2;
            this.Left.Height = this.Right.Height / 2;
            this.Left.ZIndex = this.Right.ZIndex + 1;
            this.Left.Position = this.Right.Position + new Vector2(this.Left.Width / 2, this.Left.Height);
        }

        protected override bool CheckConcretePosition()
        {
            return true;
        }
    }
}
