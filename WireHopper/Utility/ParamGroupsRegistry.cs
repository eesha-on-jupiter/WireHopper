using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;

namespace WireHopper
{
    public static class ParamGroupsRegistry
    {
        private static readonly Dictionary<string, HashSet<Type>> _groups =
            new Dictionary<string, HashSet<Type>>(StringComparer.OrdinalIgnoreCase)
            {
                { "Geometry", new HashSet<Type>
                    {
                        typeof(Param_Geometry),
                        typeof(Param_Brep),
                        typeof(Param_Surface),
                        typeof(Param_Mesh),
                        typeof(Param_Curve),
                        typeof(Param_SubD),
                        typeof(Param_Point),
                        typeof(Param_Vector),
                        typeof(Param_Line),
                        typeof(Param_Arc),
                        typeof(Param_Circle),
                        typeof(Param_Plane),
                        typeof(Param_Box),
                        typeof(Param_Rectangle)
                    }
                },
                { "Numbers", new HashSet<Type>
                    {
                        typeof(Param_Number),
                        typeof(Param_Integer),
                        typeof(Param_Complex),
                        typeof(Param_Interval)
                    }
                },
                { "Text", new HashSet<Type>
                    { typeof(Param_String) }
                },
                { "Boolean", new HashSet<Type>
                    { typeof(Param_Boolean) }
                },
                { "Colors", new HashSet<Type>
                    { typeof(Param_Colour) }
                },
                { "File & Paths", new HashSet<Type>() } // add optional types if available
            };

        public static IEnumerable<string> Groups { get { return _groups.Keys; } }

        public static bool Matches(string group, IGH_Param p)
        {
            HashSet<Type> set;
            if (!_groups.TryGetValue(group, out set)) return false;

            var t = p.GetType();
            foreach (var reg in set)
            {
                if (reg == t || reg.IsAssignableFrom(t))
                    return true;
            }
            return false;
        }
    }
}