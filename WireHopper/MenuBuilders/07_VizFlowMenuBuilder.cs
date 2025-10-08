using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WireHopper
{
    public class VizFlowMenuBuilder
    {
        public static void Build(ToolStripMenuItem flowViz)
        { 
            //Full Flow
            var fullFlow = new ToolStripMenuItem("Full Flow");
            fullFlow.Click += (s, e) => VisualizeFlowOperations.IsolateFullFlow_All();
            flowViz.DropDownItems.Add(fullFlow);
            fullFlow.Image = Resources.InOut;

            //Upstream
            var upstreamFlow = new ToolStripMenuItem("Upstream Flow");
            upstreamFlow.Click += (s, e) => VisualizeFlowOperations.IsolateFullFlow_Upstream();
            flowViz.DropDownItems.Add(upstreamFlow);
            upstreamFlow.Image = Resources.In;

            //DownStream
            var downFlow = new ToolStripMenuItem("Upstream Flow");
            downFlow.Click += (s, e) => VisualizeFlowOperations.IsolateFullFlow_Downstream();
            flowViz.DropDownItems.Add(downFlow);
            downFlow.Image = Resources.Out;
        }
    }
}
