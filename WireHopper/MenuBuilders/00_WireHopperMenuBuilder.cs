using Eto.Forms;
using Eto.IO;
using Grasshopper;
using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;
using WireHopper.Menu;
using WireHopper.MenuBuilders;
using WireHopper.Operations;

namespace WireHopper
{
    public class WireHopperMenuBuilder
    {
        public static void Build(ToolStripMenuItem WireHopperTab)
        {
            // ✅ Set all wires
            var AllWires = new ToolStripMenuItem("All Wires");
            AllWiresMenuBuilder.Build(AllWires);
            WireHopperTab.DropDownItems.Add(AllWires);
            AllWires.Image = Resources.AllWires;

            // ✅ Set selected wires
            var SelectedWires = new ToolStripMenuItem("Selected Wires");
            SelectedWiresMenuBuilder.Build(SelectedWires);
            WireHopperTab.DropDownItems.Add(SelectedWires);
            SelectedWires.Image = Resources.SelectedWires;

            // ✅ Disconnect wires
            var DisconnectWires = new ToolStripMenuItem("Disconnect Wires");
            WireHopperTab.DropDownItems.Add(DisconnectWires);
            DisconnectWireMenuBuilder.Build(DisconnectWires);
            DisconnectWires.Image = Resources.Disconnect;

            // Param Tree Functions
            var TreeFunctions = new ToolStripMenuItem("Tree Functions");
            TreeFunctionMenuBuilder.Build(TreeFunctions);
            WireHopperTab.DropDownItems.Add(TreeFunctions);
            TreeFunctions.Image = Resources.Tree;

            // Separator
            WireHopperTab.DropDownItems.Add(new ToolStripSeparator());

            // ✅ Clean by Wire Length
            var CleanByLength = new ToolStripMenuItem("Clean by Length");
            LengthMenuBuilder.Build(CleanByLength);
            WireHopperTab.DropDownItems.Add(CleanByLength);
            CleanByLength.Image = Resources.CleanByLength;

            // ✅ Clean by Param Types
            var CleanByParam = new ToolStripMenuItem("Clean by Param");
            ParamMenuBuilder.Build(CleanByParam);
            WireHopperTab.DropDownItems.Add(CleanByParam);
            CleanByParam.Image = Resources.CleanByParams;

            // ✅ Clean by Data Size
            var CleanByData = new ToolStripMenuItem("Clean by Data Size");
            WireHopperTab.DropDownItems.Add(CleanByData);
            CleanByData.Click += (s, e) => VisualizeDataOperations.SetWireModeByData();
            CleanByData.Image = Resources.DataSize;

            // Separator
            WireHopperTab.DropDownItems.Add(new ToolStripSeparator());

            // 🚀 Show single component data flow
            var flowViz = new ToolStripMenuItem("Visualize Flow");
            VizFlowMenuBuilder.Build(flowViz);
            WireHopperTab.DropDownItems.Add(flowViz);
            flowViz.Image = Resources.TraceFlow;

            // ✅ Show components with Preview ON
            var previewViz = new ToolStripMenuItem("Visualize Preview");
            previewViz.Click += (s, e) => VisualizePreviewOperations.SyncWiresWithPreview();
            WireHopperTab.DropDownItems.Add(previewViz);
            previewViz.Image = Resources.CleanPreview;

            // Separator
            WireHopperTab.DropDownItems.Add(new ToolStripSeparator());

            // ✅ Change Global Wire Settings //grid and param color
            var globalWireColor = new ToolStripMenuItem("Canvas Settings");
            globalWireColor.Click += (s, e) =>
            {
                var dialog = new WireColorDialog();
                dialog.ShowModal(Rhino.UI.RhinoEtoApp.MainWindow);
            };
            WireHopperTab.DropDownItems.Add(globalWireColor);
            globalWireColor.Image = Resources.GlobalSettings;

            // Separator
            WireHopperTab.DropDownItems.Add(new ToolStripSeparator());

            // ✅ Name Views
            var nameWireView = new ToolStripMenuItem("Name Wire View");
            nameWireView.Click += (s, e) => NameWireViewDialogue.ShowNameWireViewDialog();
            WireHopperTab.DropDownItems.Add(nameWireView);
            nameWireView.Image = Resources.SaveViews;

            // ✅ Saved Views
            var savedViews = new ToolStripMenuItem("Saved Views");
            WireViewsMenuBuilder.Build(savedViews);
            WireHopperTab.DropDownItems.Add(savedViews);
            savedViews.Image = Resources.NamedViews;

            // Separator
            WireHopperTab.DropDownItems.Add(new ToolStripSeparator());

            // ✅ Undo Wires
            var undoWires = new ToolStripMenuItem("Undo");
            undoWires.Click += (s, e) =>
            {
                var doc = Instances.ActiveCanvas?.Document;
                if (doc == null) return;
                WireUndoOperations.Undo(doc);
            };
            WireHopperTab.DropDownItems.Add(undoWires);
            undoWires.Image = Resources.Undo;
        }
    }
}
