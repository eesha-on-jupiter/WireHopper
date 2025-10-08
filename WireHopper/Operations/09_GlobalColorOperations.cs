using Grasshopper;
using Grasshopper.GUI.Canvas;
using System;
using System.Drawing;
using System.IO;
using Newtonsoft.Json;
using System.Xml;

namespace WireHopper
{
    public static class GlobalColorOperations
    {
        // In-memory default colors
        private static Color _defaultWire, _emptyWire, _selectedA, _selectedB;
        private static Color _defaultCanvas;
        private static bool _saved = false;

        // File path for cached defaults
        private static readonly string CachePath =
        Path.Combine(Path.GetDirectoryName(typeof(GlobalColorOperations).Assembly.Location),
                     "DefaultWireColors.json");

        // Helper class for JSON serialization
        private class WireCacheData
        {
            public int Canvas { get; set; }
            public int DefaultWire { get; set; }
            public int EmptyWire { get; set; }
            public int SelectedA { get; set; }
            public int SelectedB { get; set; }
        }

        /// <summary>
        /// Saves the current GH_Skin colors as the plugin’s internal defaults
        /// and caches them permanently (only once).
        /// </summary>
        public static void SaveDefaultWireColors()
        {
            if (_saved) return;

            // Capture current GH skin colors
            _defaultWire = GH_Skin.wire_default;
            _emptyWire = GH_Skin.wire_empty;
            _selectedA = GH_Skin.wire_selected_a;
            _selectedB = GH_Skin.wire_selected_b;
            _defaultCanvas = GH_Skin.canvas_back;
            _saved = true;

            // Persist to cache if it doesn't exist yet
            try
            {
                if (!File.Exists(CachePath))
                {
                    var dir = Path.GetDirectoryName(CachePath);
                    if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir);

                    var data = new WireCacheData
                    {
                        Canvas = _defaultCanvas.ToArgb(),
                        DefaultWire = _defaultWire.ToArgb(),
                        EmptyWire = _emptyWire.ToArgb(),
                        SelectedA = _selectedA.ToArgb(),
                        SelectedB = _selectedB.ToArgb()
                    };

                    var json = JsonConvert.SerializeObject(data, Newtonsoft.Json.Formatting.Indented);
                    File.WriteAllText(CachePath, json);
                }
            }
            catch
            {
                // ignore any file IO exceptions
            }
        }

        /// <summary>
        /// Restores cached default colors (if file exists), otherwise restores in-memory defaults.
        /// </summary>
        public static void RestoreDefaultWireColors()
        {
            try
            {
                if (File.Exists(CachePath))
                {
                    var json = File.ReadAllText(CachePath);
                    var data = JsonConvert.DeserializeObject<WireCacheData>(json);

                    GH_Skin.canvas_back = Color.FromArgb(data.Canvas);
                    GH_Skin.wire_default = Color.FromArgb(data.DefaultWire);
                    GH_Skin.wire_empty = Color.FromArgb(data.EmptyWire);
                    GH_Skin.wire_selected_a = Color.FromArgb(data.SelectedA);
                    GH_Skin.wire_selected_b = Color.FromArgb(data.SelectedB);
                }
                else if (_saved)
                {
                    // Fallback to runtime defaults
                    GH_Skin.wire_default = _defaultWire;
                    GH_Skin.wire_empty = _emptyWire;
                    GH_Skin.wire_selected_a = _selectedA;
                    GH_Skin.wire_selected_b = _selectedB;
                    GH_Skin.canvas_back = _defaultCanvas;
                }

                Instances.ActiveCanvas?.Invalidate();
            }
            catch
            {
                // ignore errors
            }
        }

        /// <summary>
        /// Applies user-customized wire colors to GH_Skin.
        /// </summary>
        public static void ApplyCustomWires(
            Color newCanvas,
            Color newDefaultWire,
            Color newEmptyWire,
            Color newSelectedA,
            Color newSelectedB)
        {
            GH_Skin.canvas_back = newCanvas;
            GH_Skin.wire_default = newDefaultWire;
            GH_Skin.wire_empty = newEmptyWire;
            GH_Skin.wire_selected_a = newSelectedA;
            GH_Skin.wire_selected_b = newSelectedB;

            Instances.ActiveCanvas?.Invalidate();
        }
    }
}
