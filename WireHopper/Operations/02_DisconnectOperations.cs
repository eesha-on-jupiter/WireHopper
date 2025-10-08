using Grasshopper;
using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WireHopper.Operations
{
    public class DisconnectOperations
    {
        public static void DisconnectSelected_All()
        {
            var doc = Instances.ActiveCanvas?.Document;
            if (doc == null) return;

            foreach (var obj in doc.SelectedObjects())
            {
                if (obj is IGH_Component comp)
                {
                    // Inputs
                    foreach (var input in comp.Params.Input)
                        input.RemoveAllSources();

                    // Outputs
                    foreach (var output in comp.Params.Output)
                        foreach (var recipient in output.Recipients.ToList())
                            recipient.RemoveSource(output);

                    comp.ExpireSolution(true);
                }
                else if (obj is IGH_Param param)
                {
                    param.RemoveAllSources();
                    foreach (var recipient in param.Recipients.ToList())
                        recipient.RemoveSource(param);

                    param.ExpireSolution(true);
                }
            }

            WireUndoOperations.SaveSnapshot(doc);
        }

        public static void DisconnectSelected_Inputs()
        {
            var doc = Instances.ActiveCanvas?.Document;
            if (doc == null) return;

            foreach (var obj in doc.SelectedObjects())
            {
                if (obj is IGH_Component comp)
                {
                    foreach (var input in comp.Params.Input)
                        input.RemoveAllSources();

                    comp.ExpireSolution(true);
                }
                else if (obj is IGH_Param param)
                {
                    param.RemoveAllSources();
                    param.ExpireSolution(true);
                }
            }

            WireUndoOperations.SaveSnapshot(doc);
        }

        public static void DisconnectSelected_Outputs()
        {
            var doc = Instances.ActiveCanvas?.Document;
            if (doc == null) return;

            foreach (var obj in doc.SelectedObjects())
            {
                if (obj is IGH_Component comp)
                {
                    foreach (var output in comp.Params.Output)
                        foreach (var recipient in output.Recipients.ToList())
                            recipient.RemoveSource(output);

                    comp.ExpireSolution(true);
                }
                else if (obj is IGH_Param param)
                {
                    foreach (var recipient in param.Recipients.ToList())
                        recipient.RemoveSource(param);

                    param.ExpireSolution(true);
                }
            }

            WireUndoOperations.SaveSnapshot(doc);
        }
    }
}
