using Grasshopper;
using Grasshopper.Kernel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace WireHopper
{
    public sealed class WireHopperPriority : GH_AssemblyPriority
    {
        public override GH_LoadingInstruction PriorityLoad()
        {
            // Subscribe once.
            Instances.DocumentServer.DocumentAdded += OnDocumentAdded;
            Instances.DocumentServer.DocumentRemoved += OnDocumentRemoved;

            // If there is already an open doc when plugin loads, pull it in too.
            var doc = Instances.ActiveCanvas?.Document;
            if (doc != null) WireViewOperations.LoadFromDoc(doc);

            return GH_LoadingInstruction.Proceed;
        }

        private static void OnDocumentAdded(GH_DocumentServer sender, GH_Document doc)
            => WireViewOperations.LoadFromDoc(doc);

        private static void OnDocumentRemoved(GH_DocumentServer sender, GH_Document doc)
            => WireViewOperations.ForgetDoc(doc);
    }
}
