using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WireHopper
{
    public class AllWiresMenuBuilder
    {
        public static void Build(ToolStripMenuItem SetAllWires)
        {
            // Set all wires: Default
            var AllDefault = new ToolStripMenuItem("Default");
            AllDefault.Click += (s, e) => WireOperations.SetWireModeForAll(WireOperations.WireMode.Default);
            SetAllWires.DropDownItems.Add(AllDefault);
            AllDefault.Image = Resources.Default;

            // Set all wires: Faint
            var AllFaint = new ToolStripMenuItem("Faint");
            AllFaint.Click += (s, e) => WireOperations.SetWireModeForAll(WireOperations.WireMode.Faint);
            SetAllWires.DropDownItems.Add(AllFaint);
            AllFaint.Image = Resources.Faint;

            // Set all wires: Hidden
            var AllHidden = new ToolStripMenuItem("Hidden");
            AllHidden.Click += (s, e) => WireOperations.SetWireModeForAll(WireOperations.WireMode.Hidden);
            SetAllWires.DropDownItems.Add(AllHidden);
            AllHidden.Image = Resources.Hidden;
        }
    }
}
