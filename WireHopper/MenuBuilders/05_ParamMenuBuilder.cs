using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;

namespace WireHopper.Menu
{
    public class ParamMenuBuilder
    {
        public static void Build(ToolStripMenuItem CleanByParam)
        {
            // Checkbox: Component Inputs
            var chkInputs = new ToolStripMenuItem("Target component INPUT params")
            {
                CheckOnClick = true,
                Checked = ParamOptions.IncludeInputs
            };
            chkInputs.CheckedChanged += (s, e) =>
                ParamOptions.IncludeInputs = chkInputs.Checked;
            CleanByParam.DropDownItems.Add(chkInputs);

            // Checkbox: Component Outputs
            var chkOutputs = new ToolStripMenuItem("Target component OUTPUT params")
            {
                CheckOnClick = true,
                Checked = ParamOptions.IncludeOutputs
            };
            chkOutputs.CheckedChanged += (s, e) =>
                ParamOptions.IncludeOutputs = chkOutputs.Checked;
            CleanByParam.DropDownItems.Add(chkOutputs);

            // Separator
            CleanByParam.DropDownItems.Add(new ToolStripSeparator());

            string[] groups = { "Geometry", "Numbers", "Text", "Booleans", "Colors", "File & Paths" };

            foreach (string g in groups)
            {
                // Parent item: "> X px"
                var parent = new ToolStripMenuItem($"{g}");
                switch (g)
                {
                    case "Geometry":
                        parent.Image = Resources.Geometry;
                        break;
                    case "Numbers":
                        parent.Image = Resources.Number;
                        break;
                    case "Text":
                        parent.Image = Resources.Text;
                        break;
                    case "Booleans":
                        parent.Image = Resources.Boolean;
                        break;
                    case "Colors":
                        parent.Image = Resources.Color;
                        break;
                    case "File & Paths":
                        parent.Image = Resources.FilePaths;
                        break;
                }

                // Sub-item: Default
                var defaultItem = new ToolStripMenuItem("Default");
                defaultItem.Click += (s, e) =>
                    ParamOperations.CleanWiresByParam(g, WireOperations.WireMode.Default);
                parent.DropDownItems.Add(defaultItem);
                defaultItem.Image = Resources.Default;

                // Sub-item: Faint
                var faintItem = new ToolStripMenuItem("Faint");
                faintItem.Click += (s, e) =>
                    ParamOperations.CleanWiresByParam(g, WireOperations.WireMode.Faint);
                parent.DropDownItems.Add(faintItem);
                faintItem.Image = Resources.Faint;

                // Sub-item: Hidden
                var hiddenItem = new ToolStripMenuItem("Hidden");
                hiddenItem.Click += (s, e) =>
                    ParamOperations.CleanWiresByParam(g, WireOperations.WireMode.Hidden);
                parent.DropDownItems.Add(hiddenItem);
                hiddenItem.Image = Resources.Hidden;

                CleanByParam.DropDownItems.Add(parent);
            }
        }
    }
}
