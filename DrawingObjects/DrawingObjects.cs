using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DrawingObjects
{
    public static class DrawingUtilities
    {
        public const int CONTROL_RECT_SIZE = 6;

        // create rectangle object centered at point p
        public static Rectangle CreateControlRect(Point p)
        {
            Point center = new Point(p.X - CONTROL_RECT_SIZE / 2, p.Y - CONTROL_RECT_SIZE / 2);
            return new Rectangle(center, new Size(CONTROL_RECT_SIZE, CONTROL_RECT_SIZE));
        }

        // draw control rectangle centered at point p
        public static void DrawControlRect(Graphics g, Point p)
        {
            g.FillRectangle(Brushes.White, CreateControlRect(p));
            g.DrawRectangle(Pens.Black, CreateControlRect(p));
        }

        // calculate small angle between two points on circle
        public static float CalculateAngle(Point center, Point point)
        {
            return (float)Math.Atan2(point.Y - center.Y, point.X - center.X);
        }
    }

    // TODO: implements Z-Index
    public abstract class IDrawingObject
    {
        private bool isFocus = false;
        protected List<Point> controlPoints;

        protected Color color = Color.Black;
        protected bool filledWithColor = false;
        protected Color fillColor = Color.Transparent;


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

        public void setOutlineColor(Color c)
        {
            color = c;
        }

        public void setFillColor(Color c)
        {
            if (color != Color.Transparent)
            {
                fillColor = c;
                filledWithColor = true;
            }
        }

        

        // interface for drawing object
        public void onDraw(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Pen solidPen = new Pen(color, 1.0f);
            derivedDraw(e, solidPen);

            if (isFocused())
            {
                derivedDrawLineBetweenControlPoints(e);
                drawControlRects(e);
            }

            solidPen.Dispose();
        }
        // implements these to draw specific object
        protected abstract void derivedDraw(PaintEventArgs e, Pen pen);
        protected abstract void derivedDrawLineBetweenControlPoints(PaintEventArgs e);
        private void drawControlRects(PaintEventArgs e)
        {
            foreach (var point in controlPoints)
                DrawingUtilities.DrawControlRect(e.Graphics, point);
        }

        // interface for checking is "location" belongs to object
        public bool isVisible(Point location)
        {
            GraphicsPath gPath = new GraphicsPath();
            buildGraphicsPath(gPath);
            if (isFocused())
                addControlRectsToGraphicPath(gPath);

            return gPath.IsOutlineVisible(location, new Pen(Brushes.Black, 15));
        }
        // implement these to build graphics path object to use for checking
        protected abstract void buildGraphicsPath(GraphicsPath gPath);
        private void addControlRectsToGraphicPath(GraphicsPath path)
        {
            foreach (var point in controlPoints)
                path.AddRectangle(DrawingUtilities.CreateControlRect(point));
        }
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

        public void clear()
        {
            list.Clear();
        }

        public IDrawingObject getVisible(Point location)
        {
            foreach (var obj in list)
                if (obj.isVisible(location))
                    return obj;
            return null;
        }
    }

    // Duong thang
    public class Line : IDrawingObject
    {
        private Point firstPoint;
        private Point secondPoint;

        public Line(Point firstPoint, Point secondPoint, bool isFocus = false)
        {
            this.firstPoint = firstPoint;
            this.secondPoint = secondPoint;

            controlPoints = new List<Point>();
            controlPoints.Add(firstPoint);
            controlPoints.Add(secondPoint);

            if (isFocus)
                focus();
        }

        protected override void derivedDraw(PaintEventArgs e, Pen pen)
        {
            e.Graphics.DrawLine(pen, firstPoint, secondPoint);
        }

        protected override void buildGraphicsPath(GraphicsPath gPath)
        {
            gPath.AddLine(firstPoint, secondPoint);
        }

        protected override void derivedDrawLineBetweenControlPoints(PaintEventArgs e)
        {
        }
    }

    // Hinh chu nhat
    public class Rect : IDrawingObject
    {
        private Rectangle rect;

        public Rect(Rectangle rect, bool isFocus = false)
        {
            this.rect = rect;
            controlPoints = new List<Point>();
            controlPoints.Add(new Point(rect.Left, rect.Top));
            controlPoints.Add(new Point(rect.Right, rect.Top));
            controlPoints.Add(new Point(rect.Left, rect.Bottom));
            controlPoints.Add(new Point(rect.Right, rect.Bottom));

            if (isFocus)
                focus();
        }

        protected override void derivedDraw(PaintEventArgs e, Pen pen)
        {
            e.Graphics.DrawRectangle(pen, rect);
        }

        protected override void buildGraphicsPath(GraphicsPath gPath)
        {
            gPath.AddRectangle(rect);
        }

        protected override void derivedDrawLineBetweenControlPoints(PaintEventArgs e)
        {
        }
    }

    // Hinh binh hanh
    public class Parallelogram : IDrawingObject
    {
        public Parallelogram(Point[] controlPoints, bool isFocus = false)
        {
            this.controlPoints = new List<Point>();
            for (int i = 0; i < 4; i++)
                this.controlPoints.Add(controlPoints[i]);

            if (isFocus)
                focus();
        }

        protected override void derivedDraw(PaintEventArgs e, Pen pen)
        {
            e.Graphics.DrawPolygon(pen, controlPoints.ToArray());
        }

        protected override void buildGraphicsPath(GraphicsPath gPath)
        {
            gPath.AddPolygon(controlPoints.ToArray());
        }

        protected override void derivedDrawLineBetweenControlPoints(PaintEventArgs e)
        {
        }
    }

    // Da giac
    public class Polygon : IDrawingObject
    {
        public Polygon(List<Point> controlPoints, bool isFocus = false)
        {
            if (controlPoints != null)
                this.controlPoints = controlPoints;
            if (isFocus)
                focus();
        }

        protected override void buildGraphicsPath(GraphicsPath gPath)
        {
            gPath.AddPolygon(controlPoints.ToArray());
        }

        protected override void derivedDraw(PaintEventArgs e, Pen pen)
        {
            e.Graphics.DrawPolygon(pen, controlPoints.ToArray());
        }

        protected override void derivedDrawLineBetweenControlPoints(PaintEventArgs e)
        {
        }
    }

    // Duong gap khuc
    public class BrokenLine : IDrawingObject
    {
        public BrokenLine(List<Point> controlPoints, bool isFocus = false)
        {
            this.controlPoints = controlPoints;

            if (isFocus)
                focus();
        }

        protected override void buildGraphicsPath(GraphicsPath gPath)
        {
            gPath.AddLines(controlPoints.ToArray());
        }

        protected override void derivedDraw(PaintEventArgs e, Pen pen)
        {
            e.Graphics.DrawLines(pen, controlPoints.ToArray());
        }

        protected override void derivedDrawLineBetweenControlPoints(PaintEventArgs e)
        {
        }
    }

    // Cung tron
    public class CircleArc : IDrawingObject
    {
        private Point center;
        private Point[] pivot;
        private bool smallPart;
        private float radius;

        public CircleArc(Point center, Point[] pivot, bool smallPart, float radius, bool isFocus = false)
        {
            this.center = center;
            controlPoints = new List<Point>();
            controlPoints.Add(center);

            this.pivot = new Point[2];
            for (int i = 0; i < 2; i++)
            {
                this.pivot[i] = pivot[i];
                controlPoints.Add(pivot[i]);
            }

            this.smallPart = smallPart;
            this.radius = radius;

            if (isFocus)
                focus();
        }

        private void prepareDrawingData(out RectangleF rect, out float startAngle, out float sweepAngle)
        {
            rect = new RectangleF(center.X - radius, center.Y - radius, radius * 2, radius * 2);
            startAngle = 180 * DrawingUtilities.CalculateAngle(center, pivot[0]) / (float)Math.PI;
            float endAngle = 180 * DrawingUtilities.CalculateAngle(center, pivot[1]) / (float)Math.PI;
            // if start angle is bigger than end angle
            // swap them to make sure
            // start angle is always smaller than end angle
            if (startAngle > endAngle)
            {
                float tempAngle = startAngle;
                startAngle = endAngle;
                endAngle = tempAngle;
            }

            int sweepSign = 1;
            sweepAngle = endAngle - startAngle;
            if (smallPart && sweepAngle > (360 - sweepAngle))
            {
                sweepAngle = 360 - sweepAngle;
                sweepSign = -1;
            }
            if (!smallPart && sweepAngle < (360 - sweepAngle))
            {
                sweepAngle = 360 - sweepAngle;
                sweepSign = -1;
            }
            sweepAngle *= sweepSign;
        }

        protected override void buildGraphicsPath(GraphicsPath gPath)
        {
            RectangleF bound;
            float startAngle;
            float sweepAngle;
            prepareDrawingData(out bound, out startAngle, out sweepAngle);

            gPath.AddArc(bound, startAngle, sweepAngle);
        }

        protected override void derivedDraw(PaintEventArgs e, Pen pen)
        {
            RectangleF bound;
            float startAngle;
            float sweepAngle;
            prepareDrawingData(out bound, out startAngle, out sweepAngle);

            e.Graphics.DrawArc(pen, bound, startAngle, sweepAngle);
        }

        protected override void derivedDrawLineBetweenControlPoints(PaintEventArgs e)
        {
            Pen dottedPen = new Pen(Color.Black, 1.0f);
            dottedPen.DashStyle = DashStyle.Dot;
            for (int i = 1; i < controlPoints.Count; i++)
                e.Graphics.DrawLine(dottedPen, controlPoints[0], controlPoints[i]);
            dottedPen.Dispose();
        }
    }

    // Duong tron
    public class Circle : IDrawingObject
    {
        private Point center;
        private int radius;

        public Circle(Point center, int radius, bool isFocus = false)
        {
            this.center = center;
            this.radius = radius;

            controlPoints = new List<Point>();
            controlPoints.Add(center);
            controlPoints.Add(new Point(center.X                               , center.Y - radius                      ));
            controlPoints.Add(new Point(center.X + (int)(radius / Math.Sqrt(2)), center.Y - (int)(radius / Math.Sqrt(2))));
            controlPoints.Add(new Point(center.X + radius                      , center.Y                               ));
            controlPoints.Add(new Point(center.X + (int)(radius / Math.Sqrt(2)), center.Y + (int)(radius / Math.Sqrt(2))));
            controlPoints.Add(new Point(center.X                               , center.Y + radius                      ));
            controlPoints.Add(new Point(center.X - (int)(radius / Math.Sqrt(2)), center.Y + (int)(radius / Math.Sqrt(2))));
            controlPoints.Add(new Point(center.X - radius                      , center.Y                               ));
            controlPoints.Add(new Point(center.X - (int)(radius / Math.Sqrt(2)), center.Y - (int)(radius / Math.Sqrt(2))));

            if (isFocus)
                focus();
        }

        protected override void buildGraphicsPath(GraphicsPath gPath)
        {
            gPath.AddEllipse(new Rectangle(center.X - radius, center.Y - radius, radius * 2, radius * 2));
        }

        protected override void derivedDraw(PaintEventArgs e, Pen pen)
        {
            e.Graphics.DrawEllipse(pen, new Rectangle(center.X - radius, center.Y - radius, radius * 2, radius * 2));
        }

        protected override void derivedDrawLineBetweenControlPoints(PaintEventArgs e)
        {
            Pen dottedPen = new Pen(Color.Black, 1.0f);
            dottedPen.DashStyle = DashStyle.Dot;
            for (int i = 1; i < controlPoints.Count; i++)
                e.Graphics.DrawLine(dottedPen, controlPoints[0], controlPoints[i]);
            dottedPen.Dispose();
        }
    }

    // Duong Elip
    public class Ellipse : IDrawingObject
    {
        private Point center;
        private int a;
        private int b;

        public Ellipse(Point center, int a, int b, bool isFocus = false)
        {
            this.center = center;
            this.a = a;
            this.b = b;

            controlPoints = new List<Point>();
            controlPoints.Add(center);
            controlPoints.Add(new Point(center.X, center.Y - b));
            controlPoints.Add(new Point(center.X + a, center.Y));
            controlPoints.Add(new Point(center.X, center.Y + b));
            controlPoints.Add(new Point(center.X - a, center.Y));

            if (isFocus)
                focus();
        }

        protected override void buildGraphicsPath(GraphicsPath gPath)
        {
            gPath.AddEllipse(new Rectangle(center.X - a, center.Y - b, 2 * a, 2 * b));
        }

        protected override void derivedDraw(PaintEventArgs e, Pen pen)
        {
            e.Graphics.DrawEllipse(pen, new Rectangle(center.X - a, center.Y - b, 2 * a, 2 * b));
        }

        protected override void derivedDrawLineBetweenControlPoints(PaintEventArgs e)
        {
            Pen dottedPen = new Pen(Color.Black, 1.0f);
            dottedPen.DashStyle = DashStyle.Dot;
            for (int i = 1; i < controlPoints.Count; i++)
                e.Graphics.DrawLine(dottedPen, controlPoints[0], controlPoints[i]);
            dottedPen.Dispose();
        }
    }

    // Cung Elip
    public class EllipseArc : IDrawingObject
    {
        private Point center;
        private Point[] pivot;
        private int a;
        private int b;
        private bool smallPart;

        public EllipseArc(Point center, int a, int b, Point[] pivot, bool smallPart, bool isFocus = false)
        {
            controlPoints = new List<Point>();
            controlPoints.Add(center);
            controlPoints.Add(pivot[0]);
            controlPoints.Add(pivot[1]);

            this.center = center;
            this.pivot = new Point[2];
            this.pivot[0] = pivot[0];
            this.pivot[1] = pivot[1];
            this.a = a;
            this.b = b;
            this.smallPart = smallPart;

            if (isFocus)
                focus();
        }

        private void prepareDrawingData(out Rectangle bound, out float startAngle, out float sweepAngle)
        {
            bound = new Rectangle(center.X - a, center.Y - b, 2 * a, 2 * b);

            startAngle = DrawingUtilities.CalculateAngle(center, pivot[0]) * 180 / (float)Math.PI;
            float endAngle = DrawingUtilities.CalculateAngle(center, pivot[1]) * 180 / (float)Math.PI;
            // if start angle is bigger than end angle
            // swap them to make sure start angle is always smaller than end angle
            if (startAngle > endAngle)
            {
                float tempAngle = startAngle;
                startAngle = endAngle;
                endAngle = tempAngle;
            }

            int sweepSign = 1;
            sweepAngle = endAngle - startAngle;
            if (smallPart && sweepAngle > 180)
            {
                sweepAngle = 360 - sweepAngle;
                sweepSign = -1;
            }
            if (!smallPart && sweepAngle <= 180)
            {
                sweepAngle = 360 - sweepAngle;
                sweepSign = -1;
            }
            sweepAngle *= sweepSign;
        }

        protected override void buildGraphicsPath(GraphicsPath gPath)
        {
            Rectangle bound;
            float startAngle;
            float sweepAngle;
            prepareDrawingData(out bound, out startAngle, out sweepAngle);

            gPath.AddArc(bound, startAngle, sweepAngle);
        }

        protected override void derivedDraw(PaintEventArgs e, Pen pen)
        {
            Rectangle bound;
            float startAngle;
            float sweepAngle;
            prepareDrawingData(out bound, out startAngle, out sweepAngle);

            e.Graphics.DrawArc(pen, bound, startAngle, sweepAngle);
        }

        protected override void derivedDrawLineBetweenControlPoints(PaintEventArgs e)
        {
            Pen dottedPen = new Pen(Color.Black, 1.0f);
            dottedPen.DashStyle = DashStyle.Dot;
            for (int i = 1; i < controlPoints.Count; i++)
                e.Graphics.DrawLine(dottedPen, controlPoints[0], controlPoints[i]);
            dottedPen.Dispose();
        }
    }

    // Duong Bezier
    public class Bezier : IDrawingObject
    {
        public Bezier(List<Point> controlPoints, bool isFocus = false)
        {
            this.controlPoints = controlPoints;

            if (isFocus)
                focus();
        }

        protected override void buildGraphicsPath(GraphicsPath gPath)
        {
            gPath.AddBezier(controlPoints[0], controlPoints[1], controlPoints[2], controlPoints[3]);
        }

        protected override void derivedDraw(PaintEventArgs e, Pen pen)
        {
            e.Graphics.DrawBezier(pen, controlPoints[0], controlPoints[1], controlPoints[2], controlPoints[3]);
        }

        protected override void derivedDrawLineBetweenControlPoints(PaintEventArgs e)
        {
            Pen dottedPen = new Pen(Color.Black, 1.0f);
            dottedPen.DashStyle = DashStyle.Dot;
            e.Graphics.DrawLines(dottedPen, controlPoints.ToArray());
            dottedPen.Dispose();
        }
    }

    // Text
    public class Text : IDrawingObject
    {
        private Point origin;
        private string text;
        private SizeF textSize;
        private Font font;
        private StringFormat format;

        private float resolutionX;
        private float resolutionY;

        public Text(Point origin, string text, SizeF textSize, Font font, StringFormat format, bool isFocus = false)
        {
            this.origin = origin;
            this.text = text;
            this.textSize = textSize;
            this.font = font;
            this.format = format;
            controlPoints = new List<Point>();
            controlPoints.Add(origin);
            controlPoints.Add(new Point(origin.X + (int)textSize.Width, origin.Y                       ));
            controlPoints.Add(new Point(origin.X + (int)textSize.Width, origin.Y + (int)textSize.Height));
            controlPoints.Add(new Point(origin.X                      , origin.Y + (int)textSize.Height));

            if (isFocus)
                focus();
        }

        public void setResolutionX(float resolutionX)
        {
            this.resolutionX = resolutionX;
        }

        public void setResolutionY(float resolutionY)
        {
            this.resolutionY = resolutionY;
        }

        protected override void buildGraphicsPath(GraphicsPath gPath)
        {
            gPath.AddString(text, font.FontFamily, (int)font.Style, font.Size * resolutionY / 72, origin, format);
            gPath.AddRectangle(DrawingUtilities.CreateControlRect(origin));
        }

        protected override void derivedDraw(PaintEventArgs e, Pen pen)
        {
            Brush brush = new SolidBrush(pen.Color);
            e.Graphics.DrawString(text, font, brush, origin);
        }

        protected override void derivedDrawLineBetweenControlPoints(PaintEventArgs e)
        {
            Pen dottedPen = new Pen(Color.Black, 1.0f);
            dottedPen.DashStyle = DashStyle.Dot;
            e.Graphics.DrawPolygon(dottedPen, controlPoints.ToArray());
            dottedPen.Dispose();
        }
    }
}
