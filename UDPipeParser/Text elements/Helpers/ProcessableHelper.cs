using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageGeneratorInterfaces.Graph;
using ImageGeneratorInterfaces.Graph.DrawableElement;

namespace UDPipeParsing.Text_elements.Helpers
{
    /// <summary>
    /// Helper for basic operations with ProcessableElements
    /// </summary>
    public class ProcessableHelper
    {
        /// <summary>
        /// Removes verbs and all its dependencies from graph
        /// </summary>
        /// <param name="verb">Verb to remove</param>
        /// <param name="graph">Graph</param>
        public void RemoveVerbFromGraph(Verb verb, ISentenceGraph graph)
        {
            // Remove object
            if (verb.Object != null)
                graph.RemoveVertex((IDrawable)verb.Object, true);

            // Remove depending drawables from graph
            verb.DependingDrawables.ForEach(dd => graph.RemoveVertex((IDrawable)dd, true));

            // Remove recursively
            verb.RelatedActions.ForEach(ra => RemoveVerbFromGraph(ra, graph));
        }
    }
}
