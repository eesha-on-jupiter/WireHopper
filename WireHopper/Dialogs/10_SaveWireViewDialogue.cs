using Eto.Forms;
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
    public class NameWireViewDialogue
    {
        public static void ShowNameWireViewDialog()
        {
            var dialog = new Dialog<string>
            {
                Title = "Save Wire View",
                ClientSize = new Eto.Drawing.Size(300, 100),
                Resizable = false
            };

            var nameBox = new TextBox { PlaceholderText = "Enter view name…" };
            var okButton = new Button { Text = "Save" };
            var cancelButton = new Button { Text = "Cancel" };

            okButton.Click += (s, e) =>
            {
                if (!string.IsNullOrWhiteSpace(nameBox.Text))
                {
                    var doc = Instances.ActiveCanvas?.Document;
                    if (doc != null)
                    {
                        WireViewOperations.Save(nameBox.Text, doc);
                        RhinoApp.WriteLine($"Wire view '{nameBox.Text}' saved.");
                    }

                    dialog.Result = nameBox.Text;
                    dialog.Close();
                }
            };

            cancelButton.Click += (s, e) => dialog.Close();

            dialog.Content = new DynamicLayout
            {
                Padding = 10,
                Spacing = new Eto.Drawing.Size(5, 5),
                Rows =
                {
                    new Label { Text = "Name wire view:" },
                    nameBox,
                    new TableLayout
                    {
                        Spacing = new Eto.Drawing.Size(5, 5),
                        Rows = { new TableRow(null, okButton, cancelButton) }
                    }
                }
            };

            dialog.ShowModal(Rhino.UI.RhinoEtoApp.MainWindow);
        }
    }
}
