using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WireHopper
{
    public class LengthMenuBuilder
    {
        public static void Build(ToolStripMenuItem CleanByLength)
        {
            // Set Length
            double[] thresholds = { 500, 1000, 1500, 2000 };

            foreach (double t in thresholds)
            {
                // Parent item: "> X px"
                var parent = new ToolStripMenuItem($"> {t} px");
                parent.Image = Resources.Length;

                // Sub-item: Default
                var defaultItem = new ToolStripMenuItem("Default");
                defaultItem.Click += (s, e) =>
                    LengthOperations.CleanWiresByLength(t, WireOperations.WireMode.Default);
                parent.DropDownItems.Add(defaultItem);
                defaultItem.Image = Resources.Default;

                // Sub-item: Faint
                var faintItem = new ToolStripMenuItem("Faint");
                faintItem.Click += (s, e) =>
                    LengthOperations.CleanWiresByLength(t, WireOperations.WireMode.Faint);
                parent.DropDownItems.Add(faintItem);
                faintItem.Image = Resources.Faint;

                // Sub-item: Hidden
                var hiddenItem = new ToolStripMenuItem("Hidden");
                hiddenItem.Click += (s, e) =>
                    LengthOperations.CleanWiresByLength(t, WireOperations.WireMode.Hidden);
                parent.DropDownItems.Add(hiddenItem);
                hiddenItem.Image = Resources.Hidden;

                CleanByLength.DropDownItems.Add(parent);
            }

            //Custom Length
            var CustomLength = new ToolStripMenuItem("Custom");
            CustomLength.Click += (s, e) => EtoDialogs.ShowCleanByLengthDialog();
            CleanByLength.DropDownItems.Add(CustomLength);
            CustomLength.Image = Resources.CustomLength;

            // AutoClean
            var AutoClean = new ToolStripMenuItem("Automatic Clean");
            AutoClean.Click += (s, e) => LengthOperations.SetWireModeByRelativeLength();
            CleanByLength.DropDownItems.Add(AutoClean);
            AutoClean.Image = Resources.AutomaticClean;
        }
    }
}
