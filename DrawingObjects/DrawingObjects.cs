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
        public const int CONTROL_RECT_SIZE = 10;

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

        // Restrict a point to be on a circle or an ellipse
        public static Point Restrict(Point center, float radius, Point toRestrict)
        {
            float endAngle = DrawingUtilities.CalculateAngle(center, toRestrict);
            toRestrict.X = (int)(radius * Math.Cos(endAngle)) + center.X;
            toRestrict.Y = (int)(radius * Math.Sin(endAngle)) + center.Y;
            return toRestrict;
        }
        public static Point Restrict(Point center, float a, float b, Point toRestrict)
        {
            float angle = DrawingUtilities.CalculateAngle(center, toRestrict);
            float a2 = a * a;
            float b2 = b * b;
            float sin2 = (float)(Math.Sin(angle) * Math.Sin(angle));
            float cos2 = (float)(Math.Cos(angle) * Math.Cos(angle));
            float radius = (float)Math.Sqrt(a2 * b2 / (b2 * cos2 + a2 * sin2));

            toRestrict.X = (int)(radius * Math.Cos(angle)) + center.X;
            toRestrict.Y = (int)(radius * Math.Sin(angle)) + center.Y;
            return toRestrict;
        }
    }

    // TODO: implements Z-Index
    public abstract class IDrawingObject
    {
        private bool isFocus = false;
        protected List<Point> controlPoints;

        // color
        protected Pen borderPen = new Pen(Color.Black, 1.0f);
        protected bool isFillable = false;
        protected Brush brush = null;

        // transformation
        // all transformation for drawing shape
        protected Matrix rotationMatrix = new Matrix();
        public float angle = 0;

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
            borderPen.Color = c;
        }

        public void setBrush(Brush brush)
        {
            this.brush = brush;
        }

        public Point getControlPointLocation(int index)
        {
            Point[] points = controlPoints.ToArray();
            rotationMatrix.TransformPoints(points);
            if (index < 0 || index >= points.Length)
                return new Point();
            return points[index];
        }

        // interface for drawing object
        public void onDraw(Graphics g, Pen pen = null)
        {
            bool penNotSpecified = pen == null;
            if (penNotSpecified)
                pen = borderPen;

            g.MultiplyTransform(rotationMatrix);
            derivedDraw(g, pen);

            if (isFocused())
            {
                derivedDrawLineBetweenControlPoints(g);
                g.ResetTransform();
                drawControlRects(g, rotationMatrix);
            }

            g.ResetTransform();
        }
        public void onDraw(Graphics g, int index, Point location, Pen pen = null)
        {
            bool penNotSpecified = pen == null;
            if (penNotSpecified)
                pen = borderPen;

            g.MultiplyTransform(rotationMatrix);
            derivedDraw(g, pen, index, location);

            if (isFocused())
            {
                derivedDrawLineBetweenControlPoints(g, index, location);
                g.ResetTransform();
                drawControlRects(g, rotationMatrix);
            }

            g.ResetTransform();
        }
        // implements these to draw specific object
        protected abstract void derivedDraw(Graphics g, Pen pen);
        protected virtual void derivedDraw(Graphics g, Pen pen, int index, Point location) { } // TODO: change to abstract

        protected virtual void derivedDrawLineBetweenControlPoints(Graphics g) { }
        protected virtual void derivedDrawLineBetweenControlPoints(Graphics g, int index, Point location) { }
        private void drawControlRects(Graphics g, Matrix transform = null)
        {
            Point[] points = controlPoints.ToArray();
            if (transform != null)
                transform.TransformPoints(points);
            foreach (var point in points)
                DrawingUtilities.DrawControlRect(g, point);
        }

        // interface for checking is "location" belongs to object
        public bool isVisible(Point location)
        {
            GraphicsPath gPath = new GraphicsPath();
            GraphicsPath controlRectPath = new GraphicsPath();
            buildGraphicsPath(gPath);
            if (isFocused())
                addControlRectsToGraphicPath(controlRectPath);

            gPath.Transform(rotationMatrix);
            controlRectPath.Transform(rotationMatrix);
            if (brush != null && isFillable)
            {
                return gPath.IsOutlineVisible(location, new Pen(Brushes.Black, 10)) || gPath.IsVisible(location) || controlRectPath.IsVisible(location);
            }
            return gPath.IsOutlineVisible(location, new Pen(Brushes.Black, 10)) || controlRectPath.IsVisible(location);
        }
        public int visibleControlPointIndex(Point location)
        {
            GraphicsPath gp = new GraphicsPath();
            for (int i = 0; i < controlPoints.Count; i++)
            {
                gp.AddRectangle(DrawingUtilities.CreateControlRect(controlPoints[i]));
                gp.Transform(rotationMatrix);
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

        // transform object's control points
        protected void transform(Matrix matrix)
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

            rotationMatrix.Reset();
            rotationMatrix.RotateAt(angle, getCentralPoint());
        }

        // interface for cloning object
        // that has offset with current object
        public IDrawingObject clone()
        {
            IDrawingObject obj = derivedClone();
            obj.angle = angle;
            return obj;
        }
        protected abstract IDrawingObject derivedClone();

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
            if (!canScale(xFactor, yFactor))
            {
                rotationMatrix.Reset();
                rotationMatrix.RotateAt(angle, getCentralPoint());
                return;
            }

            Point central = getCentralPoint();
            Matrix scale = new Matrix();
            scale.Translate(central.X, central.Y);
            scale.Scale(xFactor, yFactor);
            scale.Translate(-central.X, -central.Y);
            transform(scale);
            derivedScale(xFactor, yFactor);

            rotationMatrix.Reset();
            rotationMatrix.RotateAt(angle, getCentralPoint());
        }
        protected virtual void derivedScale(float xFactor, float yFactor) { }
        protected virtual bool canScale(float xFactor, float yFactor)
        {
            return true;
        }
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

        // rotate object
        public void rotate(float angle)
        {
            Point central = getCentralPoint();
            this.angle += angle;
            rotationMatrix.Reset();
            rotationMatrix.RotateAt(this.angle, central);
        }
        public Point invertRotation(Point p)
        {
            Point[] tempList = new Point[] { p };
            Matrix invert = rotationMatrix.Clone();
            invert.Invert();
            invert.TransformPoints(tempList);
            return tempList[0];
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

        public void replace(IDrawingObject oldObj, IDrawingObject newObj)
        {
            int index = -1;
            for (int i = 0; i < list.Count; i++)
                if (ReferenceEquals(oldObj, list[i]))
                    index = i;
            if (index >= 0)
            {
                list.Insert(index, newObj);
                list.Remove(oldObj);
            }
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

        public List<IDrawingObject> getAllVisible(Point location)
        {
            List<IDrawingObject> output = new List<IDrawingObject>();
            foreach (var obj in list)
                if (obj.isVisible(location))
                    output.Add(obj);
            return output;
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
        protected override IDrawingObject derivedClone()
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
            isFillable = true;

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
            if (brush != null && isFillable)
                g.FillPolygon(brush, controlPoints.ToArray());
            g.DrawPolygon(pen, controlPoints.ToArray());
        }
        protected override void derivedDraw(Graphics g, Pen pen, int index, Point location)
        {
            Point[] points = controlPoints.ToArray();
            points[index] = location;
            g.DrawPolygon(pen, points);
        }

        // choosing
        protected override void buildGraphicsPath(GraphicsPath gPath)
        {
            gPath.AddPolygon(controlPoints.ToArray());
        }

        // cloning
        protected override IDrawingObject derivedClone()
        {
            Size size = new Size(controlPoints[2].X - controlPoints[0].X, controlPoints[2].Y - controlPoints[0].Y);
            return new Rect(new Rectangle(controlPoints[0], size));
        }

        // controlling
        public override IDrawingObject changeControlPoint(int index, Point newLocation)
        {
            controlPoints[index] = newLocation;
            Point[] points = controlPoints.ToArray();
            rotationMatrix.TransformPoints(points);

            Polygon poly = new Polygon(new List<Point>(points), true);
            poly.setBrush(brush);
            poly.setOutlineColor(borderPen.Color);

            return poly;
        }
    }

    // Hinh binh hanh
    public class Parallelogram : IDrawingObject
    {
        public Parallelogram(Point[] controlPoints, bool isFocus = false)
        {
            isFillable = true;
            this.controlPoints = new List<Point>();
            for (int i = 0; i < 4; i++)
                this.controlPoints.Add(controlPoints[i]);

            if (isFocus)
                focus();
        }
        
        // drawing
        protected override void derivedDraw(Graphics g, Pen pen)
        {
            if (isFillable && brush != null)
                g.FillPolygon(brush, controlPoints.ToArray());
            g.DrawPolygon(pen, controlPoints.ToArray());
        }
        protected override void derivedDraw(Graphics g, Pen pen, int index, Point location)
        {
            Point[] points = controlPoints.ToArray();
            points[index] = location;
            g.DrawPolygon(pen, points);
        }

        // choosing
        protected override void buildGraphicsPath(GraphicsPath gPath)
        {
            gPath.AddPolygon(controlPoints.ToArray());
        }

        // cloning
        protected override IDrawingObject derivedClone()
        {
            return new Parallelogram(controlPoints.ToArray());
        }

        // controlling
        public override IDrawingObject changeControlPoint(int index, Point newLocation)
        {
            controlPoints[index] = newLocation;
            Point[] points = controlPoints.ToArray();
            rotationMatrix.TransformPoints(points);

            Polygon poly = new Polygon(new List<Point>(points), true);
            poly.setBrush(brush);
            poly.setOutlineColor(borderPen.Color);

            return poly;
        }
    }

    // Da giac
    public class Polygon : IDrawingObject
    {
        public Polygon(List<Point> controlPoints, bool isFocus = false)
        {
            isFillable = true;
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
            if (brush != null && isFillable)
                g.FillPolygon(brush, controlPoints.ToArray());
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
        protected override IDrawingObject derivedClone()
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
        protected override IDrawingObject derivedClone()
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
        private bool smallPart;
        private float radius;

        public CircleArc(Point center, Point[] pivot, bool smallPart, float radius, bool isFocus = false)
        {
            controlPoints = new List<Point>();
            controlPoints.Add(center);

            for (int i = 0; i < 2; i++)
                controlPoints.Add(pivot[i]);

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
            rect = new RectangleF(controlPoints[0].X - radius, controlPoints[0].Y - radius, radius * 2, radius * 2);
            startAngle = 180 * DrawingUtilities.CalculateAngle(controlPoints[0], controlPoints[1]) / (float)Math.PI;
            float endAngle = 180 * DrawingUtilities.CalculateAngle(controlPoints[0], controlPoints[2]) / (float)Math.PI;
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
            int other = index == 1 ? 2 : 1;
            location = DrawingUtilities.Restrict(controlPoints[0], radius, location);
            RectangleF rect = new RectangleF(controlPoints[0].X - radius, controlPoints[0].Y - radius, radius * 2, radius * 2);
            float startAngle = 180 * DrawingUtilities.CalculateAngle(controlPoints[0], location) / (float)Math.PI;
            float endAngle = 180 * DrawingUtilities.CalculateAngle(controlPoints[0], controlPoints[other]) / (float)Math.PI;
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
            float sweepAngle = endAngle - startAngle;
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

            g.DrawArc(pen, rect, startAngle, sweepAngle);
            g.DrawLine(pen, controlPoints[0], location);
            g.DrawLine(pen, controlPoints[0], controlPoints[other]);
        }

        // cloning
        protected override IDrawingObject derivedClone()
        {
            return new CircleArc(controlPoints[0], new Point[] { controlPoints[1], controlPoints[2] }, smallPart, radius);
        }

        // controlling
        public override bool controllablePoint(int index)
        {
            return index != 0;
        }

        // scaling
        public override Point getCentralPoint()
        {
            return controlPoints[0];
        }
        protected override void derivedScale(float xFactor, float yFactor)
        {
            base.derivedScale(xFactor, yFactor);
            xFactor = Math.Abs(xFactor);
            yFactor = Math.Abs(yFactor);
            radius = (Math.Min(xFactor, yFactor) * radius);

            for (int i = 1; i < controlPoints.Count; i++)
                controlPoints[i] = DrawingUtilities.Restrict(controlPoints[0], radius, controlPoints[i]);
        }

        // controlling
        public override IDrawingObject changeControlPoint(int index, Point newLocation)
        {
            controlPoints[index] = DrawingUtilities.Restrict(controlPoints[0], radius, newLocation);

            return this;
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
        protected override void derivedDraw(Graphics g, Pen pen, int index, Point location)
        {
            float dx = Math.Abs(location.X - controlPoints[0].X);
            float dy = Math.Abs(location.Y - controlPoints[0].Y);

            RectangleF bound = new RectangleF();
            if (index % 2 == 0)
            {
                bound.Width = 2 * dx;
                bound.Height = 2 * dy;
            }
            else if (index == 1 || index == 5)
            {
                bound.Width = 2 * radius;
                bound.Height = 2 * dy;
            }
            else
            {
                bound.Width = 2 * dx;
                bound.Height = 2 * radius;
            }
            bound.Location = new PointF(controlPoints[0].X - bound.Width / 2, controlPoints[0].Y - bound.Height / 2);

            g.DrawEllipse(pen, bound);
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
        protected override IDrawingObject derivedClone()
        {
            return new Circle(controlPoints[0], radius);
        }

        // controlling
        public override bool controllablePoint(int index)
        {
            return index != 0;
        }
        public override IDrawingObject changeControlPoint(int index, Point newLocation)
        {
            float dx = Math.Abs(newLocation.X - controlPoints[0].X);
            float dy = Math.Abs(newLocation.Y - controlPoints[0].Y);

            Ellipse ellipse = null;
            if (index % 2 == 0)
                ellipse = new Ellipse(controlPoints[0], dx, dy, true);
            else if (index == 1 || index == 5)
                ellipse = new Ellipse(controlPoints[0], radius, dy, true);
            else
                ellipse = new Ellipse(controlPoints[0], dx, radius, true);
            ellipse.rotate(angle);

            return ellipse;
        }

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
        private float a;
        private float b;

        public Ellipse(Point center, float a, float b, bool isFocus = false)
        {
            this.a = a;
            this.b = b;

            controlPoints = new List<Point>();
            controlPoints.Add(center);
            controlPoints.Add(new Point(center.X, (int)(center.Y - b)));
            controlPoints.Add(new Point((int)(center.X + a), center.Y));
            controlPoints.Add(new Point(center.X, (int)(center.Y + b)));
            controlPoints.Add(new Point((int)(center.X - a), center.Y));

            if (isFocus)
                focus();
        }

        // choosing
        protected override void buildGraphicsPath(GraphicsPath gPath)
        {
            gPath.AddEllipse(new RectangleF(controlPoints[0].X - a, controlPoints[0].Y - b, 2 * a, 2 * b));
        }

        // drawing
        protected override void derivedDraw(Graphics g, Pen pen)
        {
            g.DrawEllipse(pen, new RectangleF(controlPoints[0].X - a, controlPoints[0].Y - b, 2 * a, 2 * b));
        }
        protected override void derivedDraw(Graphics g, Pen pen, int index, Point location)
        {
            float dx = Math.Abs(location.X - controlPoints[0].X);
            float dy = Math.Abs(location.Y - controlPoints[0].Y);

            RectangleF bound = new RectangleF();
            if (index % 2 == 0)
            {
                bound.Width = 2 * dx;
                bound.Height = 2 * b;
            }
            else
            {
                bound.Width = 2 * a;
                bound.Height = 2 * dy;
            }
            bound.Location = new PointF(controlPoints[0].X - bound.Width / 2, controlPoints[0].Y - bound.Height / 2);

            g.DrawEllipse(pen, bound);
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
        protected override IDrawingObject derivedClone()
        {
            return new Ellipse(controlPoints[0], a, b);
        }

        // controlling
        public override bool controllablePoint(int index)
        {
            return index != 0;
        }
        public override IDrawingObject changeControlPoint(int index, Point newLocation)
        {
            float dx = Math.Abs(newLocation.X - controlPoints[0].X);
            float dy = Math.Abs(newLocation.Y - controlPoints[0].Y);

            if (index % 2 == 0)
                a = dx;
            else
                b = dy;

            Point center = controlPoints[0];
            controlPoints.Clear();
            controlPoints.Add(center);
            controlPoints.Add(new Point(center.X, (int)(center.Y - b)));
            controlPoints.Add(new Point((int)(center.X + a), center.Y));
            controlPoints.Add(new Point(center.X, (int)(center.Y + b)));
            controlPoints.Add(new Point((int)(center.X - a), center.Y));

            return this;
        }

        // scaling
        public override Point getCentralPoint()
        {
            return controlPoints[0];
        }
        protected override void derivedScale(float xFactor, float yFactor)
        {
            base.derivedScale(xFactor, yFactor);
            a = (xFactor * a);
            b = (yFactor * b);

            Point center = getCentralPoint();
            controlPoints.Clear();
            controlPoints.Add(center);
            controlPoints.Add(new Point(center.X, (int)(center.Y - b)));
            controlPoints.Add(new Point((int)(center.X + a), center.Y));
            controlPoints.Add(new Point(center.X, (int)(center.Y + b)));
            controlPoints.Add(new Point((int)(center.X - a), center.Y));
        }
    }

    // Cung Elip
    public class EllipseArc : IDrawingObject
    {
        private float a;
        private float b;
        private bool smallPart;

        public EllipseArc(Point center, float a, float b, Point[] pivot, bool smallPart, bool isFocus = false)
        {
            controlPoints = new List<Point>();
            controlPoints.Add(center);
            controlPoints.Add(pivot[0]);
            controlPoints.Add(pivot[1]);

            this.a = a;
            this.b = b;
            this.smallPart = smallPart;

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
        private void prepareDrawingData(out RectangleF bound, out float startAngle, out float sweepAngle)
        {
            bound = new RectangleF(controlPoints[0].X - a, controlPoints[0].Y - b, 2 * a, 2 * b);

            startAngle = DrawingUtilities.CalculateAngle(controlPoints[0], controlPoints[1]) * 180 / (float)Math.PI;
            float endAngle = DrawingUtilities.CalculateAngle(controlPoints[0], controlPoints[2]) * 180 / (float)Math.PI;
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
            int other = index == 1 ? 2 : 1;
            location = DrawingUtilities.Restrict(controlPoints[0], a, b, location);
            RectangleF bound = new RectangleF(controlPoints[0].X - a, controlPoints[0].Y - b, 2 * a, 2 * b);

            float startAngle = DrawingUtilities.CalculateAngle(controlPoints[0], location) * 180 / (float)Math.PI;
            float endAngle = DrawingUtilities.CalculateAngle(controlPoints[0], controlPoints[other]) * 180 / (float)Math.PI;
            // if start angle is bigger than end angle
            // swap them to make sure start angle is always smaller than end angle
            if (startAngle > endAngle)
            {
                float tempAngle = startAngle;
                startAngle = endAngle;
                endAngle = tempAngle;
            }

            int sweepSign = 1;
            float sweepAngle = endAngle - startAngle;
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

            g.DrawArc(pen, bound, startAngle, sweepAngle);
            g.DrawLine(pen, controlPoints[0], location);
            g.DrawLine(pen, controlPoints[0], controlPoints[other]);
        }

        // cloning
        protected override IDrawingObject derivedClone()
        {
            return new EllipseArc(controlPoints[0], a, b, new Point[] { controlPoints[1], controlPoints[2] }, smallPart);
        }

        // controlling
        public override bool controllablePoint(int index)
        {
            return index != 0;
        }

        // scaling
        public override Point getCentralPoint()
        {
            return controlPoints[0];
        }
        protected override void derivedScale(float xFactor, float yFactor)
        {
            base.derivedScale(xFactor, yFactor);
            a = (Math.Abs(xFactor) * a);
            b = (Math.Abs(yFactor) * b);

            controlPoints[1] = DrawingUtilities.Restrict(controlPoints[0], a, b, controlPoints[1]);
            controlPoints[2] = DrawingUtilities.Restrict(controlPoints[0], a, b, controlPoints[2]);
        }

        // controlling
        public override IDrawingObject changeControlPoint(int index, Point newLocation)
        {
            controlPoints[index] = DrawingUtilities.Restrict(controlPoints[0], a, b, newLocation);

            return this;
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

            g.DrawBezier(pen, list[0], list[1], list[2], list[3]);
        }
        protected override void derivedDrawLineBetweenControlPoints(Graphics g)
        {
            Pen dottedPen = new Pen(Color.Black, 1.0f);
            dottedPen.DashStyle = DashStyle.Dot;
            g.DrawLines(dottedPen, controlPoints.ToArray());
            dottedPen.Dispose();
        }

        // cloning
        protected override IDrawingObject derivedClone()
        {
            List<Point> newList = new List<Point>(controlPoints.Count);
            foreach (var point in controlPoints)
                newList.Add(new Point(point.X, point.Y));
            return new Bezier(newList);
        }

        // controlling
        public override IDrawingObject changeControlPoint(int index, Point newLocation)
        {
            controlPoints[index] = newLocation;

            return this;
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
        protected override IDrawingObject derivedClone()
        {
            return new Text(controlPoints[0], text, new Font(font, font.Style), new StringFormat(format));
        }

        // controlling
        //public override bool controllablePoint(int index)
        //{
        //    return index != 0;
        //}

        // scaling
        protected override void derivedScale(float xFactor, float yFactor)
        {
            base.derivedScale(xFactor, yFactor);
            font = new Font(font.FontFamily, Math.Abs(yFactor) * font.Size, font.Style);

            Size textSize = TextRenderer.MeasureText(text, font);
            Point central = getCentralPoint();

            controlPoints.Clear();
            controlPoints.Add(new Point(central.X - textSize.Width / 2, central.Y - textSize.Height / 2));
            controlPoints.Add(new Point(central.X + textSize.Width / 2, central.Y - textSize.Height / 2));
            controlPoints.Add(new Point(central.X + textSize.Width / 2, central.Y + textSize.Height / 2));
            controlPoints.Add(new Point(central.X - textSize.Width / 2, central.Y + textSize.Height / 2));
        }
    }

    // Parabol
    public class Parabola : IDrawingObject
    {
        DrawingAlgorithms.Parabola parabola;
        DrawingAlgorithms.Algorithm algorithm;

        public Parabola(List<Point> controlPoints, bool horizontal, bool isFocus = false)
        {
            this.controlPoints = controlPoints;

            parabola = newParabola(controlPoints.ToArray(), horizontal);

            // create algorithm
            algorithm = new DrawingAlgorithms.MidPoint();

            if (isFocus)
                focus();
        }

        // Get half of parabola point list
        // index = 0 : get first half
        // index = 1 : get second half
        private List<Point> getHalfOfParabola(DrawingAlgorithms.Parabola para, int index)
        {
            List<DrawingAlgorithms.Point2D> point2ds = algorithm.GeneratePoint2DList(para);
            List<Point> half = new List<Point>();
            for (int i = index; i < point2ds.Count; i += 2)
                half.Add(new Point(point2ds[i].x, point2ds[i].y));
            return half;
        }
        // create parabola from control points
        protected DrawingAlgorithms.Parabola newParabola(Point[] points, bool horizontal)
        {
            // calculate parabola members
            int dx = points[0].X - points[1].X;
            int dy = points[0].Y - points[1].Y;
            DrawingAlgorithms.Point2D pivot = new DrawingAlgorithms.Point2D(points[0].X, points[0].Y);
            int a, b, limit;
            DrawingAlgorithms.Orientation orientation;
            if (horizontal)
            {
                a = dx;
                b = dy * dy;
                limit = -dx;
                orientation = DrawingAlgorithms.Orientation.Horizontal;
            }
            else
            {
                a = dy;
                b = dx * dx;
                limit = -dy;
                orientation = DrawingAlgorithms.Orientation.Vertical;
            }
            // create parabola
            // and return it
            DrawingAlgorithms.Parabola para = new DrawingAlgorithms.Parabola(pivot, a, b, limit, orientation);
            return para;
        }

        // choosing
        protected override void buildGraphicsPath(GraphicsPath gPath)
        {
            List<Point> firstHalf = getHalfOfParabola(parabola, 0);
            firstHalf.Reverse();
            gPath.AddLines(firstHalf.ToArray());
            gPath.AddLines(getHalfOfParabola(parabola, 1).ToArray());
        }

        // drawing
        protected override void derivedDraw(Graphics g, Pen pen)
        {
            GraphicsPath gPath = new GraphicsPath();
            List<Point> firstHalf = getHalfOfParabola(parabola, 0);
            firstHalf.Reverse();
            gPath.AddLines(firstHalf.ToArray());
            gPath.AddLines(getHalfOfParabola(parabola, 1).ToArray());
            g.DrawPath(pen, gPath);
        }
        protected override void derivedDraw(Graphics g, Pen pen, int index, Point location)
        {
            bool horizontal = parabola.orientation == DrawingAlgorithms.Orientation.Horizontal;
            if (Math.Abs(location.X - controlPoints[0].X) < 1 || Math.Abs(location.Y - controlPoints[0].Y) < 1)
                return;

            Point[] points = controlPoints.ToArray();
            int other = index == 1 ? 2 : 1;
            points[index] = location;
            if (horizontal)
            {
                points[other].X = location.X;
                points[other].Y = 2 * controlPoints[0].Y - location.Y;
            }
            else
            {
                points[other].Y = location.Y;
                points[other].X = 2 * controlPoints[0].X - location.X;
            }

            // create temporary parabola
            DrawingAlgorithms.Parabola para = newParabola(points, horizontal);

            // draw it
            GraphicsPath gPath = new GraphicsPath();
            List<Point> firstHalf = getHalfOfParabola(para, 0);
            firstHalf.Reverse();
            gPath.AddLines(firstHalf.ToArray());
            gPath.AddLines(getHalfOfParabola(para, 1).ToArray());
            g.DrawPath(pen, gPath);
        }

        // cloning
        protected override IDrawingObject derivedClone()
        {
            List<Point> points = controlPoints.ToList();
            return new Parabola(points, parabola.orientation == DrawingAlgorithms.Orientation.Horizontal);
        }

        // scaling
        public override Point getCentralPoint()
        {
            return controlPoints[0];
        }
        protected override void derivedScale(float xFactor, float yFactor)
        {
            base.derivedScale(xFactor, yFactor);

            bool horizontal = parabola.orientation == DrawingAlgorithms.Orientation.Horizontal;
            parabola =  newParabola(controlPoints.ToArray(), horizontal);
        }
        protected override bool canScale(float xFactor, float yFactor)
        {
            int a, b, limit;
            if (parabola.orientation == DrawingAlgorithms.Orientation.Horizontal)
            {
                a = (int)(xFactor * xFactor * parabola.a);
                b = (int)(yFactor * parabola.b);
                limit = (int)(xFactor * parabola.limit);
            }
            else
            {
                a = (int)(yFactor * parabola.a);
                b = (int)(xFactor * xFactor * parabola.b);
                limit = (int)(yFactor * parabola.limit);
            }

            return a != 0 && b != 0 && limit != 0;
        }

        // controlling
        public override bool controllablePoint(int index)
        {
            return index != 0;
        }
        public override IDrawingObject changeControlPoint(int index, Point newLocation)
        {
            bool horizontal = parabola.orientation == DrawingAlgorithms.Orientation.Horizontal;
            if (Math.Abs(newLocation.X - controlPoints[0].X) < 1 || Math.Abs(newLocation.Y - controlPoints[0].Y) < 1)
                return this;

            Point[] points = controlPoints.ToArray();
            int other = index == 1 ? 2 : 1;
            points[index] = newLocation;
            if (horizontal)
            {
                points[other].X = newLocation.X;
                points[other].Y = 2 * controlPoints[0].Y - newLocation.Y;
            }
            else
            {
                points[other].Y = newLocation.Y;
                points[other].X = 2 * controlPoints[0].X - newLocation.X;
            }

            controlPoints[index] = newLocation;
            controlPoints[other] = points[other];
            parabola = newParabola(points, horizontal);

            return this;
        }
    }

    // Hyperbola
    public class Hyperbola : IDrawingObject
    {
        DrawingAlgorithms.Hyperbola hyperbola;
        DrawingAlgorithms.Algorithm algorithm;

        public Hyperbola(List<Point> controlPoints, bool horizontal, bool isFocus = false)
        {
            this.controlPoints = controlPoints;

            hyperbola = newHyperbola(controlPoints.ToArray(), horizontal);

            // create algorithm
            algorithm = new DrawingAlgorithms.MidPoint();

            if (isFocus)
                focus();
        }

        // Get quarter of parabola point list
        // index = 0 : get first quarter
        // index = 1 : get second quarter
        // index = 2 : get third quarter
        // index = 3 : get final quarter
        private List<Point> getQuarterOfHyperbola(DrawingAlgorithms.Hyperbola hyper, int index)
        {
            List<DrawingAlgorithms.Point2D> point2ds = algorithm.GeneratePoint2DList(hyper);
            List<Point> quarter = new List<Point>();
            for (int i = index; i < point2ds.Count; i += 4)
                quarter.Add(new Point(point2ds[i].x, point2ds[i].y));
            return quarter;
        }
        // create parabola from control points
        protected DrawingAlgorithms.Hyperbola newHyperbola(Point[] points, bool horizontal)
        {
            int a, b, limit;
            int dx = points[3].X - points[0].X;
            int dy = points[3].Y - points[0].Y;
            DrawingAlgorithms.Orientation orientation;
            if (!horizontal)
            {
                a = points[0].Y - points[1].Y;
                double temp = (dy * dy * 1.0f / (a * a) - 1);
                b = (int)Math.Sqrt(dx * dx / temp);
                limit = Math.Abs(dy);
                orientation = DrawingAlgorithms.Orientation.Vertical;
            }
            else
            {
                a = points[0].X - points[1].X;
                double temp = (dx * dx * 1.0f / (a * a) - 1);
                b = (int)Math.Sqrt(dy * dy / temp);
                limit = Math.Abs(dx);
                orientation = DrawingAlgorithms.Orientation.Horizontal;
            }

            DrawingAlgorithms.Point2D center = new DrawingAlgorithms.Point2D(points[0].X, points[0].Y);
            DrawingAlgorithms.Hyperbola hyper = new DrawingAlgorithms.Hyperbola(center, a, b, limit, orientation);
            return hyper;
        }

        // choosing
        protected override void buildGraphicsPath(GraphicsPath gPath)
        {
            bool horizontal = hyperbola.orientation == DrawingAlgorithms.Orientation.Horizontal;
            GraphicsPath firstPath = new GraphicsPath();
            GraphicsPath secondPath = new GraphicsPath();

            int[] order = horizontal ? new int[] { 0, 2, 1, 3 } : new int[] { 0, 1, 2, 3 };

            // first quarter
            List<Point> quarter = getQuarterOfHyperbola(hyperbola, order[0]);
            quarter.Reverse();
            firstPath.AddLines(quarter.ToArray());
            // second quarter
            quarter = getQuarterOfHyperbola(hyperbola, order[1]);
            firstPath.AddLines(quarter.ToArray());

            // third quarter
            quarter = getQuarterOfHyperbola(hyperbola, order[2]);
            quarter.Reverse();
            secondPath.AddLines(quarter.ToArray());
            // final quarter
            quarter = getQuarterOfHyperbola(hyperbola, order[3]);
            secondPath.AddLines(quarter.ToArray());

            gPath.AddPath(firstPath, false);
            gPath.AddPath(secondPath, false);
        }

        // drawing
        protected override void derivedDraw(Graphics g, Pen pen)
        {
            hyperbola.center = new DrawingAlgorithms.Point2D(controlPoints[0].X, controlPoints[0].Y);
            bool horizontal = hyperbola.orientation == DrawingAlgorithms.Orientation.Horizontal;
            GraphicsPath firstPath = new GraphicsPath();
            GraphicsPath secondPath = new GraphicsPath();

            int[] order = horizontal ? new int[] { 0, 2, 1, 3 } : new int[] { 0, 1, 2, 3 };

            // first quarter
            List<Point> quarter = getQuarterOfHyperbola(hyperbola, order[0]);
            quarter.Reverse();
            firstPath.AddLines(quarter.ToArray());
            // second quarter
            quarter = getQuarterOfHyperbola(hyperbola, order[1]);
            firstPath.AddLines(quarter.ToArray());

            // third quarter
            quarter = getQuarterOfHyperbola(hyperbola, order[2]);
            quarter.Reverse();
            secondPath.AddLines(quarter.ToArray());
            // final quarter
            quarter = getQuarterOfHyperbola(hyperbola, order[3]);
            secondPath.AddLines(quarter.ToArray());

            // draw
            g.DrawPath(pen, firstPath);
            g.DrawPath(pen, secondPath);
        }
        protected override void derivedDraw(Graphics g, Pen pen, int index, Point location)
        {
            bool horizontal = hyperbola.orientation == DrawingAlgorithms.Orientation.Horizontal;
            Point[] points = controlPoints.ToArray();

            if (index < 3)
            {
                int other = index == 1 ? 2 : 1;
                if (!horizontal)
                {
                    int minY = Math.Min(points[index + 2].Y, points[0].Y);
                    int maxY = Math.Max(points[index + 2].Y, points[0].Y);
                    if (location.Y <= minY || location.Y >= maxY)
                        return;
                    points[index].Y = location.Y;
                    points[other].Y = 2 * points[0].Y - location.Y;
                }
                else
                {
                    int minX = Math.Min(points[index + 2].X, points[0].X);
                    int maxX = Math.Max(points[index + 2].X, points[0].X);
                    if (location.X <= minX || location.X >= maxX)
                        return;
                    points[index].X = location.X;
                    points[other].X = 2 * points[0].X - location.X;
                }
            }
            else
            {
                int startX = Math.Min(location.X, 2 * points[0].X - location.X);
                int startY = Math.Min(location.Y, 2 * points[0].Y - location.Y);
                int endX = Math.Max(location.X, 2 * points[0].X - location.X);
                int endY = Math.Max(location.Y, 2 * points[0].Y - location.Y);
                if (!horizontal)
                {
                    int minY = points[1].Y;
                    int maxY = points[2].Y;
                    if (location.Y >= minY && location.Y <= maxY)
                        return;

                    // 6 - 3
                    // |   |
                    // 5 - 4
                    points[3] = new Point(endX, startY);
                    points[4] = new Point(endX, endY);
                    points[5] = new Point(startX, endY);
                    points[6] = new Point(startX, startY);
                }
                else
                {
                    int minX = points[1].X;
                    int maxX = points[2].X;
                    if (location.X >= minX && location.X <= maxX)
                        return;

                    // 3 - 4
                    // |   |
                    // 6 - 5
                    points[3] = new Point(startX, startY);
                    points[4] = new Point(endX, startY);
                    points[5] = new Point(endX, endY);
                    points[6] = new Point(startX, endY);
                }
            }

            // create temporary parabola
            DrawingAlgorithms.Hyperbola hyper = newHyperbola(points, horizontal);

            // draw it
            GraphicsPath firstPath = new GraphicsPath();
            GraphicsPath secondPath = new GraphicsPath();

            int[] order = horizontal ? new int[] { 0, 2, 1, 3 } : new int[] { 0, 1, 2, 3 };

            // first quarter
            List<Point> quarter = getQuarterOfHyperbola(hyper, order[0]);
            quarter.Reverse();
            firstPath.AddLines(quarter.ToArray());
            // second quarter
            quarter = getQuarterOfHyperbola(hyper, order[1]);
            firstPath.AddLines(quarter.ToArray());

            // third quarter
            quarter = getQuarterOfHyperbola(hyper, order[2]);
            quarter.Reverse();
            secondPath.AddLines(quarter.ToArray());
            // final quarter
            quarter = getQuarterOfHyperbola(hyper, order[3]);
            secondPath.AddLines(quarter.ToArray());

            // draw
            g.DrawPath(pen, firstPath);
            g.DrawPath(pen, secondPath);
        }
        protected override void derivedDrawLineBetweenControlPoints(Graphics g)
        {
            Pen dottedPen = new Pen(Color.Black, 1.0f);
            dottedPen.DashStyle = DashStyle.Dot;
            g.DrawLine(dottedPen, controlPoints[0], controlPoints[1]);
            g.DrawLine(dottedPen, controlPoints[0], controlPoints[2]);
            dottedPen.Dispose();
        }

        // cloning
        protected override IDrawingObject derivedClone()
        {
            List<Point> points = controlPoints.ToList();
            return new Hyperbola(points, hyperbola.orientation == DrawingAlgorithms.Orientation.Horizontal);
        }

        // scaling
        public override Point getCentralPoint()
        {
            return controlPoints[0];
        }
        protected override void derivedScale(float xFactor, float yFactor)
        {
            base.derivedScale(xFactor, yFactor);
            bool horizontal = hyperbola.orientation == DrawingAlgorithms.Orientation.Horizontal;
            hyperbola = newHyperbola(controlPoints.ToArray(), horizontal);
        }
        protected override bool canScale(float xFactor, float yFactor)
        {
            int minWidth, minHeight;
            if (hyperbola.orientation == DrawingAlgorithms.Orientation.Horizontal)
            {
                minWidth = (int)(xFactor * hyperbola.a);
                minHeight = (int)(yFactor * (controlPoints[5].Y - controlPoints[4].Y));
            }
            else
            {
                minHeight = (int)(xFactor * hyperbola.a);
                minWidth = (int)(yFactor * (controlPoints[4].X - controlPoints[5].X));
            }

            return minWidth > 0 && minHeight > 0;
        }

        // controlling
        public override bool controllablePoint(int index)
        {
            return index != 0;
        }
        public override IDrawingObject changeControlPoint(int index, Point newLocation)
        {
            bool horizontal = hyperbola.orientation == DrawingAlgorithms.Orientation.Horizontal;
            Point[] points = controlPoints.ToArray();

            if (index < 3)
            {
                int other = index == 1 ? 2 : 1;
                if (!horizontal)
                {
                    int minY = Math.Min(points[index + 2].Y, points[0].Y);
                    int maxY = Math.Max(points[index + 2].Y, points[0].Y);
                    if (newLocation.Y <= minY || newLocation.Y >= maxY)
                        return this;
                    points[index].Y = newLocation.Y;
                    points[other].Y = 2 * points[0].Y - newLocation.Y;
                }
                else
                {
                    int minX = Math.Min(points[index + 2].X, points[0].X);
                    int maxX = Math.Max(points[index + 2].X, points[0].X);
                    if (newLocation.X <= minX || newLocation.X >= maxX)
                        return this;
                    points[index].X = newLocation.X;
                    points[other].X = 2 * points[0].X - newLocation.X;
                }
            }
            else
            {
                int startX = Math.Min(newLocation.X, 2 * points[0].X - newLocation.X);
                int startY = Math.Min(newLocation.Y, 2 * points[0].Y - newLocation.Y);
                int endX = Math.Max(newLocation.X, 2 * points[0].X - newLocation.X);
                int endY = Math.Max(newLocation.Y, 2 * points[0].Y - newLocation.Y);
                if (!horizontal)
                {
                    int minY = points[1].Y;
                    int maxY = points[2].Y;
                    if (newLocation.Y >= minY && newLocation.Y <= maxY)
                        return this;

                    // 6 - 3
                    // |   |
                    // 5 - 4
                    points[3] = new Point(endX, startY);
                    points[4] = new Point(endX, endY);
                    points[5] = new Point(startX, endY);
                    points[6] = new Point(startX, startY);
                }
                else
                {
                    int minX = points[1].X;
                    int maxX = points[2].X;
                    if (newLocation.X >= minX && newLocation.X <= maxX)
                        return this;

                    // 3 - 4
                    // |   |
                    // 6 - 5
                    points[3] = new Point(startX, startY);
                    points[4] = new Point(endX, startY);
                    points[5] = new Point(endX, endY);
                    points[6] = new Point(startX, endY);
                }
            }

            // create temporary parabola
            hyperbola = newHyperbola(points, horizontal);

            // apply changes to control points
            for (int i = 1; i < 7; i++)
                controlPoints[i] = points[i];

            return this;
        }
    }
}
