using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WireHopper.Operations;

namespace WireHopper
{
    public class TreeFunctionMenuBuilder
    {
        public static void Build(ToolStripMenuItem TreeFunctions)
        {
            AddTreeSubmenu(TreeFunctions, "Flatten", TreeFunctionsOperations.SetFlat);
            AddTreeSubmenu(TreeFunctions, "Graft", TreeFunctionsOperations.SetGraft);
            AddTreeSubmenu(TreeFunctions, "Simplify", TreeFunctionsOperations.SetSimplify);
            AddTreeSubmenu(TreeFunctions, "Reverse", TreeFunctionsOperations.SetReverse);

            TreeFunctions.DropDownItems.Add(new ToolStripSeparator());

            var removeAll = new ToolStripMenuItem("Remove All");
            removeAll.Click += (s, e) => TreeFunctionsOperations.RemoveAll();
            TreeFunctions.DropDownItems.Add(removeAll);
            removeAll.Image = Resources.Cancel;
        }

        private static void AddTreeSubmenu(ToolStripMenuItem parent, string name,
                                           System.Action<TreeFunctionsOperations.ParamTarget> action)
        {
            var root = new ToolStripMenuItem(name);
            {
                root.Image = GetFunctionIcon(name);
                ;

                var both = new ToolStripMenuItem("Inputs + Outputs");
                both.Click += (s, e) => action(TreeFunctionsOperations.ParamTarget.Both);
                both.Image = Resources.InOut;

                var inputs = new ToolStripMenuItem("Inputs");
                inputs.Click += (s, e) => action(TreeFunctionsOperations.ParamTarget.Inputs);
                inputs.Image = Resources.In;

                var outputs = new ToolStripMenuItem("Outputs");
                outputs.Click += (s, e) => action(TreeFunctionsOperations.ParamTarget.Outputs);
                outputs.Image = Resources.Out;

                var remove = new ToolStripMenuItem("Remove");
                remove.Click += (s, e) => TreeFunctionsOperations.RemoveSpecific(name);
                remove.Image = Resources.Remove;

                root.DropDownItems.Add(both);
                root.DropDownItems.Add(inputs);
                root.DropDownItems.Add(outputs);
                root.DropDownItems.Add(remove);

                parent.DropDownItems.Add(root);
            }
        }

        private static System.Drawing.Image GetFunctionIcon(string name)
        {
            switch (name)
            {
                case "Flatten":
                    return Resources.Flatten;
                case "Graft":
                    return Resources.Graft;
                case "Simplify":
                    return Resources.Simplify;
                case "Reverse":
                   return Resources.Reverse;
                default:
                    return null;
            }
        }
    }
}
