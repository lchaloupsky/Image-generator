using ImageGeneratorInterfaces.Edges;
using ImageGeneratorInterfaces.Edges.Factory;
using ImageGeneratorInterfaces.Graph.DrawableElement;
using ImageGeneratorInterfaces.Parsing;
using ImagePositioner.Edges;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImagePositioner.Factories
{
    /// <summary>
    /// Implementation of IEdgeFactory interface
    /// </summary>
    public class EdgeFactory : IEdgeFactory
    {
        public IAbsolutePositionateEdge Create(IDrawable left, List<string> adpositions)
        {
            AbsoluteEdge edge = this.GetOneSidedEdge(string.Join(" ", adpositions));
            edge?.Add(left, null);

            return edge;
        }

        public IPositionateEdge Create(IDrawable left, IDrawable right, List<string> leftAdpositions, List<string> rightAdpositions, bool isRightSubject)
        {
            Edge edge = this.Create(left, right, leftAdpositions.Concat(rightAdpositions).ToList(), isRightSubject);
            if (edge == null)
            {
                if (isRightSubject)
                {
                    edge = this.Create(left, right, leftAdpositions, isRightSubject);
                    if (edge != null)
                        leftAdpositions.Clear();
                }
                else
                {
                    edge = this.Create(left, right, rightAdpositions, isRightSubject);
                    if (edge != null)
                        rightAdpositions.Clear();
                }
            }
            else
            {
                leftAdpositions.Clear();
                rightAdpositions.Clear();
            }

            return edge;
        }

        /// <summary>
        /// Tries to create edge with given adpositions 
        /// </summary>
        /// <param name="left">Left vertex</param>
        /// <param name="right">Right vertex</param>
        /// <param name="adpositions">Adposition list</param>
        /// <param name="isRightSubject">Flag if right is subject</param>
        /// <returns>Edge if exists for given adpositions. Else null.</returns>
        private Edge Create(IDrawable left, IDrawable right, List<string> adpositions, bool isRightSubject)
        {
            Edge edge = GetEdge(string.Join(" ", adpositions).ToLower());

            if (edge == null)
                return null;

            if (isRightSubject)
                edge.Add(right, left);
            else
                edge.Add(left, right);

            return edge;
        }

        /// <summary>
        /// Creates absolute edges
        /// </summary>
        /// <param name="edgeType">adposition or adpositinal phrase</param>
        /// <returns>Edge if exists for given edgeType</returns>
        private AbsoluteEdge GetOneSidedEdge(string edgeType)
        {
            switch (edgeType)
            {
                case "at top":
                case "down from top":
                case "from top":
                case "on top": return new OnTopEdge();
                case "on left": return new ToLeftEdge();
                case "on right": return new ToRightEdge();
                case "in midst":
                case "in middle": return new InMiddleEdge();
                case "at bottom": return new AtBottomEdge();
                case "on corner":
                case "on left top corner":
                case "on top left corner":
                case "on left bottom corner":
                case "on bottom left corner":
                case "on right top corner":
                case "on top right corner":
                case "on right bottom corner":
                case "on bottom right corner":
                case "on right corner":
                case "on left corner":
                case "on bottom corner":
                case "on top corner": return (AbsoluteEdge)this.GetCornerEdge(edgeType.Replace("on ", "in "));
                case "in top left corner":
                case "in left top corner":
                case "in bottom left corner":
                case "in left bottom corner":
                case "in top right corner":
                case "in right top corner":
                case "in bottom right corner":
                case "in right bottom corner":
                case "in right corner":
                case "in left corner":
                case "in bottom corner":
                case "in top corner":
                case "in corner": return (AbsoluteEdge)this.GetCornerEdge(edgeType);
                default: return null;
            }
        }

        /// <summary>
        /// Creates two sided edge if exists
        /// </summary>
        /// <param name="edgeType">adposition or adpositional phrase</param>
        /// <returns>Edge coresponding to edgeType</returns>
        private Edge GetEdge(string edgeType)
        {
            switch (edgeType)
            {
                case "above":
                case "up":
                case "upon":
                case "over":
                case "onto":
                case "on": return new OnEdge();
                case "inside":
                case "within":
                case "into":
                case "in": return new InEdge();
                case "at": return new AtEdge();
                case "below":
                case "beneath":
                case "underneath":
                case "down":
                case "under": return new UnderEdge();
                case "on edge of":
                case "at top of":
                case "down from top of":
                case "from top of":
                case "on top of": return new OnTopEdge();
                case "in midst of":
                case "between":
                case "among":
                case "across":
                case "through":
                case "in middle of": return new InMiddleEdge();
                case "beside":
                case "by":
                case "to":
                case "against":
                case "from":
                case "towards":
                case "after":
                case "to left of": return new ToLeftEdge();
                case "near":
                case "next to":
                case "with":
                case "along":
                case "for":
                case "along with":
                case "to right of": return new ToRightEdge();
                case "at bottom of": return new AtBottomEdge();
                case "outside":
                case "outside of":
                case "around":
                case "out of":
                case "out from":
                case "in front of": return new InFrontEdge();
                case "past":
                case "beyond":
                case "in behind of":
                case "in behind from":
                case "opposite":
                case "opposite of":
                case "behind": return new BehindEdge();
                case "in behind": return new BehindEdge(true);
                case "in left top corner of":
                case "in top left corner of":
                case "in left bottom corner of":
                case "in bottom left corner of":
                case "in right top corner of":
                case "in top right corner of":
                case "in right bottom corner of":
                case "in bottom right corner of":
                case "in right corner of":
                case "in left corner of":
                case "in bottom corner of":
                case "in top corner of":
                case "in corner of":
                case "on corner of":
                case "on left top corner of":
                case "on top left corner of":
                case "on left bottom corner of":
                case "on bottom left corner of":
                case "on right top corner of":
                case "on top right corner of":
                case "on right bottom corner of":
                case "on bottom right corner of":
                case "on right corner of":
                case "on left corner of":
                case "on bottom corner of": return GetCornerEdge(edgeType.Replace(" of", ""));
                default: return null;
            }
        }

        /// <summary>
        /// Method for creating corner edges.
        /// </summary>
        /// <param name="type">corner edge type</param>
        /// <returns>Corner edge or null.</returns>
        private Edge GetCornerEdge(string type)
        {
            switch (type)
            {
                case "in top left corner":
                case "in left top corner": return new InCornerEdge(HorizontalPlace.LEFT, VerticalPlace.TOP);
                case "in bottom left corner":
                case "in left bottom corner": return new InCornerEdge(HorizontalPlace.LEFT, VerticalPlace.BOTTOM);
                case "in top right corner":
                case "in right top corner": return new InCornerEdge(HorizontalPlace.RIGHT, VerticalPlace.TOP);
                case "in bottom right corner":
                case "in right bottom corner": return new InCornerEdge(HorizontalPlace.RIGHT, VerticalPlace.BOTTOM);
                case "in right corner": return new InCornerEdge(HorizontalPlace.RIGHT);
                case "in left corner": return new InCornerEdge(HorizontalPlace.LEFT);
                case "in bottom corner": return new InCornerEdge(VerticalPlace.BOTTOM);
                case "in top corner": return new InCornerEdge(VerticalPlace.TOP);
                case "in corner": return new InCornerEdge();
                case "on top left corner":
                case "on left top corner": return new OnCornerEdge(HorizontalPlace.LEFT, VerticalPlace.TOP);
                case "on bottom left corner":
                case "on left bottom corner": return new OnCornerEdge(HorizontalPlace.LEFT, VerticalPlace.BOTTOM);
                case "on top right corner":
                case "on right top corner": return new OnCornerEdge(HorizontalPlace.RIGHT, VerticalPlace.TOP);
                case "on bottom right corner":
                case "on right bottom corner": return new OnCornerEdge(HorizontalPlace.RIGHT, VerticalPlace.BOTTOM);
                case "on right corner": return new OnCornerEdge(HorizontalPlace.RIGHT);
                case "on left corner": return new OnCornerEdge(HorizontalPlace.LEFT);
                case "on bottom corner": return new OnCornerEdge(VerticalPlace.BOTTOM);
                case "on top corner": return new OnCornerEdge(VerticalPlace.TOP);
                case "on corner": return new OnCornerEdge();
                default: return null;
            }
        }
    }
}
