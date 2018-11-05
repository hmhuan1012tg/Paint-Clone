using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DrawingObjects;

namespace DrawingTools
{
    public abstract class IDrawingTool
    {
        protected ObjectList objectList;

        public abstract void onMouseDown(object sender, MouseEventArgs e);
        public abstract void onMouseMove(object sender, MouseEventArgs e);
        public abstract void onPartialDraw(Graphics g);
    }

    public class LineTool: IDrawingTool
    {
        private Point firstPoint;
        private Point secondPoint;
        private bool firstSnap;

        public LineTool(ObjectList objectList)
        {
            firstSnap = false;
            this.objectList = objectList;
        }

        public override void onMouseDown(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Right)
            {
                firstSnap = false;
                (sender as Control).Invalidate();
                return;
            }

            if (!firstSnap)
            {
                firstSnap = true;
                firstPoint = e.Location;
                return;
            }

            // add line object to list for drawing onto picturebox
            objectList.add(new Line(firstPoint, secondPoint, true));
            (sender as Control).Invalidate();
            firstSnap = false;
        }

        public override void onMouseMove(object sender, MouseEventArgs e)
        {
            if (firstSnap)
            {
                secondPoint = e.Location;
                (sender as Control).Invalidate();
            }
        }

        public override void onPartialDraw(Graphics g)
        {
            if (!firstSnap)
                return;

            // prepare dashed pen
            Pen dashedPen = new Pen(Color.Black, 1.0f);
            float[] dashValues = { 2, 2, 2, 2 };
            dashedPen.DashPattern = dashValues;

            // draw dashed line
            g.DrawLine(dashedPen, firstPoint, secondPoint);
            dashedPen.Dispose();
        }
    }

    public class RectTool : IDrawingTool
    {
        private Point firstPoint;
        private Point secondPoint;
        private bool firstSnap;

        public RectTool(ObjectList objectList)
        {
            firstSnap = false;
            this.objectList = objectList;
        }

        public override void onMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                firstSnap = false;
                (sender as Control).Invalidate();
                return;
            }

            if (!firstSnap)
            {
                firstSnap = true;
                firstPoint = e.Location;
                return;
            }

            // prepare rectangle object
            int startX = Math.Min(firstPoint.X, secondPoint.X);
            int startY = Math.Min(firstPoint.Y, secondPoint.Y);

            int endX = Math.Max(firstPoint.X, secondPoint.X);
            int endY = Math.Max(firstPoint.Y, secondPoint.Y);

            Rectangle rect = new Rectangle(startX, startY, endX - startX + 1, endY - startY + 1);

            // add rectangle object to list for drawing onto picturebox
            objectList.add(new Rect(rect, true));
            (sender as Control).Invalidate();
            firstSnap = false;
        }

        public override void onMouseMove(object sender, MouseEventArgs e)
        {
            if (firstSnap)
            {
                secondPoint = e.Location;
                (sender as Control).Invalidate();
            }
        }

        public override void onPartialDraw(Graphics g)
        {
            if (!firstSnap)
                return;

            // prepare dashed pen
            Pen dashedPen = new Pen(Color.Black, 1.0f);
            float[] dashValues = { 2, 2, 2, 2 };
            dashedPen.DashPattern = dashValues;

            // prepare dashed rectangle
            int startX = Math.Min(firstPoint.X, secondPoint.X);
            int startY = Math.Min(firstPoint.Y, secondPoint.Y);

            int endX = Math.Max(firstPoint.X, secondPoint.X);
            int endY = Math.Max(firstPoint.Y, secondPoint.Y);

            Rectangle rect = new Rectangle(startX, startY, endX - startX + 1, endY - startY + 1);

            // draw dashed rectangle
            g.DrawRectangle(dashedPen, rect);
            dashedPen.Dispose();
        }
    }

    public class SelectTool : IDrawingTool
    {
        public SelectTool(ObjectList objectList)
        {
            this.objectList = objectList;
        }

        public override void onMouseDown(object sender, MouseEventArgs e)
        {
            IDrawingObject obj = objectList.getVisible(e.Location);
            objectList.defocusAll();
            if (obj != null)
                obj.focus();
            (sender as Control).Invalidate();
        }

        public override void onMouseMove(object sender, MouseEventArgs e)
        {
            return;
        }

        public override void onPartialDraw(Graphics g)
        {
            return;
        }
    }
}
