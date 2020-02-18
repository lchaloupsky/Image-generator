using Image_Generator.Models.Interfaces;
using Image_Generator.Models.Text_elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Image_Generator.Models.Edges
{
    /// <summary>
    /// Abstract edge as base class for all edges
    /// </summary>
    abstract class Edge : IPositionateEdge
    {
        public IDrawable Left { get; set; }
        public IDrawable Right { get; set; }

        public Edge Add(IDrawable left, IDrawable right)
        {
            this.Left = left;
            this.Right = right;

            return this;
        }

        public virtual void Positionate(int maxWidth, int maxHeight)
        {
            if (this.Right is Root)
                return; //Do here a method?
            else if (!this.Left.IsPositioned)
                this.PositionateLeft(maxWidth, maxHeight);
            else if (!this.Right.IsPositioned)
                this.PositionateRight(maxWidth, maxHeight);
            else
                Console.WriteLine("COLLISION"); //something like check some condition. then propagate change?

            Console.WriteLine(this); //debug log.
        }

        protected abstract void PositionateRight(int maxWidth, int maxHeight);

        protected abstract void PositionateLeft(int maxWidth, int maxHeight);

        protected abstract bool CheckConcretePosition();

        public virtual bool CheckPosition()
        {
            if (!this.Left.IsPositioned || !this.Right.IsPositioned)
                return false;

            return this.CheckConcretePosition();
        }

        public override string ToString()
        {
            return $"{this.GetType().Name} --> positioning: \n     Left: {this.Left} \n    Right: {this.Right}";
        }
    }
}
