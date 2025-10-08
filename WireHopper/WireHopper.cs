using Grasshopper;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Rhino;
using System.Linq;
using System.Windows.Forms;

namespace WireHopper
{
    // Loads early and injects a top-level "WireHopper" menu.
    public class WireHopper : GH_AssemblyPriority
    {
        public override GH_LoadingInstruction PriorityLoad()
        {
            if (Instances.DocumentEditor != null)
            {
                AddTopLevelMenu(Instances.DocumentEditor);
            }
            else
            {
                // wait until GH editor exists
                RhinoApp.Idle += OnIdleOnce;
            }

            return GH_LoadingInstruction.Proceed;
        }

        private void OnIdleOnce(object sender, System.EventArgs e)
        {
            if (Instances.DocumentEditor == null) return;

            RhinoApp.Idle -= OnIdleOnce;

            AddTopLevelMenu(Instances.DocumentEditor);
        }

        private void AddTopLevelMenu(GH_DocumentEditor editor)
        {
            if (editor == null) return;

            // prevent duplicates if plugin reloads
            bool exists = editor.MainMenuStrip.Items
            .Cast<ToolStripItem>()
            .Any(i => string.Equals(i.Tag as string, "WireHopperMenu"));
            if (exists) return;

            var WireHopperMenu = new ToolStripMenuItem("&WireHopper") { Tag = "WireHopperMenu" };
            WireHopperMenuBuilder.Build(WireHopperMenu);
            editor.MainMenuStrip.Items.Add(WireHopperMenu);
        }
    }
}