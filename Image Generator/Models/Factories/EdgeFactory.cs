using Image_Generator.Models.Edges;
using Image_Generator.Models.Interfaces;
using Image_Generator.Models.Text_elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Image_Generator.Models.Factories
{
    class EdgeFactory
    {
        public Edge Create<T1>(T1 left, List<Adposition> adpositions) where T1 : IDrawable, IProcessable
        {
            Edge edge = this.GetOneSidedEdge(string.Join(" ", adpositions.SelectMany(x => x.GetAdpositions())).ToLower());
            return edge?.Add(left, null);
        }

        public Edge Create<T1, T2>(T1 left, T2 right, List<string> leftAdpositions, List<string> rightAdpositions)
            where T1 : IDrawable, IProcessable
            where T2 : IDrawable, IProcessable
        {
            Edge edge = this.Create(left, right, leftAdpositions.Concat(rightAdpositions).ToList());
            if (edge == null)
            {
                if (this.CheckIfParameterIsSubject(right))
                {
                    edge = this.Create(left, right, leftAdpositions);
                    if (edge != null)
                        leftAdpositions.Clear();
                }
                else
                {
                    edge = this.Create(left, right, rightAdpositions);
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

        private Edge Create<T1, T2>(T1 left, T2 right, List<string> adpositions)
            where T1 : IDrawable, IProcessable
            where T2 : IDrawable, IProcessable
        {
            Edge edge = GetEdge(string.Join(" ", adpositions).ToLower());

            if (edge == null)
                return null;

            if (this.CheckIfParameterIsSubject(right))
                return edge.Add(right, left);

            return edge.Add(left, right);
        }

        private Edge GetOneSidedEdge(string edgeType)
        {
            switch (edgeType)
            {
                case "at top":
                case "on top": return new OnTopEdge();
                case "on left": return new ToLeftEdge();
                case "on right": return new ToRightEdge();
                case "in midst":
                case "in middle": return new InMiddleEdge();
                case "at bottom": return new AtBottomEdge();
                case "in left top corner":
                case "in left bottom corner":
                case "in right top corner":
                case "in right bottom corner":
                case "in right corner":
                case "in left corner":
                case "in bottom corner":
                case "in top corner":
                case "in corner": return this.GetCornerEdge(edgeType);
                default: return null;
            }
        }

        private Edge GetEdge(string edgeType)
        {
            switch (edgeType)
            {
                case "above":
                case "up":
                case "upon":
                case "over":
                case "on": return new OnEdge();
                case "inside":
                case "within":
                case "into":
                case "through":
                case "in": return new InEdge();
                case "at": return new AtEdge();
                case "below":
                case "beneath":
                case "underneath":
                case "under": return new UnderEdge();
                case "on edge of":
                case "at top of": 
                case "on top of": return new OnTopEdge();
                case "in midst of":
                case "in middle of": return new InMiddleEdge();
                case "beside":
                case "by":
                case "to":
                case "against":
                case "from":
                case "to left of": return new ToLeftEdge();
                case "near":
                case "next to":
                case "with":
                case "along":
                case "for":
                case "to right of": return new ToRightEdge();
                case "at bottom of": return new AtBottomEdge();
                case "outside":
                case "outside of":
                case "around":
                case "out of":
                case "in front of": return new InFrontEdge();
                case "past":
                case "beyond":
                case "in behind of":
                case "in behind from":
                case "behind": return new BehindEdge();
                case "in behind": return new BehindEdge(true);
                case "in left top corner of": 
                case "in left bottom corner of": 
                case "in right top corner of": 
                case "in right bottom corner of": 
                case "in right corner of": 
                case "in left corner of": 
                case "in bottom corner of":
                case "in top corner of":
                case "in corner of":
                case "on corner":
                case "on left top corner":
                case "on left bottom corner":
                case "on right top corner":
                case "on right bottom corner":
                case "on right corner":
                case "on left corner":
                case "on bottom corner":
                case "on top corner":
                case "on corner of":
                case "on left top corner of":
                case "on left bottom corner of":
                case "on right top corner of":
                case "on right bottom corner of":
                case "on right corner of":
                case "on left corner of":
                case "on bottom corner of": return GetCornerEdge(edgeType.Replace(" of",""));
                default: return null;
            }
        }

        private Edge GetCornerEdge(string type)
        {
            switch (type)
            {
                case "in left top corner": return new InCornerEdge(HorizontalPlace.LEFT, VerticalPlace.TOP);
                case "in left bottom corner": return new InCornerEdge(HorizontalPlace.LEFT, VerticalPlace.BOTTOM);
                case "in right top corner": return new InCornerEdge(HorizontalPlace.RIGHT, VerticalPlace.TOP);
                case "in right bottom corner": return new InCornerEdge(HorizontalPlace.RIGHT, VerticalPlace.BOTTOM);
                case "in right corner": return new InCornerEdge(HorizontalPlace.RIGHT);
                case "in left corner": return new InCornerEdge(HorizontalPlace.LEFT);
                case "in bottom corner": return new InCornerEdge(VerticalPlace.BOTTOM);
                case "in top corner": return new InCornerEdge(VerticalPlace.TOP);
                case "in corner": return new InCornerEdge();
                case "on left top corner": return new OnCornerEdge(HorizontalPlace.LEFT, VerticalPlace.TOP);
                case "on left bottom corner": return new OnCornerEdge(HorizontalPlace.LEFT, VerticalPlace.BOTTOM);
                case "on right top corner": return new OnCornerEdge(HorizontalPlace.RIGHT, VerticalPlace.TOP);
                case "on right bottom corner": return new OnCornerEdge(HorizontalPlace.RIGHT, VerticalPlace.BOTTOM);
                case "on right corner": return new OnCornerEdge(HorizontalPlace.RIGHT);
                case "on left corner": return new OnCornerEdge(HorizontalPlace.LEFT);
                case "on bottom corner": return new OnCornerEdge(VerticalPlace.BOTTOM);
                case "on top corner": return new OnCornerEdge(VerticalPlace.TOP);
                case "on corner": return new OnCornerEdge();
                default: return null;
            }
        }

        private bool CheckIfParameterIsSubject<T1>(T1 parameter) where T1 : IDrawable, IProcessable
        {
            return parameter.DependencyType == "nsubj";
        }
    }
}
