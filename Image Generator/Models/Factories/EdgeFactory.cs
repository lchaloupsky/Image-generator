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
            Edge edge = this.GetEdge(string.Join(" ", adpositions.SelectMany(x => x.GetAdpositions())).ToLower());
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
                    leftAdpositions.Clear();
                }
                else
                {
                    edge = this.Create(left, right, rightAll);
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

        private Edge GetEdge(string edgeType)
        {
            switch (edgeType)
            {
                case "on": return new OnEdge();
                case "in": return new InEdge();
                case "at": return new AtEdge();
                case "under": return new UnderEdge();
                case "on top of":
                case "on top": return new OnTopEdge();
                //default: return new DefaultEdge();
                default: return null;
            }
        }

        private bool CheckIfParameterIsSubject<T1>(T1 parameter) where T1 : IDrawable, IProcessable
        {
            return parameter.DependencyType == "nsubj";
        }
    } 
}
