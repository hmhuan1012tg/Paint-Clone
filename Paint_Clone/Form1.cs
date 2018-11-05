using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DrawingObjects;
using DrawingTools;

namespace Paint_Clone
{
    public partial class Form1 : Form
    {
        ObjectList objectList;
        IDrawingTool tool;

        public Form1()
        {
            InitializeComponent();

            // Setup form component
            selectRadio.Checked = true;

            objectList = new ObjectList(); 
            tool = new SelectTool(objectList);
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            if (tool != null)
                tool.onPartialDraw(e.Graphics);

            objectList.drawAll(e);
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (tool != null)
                tool.onMouseDown(sender, e);
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (tool != null)
                tool.onMouseMove(sender, e);
        }

        private void radio_CheckedChanged(object sender, EventArgs e)
        {
            if (lineRadio.Checked)
            {
                tool = new LineTool(objectList);
                Cursor = Cursors.Cross;
            }
            else if (rectRadio.Checked)
            {
                tool = new RectTool(objectList);
                Cursor = Cursors.Cross;
            }
            else if (selectRadio.Checked)
            {
                tool = new SelectTool(objectList);
                Cursor = Cursors.Arrow;
            }
        }
    }
}
