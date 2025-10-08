using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WireHopper
{
    public class SelectedWiresMenuBuilder
    {
        public static void Build(ToolStripMenuItem SetSelectedWires)
        {
            // Set all wires: Default
            var SelectedDefault = new ToolStripMenuItem("Default");
            SelectedDefault.Click += (s, e) => WireOperations.SetWireModeForSelection(WireOperations.WireMode.Default);
            SetSelectedWires.DropDownItems.Add(SelectedDefault);
            SelectedDefault.Image = Resources.Default;

            // Set all wires: Faint
            var SelectedFaint = new ToolStripMenuItem("Faint");
            SelectedFaint.Click += (s, e) => WireOperations.SetWireModeForSelection(WireOperations.WireMode.Faint);
            SetSelectedWires.DropDownItems.Add(SelectedFaint);
            SelectedFaint.Image = Resources.Faint;

            // Set all wires: Hidden
            var SelectedHidden = new ToolStripMenuItem("Hidden");
            SelectedHidden.Click += (s, e) => WireOperations.SetWireModeForSelection(WireOperations.WireMode.Hidden);
            SetSelectedWires.DropDownItems.Add(SelectedHidden);
            SelectedHidden.Image = Resources.Hidden;
        }
    }
}

