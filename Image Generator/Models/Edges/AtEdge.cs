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
    class AtEdge : Edge
    {
        protected override void PositionateRight(int maxWidth, int maxHeight)
        {
            this.Left.Width = this.Right.Width;
            this.Left.Height = this.Right.Height;
            this.Left.ZIndex = this.Right.ZIndex;
            this.Left.Position = this.Right.Position;

            this.Right.Width /= 2;
            this.Right.Height /= 2;
            this.Right.ZIndex += 1;
            this.Right.Position = this.Left.Position + new Vector2(this.Right.Width / 2, this.Right.Height);
        }

        protected override void PositionateLeft(int maxWidth, int maxHeight)
        {
            this.Right.Width = this.Left.Width / 2;
            this.Right.Height = this.Left.Height / 2;
            this.Right.ZIndex = this.Left.ZIndex + 1;
            this.Right.Position = this.Left.Position + new Vector2(this.Right.Width / 2, this.Right.Height);
        }

        protected override bool CheckConcretePosition()
        {
            return true;
        }
    }
}
