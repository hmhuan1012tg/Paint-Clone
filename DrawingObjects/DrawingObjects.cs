using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DrawingObjects
{
    static class DrawingConstant
    {
        public const int CONTROL_RECT_SIZE = 6;

        public static Rectangle CreateControlRect(Point p)
        {
            Point center = new Point(p.X - CONTROL_RECT_SIZE / 2, p.Y - CONTROL_RECT_SIZE / 2);
            return new Rectangle(center, new Size(CONTROL_RECT_SIZE, CONTROL_RECT_SIZE));
        }
    }

    // TODO: implements Z-Index
    public abstract class IDrawingObject
    {
        private bool isFocus = false;

        public bool isFocused()
        {
            return isFocus;
        }

        public void focus()
        {
            isFocus = true;
        }
 
        public void defocus()
        {
            isFocus = false;
        }

        public abstract void onDraw(PaintEventArgs e);

        public abstract bool isVisible(Point location);
    }

    public class ObjectList
    {
        private List<IDrawingObject> list;

        public ObjectList()
        {
            list = new List<IDrawingObject>();
        }

        public void add(IDrawingObject obj)
        {
            if (list.Count != 0)
                list.Last().defocus();
            list.Add(obj);
        }

        public void drawAll(PaintEventArgs e)
        {
            foreach (var obj in list)
                obj.onDraw(e);
        }

        public void defocusAll()
        {
            foreach (var obj in list)
                obj.defocus();
        }

        public IDrawingObject getVisible(Point location)
        {
            foreach (var obj in list)
                if (obj.isVisible(location))
                    return obj;
            return null;
        }
    }

    public class Line : IDrawingObject
    {
        private Point firstPoint;
        private Point secondPoint;

        public Line(Point firstPoint, Point secondPoint, bool isFocus = false)
        {
            this.firstPoint = firstPoint;
            this.secondPoint = secondPoint;

            if (isFocus)
                focus();
        }

        public override void onDraw(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Pen solidPen = new Pen(Color.Black, 2.0f);
            e.Graphics.DrawLine(solidPen, firstPoint, secondPoint);

            if (isFocused())
            {
                e.Graphics.FillRectangle(Brushes.White, DrawingConstant.CreateControlRect(firstPoint));
                e.Graphics.DrawRectangle(Pens.Black, DrawingConstant.CreateControlRect(firstPoint));

                e.Graphics.FillRectangle(Brushes.White, DrawingConstant.CreateControlRect(secondPoint));
                e.Graphics.DrawRectangle(Pens.Black, DrawingConstant.CreateControlRect(secondPoint));
            }

            solidPen.Dispose();
        }

        public override bool isVisible(Point location)
        {
            GraphicsPath gPath = new GraphicsPath();
            gPath.AddLine(firstPoint, secondPoint);
            return gPath.IsOutlineVisible(location, new Pen(Brushes.Black, 10));
        }
    }

    public class Rect : IDrawingObject
    {
        private Rectangle rect;

        public Rect(Rectangle rect, bool isFocus = false)
        {
            this.rect = rect;

            if (isFocus)
                focus();
        }

        public override bool isVisible(Point location)
        {
            GraphicsPath gPath = new GraphicsPath();
            gPath.AddRectangle(rect);
            return gPath.IsOutlineVisible(location, new Pen(Brushes.Black, 10));
        }

        public override void onDraw(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Pen solidPen = new Pen(Color.Black, 2.0f);
            e.Graphics.DrawRectangle(solidPen, rect);

            if (isFocused())
            {
                Point topLeft = new Point(rect.Left, rect.Top);
                e.Graphics.FillRectangle(Brushes.White, DrawingConstant.CreateControlRect(topLeft));
                e.Graphics.DrawRectangle(Pens.Black, DrawingConstant.CreateControlRect(topLeft));

                Point topRight = new Point(rect.Right, rect.Top);
                e.Graphics.FillRectangle(Brushes.White, DrawingConstant.CreateControlRect(topRight));
                e.Graphics.DrawRectangle(Pens.Black, DrawingConstant.CreateControlRect(topRight));

                Point bottomLeft = new Point(rect.Left, rect.Bottom);
                e.Graphics.FillRectangle(Brushes.White, DrawingConstant.CreateControlRect(bottomLeft));
                e.Graphics.DrawRectangle(Pens.Black, DrawingConstant.CreateControlRect(bottomLeft));

                Point bottomRight = new Point(rect.Right, rect.Bottom);
                e.Graphics.FillRectangle(Brushes.White, DrawingConstant.CreateControlRect(bottomRight));
                e.Graphics.DrawRectangle(Pens.Black, DrawingConstant.CreateControlRect(bottomRight));
            }

            solidPen.Dispose();
        }
    }
}
