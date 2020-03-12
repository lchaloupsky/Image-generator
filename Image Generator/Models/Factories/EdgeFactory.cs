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

        public Edge Create<T1, T2>(T1 left, T2 right, List<Adposition> leftAdpositions, List<Adposition> rightAdpositions)
            where T1 : IDrawable, IProcessable
            where T2 : IDrawable, IProcessable
        {
            var leftAll = leftAdpositions.SelectMany(x => x.GetAdpositions()).ToList();
            var rightAll = rightAdpositions.SelectMany(x => x.GetAdpositions()).ToList();

            Edge edge = this.Create(left, right, leftAll.Concat(rightAll).ToList());
            if (edge == null)
            {
                if (this.CheckIfParameterIsSubject(right))
                {
                    edge = this.Create(left, right, leftAll);
                    if (edge != null)
                        leftAdpositions.Clear();
                }
                else
                {
                    edge = this.Create(left, right, rightAll);
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

        public Edge Create<T1, T2>(T1 left, T2 right, List<Adposition> adpositions)
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
                case "on": return new OnEdge();
                case "inside":
                case "within":
                case "in": return new InEdge();
                case "at": return new AtEdge();
                case "below":
                case "beneath":
                case "underneath":
                case "under": return new UnderEdge();
                case "on top of": return new OnTopEdge();
                case "in midst of":
                case "in middle of": return new InMiddleEdge();
                case "beside":
                case "by":
                case "to left of": return new ToLeftEdge();
                case "near":
                case "next to":
                case "with":
                case "along":
                case "to right of": return new ToRightEdge();
                case "at bottom of": return new AtBottomEdge();
                case "outside":
                case "in front of": return new InFrontEdge();
                case "past":
                case "beyond":
                case "behind": return new BehindEdge();
                case "in left top corner of": 
                case "in left bottom corner of": 
                case "in right top corner of": 
                case "in right bottom corner of": 
                case "in right corner of": 
                case "in left corner of": 
                case "in bottom corner of":
                case "in top corner of":
                case "in corner of": return GetCornerEdge(edgeType.Replace(" of",""));
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
                default: return null;
            }
        }

        private bool CheckIfParameterIsSubject<T1>(T1 parameter) where T1 : IDrawable, IProcessable
        {
            return parameter.DependencyType == "nsubj";
        }
    }
}
