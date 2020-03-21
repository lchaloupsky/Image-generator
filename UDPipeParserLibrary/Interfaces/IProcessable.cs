﻿using Image_Generator.Models.Text_elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDPipeParserLibrary.Interfaces
{
    /// <summary>
    /// Interface for processable elements
    /// </summary>
    interface IProcessable
    {
        int Id { get; }
        string DependencyType { get; set; }

        IProcessable Process(IProcessable element, SentenceGraph graph);
        IProcessable FinalizeProcessing(SentenceGraph graph);
    }
}
