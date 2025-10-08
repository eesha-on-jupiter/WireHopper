using Grasshopper;
using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WireHopper
{
    public class VisualizePreviewOperations
    {
        public static void SyncWiresWithPreview()
        {
            var doc = Instances.ActiveCanvas?.Document;
            if (doc == null) return;

            WireUndoOperations.SaveSnapshot(doc);

            foreach (var obj in doc.Objects)
            {
                // Default: treat as visible if it doesn't implement IGH_PreviewObject
                bool previewOn = true;

                if (obj is IGH_PreviewObject pvo)
                    previewOn = pvo.IsPreviewCapable && !pvo.Hidden;

                if (obj is IGH_Param looseParam)
                {
                    WireOperations.ApplyWireMode(looseParam, previewOn ? WireOperations.WireMode.Default : WireOperations.WireMode.Hidden);
                    continue;
                }

                if (obj is IGH_Component comp)
                {
                    foreach (var input in comp.Params.Input)
                        WireOperations.ApplyWireMode(input, previewOn ? WireOperations.WireMode.Default : WireOperations.WireMode.Hidden);

                    foreach (var output in comp.Params.Output)
                        WireOperations.ApplyWireMode(output, previewOn ? WireOperations.WireMode.Default : WireOperations.WireMode.Hidden);
                }
            }

            WireUndoOperations.SaveSnapshot(doc);

            Instances.ActiveCanvas?.Refresh();
        }
    }
}
