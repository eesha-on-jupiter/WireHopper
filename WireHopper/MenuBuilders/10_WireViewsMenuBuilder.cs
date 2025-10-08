using Grasshopper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WireHopper.MenuBuilders
{
    public class WireViewsMenuBuilder
    {
        public static void Build(ToolStripMenuItem parent)
        {
            parent.DropDownOpening += (s, e) =>
            {
                parent.DropDownItems.Clear();

                foreach (var view in WireViewOperations.SavedViews)
                {
                    var viewMenu = new ToolStripMenuItem(view.Name);

                    // Restore
                    viewMenu.DropDownItems.Add("Restore", null, (ss, ee) =>
                    {
                        var doc = Instances.ActiveCanvas?.Document;
                        if (doc != null)
                            WireViewOperations.Restore(view.Name, doc);
                    });

                    // Update
                    viewMenu.DropDownItems.Add("Update", null, (ss, ee) =>
                    {
                        var doc = Instances.ActiveCanvas?.Document;
                        if (doc != null)
                            WireViewOperations.Update(view.Name, doc);
                    });

                    // Delete
                    viewMenu.DropDownItems.Add("Delete", null, (ss, ee) =>
                    {
                        WireViewOperations.Delete(view.Name);
                        Build(parent); // refresh the menu
                    });

                    parent.DropDownItems.Add(viewMenu);
                }
            };
        }
    }
}
