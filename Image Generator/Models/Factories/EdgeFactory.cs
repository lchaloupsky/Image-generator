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
        private Root Root { get; }

        public EdgeFactory(Root Root)
        {
            this.Root = Root;
        }

        public Edge Create(IDrawable left, List<Adposition> adpositions)
        {
            return GetEdge(string.Join(" ", adpositions).ToLower()).Add(left, this.Root);
        }

        public Edge Create(IDrawable left, IDrawable right, List<Adposition> adpositions)
        {
            Edge edge = GetEdge(string.Join(" ", adpositions).ToLower());
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
                default: return new DefaultEdge();
            }
        }

        private bool CheckIfParameterIsSubject(IDrawable parameter)
        {
            return ((IProcessable)parameter).DependencyType == "nsubj";
        }
    } 
}
