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

        protected string Lemma { get; set; }

        public string DependencyType { get; set; }

        public Element(int Id, string Lemma, string DependencyType)
        {
            this.Id = Id;
            this.Lemma = Lemma;
            this.DependencyType = DependencyType;
        }

        public Element(int Id)
        {
            this.Id = Id;
            this.Lemma = "";
            this.DependencyType = "";
        }

        public abstract IProcessable Process(IProcessable element);
    }
}
