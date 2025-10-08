using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WireHopper
{
    public static class ColorRegistry
    {
        private static readonly Dictionary<Guid, System.Drawing.Color> _colors;

        public static void SetColor(Guid id, System.Drawing.Color color) => _colors[id] = color;

        public static bool TryGetColor(Guid id, out System.Drawing.Color color) => _colors.TryGetValue(id, out color);
    }
}
