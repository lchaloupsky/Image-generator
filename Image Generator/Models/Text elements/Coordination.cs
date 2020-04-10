using Image_Generator.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Image_Generator.Models.Text_elements
{
    class Coordination : Element
    {
        public Coordination(int Id, string Lemma, string Dependency) : base(Id, Lemma, Dependency)
        {
             this.CoordinationType = (CoordinationType)Enum.Parse(typeof(CoordinationType),Lemma.ToUpper());
        }

        public override IProcessable ProcessElement(IProcessable element, SentenceGraph graph)
        {
            return this;
        }

        public override string ToString()
        {
            return this.CoordinationType.ToString();
        }
    }

    public enum CoordinationType
    { AND, OR, NOR, FOR, SO, YET, BUT, BOTH }
}
