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
        Cursor usedCursor;

        public Form1()
        {
            InitializeComponent();

            // Setup form component
            objectList = new ObjectList(); 
            selectRadio.Checked = true;
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            objectList.drawAll(e);
            if (tool != null)
                tool.onPartialDraw(e.Graphics);
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
                usedCursor = Cursors.Cross;
            }
            else if (rectRadio.Checked)
            {
                tool = new RectTool(objectList);
                usedCursor = Cursors.Cross;
            }
            else if (parallelogramRadio.Checked)
            {
                tool = new ParallelogramTool(objectList);
                usedCursor = Cursors.Cross;
            }
            else if (polygonRadio.Checked)
            {
                tool = new PolygonTool(objectList);
                usedCursor = Cursors.Cross;
            }
            else if (brokenLineRadio.Checked)
            {
                tool = new BrokenLineTool(objectList);
                usedCursor = Cursors.Cross;
            }
            else if (circleArcRadio.Checked)
            {
                tool = new CircleArcTool(objectList);
                usedCursor = Cursors.Cross;
            }
            else if (circleRadio.Checked)
            {
                tool = new CircleTool(objectList);
                usedCursor = Cursors.Cross;
            }
            else if (ellipseRadio.Checked)
            {
                tool = new EllipseTool(objectList);
                usedCursor = Cursors.Cross;
            }
            else if (ellipseArcRadio.Checked)
            {
                tool = new EllipseArcTool(objectList);
                usedCursor = Cursors.Cross;
            }
            else if (bezierRadio.Checked)
            {
                tool = new BezierTool(objectList);
                usedCursor = Cursors.Cross;
            }
            else if (newTextRadio.Checked)
            {
                tool = new CreateTextTool(objectList);
                usedCursor = Cursors.IBeam;
            }
            else if (parabolaRadio.Checked)
            {
                tool = new ParabolaTool(objectList);
                usedCursor = Cursors.Cross;
            }
            else if (hyperbolaRadio.Checked)
            {
                tool = new HyperbolaTool(objectList);
                usedCursor = Cursors.Cross;
            }

            // Manipulation Tools
            else if (selectRadio.Checked)
            {
                tool = new SelectTool(objectList);
                usedCursor = Cursors.Arrow;
            }
            else if (moveRadio.Checked)
            {
                tool = new MoveTool(objectList);
                usedCursor = Cursors.NoMove2D;
            }
            else if (deleteRadio.Checked)
            {
                tool = new DeleteTool(objectList);
                usedCursor = Cursors.No;
            }
            else if (controlRadio.Checked)
            {
                tool = new ControlTool(objectList);
                usedCursor = Cursors.Hand;
            }
            else if (scaleRadio.Checked)
            {
                tool = new ScaleTool(objectList);
                usedCursor = Cursors.SizeAll;
            }
            else if (rotateRadio.Checked)
            {
                tool = new RotateTool(objectList);
                usedCursor = Cursors.Hand;
            }

            pictureBox1.Invalidate();
            tool.setOwner(this);
        }

        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
            Cursor = Cursors.Arrow;
            if (tool != null && !newTextRadio.Checked)
                tool.reset(sender);
        }

        private void pictureBox1_MouseEnter(object sender, EventArgs e)
        {
            Cursor = usedCursor;
        }

        private void clearBtn_Click(object sender, EventArgs e)
        {
            objectList.clear();
            if (tool != null)
                tool.reset(pictureBox1);
            pictureBox1.Invalidate();
            pictureBox1.Focus();
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (newTextRadio.Checked)
            {
                (tool as CreateTextTool).onKeyPress(pictureBox1, e);
            }
        }
    }
}
