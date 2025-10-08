using Grasshopper;
using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WireHopper
{
    public static class ParamOptions
    {
        public static bool IncludeInputs = true;
        public static bool IncludeOutputs = true;
    }
    public class ParamOperations
    {
        /// <summary>
        /// Change wires only for params that belong to a registry group.
        /// Apply wire display (Hidden/Faint/Default) to all params in the given group.
        /// Can toggle loose params + component inputs + component outputs.
        /// </summary>
        public static void CleanWiresByParam(string group, WireOperations.WireMode mode)
        {
            var doc = Instances.ActiveCanvas != null ? Instances.ActiveCanvas.Document : null;
            if (doc == null) return;

            WireUndoOperations.SaveSnapshot(doc);

            var allParams = new List<IGH_Param>();

            foreach (var o in doc.Objects)
            {
                // Loose / standalone params (sliders, panels, etc.)
                var p = o as IGH_Param;
                if (p != null) allParams.Add(p);

                // Component inputs/outputs (only if toggles are on)
                var c = o as IGH_Component;
                if (c != null)
                {
                    if (ParamOptions.IncludeInputs)
                        allParams.AddRange(c.Params.Input);

                    if (ParamOptions.IncludeOutputs)
                        allParams.AddRange(c.Params.Output);
                }
            }

            int wiresChanged = 0;
            foreach (var param in allParams)
            {
                if (ParamGroupsRegistry.Matches(group, param))
                {
                    WireOperations.ApplyWireMode(param, mode);
                    if (param.Attributes != null) param.Attributes.ExpireLayout();
                    wiresChanged++;
                }
            }

            WireUndoOperations.SaveSnapshot(doc);

            // Redraw Canvas
            if (Instances.ActiveCanvas != null)
            {
                Instances.ActiveCanvas.Invalidate();
                Instances.ActiveCanvas.Refresh();
            }

            if (wiresChanged > 0)
                MessageBox.Show($"{wiresChanged} wires updated for the {group} parameter group)",
                                "WireHopper",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
            else
                MessageBox.Show($"No matching wires were found for the {group} parameter group..",
                                "WireHopper",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
        }
    }
}
