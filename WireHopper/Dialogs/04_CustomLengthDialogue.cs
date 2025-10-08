using Eto.Forms;
using Eto.Drawing;
using Grasshopper;
using Rhino;

namespace WireHopper
{
    public static class EtoDialogs
    {
        public static void ShowCleanByLengthDialog()
        {
            var dialog = new Dialog
            {
                Title = "Clean Wires by Length",
                ClientSize = new Size(280, 135), // tighter fit
                Resizable = false,
                Padding = 8
            };

            var label = new Label { Text = "Enter wire length (px):" };
            var textBox = new TextBox { Text = "500", Width = 260 };

            var defaultButton = new Button { Text = "Default" };
            var faintButton = new Button { Text = "Faint" };
            var hiddenButton = new Button { Text = "Hidden" };
            var cancelButton = new Button { Text = "Cancel", Width = 100 };

            void Clean(WireOperations.WireMode mode)
            {
                if (int.TryParse(textBox.Text, out int length))
                {
                    LengthOperations.CleanWiresByLength(length, mode);
                    dialog.Close();
                }
                else MessageBox.Show("Please enter a valid number.");
            }

            defaultButton.Click += (s, e) => Clean(WireOperations.WireMode.Default);
            faintButton.Click += (s, e) => Clean(WireOperations.WireMode.Faint);
            hiddenButton.Click += (s, e) => Clean(WireOperations.WireMode.Hidden);
            cancelButton.Click += (s, e) => dialog.Close();

            var buttonRow = new TableLayout
            {
                Spacing = new Size(6, 6),
                Padding = new Padding(0),
                Rows =
                {
                    new TableRow(defaultButton, faintButton, hiddenButton)
                }
            };

            dialog.Content = new StackLayout
            {
                Orientation = Orientation.Vertical,
                Padding = new Padding(0),
                Spacing = 6,
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                Items =
                {
                    label,
                    textBox,
                    buttonRow,
                    new StackLayoutItem(cancelButton, HorizontalAlignment.Center)
                }
            };

            dialog.ShowModal(Rhino.UI.RhinoEtoApp.MainWindow);
        }
    }
}