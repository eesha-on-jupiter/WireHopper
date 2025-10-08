using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WireHopper.Operations
{
    public static class TreeFunctionsOperations
    {
        public enum ParamTarget
        {
            Inputs,
            Outputs,
            Both
        }

        public static void ApplyToSelected(Action<IGH_Param> action, ParamTarget target)
        {
            var doc = Instances.ActiveCanvas?.Document;
            if (doc == null) return;

            foreach (var obj in doc.SelectedObjects())
            {
                if (obj is IGH_Component comp)
                {
                    if (target == ParamTarget.Inputs || target == ParamTarget.Both)
                    {
                        foreach (var p in comp.Params.Input)
                        {
                            action(p);
                            p.Attributes?.ExpireLayout();
                        }
                    }

                    if (target == ParamTarget.Outputs || target == ParamTarget.Both)
                    {
                        foreach (var p in comp.Params.Output)
                        {
                            action(p);
                            p.Attributes?.ExpireLayout();
                        }
                    }

                    comp.ExpireSolution(true);
                }
                else if (obj is IGH_Param param)
                {
                    if (target == ParamTarget.Both)
                    {
                        action(param);
                        param.Attributes?.ExpireLayout();
                        param.ExpireSolution(true);
                    }
                }
            }

            WireUndoOperations.SaveSnapshot(doc);

            doc.NewSolution(true);
        }

        private static void ToggleDataMapping(IGH_Param p, string mappingName)
        {
            var prop = p.GetType().GetProperty("DataMapping");
            if (prop == null) return;

            var enumType = prop.PropertyType;
            var current = prop.GetValue(p);
            var none = Enum.Parse(enumType, "None");
            var target = Enum.Parse(enumType, mappingName);

            prop.SetValue(p, Equals(current, target) ? none : target);
        }

        public static void RemoveSpecific(string name, ParamTarget target = ParamTarget.Both)
        {
            ApplyToSelected(p =>
            {
                // Handle Flatten & Graft (via DataMapping)
                if (name == "Flatten" || name == "Graft")
                {
                    var prop = p.GetType().GetProperty("DataMapping");
                    if (prop != null)
                    {
                        var enumType = prop.PropertyType;
                        var current = prop.GetValue(p);
                        var none = Enum.Parse(enumType, "None");
                        var targetEnum = Enum.Parse(enumType, name);

                        // Only clear this specific mapping type
                        if (Equals(current, targetEnum))
                            prop.SetValue(p, none);
                    }
                }

                // Handle Simplify & Reverse (simple bool flags)
                if (name == "Simplify" && p.Simplify)
                    p.Simplify = false;
                if (name == "Reverse" && p.Reverse)
                    p.Reverse = false;

            }, target);
        }

        public static void SetFlat(ParamTarget target = ParamTarget.Both)
        {
            ApplyToSelected(p => ToggleDataMapping(p, "Flatten"), target);
        }

        public static void SetGraft(ParamTarget target = ParamTarget.Both)
        {
            ApplyToSelected(p => ToggleDataMapping(p, "Graft"), target);
        }

        public static void SetSimplify(ParamTarget target = ParamTarget.Both)
        {
            ApplyToSelected(p => p.Simplify = !p.Simplify, target);
        }

        public static void SetReverse(ParamTarget target = ParamTarget.Both)
        {
            ApplyToSelected(p => p.Reverse = !p.Reverse, target);
        }

        public static void RemoveAll(ParamTarget target = ParamTarget.Both)
        {
            ApplyToSelected(p =>
            {
                var prop = p.GetType().GetProperty("DataMapping");
                if (prop != null)
                {
                    var enumType = prop.PropertyType;
                    var none = Enum.Parse(enumType, "None");
                    prop.SetValue(p, none);
                }
                p.Simplify = false;
                p.Reverse = false;
            }, target);
        }
    }
}
