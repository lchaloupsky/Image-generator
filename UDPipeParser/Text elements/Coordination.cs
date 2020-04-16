using ImageGeneratorInterfaces.Graph;
using ImageGeneratorInterfaces.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDPipeParsing.Text_elements
{
    public class Coordination : Element
    {
        public Coordination(int Id, string Lemma, string Dependency) : base(Id, Lemma, Dependency)
        {
             this.CoordinationType = (CoordinationType)Enum.Parse(typeof(CoordinationType),Lemma.ToUpper());
        }

        public override IProcessable ProcessElement(IProcessable element, ISentenceGraph graph)
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
