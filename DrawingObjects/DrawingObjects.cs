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

        // calculate length from one point to another
        public static float CalculateLength(Point start, Point end)
        {
            int dx = start.X - end.X;
            int dy = start.Y - end.Y;
            return (float)Math.Sqrt(dx * dx + dy * dy);
        }
    }

    // TODO: implements Z-Index
    public abstract class IDrawingObject
    {
        private bool isFocus = false;
        protected List<Point> controlPoints;

        // color
        protected Color color = Color.Black;
        protected bool filledWithColor = false;
        protected Color fillColor = Color.Transparent;

        // transformation
        // all transformation for drawing shape
        protected Matrix rotate = new Matrix();

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
            else
            {
                filledWithColor = false;
            }
        }

        // interface for drawing object
        public void onDraw(Graphics g, Pen pen = null)
        {
            bool penNotSpecified = pen == null;
            if (penNotSpecified)
                pen = new Pen(color, 1.0f);

            derivedDraw(g, pen);

            if (isFocused())
            {
                derivedDrawLineBetweenControlPoints(g);
                drawControlRects(g);
            }

            if (penNotSpecified)
                pen.Dispose();
        }
        public void onDraw(Graphics g, Matrix transform, Pen pen = null)
        {
            bool penNotSpecified = pen == null;
            if (penNotSpecified)
                pen = new Pen(color, 1.0f);

            g.MultiplyTransform(transform);
            derivedDraw(g, pen);

            if (isFocused())
            {
                derivedDrawLineBetweenControlPoints(g);
                g.ResetTransform();
                drawControlRects(g, transform);
            }

            g.ResetTransform();

            if (penNotSpecified)
                pen.Dispose();
        }
        public void onDraw(Graphics g, int index, Point location, Pen pen = null)
        {
            bool penNotSpecified = pen == null;
            if (penNotSpecified)
                pen = new Pen(color, 1.0f);

            derivedDraw(g, pen, index, location);

            if (isFocused())
            {
                derivedDrawLineBetweenControlPoints(g, index, location);
                drawControlRects(g);
            }

            if (penNotSpecified)
                pen.Dispose();
        }
        // implements these to draw specific object
        protected abstract void derivedDraw(Graphics g, Pen pen);
        protected virtual void derivedDraw(Graphics g, Pen pen, int index, Point location) { } // TODO: change to abstract

        protected virtual void derivedDrawLineBetweenControlPoints(Graphics g) { }
        protected virtual void derivedDrawLineBetweenControlPoints(Graphics g, int index, Point location) { }
        private void drawControlRects(Graphics g, Matrix transform = null)
        {
            foreach (var point in controlPoints)
                DrawingUtilities.DrawControlRect(g, point);
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
        public int visibleControlPointIndex(Point location)
        {
            GraphicsPath gp = new GraphicsPath();
            for (int i = 0; i < controlPoints.Count; i++)
            {
                gp.AddRectangle(DrawingUtilities.CreateControlRect(controlPoints[i]));
                if (gp.IsVisible(location))
                    return i;
                gp.Reset();
            }
            return -1;
        }

        // implement these to build graphics path object to use for checking
        protected abstract void buildGraphicsPath(GraphicsPath gPath);
        private void addControlRectsToGraphicPath(GraphicsPath path)
        {
            foreach (var point in controlPoints)
                path.AddRectangle(DrawingUtilities.CreateControlRect(point));
        }

        // transform object
        protected virtual void transform(Matrix matrix)
        {
            Point[] controls = controlPoints.ToArray();
            matrix.TransformPoints(controls);
            for (int i = 0; i < controlPoints.Count; i++)
                controlPoints[i] = controls[i];
        }

        // move object
        // prepend translation matrix
        public void move(int xOffset, int yOffset)
        {
            Matrix translate = new Matrix();
            translate.Translate(xOffset, yOffset);
            transform(translate);
        }

        // interface for cloning object
        // that has offset with current object
        public abstract IDrawingObject clone();

        // interface for controlling object
        // using control points
        public virtual bool controllablePoint(int index) { return true; }
        // TODO: change to abstract
        public virtual IDrawingObject changeControlPoint(int index, Point newLocation)
        {
            return this;
        }

        // scale object
        // using scaling factor
        public void scale(float xFactor, float yFactor)
        {
            Point central = getCentralPoint();
            Matrix scale = new Matrix();
            scale.Translate(central.X, central.Y);
            scale.Scale(xFactor, yFactor);
            scale.Translate(-central.X, -central.Y);
            transform(scale);

            derivedScale(xFactor, yFactor);
        }
        protected virtual void derivedScale(float xFactor, float yFactor) { }
        public virtual Point getCentralPoint()
        {
            Point output = new Point();
            foreach(var point in controlPoints)
            {
                output.X += point.X;
                output.Y += point.Y;
            }
            output.X /= controlPoints.Count;
            output.Y /= controlPoints.Count;
            return output;
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

        public void remove(IDrawingObject obj)
        {
            list.Remove(obj);
        }

        public void drawAll(PaintEventArgs e)
        {
            foreach (var obj in list)
                obj.onDraw(e.Graphics);
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
        public Line(Point firstPoint, Point secondPoint, bool isFocus = false)
        {
            controlPoints = new List<Point>();
            controlPoints.Add(firstPoint);
            controlPoints.Add(secondPoint);

            if (isFocus)
                focus();
        }

        // drawing
        protected override void derivedDraw(Graphics g, Pen pen)
        {
            g.DrawLine(pen, controlPoints[0], controlPoints[1]);
        }
        protected override void derivedDraw(Graphics g, Pen pen, int index, Point location)
        {
            if (index == 0)
                g.DrawLine(pen, location, controlPoints[1]);
            else if (index == 1)
                g.DrawLine(pen, controlPoints[0], location);
        }

        // choosing
        protected override void buildGraphicsPath(GraphicsPath gPath)
        {
            gPath.AddLine(controlPoints[0], controlPoints[1]);
        }

        // cloning
        public override IDrawingObject clone()
        {
            return new Line(controlPoints[0], controlPoints[1]);
        }

        // controlling
        public override IDrawingObject changeControlPoint(int index, Point newLocation)
        {
            controlPoints[index] = newLocation;
            return this;
        }
    }

    // Hinh chu nhat
    public class Rect : IDrawingObject
    {
        public Rect(Rectangle rect, bool isFocus = false)
        {
            controlPoints = new List<Point>();
            controlPoints.Add(new Point(rect.Left, rect.Top));
            controlPoints.Add(new Point(rect.Right, rect.Top));
            controlPoints.Add(new Point(rect.Right, rect.Bottom));
            controlPoints.Add(new Point(rect.Left, rect.Bottom));

            if (isFocus)
                focus();
        }

        // drawing
        protected override void derivedDraw(Graphics g, Pen pen)
        {
            g.DrawPolygon(pen, controlPoints.ToArray());
        }
        protected override void derivedDraw(Graphics g, Pen pen, int index, Point location)
        {
            int fixedIndex = 0;
            if (index == 0)
                fixedIndex = 2;
            else if (index == 1)
                fixedIndex = 3;
            else if (index == 3)
                fixedIndex = 1;

            int startX = Math.Min(location.X, controlPoints[fixedIndex].X);
            int startY = Math.Min(location.Y, controlPoints[fixedIndex].Y);

            int endX = Math.Max(location.X, controlPoints[fixedIndex].X);
            int endY = Math.Max(location.Y, controlPoints[fixedIndex].Y);

            Rectangle rect = new Rectangle(startX, startY, endX - startX, endY - startY);
            g.DrawRectangle(pen, rect);
        }

        // choosing
        protected override void buildGraphicsPath(GraphicsPath gPath)
        {
            gPath.AddPolygon(controlPoints.ToArray());
        }

        // cloning
        public override IDrawingObject clone()
        {
            Size size = new Size(controlPoints[2].X - controlPoints[0].X, controlPoints[2].Y - controlPoints[0].Y);
            return new Rect(new Rectangle(controlPoints[0], size));
        }

        // controlling
        public override IDrawingObject changeControlPoint(int index, Point newLocation)
        {
            int fixedIndex = 0;
            if (index == 0)
                fixedIndex = 2;
            else if (index == 1)
                fixedIndex = 3;
            else if (index == 3)
                fixedIndex = 1;

            int startX = Math.Min(newLocation.X, controlPoints[fixedIndex].X);
            int startY = Math.Min(newLocation.Y, controlPoints[fixedIndex].Y);

            int endX = Math.Max(newLocation.X, controlPoints[fixedIndex].X);
            int endY = Math.Max(newLocation.Y, controlPoints[fixedIndex].Y);

            controlPoints[0] = new Point(startX, startY);
            controlPoints[1] = new Point(endX, startY);
            controlPoints[2] = new Point(endX, endY);
            controlPoints[3] = new Point(startX, endY);

            return this;
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
        
        // drawing
        protected override void derivedDraw(Graphics g, Pen pen)
        {
            g.DrawPolygon(pen, controlPoints.ToArray());
        }
        protected override void derivedDraw(Graphics g, Pen pen, int index, Point location)
        {
            Point firstFixed = new Point();
            Point secondFixed = new Point();
            Point symmetricPoint = new Point(0, 0);

            if (index == 0 || index == 2)
            {
                firstFixed = controlPoints[1];
                secondFixed = controlPoints[3];
            }
            else if (index == 1 || index == 3)
            {
                firstFixed = controlPoints[0];
                secondFixed = controlPoints[2];
            }

            symmetricPoint.X = firstFixed.X + secondFixed.X - location.X;
            symmetricPoint.Y = firstFixed.Y + secondFixed.Y - location.Y;

            List<Point> list = new List<Point>();
            list.Add(location);
            list.Add(firstFixed);
            list.Add(symmetricPoint);
            list.Add(secondFixed);

            g.DrawPolygon(pen, list.ToArray());
        }

        // choosing
        protected override void buildGraphicsPath(GraphicsPath gPath)
        {
            gPath.AddPolygon(controlPoints.ToArray());
        }

        // cloning
        public override IDrawingObject clone()
        {
            return new Parallelogram(controlPoints.ToArray());
        }

        // controlling
        public override IDrawingObject changeControlPoint(int index, Point newLocation)
        {
            Point firstFixed = new Point();
            Point secondFixed = new Point();
            Point symmetricPoint = new Point(0, 0);

            if (index == 0 || index == 2)
            {
                firstFixed = controlPoints[1];
                secondFixed = controlPoints[3];
            }
            else if (index == 1 || index == 3)
            {
                firstFixed = controlPoints[0];
                secondFixed = controlPoints[2];
            }

            symmetricPoint.X = firstFixed.X + secondFixed.X - newLocation.X;
            symmetricPoint.Y = firstFixed.Y + secondFixed.Y - newLocation.Y;

            controlPoints.Clear();
            controlPoints.Add(newLocation);
            controlPoints.Add(firstFixed);
            controlPoints.Add(symmetricPoint);
            controlPoints.Add(secondFixed);

            return this;
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

        // choosing
        protected override void buildGraphicsPath(GraphicsPath gPath)
        {
            gPath.AddPolygon(controlPoints.ToArray());
        }

        // drawing
        protected override void derivedDraw(Graphics g, Pen pen)
        {
            g.DrawPolygon(pen, controlPoints.ToArray());
        }
        protected override void derivedDraw(Graphics g, Pen pen, int index, Point location)
        {
            List<Point> list = new List<Point>();
            for (int i = 0; i < controlPoints.Count; i++)
            {
                if (i == index)
                    list.Add(location);
                else
                    list.Add(controlPoints[i]);
            }

            g.DrawPolygon(pen, list.ToArray());
        }

        // cloning
        public override IDrawingObject clone()
        {
            List<Point> newList = new List<Point>(controlPoints.Count);
            foreach (var point in controlPoints)
                newList.Add(new Point(point.X, point.Y));
            return new Polygon(newList);
        }

        // controlling
        public override IDrawingObject changeControlPoint(int index, Point newLocation)
        {
            controlPoints[index] = newLocation;

            return this;
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

        // choosing
        protected override void buildGraphicsPath(GraphicsPath gPath)
        {
            gPath.AddLines(controlPoints.ToArray());
        }

        // drawing
        protected override void derivedDraw(Graphics g, Pen pen)
        {
            g.DrawLines(pen, controlPoints.ToArray());
        }
        protected override void derivedDraw(Graphics g, Pen pen, int index, Point location)
        {
            List<Point> list = new List<Point>();
            for (int i = 0; i < controlPoints.Count; i++)
            {
                if (i == index)
                    list.Add(location);
                else
                    list.Add(controlPoints[i]);
            }

            g.DrawLines(pen, list.ToArray());
        }

        // cloning
        public override IDrawingObject clone()
        {
            List<Point> newList = new List<Point>(controlPoints.Count);
            foreach (var point in controlPoints)
                newList.Add(new Point(point.X, point.Y));
            return new BrokenLine(newList);
        }

        // controlling
        public override IDrawingObject changeControlPoint(int index, Point newLocation)
        {
            controlPoints[index] = newLocation;

            return this;
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

        // choosing
        protected override void buildGraphicsPath(GraphicsPath gPath)
        {
            RectangleF bound;
            float startAngle;
            float sweepAngle;
            prepareDrawingData(out bound, out startAngle, out sweepAngle);

            gPath.AddArc(bound, startAngle, sweepAngle);
        }

        // drawing
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
        protected override void derivedDraw(Graphics g, Pen pen)
        {
            RectangleF bound;
            float startAngle;
            float sweepAngle;
            prepareDrawingData(out bound, out startAngle, out sweepAngle);

            g.DrawArc(pen, bound, startAngle, sweepAngle);
        }
        protected override void derivedDrawLineBetweenControlPoints(Graphics g)
        {
            Pen dottedPen = new Pen(Color.Black, 1.0f);
            dottedPen.DashStyle = DashStyle.Dot;
            for (int i = 1; i < controlPoints.Count; i++)
                g.DrawLine(dottedPen, controlPoints[0], controlPoints[i]);
            dottedPen.Dispose();
        }
        protected override void derivedDraw(Graphics g, Pen pen, int index, Point location)
        {
            List<Point> list = new List<Point>();
            for (int i = 0; i < controlPoints.Count; i++)
            {
                if (i == index)
                    list.Add(location);
                else
                    list.Add(controlPoints[i]);
            }

            g.DrawLines(pen, list.ToArray());
        }

        // cloning
        public override IDrawingObject clone()
        {
            return new CircleArc(center, pivot, smallPart, radius);
        }

        // controlling
        public override bool controllablePoint(int index)
        {
            return index != 0;
        }

        // transforming
        protected override void transform(Matrix matrix)
        {
            base.transform(matrix);
            Point[] list = new Point[3];
            list[0] = center;
            list[1] = pivot[0];
            list[2] = pivot[1];
            matrix.TransformPoints(list);

            center = list[0];
            pivot[0] = list[1];
            pivot[1] = list[2];
        }

        // scaling
        public override Point getCentralPoint()
        {
            return center;
        }
        protected override void derivedScale(float xFactor, float yFactor)
        {
            base.derivedScale(xFactor, yFactor);
            radius = (int)(Math.Min(xFactor, yFactor) * radius);
        }
    }

    // Duong tron
    public class Circle : IDrawingObject
    {
        private int radius;

        public Circle(Point center, int radius, bool isFocus = false)
        {
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

        // choosing
        protected override void buildGraphicsPath(GraphicsPath gPath)
        {
            gPath.AddEllipse(new Rectangle(controlPoints[0].X - radius, controlPoints[0].Y - radius, radius * 2, radius * 2));
        }

        // drawing
        protected override void derivedDraw(Graphics g, Pen pen)
        {
            g.DrawEllipse(pen, new Rectangle(controlPoints[0].X - radius, controlPoints[0].Y - radius, radius * 2, radius * 2));
        }
        protected override void derivedDrawLineBetweenControlPoints(Graphics g)
        {
            Pen dottedPen = new Pen(Color.Black, 1.0f);
            dottedPen.DashStyle = DashStyle.Dot;
            for (int i = 1; i < controlPoints.Count; i++)
                g.DrawLine(dottedPen, controlPoints[0], controlPoints[i]);
            dottedPen.Dispose();
        }

        // cloning
        public override IDrawingObject clone()
        {
            return new Circle(controlPoints[0], radius);
        }

        // controlling
        public override bool controllablePoint(int index)
        {
            return index != 0;
        }

        // transforming
        protected override void transform(Matrix matrix) { }

        // scaling
        public override Point getCentralPoint()
        {
            return controlPoints[0];
        }
        protected override void derivedScale(float xFactor, float yFactor)
        {
            base.derivedScale(xFactor, yFactor);
            radius = (int)(Math.Min(xFactor, yFactor) * radius);

            Point center = controlPoints[0];
            controlPoints.Clear();
            controlPoints.Add(center);
            controlPoints.Add(new Point(center.X, center.Y - radius));
            controlPoints.Add(new Point(center.X + (int)(radius / Math.Sqrt(2)), center.Y - (int)(radius / Math.Sqrt(2))));
            controlPoints.Add(new Point(center.X + radius, center.Y));
            controlPoints.Add(new Point(center.X + (int)(radius / Math.Sqrt(2)), center.Y + (int)(radius / Math.Sqrt(2))));
            controlPoints.Add(new Point(center.X, center.Y + radius));
            controlPoints.Add(new Point(center.X - (int)(radius / Math.Sqrt(2)), center.Y + (int)(radius / Math.Sqrt(2))));
            controlPoints.Add(new Point(center.X - radius, center.Y));
            controlPoints.Add(new Point(center.X - (int)(radius / Math.Sqrt(2)), center.Y - (int)(radius / Math.Sqrt(2))));
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

        // choosing
        protected override void buildGraphicsPath(GraphicsPath gPath)
        {
            gPath.AddEllipse(new Rectangle(center.X - a, center.Y - b, 2 * a, 2 * b));
        }

        // drawing
        protected override void derivedDraw(Graphics g, Pen pen)
        {
            g.DrawEllipse(pen, new Rectangle(center.X - a, center.Y - b, 2 * a, 2 * b));
        }
        protected override void derivedDrawLineBetweenControlPoints(Graphics g)
        {
            Pen dottedPen = new Pen(Color.Black, 1.0f);
            dottedPen.DashStyle = DashStyle.Dot;
            for (int i = 1; i < controlPoints.Count; i++)
                g.DrawLine(dottedPen, controlPoints[0], controlPoints[i]);
            dottedPen.Dispose();
        }

        // cloning
        public override IDrawingObject clone()
        {
            return new Ellipse(center, a, b);
        }

        // controlling
        public override bool controllablePoint(int index)
        {
            return index != 0;
        }

        // transforming
        protected override void transform(Matrix matrix)
        {
            base.transform(matrix);
            Point[] list = new Point[1];
            list[0] = center;
            matrix.TransformPoints(list);

            center = list[0];
        }

        // scaling
        public override Point getCentralPoint()
        {
            return center;
        }
        protected override void derivedScale(float xFactor, float yFactor)
        {
            base.derivedScale(xFactor, yFactor);
            a = (int)(xFactor * a);
            b = (int)(yFactor * b);
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

        // choosing
        protected override void buildGraphicsPath(GraphicsPath gPath)
        {
            Rectangle bound;
            float startAngle;
            float sweepAngle;
            prepareDrawingData(out bound, out startAngle, out sweepAngle);

            gPath.AddArc(bound, startAngle, sweepAngle);
        }

        // drawing
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
        protected override void derivedDraw(Graphics g, Pen pen)
        {
            Rectangle bound;
            float startAngle;
            float sweepAngle;
            prepareDrawingData(out bound, out startAngle, out sweepAngle);

            g.DrawArc(pen, bound, startAngle, sweepAngle);
        }
        protected override void derivedDrawLineBetweenControlPoints(Graphics g)
        {
            Pen dottedPen = new Pen(Color.Black, 1.0f);
            dottedPen.DashStyle = DashStyle.Dot;
            for (int i = 1; i < controlPoints.Count; i++)
                g.DrawLine(dottedPen, controlPoints[0], controlPoints[i]);
            dottedPen.Dispose();
        }

        // cloning
        public override IDrawingObject clone()
        {
            return new EllipseArc(center, a, b, pivot, smallPart);
        }

        // controlling
        public override bool controllablePoint(int index)
        {
            return index != 0;
        }

        // transforming
        protected override void transform(Matrix matrix)
        {
            base.transform(matrix);
            Point[] list = new Point[3];
            list[0] = center;
            list[1] = pivot[0];
            list[2] = pivot[1];
            matrix.TransformPoints(list);

            center = list[0];
            pivot[0] = list[1];
            pivot[1] = list[2];
        }

        // scaling
        public override Point getCentralPoint()
        {
            return center;
        }
        protected override void derivedScale(float xFactor, float yFactor)
        {
            base.derivedScale(xFactor, yFactor);
            a = (int)(xFactor * a);
            b = (int)(yFactor * b);
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

        // choosing
        protected override void buildGraphicsPath(GraphicsPath gPath)
        {
            gPath.AddBezier(controlPoints[0], controlPoints[1], controlPoints[2], controlPoints[3]);
        }

        // drawing
        protected override void derivedDraw(Graphics g, Pen pen)
        {
            g.DrawBezier(pen, controlPoints[0], controlPoints[1], controlPoints[2], controlPoints[3]);
        }
        protected override void derivedDrawLineBetweenControlPoints(Graphics g)
        {
            Pen dottedPen = new Pen(Color.Black, 1.0f);
            dottedPen.DashStyle = DashStyle.Dot;
            g.DrawLines(dottedPen, controlPoints.ToArray());
            dottedPen.Dispose();
        }

        // cloning
        public override IDrawingObject clone()
        {
            List<Point> newList = new List<Point>(controlPoints.Count);
            foreach (var point in controlPoints)
                newList.Add(new Point(point.X, point.Y));
            return new Bezier(newList);
        }
    }

    // Text
    public class Text : IDrawingObject
    {
        private string text;
        private Font font;
        private StringFormat format;

        private float resolutionX;
        private float resolutionY;

        public Text(Point origin, string text, Font font, StringFormat format, bool isFocus = false)
        {
            this.text = text;
            this.font = font;
            this.format = format;

            Size textSize = TextRenderer.MeasureText(text, font);

            controlPoints = new List<Point>();
            controlPoints.Add(origin);
            controlPoints.Add(new Point(origin.X + textSize.Width, origin.Y                  ));
            controlPoints.Add(new Point(origin.X + textSize.Width, origin.Y + textSize.Height));
            controlPoints.Add(new Point(origin.X                 , origin.Y + textSize.Height));

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

        // choosing
        protected override void buildGraphicsPath(GraphicsPath gPath)
        {
            gPath.AddString(text, font.FontFamily, (int)font.Style, font.Size * resolutionY / 72, controlPoints[0], format);
            gPath.AddRectangle(DrawingUtilities.CreateControlRect(controlPoints[0]));
        }

        // drawing
        protected override void derivedDraw(Graphics g, Pen pen)
        {
            Brush brush = new SolidBrush(pen.Color);
            g.DrawString(text, font, brush, controlPoints[0]);
        }
        protected override void derivedDrawLineBetweenControlPoints(Graphics g)
        {
            Pen dottedPen = new Pen(Color.Black, 1.0f);
            dottedPen.DashStyle = DashStyle.Dot;
            g.DrawPolygon(dottedPen, controlPoints.ToArray());
            dottedPen.Dispose();
        }

        // cloning
        public override IDrawingObject clone()
        {
            return new Text(controlPoints[0], text, new Font(font, font.Style), new StringFormat(format));
        }

        // controlling
        public override bool controllablePoint(int index)
        {
            return index == 0;
        }
    }
}
