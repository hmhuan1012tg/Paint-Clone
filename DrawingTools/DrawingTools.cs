using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Forms;
using DrawingObjects;

namespace DrawingTools
{
    public abstract class IDrawingTool
    {
        protected Form owner;
        protected ObjectList objectList;

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
            g.DrawLines(dashedPen, controlPoints.ToArray());

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
                {
                    float endAngle = DrawingUtilities.CalculateAngle(center, pivot[1]);
                    pivot[1].X = (int)(radius * Math.Cos(endAngle)) + center.X;
                    pivot[1].Y = (int)(radius * Math.Sin(endAngle)) + center.Y;
                }
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
                if((snapAngle >= startAngle && snapAngle <= endAngle && sweepAngle < (360 - sweepAngle))
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
            if (!centerSnap)
                return;

            // prepare dashed pen
            Pen dashedPen = new Pen(Color.Black, 1.0f);
            dashedPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;

            // draw a line connecting first pivot and center
            g.DrawLine(dashedPen, center, pivot[0]);

            // if first pivot is chosen
            // it means that second pivot is being chosen
            if(pivotSnaps > 0)
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
            int startX = Math.Min(firstPoint.X, secondPoint.X);
            int startY = Math.Min(firstPoint.Y, secondPoint.Y);

            int endX = Math.Max(firstPoint.X, secondPoint.X);
            int endY = Math.Max(firstPoint.Y, secondPoint.Y);

            Point center = new Point((endX + startX) / 2, (endY + startY) / 2);
            int radius = (endX - startX) / 2;

            Circle circle = new Circle(center, radius, true);
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

            g.DrawEllipse(dashedPen, new Rectangle(startX, startY, size, size));

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

            Ellipse ellipse = new Ellipse(center, (endX - startX) / 2, (endY - startY) / 2, true);
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
            dashedPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;

            // draw dashed ellipse
            int startX = Math.Min(firstPoint.X, secondPoint.X);
            int startY = Math.Min(firstPoint.Y, secondPoint.Y);

            int endX = Math.Max(firstPoint.X, secondPoint.X);
            int endY = Math.Max(firstPoint.Y, secondPoint.Y);

            g.DrawEllipse(dashedPen, new Rectangle(startX, startY, endX - startX, endY - startY));

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
        private int a;
        private int b;
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
                secondSnap = true;
                a = Math.Abs(firstPoint.X - secondPoint.X) / 2;
                b = Math.Abs(firstPoint.Y - secondPoint.Y) / 2;
                center.X = (firstPoint.X + secondPoint.X) / 2;
                center.Y = (firstPoint.Y + secondPoint.Y) / 2;
                return;
            }

            // choose pivot points
            if (pivotSnaps < 2)
            {
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
                pivot[pivotSnaps] = e.Location;

                float angle = DrawingUtilities.CalculateAngle(center, pivot[pivotSnaps]);
                int a2 = a * a;
                int b2 = b * b;
                float sin2 = (float)(Math.Sin(angle) * Math.Sin(angle));
                float cos2 = (float)(Math.Cos(angle) * Math.Cos(angle));
                float radius = (float)Math.Sqrt(a2 * b2 / (b2 * cos2 + a2 * sin2));

                pivot[pivotSnaps].X = (int)(radius * Math.Cos(angle)) + center.X;
                pivot[pivotSnaps].Y = (int)(radius * Math.Sin(angle)) + center.Y;

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
            dashedPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;

            if (!secondSnap)
            {
                // draw dashed ellipse
                int startX = Math.Min(firstPoint.X, secondPoint.X);
                int startY = Math.Min(firstPoint.Y, secondPoint.Y);

                int endX = Math.Max(firstPoint.X, secondPoint.X);
                int endY = Math.Max(firstPoint.Y, secondPoint.Y);

                g.DrawEllipse(dashedPen, new Rectangle(startX, startY, endX - startX, endY - startY));
                return;
            }

            // draw a line connecting first pivot and center
            g.DrawLine(dashedPen, center, pivot[0]);

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

        public override void reset(object sender)
        {
        }
    }
}
