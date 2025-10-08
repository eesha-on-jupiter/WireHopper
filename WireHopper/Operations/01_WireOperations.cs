using Grasshopper;
using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WireHopper
{
    public class WireOperations
    {

        public enum WireMode
        {
            Hidden = 0,
            Faint = 1,
        // Define Enum for WireModes
            Default = 2
        }

        /// <summary>
        /// Set ALL wires to Hidden (0), Faint (1) or Default (2)
        /// </summary>
        public static void SetWireModeForAll(WireMode mode)
        {
            var doc = Instances.ActiveCanvas?.Document;
            if (doc == null) return;

            foreach (var obj in doc.Objects)
            {
                // Case 1: Standalone parameter (sliders, panels, etc.)
                if (obj is IGH_Param param)
                {
                    ApplyWireMode(param, mode);
                }

                // Case 2: Any component (with inputs and outputs)
                if (obj is IGH_Component comp)
                {
                    foreach (var input in comp.Params.Input)
                        ApplyWireMode(input, mode);

                    foreach (var output in comp.Params.Output)
                        ApplyWireMode(output, mode);
                }
            }

            // Set the global setting (so the canvas draws consistently)
            Instances.Settings.SetValue("Draw Wires", (int)mode);

            WireUndoOperations.SaveSnapshot(doc);

            // Redraw
            Instances.ActiveCanvas?.Refresh();
        }

        public static void SetWireModeForSelection(WireMode mode)
        {
            var doc = Instances.ActiveCanvas?.Document;
            if (doc == null) return;

            // Loop through only the selected objects
            foreach (var obj in doc.SelectedObjects())
            {
                // Case 1: Standalone parameter (slider, panel, etc.)
                if (obj is IGH_Param param)
                {
                    ApplyWireMode(param, mode);
                }

                // Case 2: Components with inputs/outputs
                if (obj is IGH_Component comp)
                {
                    foreach (var input in comp.Params.Input)
                        ApplyWireMode(input, mode);

                    foreach (var output in comp.Params.Output)
                        ApplyWireMode(output, mode);
                }
            }

            WireUndoOperations.SaveSnapshot(doc);

            // Redraw
            Instances.ActiveCanvas?.Refresh();
        }

        public static void ApplyWireMode(IGH_Param param, WireMode mode)
        {
            switch (mode)
            {
                case WireMode.Hidden:
                    param.WireDisplay = GH_ParamWireDisplay.hidden;
                    break;
                case WireMode.Faint:
                    param.WireDisplay = GH_ParamWireDisplay.faint;
                    break;
                case WireMode.Default:
                    param.WireDisplay = GH_ParamWireDisplay.@default;
                    break;
            }
        }
    }
}
