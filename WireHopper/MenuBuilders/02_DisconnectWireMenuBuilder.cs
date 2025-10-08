using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WireHopper.Operations;

namespace WireHopper.MenuBuilders
{
    public class DisconnectWireMenuBuilder
    {
        public static void Build(ToolStripMenuItem DisconnectWires)
        {
            //All - Inputs and Outputs
            var DisconnectAll = new ToolStripMenuItem("Inputs + Outputs");
            DisconnectAll.Click += (s, e) => DisconnectOperations.DisconnectSelected_All();
            DisconnectWires.DropDownItems.Add(DisconnectAll);
            DisconnectAll.Image = Resources.InOut;

            //Inputs Only
            var DisconnectInputs = new ToolStripMenuItem("Inputs");
            DisconnectInputs.Click += (s, e) => DisconnectOperations.DisconnectSelected_Inputs();
            DisconnectWires.DropDownItems.Add(DisconnectInputs);
            DisconnectInputs.Image = Resources.In;

            //Outputs Only
            var DisconnectOutputs = new ToolStripMenuItem("Outputs");
            DisconnectOutputs.Click += (s, e) => DisconnectOperations.DisconnectSelected_Outputs();
            DisconnectWires.DropDownItems.Add(DisconnectOutputs);
            DisconnectOutputs.Image = Resources.Out;
        }
    }
}
