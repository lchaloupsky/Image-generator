using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDPipeParsing.Text_elements.Helpers
{
    /// <summary>
    /// Helper for checking dependency types
    /// </summary>
    public class DependencyTypeHelper
    {
        /// <summary>
        /// Checks if dependency expresses "object" relation
        /// </summary>
        /// <param name="dependencyType">dependency type</param>
        /// <returns>True if satisfied</returns>
        public bool IsObject(string dependencyType)
        {
            return dependencyType == "dobj";
        }

        /// <summary>
        /// Checks if dependency expresses "subject" relation
        /// </summary>
        /// <param name="dependencyType">dependency type</param>
        /// <returns>True if satisfied</returns>
        public bool IsSubject(string dependencyType)
        {
            return dependencyType == "nsubj" || dependencyType == "nsubjpass";
        }

        /// <summary>
        /// Checks if dependency expresses "compound" relation
        /// </summary>
        /// <param name="dependencyType">dependency type</param>
        /// <returns>True if satisfied</returns>
        public bool IsCompound(string dependencyType)
        {
            return dependencyType.Contains("compound");
        }

        /// <summary>
        /// Checks if dependency expresses "appositional" relation
        /// </summary>
        /// <param name="dependencyType">dependency type</param>
        /// <returns>True if satisfied</returns>
        public bool IsAppositional(string dependencyType)
        {
            return dependencyType == "appos";
        }

        /// <summary>
        /// Checks if dependency expresses "nominal possesive" relation
        /// </summary>
        /// <param name="dependencyType">dependency type</param>
        /// <returns>True if satisfied</returns>
        public bool IsNominalPossesive(string dependencyType)
        {
            return dependencyType == "nmod:poss";
        }

        /// <summary>
        /// Checks if dependency expresses "possesive" relation
        /// </summary>
        /// <param name="dependencyType">dependency type</param>
        /// <returns>True if satisfied</returns>
        public bool IsPossesive(string dependencyType)
        {
            return dependencyType.Contains(":poss");
        }

        /// <summary>
        /// Checks if dependency expresses "conjuction" relation
        /// </summary>
        /// <param name="dependencyType">dependency type</param>
        /// <returns>True if satisfied</returns>
        public bool IsConjuction(string dependencyType)
        {
            return dependencyType == "conj";
        }

        /// <summary>
        /// Checks if dependency expresses "copula" relation
        /// </summary>
        /// <param name="dependencyType">dependency type</param>
        /// <returns>True if satisfied</returns>
        public bool IsCopula(string dependencyType)
        {
            return dependencyType == "cop";
        }

        /// <summary>
        /// Checks if dependency expresses "numeral modifier" relation
        /// </summary>
        /// <param name="dependencyType">dependency type</param>
        /// <returns>True if satisfied</returns>
        public bool IsNumeralModifier(string dependencyType)
        {
            return dependencyType == "nummod";
        }

        /// <summary>
        /// Checks if dependency expresses "noun phrase" relation
        /// </summary>
        /// <param name="dependencyType">dependency type</param>
        /// <returns>True if satisfied</returns>
        public bool IsNounPhrase(string dependencyType)
        {
            return dependencyType == "nmod:npmod";
        }

        /// <summary>
        /// Checks if dependency expresses "adjectival modifier" relation
        /// </summary>
        /// <param name="dependencyType">dependency type</param>
        /// <returns>True if satisfied</returns>
        public bool IsAdjectivalModifier(string dependencyType)
        {
            return dependencyType == "amod";
        }

        /// <summary>
        /// Checks if dependency expresses "root" relation of the whole dependency tree
        /// </summary>
        /// <param name="dependencyType">dependency type</param>
        /// <returns>True if satisfied</returns>
        public bool IsRoot(string dependencyType)
        {
            return dependencyType == "root";
        }

        /// <summary>
        /// Checks if dependency expresses "name" relation
        /// </summary>
        /// <param name="dependencyType">dependency type</param>
        /// <returns>True if satisfied</returns>
        public bool IsName(string dependencyType)
        {
            return dependencyType == "name";
        }

        /// <summary>
        /// Checks if dependency expresses "mark" relation
        /// </summary>
        /// <param name="dependencyType">dependency type</param>
        /// <returns>True if satisfied</returns>
        public bool IsMark(string dependencyType)
        {
            return dependencyType == "mark";
        }
    }
}
