using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Forms;
using DrawingObjects;
using System.Drawing.Drawing2D;

namespace DrawingTools
{
    public abstract class IDrawingTool
    {
        protected Form owner;
        protected ObjectList objectList;

        // color
        protected Brush brush;
        protected Color borderColor;

        public void setOwner(Form owner)
        {
            this.owner = owner;
        }

        public abstract void onMouseDown(object sender, MouseEventArgs e);
        public abstract void onMouseMove(object sender, MouseEventArgs e);
        public abstract void onPartialDraw(Graphics g);
        public abstract void reset(object sender);
    }

    // Duong thang
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
            dashedPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;

            // draw dashed line
            g.DrawLine(dashedPen, firstPoint, secondPoint);
            dashedPen.Dispose();
        }

        public override void reset(object sender)
        {
            firstSnap = false;
            (sender as Control).Invalidate();
        }
    }

    // Hinh chu nhat
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

            Rectangle bound = new Rectangle(startX, startY, endX - startX + 1, endY - startY + 1);

            // add rectangle object to list for drawing onto picturebox
            Rect rect = new Rect(bound, true);
            rect.setBrush(new SolidBrush(Color.Yellow));
            objectList.add(rect);
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
            dashedPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;


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

        public override void reset(object sender)
        {
            firstSnap = false;
            (sender as Control).Invalidate();
        }
    }

    // Hinh binh hanh
    public class ParallelogramTool: IDrawingTool
    {
        private Point[] controlPoints;
        private int snaps;

        public ParallelogramTool(ObjectList objectList)
        {
            snaps = 0;
            controlPoints = new Point[4];

            this.objectList = objectList;
        }

        public override void onMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                snaps = 0;
                (sender as Control).Invalidate();
                return;
            }

            if (snaps < 3)
            {
                controlPoints[snaps] = e.Location;
                snaps++;
                if (snaps < 3)
                    return;
            }

            // prepare parallelogram object
            controlPoints[3].X = controlPoints[0].X + controlPoints[2].X - controlPoints[1].X;
            controlPoints[3].Y = controlPoints[0].Y + controlPoints[2].Y - controlPoints[1].Y;
            Parallelogram para = new Parallelogram(controlPoints, true);            

            // add parallelogram object to list for drawing onto picturebox
            objectList.add(para);
            (sender as Control).Invalidate();
            snaps = 0;
        }

        public override void onMouseMove(object sender, MouseEventArgs e)
        {
            if (snaps == 0)
                return;
            controlPoints[snaps] = e.Location;
            (sender as Control).Invalidate();
        }

        public override void onPartialDraw(Graphics g)
        {
            if (snaps == 0)
                return;

            // prepare dashed pen
            Pen dashedPen = new Pen(Color.Black, 1.0f);
            dashedPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;

            // draw dashed lines
            for (int i = 0; i < snaps; i++)
                g.DrawLine(dashedPen, controlPoints[i], controlPoints[i + 1]);
            if(snaps == 2)
            {
                int xCoord = controlPoints[0].X + controlPoints[2].X - controlPoints[1].X;
                int yCoord = controlPoints[0].Y + controlPoints[2].Y - controlPoints[1].Y;
                Point lastPoint = new Point(xCoord, yCoord);
                g.DrawLine(dashedPen, controlPoints[snaps], lastPoint);
                g.DrawLine(dashedPen, lastPoint, controlPoints[0]);
            }

            dashedPen.Dispose();
        }

        public override void reset(object sender)
        {
            snaps = 0;
            (sender as Control).Invalidate();
        }
    }

    // Hinh da giac
    public class PolygonTool : IDrawingTool
    {
        private List<Point> controlPoints;

        public PolygonTool(ObjectList objectList)
        {
            controlPoints = new List<Point>();

            this.objectList = objectList;
        }

        public override void onMouseDown(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Right)
            {
                controlPoints.Clear();
                (sender as Control).Invalidate();
                return;
            }

            if (controlPoints.Count == 0)
            {
                controlPoints.Add(e.Location);
                controlPoints.Add(e.Location);
                return;
            }

            if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
            {
                controlPoints.Add(e.Location);
                return;
            }

            if (controlPoints.Count < 3)
                return;

            Polygon poly = new Polygon(controlPoints, true);
            objectList.add(poly);
            (sender as Control).Invalidate();
            controlPoints = new List<Point>();
        }

        public override void onMouseMove(object sender, MouseEventArgs e)
        {
            if (controlPoints.Count < 2)
                return;
            controlPoints[controlPoints.Count - 1] = e.Location;
            (sender as Control).Invalidate();
        }

        public override void onPartialDraw(Graphics g)
        {
            if (controlPoints.Count < 2)
                return;

            // prepare dashed pen
            Pen dashedPen = new Pen(Color.Black, 1.0f);
            dashedPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;

            // draw dashed lines
            g.DrawPolygon(dashedPen, controlPoints.ToArray());

            dashedPen.Dispose();
        }

        public override void reset(object sender)
        {
            controlPoints.Clear();
            (sender as Control).Invalidate();
        }
    }

    // Duong gap khuc
    public class BrokenLineTool : IDrawingTool
    {
        private List<Point> controlPoints;

        public BrokenLineTool(ObjectList objectList)
        {
            controlPoints = new List<Point>();

            this.objectList = objectList;
        }

        public override void onMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                controlPoints.Clear();
                (sender as Control).Invalidate();
                return;
            }

            if (controlPoints.Count == 0)
            {
                controlPoints.Add(e.Location);
                controlPoints.Add(e.Location);
                return;
            }

            // if ctrl + click, finish
            // else add new vertice
            if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
            {
                controlPoints.Add(e.Location);
                return;
            }

            BrokenLine line = new BrokenLine(controlPoints, true);
            objectList.add(line);
            (sender as Control).Invalidate();
            controlPoints = new List<Point>();
        }

        public override void onMouseMove(object sender, MouseEventArgs e)
        {
            if (controlPoints.Count < 2)
                return;
            controlPoints[controlPoints.Count - 1] = e.Location;
            (sender as Control).Invalidate();
        }

        public override void onPartialDraw(Graphics g)
        {
            if (controlPoints.Count < 2)
                return;

            // prepare dashed pen
            Pen dashedPen = new Pen(Color.Black, 1.0f);
            dashedPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;

            // draw dashed lines
            g.DrawLines(dashedPen, controlPoints.ToArray());

            dashedPen.Dispose();
        }

        public override void reset(object sender)
        {
            controlPoints.Clear();
            (sender as Control).Invalidate();
        }
    }

    // Cung tron
    public class CircleArcTool : IDrawingTool
    {
        private Point center;
        private Point[] pivot;
        private float radius;
        private bool smallPart;

        private bool centerSnap;
        private int pivotSnaps;

        public CircleArcTool(ObjectList objectList)
        {
            this.objectList = objectList;

            centerSnap = false;
            pivotSnaps = 0;
            pivot = new Point[2];
            smallPart = false;
        }

        public override void onMouseDown(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Right)
            {
                centerSnap = false;
                pivotSnaps = 0;
                smallPart = false;
                (sender as Control).Invalidate();
                return;
            }

            // choose center
            if (!centerSnap)
            {
                centerSnap = true;
                center = e.Location;
                return;
            }

            // choose first pivot point
            // calculate radius
            if(pivotSnaps < 2)
            {
                if (pivotSnaps == 0)
                {
                    int dx = pivot[0].X - center.X;
                    int dy = pivot[0].Y - center.Y;
                    radius = (float)Math.Sqrt(dx * dx + dy * dy);
                    if (radius < 0.1f)
                        return;
                }
                pivotSnaps++;
                return;
            }

            // both pivot points chosen
            // add new circle segment to object list for drawing onto picture box
            CircleArc circle = new CircleArc(center, pivot, smallPart, radius, true);
            objectList.add(circle);
            (sender as Control).Invalidate();
            pivotSnaps = 0;
            centerSnap = false;
        }

        public override void onMouseMove(object sender, MouseEventArgs e)
        {
            if (!centerSnap)
                return;
            if (pivotSnaps < 2)
            {
                pivot[pivotSnaps] = e.Location;

                if(pivotSnaps == 1)
                    pivot[1] = DrawingUtilities.Restrict(center, radius, e.Location);
            }

            // both pivot are chosen
            // now choosing which part of circle to render
            if(pivotSnaps == 2)
            {
                float startAngle = 180 * DrawingUtilities.CalculateAngle(center, pivot[0]) / (float)Math.PI;
                float endAngle = 180 * DrawingUtilities.CalculateAngle(center, pivot[1]) / (float)Math.PI;
                float snapAngle = 180 * DrawingUtilities.CalculateAngle(center, e.Location) / (float)Math.PI;
                // if start angle is bigger than end angle
                // swap them to make sure
                // start angle is always smaller than end angle
                if (startAngle > endAngle)
                {
                    float tempAngle = startAngle;
                    startAngle = endAngle;
                    endAngle = tempAngle;
                }

                float sweepAngle = endAngle - startAngle;
                if((snapAngle >= startAngle && snapAngle <= endAngle && sweepAngle <= 180)
                || ((snapAngle < startAngle || snapAngle > endAngle) && sweepAngle > 180))
                {
                    smallPart = true;
                }
                else
                {
                    smallPart = false;
                }
            }
            
            (sender as Control).Invalidate();
        }

        public override void onPartialDraw(Graphics g)
        {
            if (!centerSnap)
                return;

            // prepare dashed pen
            Pen dashedPen = new Pen(Color.Black, 1.0f);
            dashedPen.DashStyle = DashStyle.Dot;

            // draw a line connecting first pivot and center
            g.DrawLine(dashedPen, center, pivot[0]);
            if(pivotSnaps == 0)
            {
                int dx = pivot[0].X - center.X;
                int dy = pivot[0].Y - center.Y;
                radius = (float)Math.Sqrt(dx * dx + dy * dy);
                g.DrawEllipse(dashedPen, center.X - radius, center.Y - radius, radius * 2, radius * 2);
            }

            // if first pivot is chosen
            // it means that second pivot is being chosen
            if (pivotSnaps > 0)
            {
                g.DrawLine(dashedPen, center, pivot[1]);

                if(pivotSnaps == 2)
                {
                    RectangleF bound = new RectangleF(center.X - radius, center.Y - radius, radius * 2, radius * 2);
                    float startAngle = 180 * DrawingUtilities.CalculateAngle(center, pivot[0]) / (float)Math.PI;
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
                    float sweepAngle = endAngle - startAngle;
                    if (smallPart && sweepAngle >= 180)
                    {
                        sweepAngle = 360 - sweepAngle;
                        sweepSign = -1;
                    }
                    if (!smallPart && sweepAngle < 180)
                    {
                        sweepAngle = 360 - sweepAngle;
                        sweepSign = -1;
                    }

                    // draw the line connecting center and second pivot
                    // and the partial arc
                    g.DrawArc(dashedPen, bound, startAngle, sweepAngle * sweepSign);
                }
                else
                {
                    g.DrawEllipse(dashedPen, center.X - radius, center.Y - radius, radius * 2, radius * 2);
                }
            }
            
            dashedPen.Dispose();
        }

        public override void reset(object sender)
        {
            centerSnap = false;
            pivotSnaps = 0;
            smallPart = false;
            (sender as Control).Invalidate();
        }
    }

    // Duong tron
    public class CircleTool : IDrawingTool
    {
        private Point firstPoint;
        private Point secondPoint;
        private bool firstSnap;

        public CircleTool(ObjectList objectList)
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
                firstPoint = e.Location;
                firstSnap = true;
                return;
            }

            // prepare circle object
            int width = Math.Abs(firstPoint.X - secondPoint.X);
            int height = Math.Abs(firstPoint.Y - secondPoint.Y);
            int size = Math.Min(width, height);

            int startX = firstPoint.X;
            int startY = firstPoint.Y;
            if (firstPoint.X > secondPoint.X)
                startX = firstPoint.X - size;
            if (firstPoint.Y > secondPoint.Y)
                startY = firstPoint.Y - size;
            Point center = new Point(startX + size / 2, startY + size / 2);

            Circle circle = new Circle(center, size / 2, true);
            objectList.add(circle);
            firstSnap = false;
            (sender as Control).Invalidate();
        }

        public override void onMouseMove(object sender, MouseEventArgs e)
        {
            if (!firstSnap)
                return;
            secondPoint = e.Location;
            (sender as Control).Invalidate();
        }

        public override void onPartialDraw(Graphics g)
        {
            if (!firstSnap)
                return;

            // prepare dashed pen
            Pen dashedPen = new Pen(Color.Black, 1.0f);
            dashedPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;

            // draw dashed circle
            int width = Math.Abs(firstPoint.X - secondPoint.X);
            int height = Math.Abs(firstPoint.Y - secondPoint.Y);
            int size = Math.Min(width, height);

            int startX = firstPoint.X;
            int startY = firstPoint.Y;
            if (firstPoint.X > secondPoint.X)
                startX = firstPoint.X - size;
            if (firstPoint.Y > secondPoint.Y)
                startY = firstPoint.Y - size;

            Rectangle bound = new Rectangle(startX, startY, size, size);
            g.DrawEllipse(dashedPen, bound);
            g.DrawRectangle(dashedPen, bound);

            dashedPen.Dispose();
        }

        public override void reset(object sender)
        {
            firstSnap = false;
            (sender as Control).Invalidate();
        }
    }

    // Duong Elip
    public class EllipseTool : IDrawingTool
    {
        private Point firstPoint;
        private Point secondPoint;
        private bool firstSnap;

        public EllipseTool(ObjectList objectList)
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
                firstPoint = e.Location;
                firstSnap = true;
                return;
            }

            // prepare circle object
            int startX = Math.Min(firstPoint.X, secondPoint.X);
            int startY = Math.Min(firstPoint.Y, secondPoint.Y);

            int endX = Math.Max(firstPoint.X, secondPoint.X);
            int endY = Math.Max(firstPoint.Y, secondPoint.Y);

            Point center = new Point((endX + startX) / 2, (endY + startY) / 2);

            Ellipse ellipse = new Ellipse(center, (endX - startX) / 2.0f, (endY - startY) / 2.0f, true);
            objectList.add(ellipse);
            firstSnap = false;
            (sender as Control).Invalidate();
        }

        public override void onMouseMove(object sender, MouseEventArgs e)
        {
            if (!firstSnap)
                return;
            secondPoint = e.Location;
            (sender as Control).Invalidate();
        }

        public override void onPartialDraw(Graphics g)
        {
            if (!firstSnap)
                return;

            // prepare dashed pen
            Pen dashedPen = new Pen(Color.Black, 1.0f);
            dashedPen.DashStyle = DashStyle.Dot;

            // draw dashed ellipse
            int startX = Math.Min(firstPoint.X, secondPoint.X);
            int startY = Math.Min(firstPoint.Y, secondPoint.Y);

            int endX = Math.Max(firstPoint.X, secondPoint.X);
            int endY = Math.Max(firstPoint.Y, secondPoint.Y);

            Rectangle bound = new Rectangle(startX, startY, endX - startX, endY - startY);
            g.DrawEllipse(dashedPen, bound);
            g.DrawRectangle(dashedPen, bound);

            dashedPen.Dispose();
        }

        public override void reset(object sender)
        {
            firstSnap = false;
            (sender as Control).Invalidate();
        }
    }

    // Cung Elip
    public class EllipseArcTool : IDrawingTool
    {
        private Point firstPoint;
        private Point secondPoint;
        private Point center;
        private float a;
        private float b;
        private Point[] pivot;
        private bool smallPart;

        private bool firstSnap;
        private bool secondSnap;
        private int pivotSnaps;

        public EllipseArcTool(ObjectList objectList)
        {
            this.objectList = objectList;

            firstSnap = false;
            secondSnap = false;
            pivotSnaps = 0;
            pivot = new Point[2];
            smallPart = false;
        }

        public override void onMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                firstSnap = false;
                secondSnap = false;
                pivotSnaps = 0;
                smallPart = false;
                (sender as Control).Invalidate();
                return;
            }

            // choose bounding box for ellipse
            if (!firstSnap)
            {
                firstSnap = true;
                firstPoint = e.Location;
                return;
            }
            if (!secondSnap)
            {
                a = Math.Abs(firstPoint.X - secondPoint.X) / 2.0f;
                b = Math.Abs(firstPoint.Y - secondPoint.Y) / 2.0f;
                center.X = (firstPoint.X + secondPoint.X) / 2;
                center.Y = (firstPoint.Y + secondPoint.Y) / 2;
                if (a < 5 || b < 5)
                    return;
                secondSnap = true;
                return;
            }

            // choose pivot points
            if (pivotSnaps < 2)
            {
                if (pivotSnaps == 1 && pivot[0].Equals(e.Location))
                    return;
                pivotSnaps++;
                return;
            }

            // both pivot points chosen
            // add new ellipse segment to object list for drawing onto picture box
            EllipseArc ellipse = new EllipseArc(center, a, b, pivot, smallPart, true);
            objectList.add(ellipse);
            (sender as Control).Invalidate();
            pivotSnaps = 0;
            firstSnap = false;
            secondSnap = false;
        }

        public override void onMouseMove(object sender, MouseEventArgs e)
        {
            // choosing bounding box
            if (!firstSnap)
                return;
            if (!secondSnap)
            {
                secondPoint = e.Location;
                (sender as Control).Invalidate();
                return;
            }

            // choosing pivot points
            if (pivotSnaps < 2)
            {
                pivot[pivotSnaps] = DrawingUtilities.Restrict(center, a, b, e.Location);
                (sender as Control).Invalidate();
                return;
            }

            // both pivot are chosen
            // now choosing which part of ellipse to render
            if (pivotSnaps == 2)
            {
                float startAngle = 180 * DrawingUtilities.CalculateAngle(center, pivot[0]) / (float)Math.PI;
                float endAngle = 180 * DrawingUtilities.CalculateAngle(center, pivot[1]) / (float)Math.PI;
                float snapAngle = 180 * DrawingUtilities.CalculateAngle(center, e.Location) / (float)Math.PI;
                // if start angle is bigger than end angle
                // swap them to make sure
                // start angle is always smaller than end angle
                if (startAngle > endAngle)
                {
                    float tempAngle = startAngle;
                    startAngle = endAngle;
                    endAngle = tempAngle;
                }

                float sweepAngle = endAngle - startAngle;
                if ((snapAngle >= startAngle && snapAngle <= endAngle && sweepAngle < (360 - sweepAngle))
                || ((snapAngle < startAngle || snapAngle > endAngle) && sweepAngle > (360 - sweepAngle)))
                {
                    smallPart = true;
                }
                else
                {
                    smallPart = false;
                }
            }

            (sender as Control).Invalidate();
        }

        public override void onPartialDraw(Graphics g)
        {
            if (!firstSnap)
                return;

            // prepare dashed pen
            Pen dashedPen = new Pen(Color.Black, 1.0f);
            dashedPen.DashStyle = DashStyle.Dot;

            if (!secondSnap)
            {
                // draw dashed ellipse
                int startX = Math.Min(firstPoint.X, secondPoint.X);
                int startY = Math.Min(firstPoint.Y, secondPoint.Y);

                int endX = Math.Max(firstPoint.X, secondPoint.X);
                int endY = Math.Max(firstPoint.Y, secondPoint.Y);

                Rectangle bound = new Rectangle(startX, startY, endX - startX, endY - startY);
                g.DrawEllipse(dashedPen, bound);
                g.DrawRectangle(dashedPen, bound);
                return;
            }

            // draw a line connecting first pivot and center
            g.DrawLine(dashedPen, center, pivot[0]);
            // draw ellipse
            if(pivotSnaps < 2)
            {
                // draw dashed ellipse
                int startX = Math.Min(firstPoint.X, secondPoint.X);
                int startY = Math.Min(firstPoint.Y, secondPoint.Y);

                int endX = Math.Max(firstPoint.X, secondPoint.X);
                int endY = Math.Max(firstPoint.Y, secondPoint.Y);

                g.DrawEllipse(dashedPen, new Rectangle(startX, startY, endX - startX, endY - startY));
            }

            // if first pivot is chosen
            // it means that second pivot is being chosen
            if (pivotSnaps > 0)
            {
                g.DrawLine(dashedPen, center, pivot[1]);

                if (pivotSnaps == 2)
                {
                    RectangleF bound = new RectangleF(center.X - a, center.Y - b, 2 * a, 2 * b);
                    float startAngle = 180 * DrawingUtilities.CalculateAngle(center, pivot[0]) / (float)Math.PI;
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

                    // draw the line connecting center and second pivot
                    // and the partial arc
                    g.DrawArc(dashedPen, bound, startAngle, sweepAngle * sweepSign);
                }
            }

            dashedPen.Dispose();
        }

        public override void reset(object sender)
        {
            firstSnap = false;
            secondSnap = false;
            pivotSnaps = 0;
            smallPart = false;
            (sender as Control).Invalidate();
        }
    }

    // Duong Bezier
    public class BezierTool : IDrawingTool
    {
        private List<Point> controlPoints;
        private int controlSnaps;

        public BezierTool(ObjectList objectList)
        {
            this.objectList = objectList;

            controlPoints = new List<Point>();
            controlSnaps = 0;
        }

        public override void onMouseDown(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Right)
            {
                reset(sender);
                return;
            }

            if (controlSnaps < 4)
            {
                if (controlSnaps < 3)
                    controlPoints.Add(e.Location);
                if (controlSnaps == 0)
                    controlPoints.Add(e.Location);
                controlSnaps++;
                if (controlSnaps < 4)
                    return;
            }

            // prepare bezier object
            Bezier bezier = new Bezier(controlPoints, true);
            // add bezier to object list for drawing onto picture box
            objectList.add(bezier);

            // redraw sender
            (sender as Control).Invalidate();
            // reset tool
            controlPoints = new List<Point>();
            controlSnaps = 0;
        }

        public override void onMouseMove(object sender, MouseEventArgs e)
        {
            if (controlSnaps == 0)
                return;
            controlPoints[controlSnaps] = e.Location;
            (sender as Control).Invalidate();
        }

        public override void onPartialDraw(Graphics g)
        {
            if (controlSnaps == 0)
                return;

            Pen dottedPen = new Pen(Color.Black, 1.0f);
            dottedPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
            g.DrawLines(dottedPen, controlPoints.ToArray());
            dottedPen.Dispose();
        }

        public override void reset(object sender)
        {
            controlPoints.Clear();
            controlSnaps = 0;
            (sender as Control).Invalidate();
        }
    }

    // Text
    public class CreateTextTool : IDrawingTool
    {
        private Point origin;
        private bool originSnap;
        private StringBuilder builder;
        private float resolutionX;
        private float resolutionY;

        public CreateTextTool(ObjectList objectList)
        {
            this.objectList = objectList;

            originSnap = false;
            builder = new StringBuilder();
            builder.Append('_');
        }

        public override void onMouseDown(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Right)
            {
                reset(sender);
                return;
            }
            if (!originSnap)
            {
                origin = e.Location;
                originSnap = true;
                (sender as Control).Invalidate();
            }
        }

        public override void onMouseMove(object sender, MouseEventArgs e)
        {
        }

        public void onKeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 27) // Escape
            {
                Font font = new Font(FontFamily.GenericMonospace, 30.0f, FontStyle.Regular);
                StringFormat format = StringFormat.GenericDefault;
                Text textObj = new Text(origin, builder.ToString().Substring(0, builder.Length - 1), font, format, true);
                textObj.setResolutionX(resolutionX);
                textObj.setResolutionY(resolutionY);
                objectList.add(textObj);

                originSnap = false;
                builder.Length = 0;
                builder.Append('_');
            }
            else if (e.KeyChar == 8) // Backspace
            {
                if (builder.Length > 1)
                    builder.Remove(builder.Length - 2, 1);
            }
            else
            {
                if (e.KeyChar == '\n' || e.KeyChar == '\r')
                    builder.Insert(builder.Length - 1, '\n');
                else
                    builder.Insert(builder.Length - 1, e.KeyChar);
            }
            (sender as Control).Invalidate();
        }

        public override void onPartialDraw(Graphics g)
        {
            if (originSnap)
            {
                // set resolution
                resolutionX = g.DpiX;
                resolutionY = g.DpiY;

                // draw string
                DrawingUtilities.DrawControlRect(g, origin);
                Font font = new Font(FontFamily.GenericMonospace, 30.0f, FontStyle.Regular);
                g.DrawString(builder.ToString(), font, Brushes.Gray, origin);

                // calculate and draw text bounding box
                Size tempTextSize = TextRenderer.MeasureText(builder.ToString(), font);
                Pen dottedPen = new Pen(Color.Gray, 1.0f);
                dottedPen.DashStyle = DashStyle.Dot;
                Rectangle bound = new Rectangle(origin, tempTextSize);
                g.DrawRectangle(dottedPen, bound);
                dottedPen.Dispose();
            }
        }

        public override void reset(object sender)
        {
            originSnap = false;
            builder.Length = 0;
            builder.Append('_');
            (sender as Control).Invalidate();
        }
    }

    // Parabola
    public class ParabolaTool : IDrawingTool
    {
        private Point firstPoint;
        private Point secondPoint;
        private Point currentLocation;
        private Rectangle bound;
        private bool firstSnap;
        private bool secondSnap;

        public ParabolaTool(ObjectList objectList)
        {
            firstSnap = false;
            secondSnap = false;
            this.objectList = objectList;
        }

        private bool prepareSideAndDirection(Point currentLocation, List<Point> controls, out bool horizontal)
        {
            horizontal = true;
            // mouse inside bound (M) --> no side chosen
            if (bound.Contains(currentLocation))
                return false;
            // NW, NE, SW, SE --> no side chosen
            // ---------------
            // | NW | N | NE |
            // ---------------
            // | W  | M | E  |
            // ---------------
            // | SW | S | SE |
            // ---------------
            if (currentLocation.X <= bound.Left && currentLocation.Y <= bound.Top)
                return false;
            if (currentLocation.X >= bound.Right && currentLocation.Y <= bound.Top)
                return false;
            if (currentLocation.X <= bound.Left && currentLocation.Y >= bound.Bottom)
                return false;
            if (currentLocation.X >= bound.Right && currentLocation.Y >= bound.Bottom)
                return false;

            // West
            if (currentLocation.X < bound.Left)
            {
                controls.Add(new Point(bound.Right, bound.Top + bound.Height / 2));
                controls.Add(new Point(bound.Left, bound.Top));
                controls.Add(new Point(bound.Left, bound.Bottom));
            }
            // East
            else if (currentLocation.X > bound.Right)
            {
                controls.Add(new Point(bound.Left, bound.Top + bound.Height / 2));
                controls.Add(new Point(bound.Right, bound.Top));
                controls.Add(new Point(bound.Right, bound.Bottom));
            }
            // North
            else if (currentLocation.Y < bound.Top)
            {
                controls.Add(new Point(bound.Left + bound.Width / 2, bound.Bottom));
                controls.Add(new Point(bound.Left, bound.Top));
                controls.Add(new Point(bound.Right, bound.Top));
                horizontal = false;
            }
            // South
            else
            {
                controls.Add(new Point(bound.Left + bound.Width / 2, bound.Top));
                controls.Add(new Point(bound.Left, bound.Bottom));
                controls.Add(new Point(bound.Right, bound.Bottom));
                horizontal = false;
            }

            return true;
        }

        public override void onMouseDown(object sender, MouseEventArgs e)
        {
            // right click --> cancel action
            if (e.Button == MouseButtons.Right)
            {
                reset(sender);
                return;
            }

            if (!firstSnap)
            {
                firstSnap = true;
                firstPoint = e.Location;
                return;
            }
            if (!secondSnap)
            {
                // prepare rectangle object
                int startX = Math.Min(firstPoint.X, secondPoint.X);
                int startY = Math.Min(firstPoint.Y, secondPoint.Y);

                int endX = Math.Max(firstPoint.X, secondPoint.X);
                int endY = Math.Max(firstPoint.Y, secondPoint.Y);

                bound = new Rectangle(startX, startY, endX - startX + 1, endY - startY + 1);
                secondSnap = true;
                return;
            }

            List<Point> controls = new List<Point>(3);
            bool horizontal = true;
            if (!prepareSideAndDirection(e.Location, controls, out horizontal))
                return;

            // add parabola object to list for drawing onto picturebox
            Parabola para = new Parabola(controls, horizontal, true);
            objectList.add(para);
            (sender as Control).Invalidate();
            firstSnap = false;
        }

        public override void onMouseMove(object sender, MouseEventArgs e)
        {
            if (firstSnap && !secondSnap)
            {
                secondPoint = e.Location;
                (sender as Control).Invalidate();
            }
            if (firstSnap && secondSnap)
            {
                currentLocation = e.Location;
                (sender as Control).Invalidate();
            }
        }

        public override void onPartialDraw(Graphics g)
        {
            if (!firstSnap)
                return;

            // prepare dashed pen
            Pen dashedPen = new Pen(Color.Black, 1.0f);
            dashedPen.DashStyle = DashStyle.Dot;

            if (!secondSnap)
            {
                // prepare dashed rectangle
                int startX = Math.Min(firstPoint.X, secondPoint.X);
                int startY = Math.Min(firstPoint.Y, secondPoint.Y);

                int endX = Math.Max(firstPoint.X, secondPoint.X);
                int endY = Math.Max(firstPoint.Y, secondPoint.Y);

                Rectangle rect = new Rectangle(startX, startY, endX - startX + 1, endY - startY + 1);

                // draw dashed rectangle
                g.DrawRectangle(dashedPen, rect);
            }
            else
            {
                g.DrawRectangle(dashedPen, bound);

                List<Point> controls = new List<Point>(3);
                bool horizontal = true;
                if (!prepareSideAndDirection(currentLocation, controls, out horizontal))
                    return;

                // create temporary parabola and draw it with dashed pen
                IDrawingObject para = new Parabola(controls, horizontal);
                para.onDraw(g, dashedPen);
            }

            dashedPen.Dispose();
        }

        public override void reset(object sender)
        {
            firstSnap = false;
            secondSnap = false;
            (sender as Control).Invalidate();
        }
    }

    // Hyperbola
    public class HyperbolaTool: IDrawingTool
    {
        private Point firstPoint;
        private Point secondPoint;
        private Point currentLocation;
        private Rectangle bound;
        private bool firstSnap;
        private bool secondSnap;
        private bool sideChosen;
        private bool horizontal;

        public HyperbolaTool(ObjectList objectList)
        {
            firstSnap = false;
            secondSnap = false;
            sideChosen = false;
            horizontal = true;
            this.objectList = objectList;
        }

        private bool prepareSideAndDirection(Point location)
        {
            // mouse inside bound (M) --> no side chosen
            if (bound.Contains(location))
                return false;
            // NW, NE, SW, SE --> no side chosen
            // ---------------
            // | NW | N | NE |
            // ---------------
            // | W  | M | E  |
            // ---------------
            // | SW | S | SE |
            // ---------------
            if (location.X <= bound.Left && location.Y <= bound.Top)
                return false;
            if (location.X >= bound.Right && location.Y <= bound.Top)
                return false;
            if (location.X <= bound.Left && location.Y >= bound.Bottom)
                return false;
            if (location.X >= bound.Right && location.Y >= bound.Bottom)
                return false;

            // West, East --> horizontal
            // North, South --> vertical
            if (location.Y < bound.Top || location.Y > bound.Bottom)
                horizontal = false;
            else
                horizontal = true;

            return true;
        }

        private bool checkSidePoint()
        {
            if (currentLocation.X <= bound.Left || currentLocation.X >= bound.Right)
                return false;
            if (currentLocation.Y <= bound.Top || currentLocation.Y >= bound.Bottom)
                return false;
            int midX = bound.Left + bound.Width / 2;
            int midY = bound.Top + bound.Height / 2;
            if (horizontal && currentLocation.X == midX)
                return false;
            if (!horizontal && currentLocation.Y == midY)
                return false;

            return true;
        }

        public override void onMouseDown(object sender, MouseEventArgs e)
        {
            // right click --> cancel action
            if (e.Button == MouseButtons.Right)
            {
                reset(sender);
                return;
            }

            if (!firstSnap)
            {
                firstSnap = true;
                firstPoint = e.Location;
                return;
            }
            if (!secondSnap)
            {
                // prepare rectangle object
                int startX = Math.Min(firstPoint.X, secondPoint.X);
                int startY = Math.Min(firstPoint.Y, secondPoint.Y);

                int endX = Math.Max(firstPoint.X, secondPoint.X);
                int endY = Math.Max(firstPoint.Y, secondPoint.Y);

                bound = new Rectangle(startX, startY, endX - startX + 1, endY - startY + 1);
                secondSnap = true;
                return;
            }

            if (!sideChosen)
            {
                if (prepareSideAndDirection(e.Location))
                    sideChosen = true;
                return;
            }

            if (!checkSidePoint())
                return;

            // construct list of control points
            List<Point> controls = new List<Point>();
            controls.Add(new Point(bound.Left + bound.Width / 2, bound.Top + bound.Height / 2));
            if (horizontal)
            {
                int dx = Math.Abs(controls[0].X - currentLocation.X);
                controls.Add(new Point(controls[0].X - dx, controls[0].Y));
                controls.Add(new Point(controls[0].X + dx, controls[0].Y));
                controls.Add(new Point(bound.Left, bound.Top));
                controls.Add(new Point(bound.Right, bound.Top));
                controls.Add(new Point(bound.Right, bound.Bottom));
                controls.Add(new Point(bound.Left, bound.Bottom));
            }
            else
            {
                int dy = Math.Abs(controls[0].Y - currentLocation.Y);
                controls.Add(new Point(controls[0].X, controls[0].Y - dy));
                controls.Add(new Point(controls[0].X, controls[0].Y + dy));
                controls.Add(new Point(bound.Right, bound.Top));
                controls.Add(new Point(bound.Right, bound.Bottom));
                controls.Add(new Point(bound.Left, bound.Bottom));
                controls.Add(new Point(bound.Left, bound.Top));
            }

            // add parabola object to list for drawing onto picturebox
            Hyperbola hyper = new Hyperbola(controls, horizontal, true);
            objectList.add(hyper);
            (sender as Control).Invalidate();
            reset(sender);
        }

        public override void onMouseMove(object sender, MouseEventArgs e)
        {
            if (firstSnap && !secondSnap)
            {
                secondPoint = e.Location;
                (sender as Control).Invalidate();
            }
            if (firstSnap && secondSnap)
            {
                currentLocation = e.Location;
                (sender as Control).Invalidate();
            }
        }

        public override void onPartialDraw(Graphics g)
        {
            if (!firstSnap)
                return;

            // prepare dashed pen
            Pen dashedPen = new Pen(Color.Black, 1.0f);
            dashedPen.DashStyle = DashStyle.Dot;

            if (!secondSnap)
            {
                // prepare dashed rectangle
                int startX = Math.Min(firstPoint.X, secondPoint.X);
                int startY = Math.Min(firstPoint.Y, secondPoint.Y);

                int endX = Math.Max(firstPoint.X, secondPoint.X);
                int endY = Math.Max(firstPoint.Y, secondPoint.Y);

                Rectangle rect = new Rectangle(startX, startY, endX - startX + 1, endY - startY + 1);

                // draw dashed rectangle
                g.DrawRectangle(dashedPen, rect);
            }
            else
            {
                g.DrawRectangle(dashedPen, bound);

                if (!sideChosen && !prepareSideAndDirection(currentLocation))
                        return;

                // draw a middle line to indicate chosen orientation
                Point startPoint = new Point();
                Point endPoint = new Point();
                if (horizontal)
                {
                    startPoint.X = endPoint.X = bound.Left + bound.Width / 2;
                    startPoint.Y = bound.Top;
                    endPoint.Y = bound.Bottom;
                }
                else
                {
                    startPoint.Y = endPoint.Y = bound.Top + bound.Height / 2;
                    startPoint.X = bound.Left;
                    endPoint.X = bound.Right;
                }
                g.DrawLine(dashedPen, startPoint, endPoint);
                if (!checkSidePoint())
                    return;

                // construct list of control points
                List<Point> controls = new List<Point>();
                controls.Add(new Point(bound.Left + bound.Width / 2, bound.Top + bound.Height / 2));
                if (horizontal)
                {
                    int dx = Math.Abs(controls[0].X - currentLocation.X);
                    controls.Add(new Point(controls[0].X - dx, controls[0].Y));
                    controls.Add(new Point(controls[0].X + dx, controls[0].Y));
                    controls.Add(new Point(bound.Left, bound.Top));
                    controls.Add(new Point(bound.Right, bound.Top));
                    controls.Add(new Point(bound.Right, bound.Bottom));
                    controls.Add(new Point(bound.Left, bound.Bottom));
                }
                else
                {
                    int dy = Math.Abs(controls[0].Y - currentLocation.Y);
                    controls.Add(new Point(controls[0].X, controls[0].Y - dy));
                    controls.Add(new Point(controls[0].X, controls[0].Y + dy));
                    controls.Add(new Point(bound.Right, bound.Top));
                    controls.Add(new Point(bound.Right, bound.Bottom));
                    controls.Add(new Point(bound.Left, bound.Bottom));
                    controls.Add(new Point(bound.Left, bound.Top));
                }

                // create temporary parabola and draw it with dashed pen
                IDrawingObject hyper = new Hyperbola(controls, horizontal);
                hyper.onDraw(g, dashedPen);
            }

            dashedPen.Dispose();
        }

        public override void reset(object sender)
        {
            firstSnap = false;
            secondSnap = false;
            sideChosen = false;
            horizontal = true;
            (sender as Control).Invalidate();
        }
    }

    // Select Tool
    public class SelectTool : IDrawingTool
    {
        public SelectTool(ObjectList objectList)
        {
            this.objectList = objectList;
            objectList.defocusAll();
        }

        public override void onMouseDown(object sender, MouseEventArgs e)
        {
            List<IDrawingObject> objList = objectList.getAllVisible(e.Location);
            objectList.defocusAll();
            if (objList.Count > 0)
                foreach (var obj in objList)
                    obj.focus();
            (sender as Control).Invalidate();
        }

        public override void onMouseMove(object sender, MouseEventArgs e)
        {
        }

        public override void onPartialDraw(Graphics g)
        {
        }

        public override void reset(object sender)
        {
        }
    }

    // Move Tool
    public class MoveTool : IDrawingTool
    {
        private IDrawingObject chosenObject;
        private Point oldLocation;
        private Point newLocation;

        public MoveTool(ObjectList objectList)
        {
            this.objectList = objectList;
            objectList.defocusAll();

            chosenObject = null;
        }

        public override void onMouseDown(object sender, MouseEventArgs e)
        {
            // if right click
            // defocus chosen object
            if (e.Button == MouseButtons.Right)
            {
                reset(sender);
                return;
            }

            // if no object is chosen
            // choose one
            if (chosenObject == null)
            {
                chosenObject = objectList.getVisible(e.Location);
                objectList.defocusAll();
                if (chosenObject != null)
                {
                    chosenObject.focus();
                    oldLocation = e.Location;
                }
                else
                {
                    (sender as Control).Invalidate();
                }
                return;
            }
            
            // else, user has specified new location
            // move object using offset from new to old location
            chosenObject.move(newLocation.X - oldLocation.X, newLocation.Y - oldLocation.Y);
            (sender as Control).Invalidate();

            // reset
            chosenObject = null;
        }

        public override void onMouseMove(object sender, MouseEventArgs e)
        {
            if (chosenObject == null)
                return;

            newLocation = e.Location;
            (sender as Control).Invalidate();
        }

        public override void onPartialDraw(Graphics g)
        {
            if (chosenObject == null)
                return;

            Pen dottedPen = new Pen(Color.Gray, 1.0f);
            dottedPen.DashStyle = DashStyle.Dot;

            IDrawingObject clone = chosenObject.clone();
            clone.move(newLocation.X - oldLocation.X, newLocation.Y - oldLocation.Y);
            clone.onDraw(g, dottedPen);

            dottedPen.Dispose();
        }

        public override void reset(object sender)
        {
            if (chosenObject != null)
            {
                chosenObject.defocus();
                chosenObject = null;
            }
            (sender as Control).Invalidate();
        }
    }

    // Delete Tool
    public class DeleteTool : IDrawingTool
    {
        private IDrawingObject chosenObject;

        public DeleteTool(ObjectList objectList)
        {
            this.objectList = objectList;
            objectList.defocusAll();
            chosenObject = null;
        }

        public override void onMouseDown(object sender, MouseEventArgs e)
        {
            // if no object is chosen
            // choose one
            if (chosenObject == null)
            {
                chosenObject = objectList.getVisible(e.Location);
                objectList.defocusAll();
                (sender as Control).Invalidate();
                if (chosenObject == null)
                    return;
            }

            // else, user has specified an object
            // delete object from object list
            objectList.remove(chosenObject);
            (sender as Control).Invalidate();

            // reset
            chosenObject = null;
        }

        public override void onMouseMove(object sender, MouseEventArgs e) { }

        public override void onPartialDraw(Graphics g) { }

        public override void reset(object sender) { }
    }

    // Control Tool
    // NOT FINISHED YET
    public class ControlTool : IDrawingTool
    {
        private IDrawingObject chosenObject;
        private int index;
        private Point location;
        private bool controlSnap;

        public ControlTool(ObjectList objectList)
        {
            this.objectList = objectList;
            objectList.defocusAll();
            chosenObject = null;
            index = -1;
            controlSnap = false;
        }

        public override void onMouseDown(object sender, MouseEventArgs e)
        {
            // if right click
            // defocus chosen object
            if (e.Button == MouseButtons.Right)
            {
                reset(sender);
                return;
            }

            // choose control point
            if (!controlSnap)
            {
                // if no object is chosen
                // choose one
                IDrawingObject visibleObj = objectList.getVisible(e.Location);

                if (visibleObj != null && !ReferenceEquals(visibleObj, chosenObject))
                {
                    chosenObject = visibleObj;
                    if (!chosenObject.isFocused())
                    {
                        objectList.defocusAll();
                        chosenObject.focus();
                        (sender as Control).Invalidate();
                    }
                    return;
                }
                else if (visibleObj == null)
                {
                    chosenObject = null;
                    objectList.defocusAll();
                    (sender as Control).Invalidate();
                    return;
                }

                index = chosenObject.visibleControlPointIndex(e.Location);
                if (index >= 0 && chosenObject.controllablePoint(index))
                    controlSnap = true;
                return;
            }

            // else, user has specified new location
            // move object using offset from new to old location
            IDrawingObject changed = chosenObject.changeControlPoint(index, location);
            if (!ReferenceEquals(changed, chosenObject))
            {
                //objectList.add(changed);
                objectList.replace(chosenObject, changed);
            }
            (sender as Control).Invalidate();

            // reset
            controlSnap = false;
        }

        public override void onMouseMove(object sender, MouseEventArgs e)
        {
            if (!controlSnap)
                return;

            location = chosenObject.invertRotation(e.Location);
            (sender as Control).Invalidate();
        }

        public override void onPartialDraw(Graphics g)
        {
            if (!controlSnap)
                return;

            Pen dottedPen = new Pen(Color.Black, 1.0f);
            dottedPen.DashStyle = DashStyle.Dot;

            chosenObject.onDraw(g, index, location, dottedPen);

            dottedPen.Dispose();
        }

        public override void reset(object sender)
        {
            controlSnap = false;
            if (chosenObject != null)
            {
                chosenObject.defocus();
                chosenObject = null;
            }
            (sender as Control).Invalidate();
        }
    }

    // Scale Tool
    public class ScaleTool : IDrawingTool
    {
        private IDrawingObject chosenObject;
        private Point central;
        private Point oldLocation;
        private Point newLocation;
        private bool controlSnap;

        public ScaleTool(ObjectList objectList)
        {
            this.objectList = objectList;
            objectList.defocusAll();

            chosenObject = null;
            controlSnap = false;
        }

        private void calculateScalingFactors(out float xFactor, out float yFactor)
        {
            xFactor = yFactor = 1;
            // calculate xFactor and yFactor
            if (chosenObject is Circle || chosenObject is CircleArc)
            {
                float oldLength = DrawingUtilities.CalculateLength(oldLocation, central);
                float newLength = DrawingUtilities.CalculateLength(newLocation, central);

                xFactor = yFactor = newLength / oldLength;
                if ((newLocation.X - central.X) * (oldLocation.X - central.X) < 0)
                    xFactor = -xFactor;
                if ((newLocation.Y - central.Y) * (oldLocation.Y - central.Y) < 0)
                    yFactor = -yFactor;
            }
            else
            {
                if (oldLocation.X == central.X && oldLocation.Y == central.Y)
                    return;
                if (oldLocation.Y != central.Y)
                    yFactor = (float)(newLocation.Y - central.Y) / (oldLocation.Y - central.Y);
                if (oldLocation.X != central.X)
                    xFactor = (float)(newLocation.X - central.X) / (oldLocation.X - central.X);
            }

            if (Math.Abs(xFactor) < 0.1)
                xFactor = xFactor < 0 ? -0.1f : 0.1f;
            if (Math.Abs(yFactor) < 0.1)
                yFactor = yFactor < 0 ? -0.1f : 0.1f;
        }

        public override void onMouseDown(object sender, MouseEventArgs e)
        {
            // if right click
            // defocus chosen object
            if (e.Button == MouseButtons.Right)
            {
                reset(sender);
                return;
            }

            // choose control point
            if (!controlSnap)
            {
                // if no object is chosen
                // choose one
                IDrawingObject visibleObj = objectList.getVisible(e.Location);

                if (visibleObj != null && !ReferenceEquals(visibleObj, chosenObject))
                {
                    chosenObject = visibleObj;
                    central = chosenObject.getCentralPoint();
                    if (!chosenObject.isFocused())
                    {
                        objectList.defocusAll();
                        chosenObject.focus();
                        (sender as Control).Invalidate();
                    }
                    return;
                }
                else if (visibleObj == null)
                {
                    chosenObject = null;
                    objectList.defocusAll();
                    (sender as Control).Invalidate();
                    return;
                }

                int index = chosenObject.visibleControlPointIndex(e.Location);
                if (index >= 0 && chosenObject.controllablePoint(index))
                {
                    oldLocation = chosenObject.invertRotation(chosenObject.getControlPointLocation(index));
                    controlSnap = true;
                }
                return;
            }

            // else, user has specified new location
            // scale object using xFactor and yFactor
            // calculate xFactor and yFactor
            float xFactor, yFactor;
            calculateScalingFactors(out xFactor, out yFactor);

            // scale obect
            chosenObject.scale(xFactor, yFactor);
            (sender as Control).Invalidate();

            // reset
            controlSnap = false;
        }

        public override void onMouseMove(object sender, MouseEventArgs e)
        {
            if (!controlSnap)
                return;

            newLocation = chosenObject.invertRotation(e.Location);

            (sender as Control).Invalidate();
        }

        public override void onPartialDraw(Graphics g)
        {
            if (!controlSnap)
                return;

            // calculate xFactor and yFactor
            float xFactor, yFactor;
            calculateScalingFactors(out xFactor, out yFactor);

            Pen dottedPen = new Pen(Color.Gray, 1.0f);
            dottedPen.DashStyle = DashStyle.Dot;

            IDrawingObject clone = chosenObject.clone();
            clone.scale(xFactor, yFactor);
            clone.focus();
            clone.onDraw(g, dottedPen);

            dottedPen.Dispose();
        }

        public override void reset(object sender)
        {
            controlSnap = false;
            if (chosenObject != null)
            {
                chosenObject.defocus();
                chosenObject = null;
            }
            (sender as Control).Invalidate();
        }
    }

    // Scale Tool
    public class RotateTool : IDrawingTool
    {
        private IDrawingObject chosenObject;
        private Point central;
        private Point oldLocation;
        private Point newLocation;
        private bool controlSnap;

        public RotateTool(ObjectList objectList)
        {
            this.objectList = objectList;
            objectList.defocusAll();

            chosenObject = null;
            controlSnap = false;
        }

        private float calculateAngle(Point central, Point oldLocation, Point newLocation)
        {
            float oldAngle = DrawingUtilities.CalculateAngle(central, oldLocation) * 180 / (float)Math.PI;
            float newAngle = DrawingUtilities.CalculateAngle(central, newLocation) * 180 / (float)Math.PI;
            return newAngle - oldAngle;
        }

        public override void onMouseDown(object sender, MouseEventArgs e)
        {
            // if right click
            // defocus chosen object
            if (e.Button == MouseButtons.Right)
            {
                reset(sender);
                return;
            }

            // choose control point
            if (!controlSnap)
            {
                // if no object is chosen
                // choose one
                IDrawingObject visibleObj = objectList.getVisible(e.Location);

                if (visibleObj != null && !ReferenceEquals(visibleObj, chosenObject))
                {
                    chosenObject = visibleObj;
                    central = chosenObject.getCentralPoint();
                    if (!chosenObject.isFocused())
                    {
                        objectList.defocusAll();
                        chosenObject.focus();
                        (sender as Control).Invalidate();
                    }
                    return;
                }
                else if (visibleObj == null)
                {
                    chosenObject = null;
                    objectList.defocusAll();
                    (sender as Control).Invalidate();
                    return;
                }

                int index = chosenObject.visibleControlPointIndex(e.Location);
                if (index >= 0 && chosenObject.controllablePoint(index))
                {
                    oldLocation = chosenObject.getControlPointLocation(index);
                    controlSnap = true;
                }
                return;
            }

            // else, user has specified new location
            // calculate angle between old and new location
            // and rotate chosen object using that angle
            float angle = calculateAngle(chosenObject.getCentralPoint(), oldLocation, newLocation);
            chosenObject.rotate(angle);
            (sender as Control).Invalidate();

            // reset
            controlSnap = false;
        }

        public override void onMouseMove(object sender, MouseEventArgs e)
        {
            if (!controlSnap)
                return;

            newLocation = e.Location;

            (sender as Control).Invalidate();
        }

        public override void onPartialDraw(Graphics g)
        {
            if (!controlSnap)
                return;

            // calculate angle between old location and new location
            float angle = calculateAngle(chosenObject.getCentralPoint(), oldLocation, newLocation);

            Pen dottedPen = new Pen(Color.Gray, 1.0f);
            dottedPen.DashStyle = DashStyle.Dot;

            IDrawingObject clone = chosenObject.clone();
            clone.rotate(angle);
            clone.focus();
            clone.onDraw(g, dottedPen);

            dottedPen.Dispose();
        }

        public override void reset(object sender)
        {
            controlSnap = false;
            if (chosenObject != null)
            {
                chosenObject.defocus();
                chosenObject = null;
            }
            (sender as Control).Invalidate();
        }
    }
}
