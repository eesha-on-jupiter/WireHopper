using Eto.Forms;
using Grasshopper;
using Grasshopper.Kernel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WireHopper
{
    public class WireViewState
    {
        public Guid ParamId { get; set; }
        public GH_ParamWireDisplay WireDisplay { get; set; }
    }

    public class WireView
    {
        public string Name { get; set; }
        public List<WireViewState> States { get; set; } = new List<WireViewState>();
    }

    public static class WireViewOperations
    {
        // Per-document storage
        private const string DocKey = "WireHopper.WireViews.v1";
        private static readonly Dictionary<Guid, List<WireView>> _viewsPerDoc = new Dictionary<Guid, List<WireView>>();
        private static readonly List<WireView> _empty = new List<WireView>();

        public static List<WireView> SavedViews
        {
            get
            {
                var doc = Instances.ActiveCanvas?.Document;
                return doc != null ? GetListFor(doc) : _empty;
            }
        }

        private static List<WireView> GetListFor(GH_Document doc)
        {
            if (!_viewsPerDoc.TryGetValue(doc.DocumentID, out var list))
            {
                list = new List<WireView>();
                _viewsPerDoc[doc.DocumentID] = list;
            }
            return list;
        }

        private static void PersistToDoc(GH_Document doc)
        {
            var list = GetListFor(doc);
            var json = JsonConvert.SerializeObject(list);
            doc.ValueTable.SetValue(DocKey, json);
            if (!doc.IsModified) doc.IsModified = true;
        }

        internal static void LoadFromDoc(GH_Document doc)
        {
            var json = doc.ValueTable.GetValue(DocKey, string.Empty);
            if (!string.IsNullOrEmpty(json))
            {
                try
                {
                    var list = JsonConvert.DeserializeObject<List<WireView>>(json) ?? new List<WireView>();
                    _viewsPerDoc[doc.DocumentID] = list;
                }
                catch
                {
                    _viewsPerDoc[doc.DocumentID] = new List<WireView>();
                }
            }
            else
            {
                _viewsPerDoc[doc.DocumentID] = new List<WireView>();
            }
        }

        internal static void ForgetDoc(GH_Document doc)
        {
            _viewsPerDoc.Remove(doc.DocumentID);
        }

        public static void Save(string name, GH_Document doc)
        {
            if (doc == null) return;

            var snapshot = new WireView { Name = name };
            foreach (var obj in doc.Objects)
            {
                if (obj is IGH_Param p)
                {
                    snapshot.States.Add(new WireViewState
                    {
                        ParamId = p.InstanceGuid,
                        WireDisplay = p.WireDisplay
                    });
                }

                if (obj is IGH_Component comp)
                {
                    foreach (var input in comp.Params.Input)
                    {
                        snapshot.States.Add(new WireViewState
                        {
                            ParamId = input.InstanceGuid,
                            WireDisplay = input.WireDisplay
                        });
                    }

                    foreach (var output in comp.Params.Output)
                    {
                        snapshot.States.Add(new WireViewState
                        {
                            ParamId = output.InstanceGuid,
                            WireDisplay = output.WireDisplay
                        });
                    }
                }
            }

            var list = GetListFor(doc);
            list.RemoveAll(v => v.Name == name);
            list.Add(snapshot);

            PersistToDoc(doc);
        }

        public static void Restore(string name, GH_Document doc)
        {
            if (doc == null) return;

            var list = GetListFor(doc);
            var view = list.FirstOrDefault(v => v.Name == name);
            if (view == null) return;

            foreach (var state in view.States)
            {
                IGH_Param param = null;

                param = doc.Objects.FirstOrDefault(o => o.InstanceGuid == state.ParamId) as IGH_Param;

                if (param == null)
                {
                    foreach (var comp in doc.Objects.OfType<IGH_Component>())
                    {
                        param = comp.Params.Input.FirstOrDefault(p => p.InstanceGuid == state.ParamId)
                             ?? comp.Params.Output.FirstOrDefault(p => p.InstanceGuid == state.ParamId);
                        if (param != null) break;
                    }
                }

                if (param != null)
                {
                    param.WireDisplay = state.WireDisplay;
                    param.Attributes?.ExpireLayout();
                }
            }

            Instances.ActiveCanvas?.Refresh();
        }

        public static void Update(string name, GH_Document doc)
        {
            if (doc == null) return;

            var list = GetListFor(doc);
            var existing = list.FirstOrDefault(v => v.Name == name);
            if (existing == null) return;

            existing.States.Clear();

            foreach (var obj in doc.Objects)
            {
                if (obj is IGH_Param p)
                {
                    existing.States.Add(new WireViewState
                    {
                        ParamId = p.InstanceGuid,
                        WireDisplay = p.WireDisplay
                    });
                }

                if (obj is IGH_Component comp)
                {
                    foreach (var input in comp.Params.Input)
                    {
                        existing.States.Add(new WireViewState
                        {
                            ParamId = input.InstanceGuid,
                            WireDisplay = input.WireDisplay
                        });
                    }

                    foreach (var output in comp.Params.Output)
                    {
                        existing.States.Add(new WireViewState
                        {
                            ParamId = output.InstanceGuid,
                            WireDisplay = output.WireDisplay
                        });
                    }
                }
            }

            PersistToDoc(doc);
        }

        public static void Delete(string name)
        {
            var result = MessageBox.Show(
                $"Are you sure you want to delete wire view '{name}'?",
                MessageBoxButtons.YesNo,
                MessageBoxType.Question
            );

            if (result == DialogResult.Yes)
            {
                var doc = Instances.ActiveCanvas?.Document;
                if (doc == null) return;

                var list = GetListFor(doc);
                list.RemoveAll(v => v.Name == name);
                PersistToDoc(doc);
            }
        }
    }
}
