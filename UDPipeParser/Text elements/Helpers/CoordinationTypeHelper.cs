namespace UDPipeParsing.Text_elements.Helpers
{
    /// <summary>
    /// Helping with coordination types and their semantics
    /// </summary>
    public class CoordinationTypeHelper
    {
        /// <summary>
        /// Check if actual coordination is allowed
        /// </summary>
        /// <returns></returns>
        public bool IsAllowedCoordination(CoordinationType type)
        {
            return type != CoordinationType.OR && type != CoordinationType.BUT;
        }

        /// <summary>
        /// Check if actual coordination is supposed to merge elements
        /// </summary>
        /// <returns></returns>
        public bool IsMergingCoordination(CoordinationType type)
        {
            return type == CoordinationType.AND || type == CoordinationType.NOR;
        }
    }
}
