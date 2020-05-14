using ImageGeneratorInterfaces.Edges;
using ImageGeneratorInterfaces.Edges.Factory;
using ImageGeneratorInterfaces.Graph;
using ImageGeneratorInterfaces.Graph.DrawableElement;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UDPipeParsing.Text_elements.Helpers
{
    public class DrawableHelper
    {
        /// <summary>
        /// Processes two drawable elements.
        /// Tries to create edge between them.
        /// Also checks other options.
        /// </summary>
        /// <param name="graph">Sentence graph</param>
        /// <param name="left">left vertex</param>
        /// <param name="right">right vertex</param>
        /// <param name="leftAdpositions">left adpositions</param>
        /// <param name="rightAdpositions">right adpositions</param>
        /// <param name="finalAction">Actual to do if "of" is found</param>
        /// <param name="isRightSubject">Flag if right vertex is subject</param>
        /// <returns>New edge or null</returns>
        public bool ProcessEdge(ISentenceGraph graph, IEdgeFactory edgeFactory, IDrawable left, IDrawable right, List<Adposition> leftAdpositions, List<Adposition> rightAdpositions, bool isRightSubject, Action finalAction)
        {
            // Get adpositions from adpositions combinations
            List<string> leftAdp = leftAdpositions.SelectMany(a => a.GetAdpositions()).Select(a => a.ToString()).ToList();
            List<string> rightAdp = rightAdpositions.SelectMany(a => a.GetAdpositions()).Select(a => a.ToString()).ToList();
            IPositionateEdge edge = edgeFactory.Create(left, right, leftAdp, rightAdp, isRightSubject);

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
                var oldGroup = right.Group;
                group.CombineIntoGroup(right);
                right.Group.Dispose();
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
                ratio = this.GetDimensionsRatio(drawable.Image.Width, drawable.Image.Height);

            drawable.Width = (int)(drawable.Height * ratio);
        }

        /// <summary>
        /// Gets ration of given dimensions
        /// </summary>
        /// <param name="width">Width</param>
        /// <param name="height">Height</param>
        /// <returns>Calculated ratio</returns>
        public float GetDimensionsRatio(int width, int height)
        {
            return width * 1f / height;
        }
    }
}
