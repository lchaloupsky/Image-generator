﻿using ImageGeneratorInterfaces.Graph;
using ImageGeneratorInterfaces.Parsing;
using System;

namespace UDPipeParsing.Text_elements
{
    /// <summary>
    /// Represents coordination conjunction in the sentence
    /// </summary>
    public class Coordination : Element
    {
        public Coordination(int id, string lemma, string dependency) : base(id, lemma, dependency)
        {
            var stringEnumValue = Lemma.ToUpper();

            if(Enum.IsDefined(typeof(CoordinationType), stringEnumValue))
                this.CoordinationType = (CoordinationType)Enum.Parse(typeof(CoordinationType), Lemma.ToUpper());
            else
                this.CoordinationType = CoordinationType.DEFAULT;
        }

        protected override IProcessable ProcessElement(IProcessable element, ISentenceGraph graph)
        {
            return this;
        }

        public override string ToString()
        {
            return this.CoordinationType.ToString();
        }
    }

    /// <summary>
    /// Enum representing coordination type
    /// </summary>
    public enum CoordinationType
    {
        AND, OR, NOR, FOR, SO, YET, BUT, BOTH, DEFAULT
    }
}
