﻿using Image_Generator.Models.Text_elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Image_Generator.Models.Interfaces
{
    /// <summary>
    /// Interface for processable elements
    /// </summary>
    interface IProcessable
    {
        int Id { get; }
        string DependencyType { get; set; }
        bool IsNegated { get; }

        IProcessable Process(IProcessable element, SentenceGraph graph);
        IProcessable FinalizeProcessing(SentenceGraph graph);
    }
}
