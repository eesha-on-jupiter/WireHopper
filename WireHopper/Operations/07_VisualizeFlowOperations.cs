using Grasshopper;
using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WireHopper
{
    public class VisualizeFlowOperations
    {
        private static HashSet<Guid> _visited = new HashSet<Guid>();

        public static void IsolateFullFlow_All()
        {
            var doc = Instances.ActiveCanvas?.Document;
            if (doc == null) return;

            var selectedObj = doc.SelectedObjects().FirstOrDefault();
            if (selectedObj == null) return;

            HideAllWires(doc);
            _visited.Clear();

            IGH_Component compSel = selectedObj as IGH_Component
                ?? (selectedObj as IGH_Param)?.Attributes?.GetTopLevel.DocObject as IGH_Component;

            if (compSel == null) return;

            TraverseUpstream(compSel, doc);

            _visited.Clear();

            TraverseDownstream(compSel, doc);

            WireUndoOperations.SaveSnapshot(doc);
            Instances.ActiveCanvas?.Refresh();
        }

        public static void IsolateFullFlow_Upstream()
        {
            var doc = Instances.ActiveCanvas?.Document;
            if (doc == null) return;

            var selectedObj = doc.SelectedObjects().FirstOrDefault();
            if (selectedObj == null) return;

            HideAllWires(doc);
            _visited.Clear();

            IGH_Component compSel = selectedObj as IGH_Component
                ?? (selectedObj as IGH_Param)?.Attributes?.GetTopLevel.DocObject as IGH_Component;

            if (compSel != null)
                TraverseUpstream(compSel, doc);

            WireUndoOperations.SaveSnapshot(doc);

            Instances.ActiveCanvas?.Refresh();
        }

        public static void IsolateFullFlow_Downstream()
        {
            var doc = Instances.ActiveCanvas?.Document;
            if (doc == null) return;

            var selectedObj = doc.SelectedObjects().FirstOrDefault();
            if (selectedObj == null) return;

            HideAllWires(doc);
            _visited.Clear();

            IGH_Component compSel = selectedObj as IGH_Component
                ?? (selectedObj as IGH_Param)?.Attributes?.GetTopLevel.DocObject as IGH_Component;

            if (compSel != null)
                TraverseDownstream(compSel, doc);

            WireUndoOperations.SaveSnapshot(doc);

            Instances.ActiveCanvas?.Refresh();
        }

        private static void HideAllWires(GH_Document doc)
        {
            foreach (var obj in doc.Objects)
            {
                if (obj is IGH_Param param)
                    param.WireDisplay = GH_ParamWireDisplay.hidden;

                if (obj is IGH_Component comp)
                {
                    foreach (var input in comp.Params.Input)
                        input.WireDisplay = GH_ParamWireDisplay.hidden;
                    foreach (var output in comp.Params.Output)
                        output.WireDisplay = GH_ParamWireDisplay.hidden;
                }
            }
        }

        private static void TraverseUpstream(IGH_Component comp, GH_Document doc)
        {
            if (comp == null || doc == null) return;
            if (!_visited.Add(comp.InstanceGuid)) return;

            foreach (var input in comp.Params.Input)
                input.WireDisplay = GH_ParamWireDisplay.@default;

            foreach (var input in comp.Params.Input)
            {
                foreach (var source in input.Sources)
                {
                    source.WireDisplay = GH_ParamWireDisplay.@default;

                    var upstreamComp = GetOwningComponent(source, doc) ?? FollowUntilComponent(source, doc);
                    if (upstreamComp != null)
                        TraverseUpstream(upstreamComp, doc);
                }
            }
        }

        private static void TraverseDownstream(IGH_Component comp, GH_Document doc)
        {
            if (comp == null || doc == null) return;
            if (!_visited.Add(comp.InstanceGuid)) return;

            foreach (var output in comp.Params.Output)
            {
                output.WireDisplay = GH_ParamWireDisplay.@default;

                foreach (var recipient in output.Recipients)
                {
                    // Set the recipient (input) side visible so the wire draws for sure
                    recipient.WireDisplay = GH_ParamWireDisplay.@default;

                    var downstreamComp = GetOwningComponent(recipient, doc) ?? FollowUntilComponent(recipient, doc);
                    if (downstreamComp != null)
                        TraverseDownstream(downstreamComp, doc);
                }
            }
        }

        private static IGH_Component GetOwningComponent(IGH_Param p, GH_Document doc)
        {
            var viaAttrs = p?.Attributes?.GetTopLevel?.DocObject as IGH_Component;
            if (viaAttrs != null) return viaAttrs;

            if (p == null || doc == null) return null;

            // Fallback: find a component whose Inputs/Outputs contain this param (by GUID)
            foreach (var c in doc.Objects.OfType<IGH_Component>())
            {
                if (c.Params.Input.Any(ip => ip.InstanceGuid == p.InstanceGuid) ||
                    c.Params.Output.Any(op => op.InstanceGuid == p.InstanceGuid))
                    return c;
            }
            return null;
        }

        private static IGH_Component FollowUntilComponent(IGH_Param start, GH_Document doc)
        {
            if (start == null || doc == null) return null;

            var q = new Queue<IGH_Param>();
            var seen = new HashSet<Guid>();
            q.Enqueue(start);

            while (q.Count > 0)
            {
                var p = q.Dequeue();
                if (!seen.Add(p.InstanceGuid)) continue;

                var c = GetOwningComponent(p, doc);
                if (c != null) return c;

                foreach (var r in p.Recipients)
                {
                    r.WireDisplay = GH_ParamWireDisplay.@default;
                    q.Enqueue(r);
                }
            }
            return null;
        }
    }
}