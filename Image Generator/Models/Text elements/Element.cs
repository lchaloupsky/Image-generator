using Image_Generator.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Image_Generator.Models.Text_elements
{
    abstract class Element : IProcessable
    {
        public int Id { get; }

        public string Lemma { get; set; }

        public string DependencyType { get; private set; }

        public Element(int Id, string Lemma, string DependencyType)
        {
            this.Id = Id;
            this.Lemma = Lemma;
            this.DependencyType = DependencyType;
        }

        public Element(int Id) : this(Id, "", "") { }

        public override string ToString()
        {
            return this.Lemma;
        }

        public abstract IProcessable Process(IProcessable element);
    }
}
