using Grasshopper;
using Grasshopper.Kernel;
using Rhino;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WireHopper
{
    public class WireParamState
    {
        public Guid ParamId { get; set; }
        public GH_ParamWireDisplay WireDisplay { get; set; }
    }

    public class WireConnectionState
    {
        public Guid SourceId { get; set; }
        public Guid TargetId { get; set; }
    }

    public static class WireUndoOperations
    {
        private static readonly List<(List<WireParamState> Params, List<WireConnectionState> Connections, int GlobalWireMode)> _history
            = new List<(List<WireParamState>, List<WireConnectionState>, int)>();

        private static int _currentIndex = -1;

        // ---- SAVE SNAPSHOT ----
        public static void SaveSnapshot(GH_Document doc)
        {
            if (doc == null) return;

            var snapshot = CaptureCurrentState(doc);

            // Avoid duplicate consecutive saves
            if (_history.Count > 0 && CompareSnapshots(_history.Last(), snapshot))
                return;

            // If user undid and now changes something, truncate forward history
            if (_currentIndex < _history.Count - 1)
                _history.RemoveRange(_currentIndex + 1, _history.Count - _currentIndex - 1);

            _history.Add(snapshot);
            _currentIndex = _history.Count - 1;

            RhinoApp.WriteLine($"[WireHopper] Snapshot saved {_currentIndex + 1}/{_history.Count}");
        }

        // ---- UNDO ----
        public static void Undo(GH_Document doc)
        {
            if (_currentIndex <= 0)
            {
                RhinoApp.WriteLine("[WireHopper] Nothing left to undo.");
                return;
            }

            _currentIndex--;
            RestoreSnapshot(doc, _history[_currentIndex]);

            RhinoApp.WriteLine($"[WireHopper] Undo → now at {_currentIndex + 1}/{_history.Count}");
        }

        // ---- REDO ----
        public static void Redo(GH_Document doc)
        {
            if (_currentIndex >= _history.Count - 1)
            {
                RhinoApp.WriteLine("[WireHopper] Nothing left to redo.");
                return;
            }

            _currentIndex++;
            RestoreSnapshot(doc, _history[_currentIndex]);

            RhinoApp.WriteLine($"[WireHopper] Redo → now at {_currentIndex + 1}/{_history.Count}");
        }

        // ---- CAPTURE ----
        private static (List<WireParamState>, List<WireConnectionState>, int) CaptureCurrentState(GH_Document doc)
        {
            var paramStates = new List<WireParamState>();
            var connections = new List<WireConnectionState>();

            foreach (var obj in doc.Objects)
            {
                if (obj is IGH_Param p)
                {
                    paramStates.Add(new WireParamState { ParamId = p.InstanceGuid, WireDisplay = p.WireDisplay });
                    foreach (var src in p.Sources)
                        connections.Add(new WireConnectionState { SourceId = src.InstanceGuid, TargetId = p.InstanceGuid });
                }
                else if (obj is IGH_Component c)
                {
                    foreach (var input in c.Params.Input)
                    {
                        paramStates.Add(new WireParamState { ParamId = input.InstanceGuid, WireDisplay = input.WireDisplay });
                        foreach (var src in input.Sources)
                            connections.Add(new WireConnectionState { SourceId = src.InstanceGuid, TargetId = input.InstanceGuid });
                    }

                    foreach (var output in c.Params.Output)
                        paramStates.Add(new WireParamState { ParamId = output.InstanceGuid, WireDisplay = output.WireDisplay });
                }
            }

            int globalWireMode = Instances.Settings.GetValue("Draw Wires", 2);
            return (paramStates, connections, globalWireMode);
        }

        // ---- RESTORE ----
        private static void RestoreSnapshot(GH_Document doc, (List<WireParamState> Params, List<WireConnectionState> Connections, int GlobalWireMode) snapshot)
        {
            // 1️ Remove existing connections
            foreach (var obj in doc.Objects)
            {
                if (obj is IGH_Param p)
                    p.RemoveAllSources();
                else if (obj is IGH_Component c)
                    foreach (var i in c.Params.Input)
                        i.RemoveAllSources();
            }

            // 2️ Restore connections
            foreach (var conn in snapshot.Connections)
            {
                var src = FindParam(doc, conn.SourceId);
                var tgt = FindParam(doc, conn.TargetId);
                if (src != null && tgt != null && !tgt.Sources.Contains(src))
                    tgt.AddSource(src);
            }

            // 3️ Restore per-param wire display
            foreach (var state in snapshot.Params)
            {
                var param = FindParam(doc, state.ParamId);
                if (param != null)
                    param.WireDisplay = state.WireDisplay;
            }

            // 4️ Restore global setting
            Instances.Settings.SetValue("Draw Wires", snapshot.GlobalWireMode);

            // 5️ Expire once, recompute once
            foreach (var obj in doc.Objects)
            {
                if (obj is IGH_Component comp)
                    comp.ExpireSolution(false);
                else if (obj is IGH_Param param)
                    param.ExpireSolution(false);
            }

            doc.NewSolution(false);
            Instances.ActiveCanvas?.Refresh();
        }

        // ---- HELPERS ----
        private static bool CompareSnapshots(
            (List<WireParamState> Params, List<WireConnectionState> Connections, int GlobalWireMode) a,
            (List<WireParamState> Params, List<WireConnectionState> Connections, int GlobalWireMode) b)
        {
            if (a.GlobalWireMode != b.GlobalWireMode) return false;
            if (a.Params.Count != b.Params.Count || a.Connections.Count != b.Connections.Count) return false;
            return !a.Params.Where((t, i) => t.WireDisplay != b.Params[i].WireDisplay).Any();
        }

        private static IGH_Param FindParam(GH_Document doc, Guid id)
        {
            foreach (var obj in doc.Objects)
            {
                if (obj is IGH_Param loose && loose.InstanceGuid == id)
                    return loose;
                if (obj is IGH_Component comp)
                {
                    foreach (var p in comp.Params.Input)
                        if (p.InstanceGuid == id) return p;
                    foreach (var p in comp.Params.Output)
                        if (p.InstanceGuid == id) return p;
                }
            }
            return null;
        }
    }
}
