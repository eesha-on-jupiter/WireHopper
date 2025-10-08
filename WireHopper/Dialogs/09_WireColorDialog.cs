using Eto.Forms;
using Eto.Drawing;
using Grasshopper;
using Grasshopper.GUI.Canvas;
using System.Collections.Generic;

namespace WireHopper
{
    public class WireColorDialog : Dialog
    {
        private DynamicLayout layout;
        private readonly Dictionary<string, Panel> swatchMap = new Dictionary<string, Panel>();

        // Store current temporary color selections
        private System.Drawing.Color _canvas, _defaultWire, _emptyWire, _selectedA, _selectedB;

        public WireColorDialog()
        {
            Title = "WireHopper – Wire Colors";
            ClientSize = new Size(450, 280);
            Resizable = false;

            // Save whatever colors are active when dialog opens
            GlobalColorOperations.SaveDefaultWireColors();

            // Initialize working copies with current GH values
            _canvas = GH_Skin.canvas_back;
            _defaultWire = GH_Skin.wire_default;
            _emptyWire = GH_Skin.wire_empty;
            _selectedA = GH_Skin.wire_selected_a;
            _selectedB = GH_Skin.wire_selected_b;

            layout = new DynamicLayout
            {
                Padding = new Padding(10),
                Spacing = new Size(5, 5),
                DefaultSpacing = new Size(5, 5),
                DefaultPadding = Padding.Empty
            };

            // Add rows for each editable color
            AddColorRow("Canvas Back", () => _canvas, c => { _canvas = c; ApplyAll(); });
            AddColorRow("Wire Default", () => _defaultWire, c => { _defaultWire = c; ApplyAll(); });
            AddColorRow("Wire Empty", () => _emptyWire, c => { _emptyWire = c; ApplyAll(); });
            AddColorRow("Wire Selected A", () => _selectedA, c => { _selectedA = c; ApplyAll(); });
            AddColorRow("Wire Selected B", () => _selectedB, c => { _selectedB = c; ApplyAll(); });

            // Restore Defaults button
            var restoreButton = new Button { Text = "Restore Defaults" };
            restoreButton.Click += (s, e) =>
            {
                GlobalColorOperations.RestoreDefaultWireColors();

                // Re-read from GH_Skin to update working copies
                _canvas = GH_Skin.canvas_back;
                _defaultWire = GH_Skin.wire_default;
                _emptyWire = GH_Skin.wire_empty;
                _selectedA = GH_Skin.wire_selected_a;
                _selectedB = GH_Skin.wire_selected_b;

                UpdateSwatches();
            };
            layout.AddRow(restoreButton);

            Content = layout;
            var pref = layout.GetPreferredSize(Size.MaxValue);
            ClientSize = new Size((int)pref.Width, (int)pref.Height);
            Resizable = false;
        }

        private void AddColorRow(string labelText,
            System.Func<System.Drawing.Color> getter,
            System.Action<System.Drawing.Color> setter)
        {
            var label = new Label { Text = labelText, Width = 120 };

            var swatch = new Panel
            {
                BackgroundColor = Color.FromArgb(getter().ToArgb()),
                Size = new Size(40, 20)
            };

            // Store swatch so we can update later
            swatchMap[labelText] = swatch;

            var button = new Button { Text = "Pick..." };
            button.Click += (s, e) =>
            {
                var cd = new ColorDialog { Color = Color.FromArgb(getter().ToArgb()) };
                if (cd.ShowDialog(this) == DialogResult.Ok)
                {
                    var newColor = System.Drawing.Color.FromArgb(cd.Color.ToArgb());
                    setter(newColor);
                    swatch.BackgroundColor = cd.Color;
                }
            };

            var row = new StackLayout
            {
                Orientation = Orientation.Horizontal,
                Spacing = 10,
                VerticalContentAlignment = VerticalAlignment.Center,
                Items = { label, swatch, button }
            };

            layout.Add(row);
        }

        private void UpdateSwatches()
        {
            swatchMap["Canvas Back"].BackgroundColor = Color.FromArgb(_canvas.ToArgb());
            swatchMap["Wire Default"].BackgroundColor = Color.FromArgb(_defaultWire.ToArgb());
            swatchMap["Wire Empty"].BackgroundColor = Color.FromArgb(_emptyWire.ToArgb());
            swatchMap["Wire Selected A"].BackgroundColor = Color.FromArgb(_selectedA.ToArgb());
            swatchMap["Wire Selected B"].BackgroundColor = Color.FromArgb(_selectedB.ToArgb());

            Instances.ActiveCanvas?.Invalidate();
        }

        private void ApplyAll()
        {
            GlobalColorOperations.ApplyCustomWires(_canvas, _defaultWire, _emptyWire, _selectedA, _selectedB);
        }
    }
}
