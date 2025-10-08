using Grasshopper;
using Grasshopper.Kernel;
using Rhino;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WireHopper.Operations
{
    public class VisualizeDataOperations
    {
        public static void SetWireModeByData()
        {
            var doc = Instances.ActiveCanvas?.Document;
            if (doc == null) return;

            WireUndoOperations.SaveSnapshot(doc);

            var allParams = new List<IGH_Param>();
            foreach (var obj in doc.Objects)
            {
                if (obj is IGH_Param param)
                    allParams.Add(param);

                if (obj is IGH_Component comp)
                {
                    allParams.AddRange(comp.Params.Input);
                    allParams.AddRange(comp.Params.Output);
                }
            }

            if (allParams.Count == 0) return;

            // Find the largest VolatileDataCount
            int maxCount = allParams.Max(p => p.VolatileDataCount);
            if (maxCount == 0) maxCount = 1; 

            // Compute thresholds (thirds of max)
            int tier1 = maxCount / 3; 
            int tier2 = (maxCount * 2) / 3; 

            foreach (var param in allParams)
            {
                int count = param.VolatileDataCount;

                WireOperations.WireMode mode;
                if (count >= tier2)
                    mode = WireOperations.WireMode.Default;  // top third
                else if (count >= tier1)
                    mode = WireOperations.WireMode.Faint;    // middle third
                else
                    mode = WireOperations.WireMode.Hidden;   // bottom third

                WireOperations.ApplyWireMode(param, mode);
            }

            WireUndoOperations.SaveSnapshot(doc);

            Instances.ActiveCanvas?.Refresh();
        }
    }
}
