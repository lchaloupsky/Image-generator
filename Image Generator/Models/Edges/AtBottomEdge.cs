﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Image_Generator.Models.Edges
{
    class AtBottomEdge : Edge
    {
        private int MaxWidth { get => this.Right.Width / 2; }
        private int MaxHeight { get => this.Right.Height / 2; }

        protected override void PositionateAgainstRoot(int maxWidth, int maxHeight)
        {
            this.Left.Position = new Vector2(this.GetShift(maxWidth, this.Left.Width), maxHeight - this.Left.Height);
        }

        protected override void PositionateRight(int maxWidth, int maxHeight)
        {
            this.CopyPosition(this.Left, this.Right);
            this.PositionateLeft(MaxWidth, maxHeight);
        }

        protected override void PositionateLeft(int maxWidth, int maxHeight)
        {
            this.RescaleWithMax(this.MaxWidth, this.Left.Width, this.Left);
            this.RescaleWithMax(this.MaxHeight, this.Left.Height, this.Left);
            this.Left.ZIndex++;
            this.Left.Position = this.Right.Position + new Vector2(this.GetShift(this.Right.Width, this.Left.Width), this.Right.Height - this.Left.Height); // maybe do padding? Maybe not. One simple call.
        }
    }
}
