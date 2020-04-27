using ImageGeneratorInterfaces.Edges;
using ImageGeneratorInterfaces.Edges.Factory;
using ImageGeneratorInterfaces.Graph;
using ImageGeneratorInterfaces.Graph.DrawableElement;
using ImageGeneratorInterfaces.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UDPipeParsing.Text_elements;

namespace UDPipeParsing.Text_elements.Helpers
{
    class DrawableHelper
    {
        /// <summary>
        /// Processes two drawable elements.
        /// Tries to create edge between them.
        /// Also checks other options.
        /// </summary>
        /// <typeparam name="T">Type of first vertex</typeparam>
        /// <typeparam name="U">Type of second vertex</typeparam>
        /// <param name="graph">Sentence graph</param>
        /// <param name="left">left vertex</param>
        /// <param name="right">right vertex</param>
        /// <param name="leftAdpositions">left adpositions</param>
        /// <param name="rightAdpositions">right adpositions</param>
        /// <param name="finalAction">Actial to do if "of" is found</param>
        /// <returns></returns>
        public bool ProcessEdge<T, U>(ISentenceGraph graph, IEdgeFactory factory, T left, U right, List<Adposition> leftAdpositions, List<Adposition> rightAdpositions, Action finalAction)
            where T : IProcessable, IDrawable
            where U : IProcessable, IDrawable
        {
            // Get adpositions from adpositions combinations
            List<string> leftAdp = leftAdpositions.SelectMany(a => a.GetAdpositions()).Select(a => a.ToString()).ToList();
            List<string> rightAdp = rightAdpositions.SelectMany(a => a.GetAdpositions()).Select(a => a.ToString()).ToList();
            IPositionateEdge edge = this.EdgeFactory.Create(left, right, leftAdp, rightAdp);

            // Clear used adpositions
            if (leftAdp.Count == 0)
                leftAdpositions.Clear();
            if (rightAdp.Count == 0)
                rightAdpositions.Clear();

            // Add only not null edge
            if (edge != null)
                graph.AddEdge(edge);
            else
            {
                // Check if drawable contains "of" -> then it is an extension of this
                if (rightAdpositions.Count == 1 && rightAdpositions[0].ToString() == "of")
                {
                    // Replace vertex in graph
                    graph.ReplaceVertex(left, right);

                    // Run final action
                    finalAction();
                }
                else
                    graph.AddVertex(right);
            }

            return edge != null;
        }

        /// <summary>
        /// Combines two drawables into one group. Sets the group for both
        /// </summary>
        /// <param name="left">Left drawable of edge</param>
        /// <param name="right">Right drawable of edge</param>
        public void CombineIntoGroup(IDrawable left, IDrawable right)
        {
            // If drawable is already in group, use that group
            if (right is MetaNoun)
            {
                right.CombineIntoGroup(left);
                left.Group = (MetaNoun)right;
                return;
            }

            IDrawableGroup group = null;

            //Create new group
            if (left.Group == null && right.Group == null)
                group = new MetaNoun(left, right);

            // Use drawable group
            else if (left.Group == null)
            {
                group = right.Group;
                group.CombineIntoGroup(left);
            }

            // Use this group
            else
            {
                group = left.Group;
                group.CombineIntoGroup(right);
            }

            // Set to both same group
            left.Group = group;
            right.Group = group;
        }

        /// <summary>
        /// Resizes drawable to preserve width/height ration of the loaded image
        /// </summary>
        /// <param name="drawable">Drawable to resize</param>
        public void ResizeToImage(IDrawable drawable)
        {
            float ratio;
            lock (drawable.Image)
                ratio = drawable.Image.Width * 1f / drawable.Image.Height;

            drawable.Width = (int)(drawable.Height * ratio);
        }
    }
}
