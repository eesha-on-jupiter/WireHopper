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
    public class LengthOperations
    {
        /// <summary>
        /// Hide wires shorter than a given pixel length (e.g. 100px).
        /// </summary>
        public static void CleanWiresByLength(double maxLength, WireOperations.WireMode mode)
        {
            var doc = Instances.ActiveCanvas?.Document;
            if (doc == null) return;          

            int wiresChanged = 0;

            foreach (var obj in doc.Objects)
            {
                // Case 1: Standalone param
                if (obj is IGH_Param param)
                {
                    CheckAndHideLongConnections(param, maxLength, mode, ref wiresChanged);
                }

                // Case 2: Component inputs/outputs
                if (obj is IGH_Component comp)
                {
                    foreach (var input in comp.Params.Input)
                        CheckAndHideLongConnections(input, maxLength, mode, ref wiresChanged);

                    foreach (var output in comp.Params.Output)
                        CheckAndHideLongConnections(output, maxLength, mode, ref wiresChanged);
                }

                // Case 3: Param containers with nested sources/recipients
                if (obj is IGH_Param container)
                {
                    foreach (var src in container.Sources)
                        CheckAndHideLongConnections(src, maxLength, mode, ref wiresChanged);

                    foreach (var rec in container.Recipients)
                        CheckAndHideLongConnections(rec, maxLength, mode, ref wiresChanged);
                }
            }

            WireUndoOperations.SaveSnapshot(doc);

            Instances.ActiveCanvas?.Refresh();

            if (wiresChanged > 0)
                MessageBox.Show($"{wiresChanged} wires changed (>{maxLength}px)",
                                "WireHopper",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
            else
                MessageBox.Show($"No wires longer than {maxLength}px were found.",
                                "WireHopper",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
        }

        private static void CheckAndHideLongConnections(IGH_Param param, double maxLength, WireOperations.WireMode mode, ref int WiresChanged)
        {
            foreach (var source in param.Sources.ToList())
            {
                PointF srcPt = source.Attributes.OutputGrip;
                PointF dstPt = param.Attributes.InputGrip;

                double length = Distance(srcPt, dstPt);

                if (length > maxLength)
                {
                    WireOperations.ApplyWireMode(param, mode);
                    WiresChanged++;
                }
            }
        }

        /// <summary>
        /// Set wire modes based on their relative length.
        /// Top 1/3 longest wires = Default, middle 1/3 = Faint, bottom 1/3 = Hidden.
        /// </summary>
        public static void SetWireModeByRelativeLength()
        {
            var doc = Instances.ActiveCanvas?.Document;
            if (doc == null) return;

            // Collect all lengths
            var wireLengths = new List<(IGH_Param param, double length)>();

            foreach (var obj in doc.Objects)
            {
                if (obj is IGH_Param param)
                    CollectWireLengths(param, wireLengths);

                if (obj is IGH_Component comp)
                {
                    foreach (var input in comp.Params.Input)
                        CollectWireLengths(input, wireLengths);

                    foreach (var output in comp.Params.Output)
                        CollectWireLengths(output, wireLengths);
                }
            }

            if (wireLengths.Count == 0) return;

            // Find max length
            double maxLength = wireLengths.Max(w => w.length);
            if (maxLength <= 0) return;

            // Compute thresholds
            double tier1 = maxLength / 3.0;
            double tier2 = (maxLength * 2.0) / 3.0;

            Rhino.RhinoApp.WriteLine($"[WireHopper] Max length = {maxLength:F2}, Tier1 = {tier1:F2}, Tier2 = {tier2:F2}");

            // Assign modes
            foreach (var (param, length) in wireLengths)
            {
                WireOperations.WireMode mode;
                if (length >= tier2)
                    mode = WireOperations.WireMode.Hidden; // longest
                else if (length >= tier1)
                    mode = WireOperations.WireMode.Faint;   // middle
                else
                    mode = WireOperations.WireMode.Default;  // shortest

                WireOperations.ApplyWireMode(param, mode);

                Rhino.RhinoApp.WriteLine($"  Param {param.NickName} (ID: {param.InstanceGuid}) -> Length {length:F2}, Mode {mode}");
            }

            WireUndoOperations.SaveSnapshot(doc);

            Instances.ActiveCanvas?.Refresh();
        }

        private static void CollectWireLengths(IGH_Param param, List<(IGH_Param, double)> lengths)
        {
            foreach (var source in param.Sources)
            {
                PointF srcPt = source.Attributes.OutputGrip;
                PointF dstPt = param.Attributes.InputGrip;
                double length = Distance(srcPt, dstPt);

                lengths.Add((param, length));
            }
        }
        private static double Distance(PointF a, PointF b)
        {
            float dx = a.X - b.X;
            float dy = a.Y - b.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }
    }
}
