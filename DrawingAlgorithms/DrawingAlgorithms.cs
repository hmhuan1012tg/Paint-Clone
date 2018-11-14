using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawingAlgorithms
{
    public struct Point2D
    {
        public int x;
        public int y;
        public float brightness;

        public Point2D(int aX, int aY)
        {
            x = aX;
            y = aY;
            brightness = 1;
        }

        public Point2D(int aX, int aY, float br)
        {
            x = aX;
            y = aY;
            brightness = br;
        }

        public Point2D(Point2D p)
        {
            x = p.x;
            y = p.y;
            brightness = p.brightness;
        }

        public static bool operator ==(Point2D p1, Point2D p2)
        {
            return p1.x == p2.x && p1.y == p2.y;
        }
        public static bool operator !=(Point2D p1, Point2D p2)
        {
            return !(p1 == p2);
        }
    }

    public abstract class Shape2D
    {
        public abstract List<Shape2D> GenerateRandoms(int xLower, int xUpper, int yLower, int yUpper, int size);
    }

    public class Line: Shape2D
    {
        public Point2D first;
        public Point2D second;
        private static Random rand = new Random();

        public Line(Point2D p1, Point2D p2)
        {
            first = p1;
            second = p2;
        }

        public override List<Shape2D> GenerateRandoms(int xLower, int xUpper, int yLower, int yUpper, int size)
        {
            List<Shape2D> list = new List<Shape2D>(size);

            for (int i = 0; i < size; i++)
            {
                int xStart = rand.Next(xLower, xUpper + 1);
                int xEnd = rand.Next(xLower, xUpper + 1);
                int yStart = rand.Next(yLower, yUpper + 1);
                int yEnd = rand.Next(yLower, yUpper + 1);

                list.Add(new Line(new Point2D(xStart, yStart), new Point2D(xEnd, yEnd)));
            }

            return list;
        }
    }

    public class Ellipse: Shape2D
    {
        private static Random rand = new Random();

        public Point2D center;
        public int a;
        public int b;

        public Ellipse(Point2D c, int aA, int aB)
        {
            center = c;
            a = aA;
            b = aB;
        }

        public override List<Shape2D> GenerateRandoms(int xLower, int xUpper, int yLower, int yUpper, int size)
        {
            List<Shape2D> list = new List<Shape2D>(size);

            for (int i = 0; i < size; i++)
            {
                int xCenter = rand.Next(xLower, xUpper - 1);
                int yCenter = rand.Next(xLower, xUpper - 1);
                int a = rand.Next(100, xUpper / 2 - 1);
                int b = rand.Next(100, yUpper / 2 - 1);

                list.Add(new Ellipse(new Point2D(xCenter, yCenter), a, b));
            }

            return list;
        }
    }

    public class Circle : Shape2D
    {
        private static Random rand = new Random();

        public Point2D center;
        public int radius;

        public Circle(Point2D c, int r)
        {
            center = c;
            radius = r;
        }

        public override List<Shape2D> GenerateRandoms(int xLower, int xUpper, int yLower, int yUpper, int size)
        {
            List<Shape2D> list = new List<Shape2D>(size);

            for (int i = 0; i < size; i++)
            {
                int xCenter = rand.Next(xLower, xUpper + 1);
                int yCenter = rand.Next(xLower, xUpper + 1);
                int r = rand.Next(20, (xUpper < yUpper ? xUpper : yUpper) / 2 + 1);

                list.Add(new Circle(new Point2D(xCenter, yCenter), r));
            }

            return list;
        }
    }

    public enum Orientation
    {
        Vertical,
        Horizontal
    }

    public class Parabola : Shape2D
    {
        private static Random rand = new Random();

        public Point2D pivot;
        public int a;
        public int b;
        public int limit;
        public Orientation orientation;

        public Parabola(Point2D p, int aA, int aB, int l, Orientation o)
        {
            pivot = p;
            a = aA;
            b = aB;
            limit = l;
            orientation = o;
        }

        public override List<Shape2D> GenerateRandoms(int xLower, int xUpper, int yLower, int yUpper, int size)
        {
            List<Shape2D> list = new List<Shape2D>(size);

            for(int i = 0; i < size; i++)
            {
                int px = rand.Next(xLower, xUpper + 1);
                int py = rand.Next(yLower, yUpper + 1);
                int a = rand.Next(1, 100) * (rand.Next(0, 2) == 0 ? 1 : -1);
                int b = rand.Next(1, 100) * (rand.Next(0, 2) == 0 ? 1 : -1);
                Orientation o = (Orientation)rand.Next(0, 2);
                int limit = 0;
                if (o == Orientation.Vertical)
                {
                    if (a * b < 0)
                        limit = yUpper - py;
                    else
                        limit = yLower - py;
                }
                else if (o == Orientation.Horizontal)
                {
                    if (a * b < 0)
                        limit = xUpper - px;
                    else
                        limit = xLower - px;
                }

                list.Add(new Parabola(new Point2D(px, py), a, b, limit, o));
            }

            return list;
        }
    }

    public class Hyperbola : Shape2D
    {
        private static Random rand = new Random();

        public Point2D center;
        public int a;
        public int b;
        public int limit;
        public Orientation orientation;

        public Hyperbola(Point2D c, int aA, int aB, int l, Orientation o)
        {
            center = c;
            a = aA;
            b = aB;
            limit = l;
            orientation = o;
        }

        public override List<Shape2D> GenerateRandoms(int xLower, int xUpper, int yLower, int yUpper, int size)
        {
            List<Shape2D> list = new List<Shape2D>(size);

            for(int i = 0; i < size; i++)
            {
                int x = rand.Next(xLower, xUpper + 1);
                int y = rand.Next(yLower, yUpper + 1);
                int a = rand.Next(1, xUpper / 2 + 1);
                int b = rand.Next(1, yUpper / 2 + 1);
                int limX = (xUpper - x) < (x - xLower) ? (x - xLower) : (xUpper - x);
                int limY = (yUpper - y) < (y - yLower) ? (y - yLower) : (yUpper - y);
                Orientation o = (Orientation)rand.Next(0, 2);
                int limit = o == Orientation.Horizontal ? limX : limY;

                list.Add(new Hyperbola(new Point2D(x, y), a, b, limit, o));
            }

            return list;
        }
    }

    public abstract class Algorithm
    {
        public List<Point2D> GeneratePoint2DList(Shape2D shape)
        {
            if (shape is Line)
                return GeneratePoint2DList(shape as Line);
            else if (shape is Ellipse)
                return GeneratePoint2DList(shape as Ellipse);
            else if (shape is Circle)
                return GeneratePoint2DList(shape as Circle);
            else if (shape is Parabola)
                return GeneratePoint2DList(shape as Parabola);
            else if (shape is Hyperbola)
                return GeneratePoint2DList(shape as Hyperbola);
            else
                throw new InvalidProgramException("Shape2D object type not supported");
        }

        protected abstract List<Point2D> GeneratePoint2DList(Line line);
        protected abstract List<Point2D> GeneratePoint2DList(Ellipse ellipse);
        protected abstract List<Point2D> GeneratePoint2DList(Circle circle);
        protected abstract List<Point2D> GeneratePoint2DList(Parabola para);
        protected abstract List<Point2D> GeneratePoint2DList(Hyperbola hyper);
        protected List<Point2D> Generate4SymmetricPoint2D(List<Point2D> list, Point2D center)
        {
            List<Point2D> output = new List<Point2D>();
            foreach(var point in list)
            {
                output.Add(new Point2D(center.x + point.x, center.y + point.y));
                output.Add(new Point2D(center.x - point.x, center.y + point.y));
                output.Add(new Point2D(center.x + point.x, center.y - point.y));
                output.Add(new Point2D(center.x - point.x, center.y - point.y));
            }
            return output;
        }
        protected List<Point2D> Generate8SymmetricPoint2D(List<Point2D> list, Point2D center)
        {
            List<Point2D> output = new List<Point2D>();
            foreach (var point in list)
            {
                output.Add(new Point2D(center.x + point.x, center.y + point.y));
                output.Add(new Point2D(center.x - point.x, center.y + point.y));
                output.Add(new Point2D(center.x + point.x, center.y - point.y));
                output.Add(new Point2D(center.x - point.x, center.y - point.y));

                output.Add(new Point2D(center.x + point.y, center.y + point.x));
                output.Add(new Point2D(center.x - point.y, center.y + point.x));
                output.Add(new Point2D(center.x + point.y, center.y - point.x));
                output.Add(new Point2D(center.x - point.y, center.y - point.x));
            }
            return output;
        }
        protected List<Point2D> GenerateOnXSymmetricPoint2D(List<Point2D> list, Point2D pivot)
        {
            List<Point2D> output = new List<Point2D>();

            foreach(var point in list)
            {
                output.Add(new Point2D(pivot.x + point.x, pivot.y + point.y));
                output.Add(new Point2D(pivot.x + point.x, pivot.y - point.y));
            }

            return output;
        }
        protected List<Point2D> GenerateOnYSymmetricPoint2D(List<Point2D> list, Point2D pivot)
        {
            List<Point2D> output = new List<Point2D>();

            foreach (var point in list)
            {
                output.Add(new Point2D(pivot.x + point.x, pivot.y + point.y));
                output.Add(new Point2D(pivot.x - point.x, pivot.y + point.y));
            }

            return output;
        }
    }

    public class DDA: Algorithm
    {
        protected override List<Point2D> GeneratePoint2DList(Line line)
        {
            List<Point2D> list = new List<Point2D>();

            // Add start Point
            list.Add(line.first);
            // If end Point is the same as start Point
            // Return list immediately
            if (line.first == line.second)
                return list;

            int dx = line.second.x - line.first.x;
            int dy = line.second.y - line.first.y;

            // Add middle Points
            if (Math.Abs(dx) > Math.Abs(dy))
            {
                double m = (dy * 1.0) / dx;
                int xInc = (dx < 0 ? -1 : 1);
                double yInc = m * xInc;

                int x = line.first.x + xInc;
                double y = line.first.y + yInc;
                while(x != line.second.x)
                {
                    list.Add(new Point2D(x, (int)Math.Round(y)));
                    x += xInc;
                    y += yInc;
                }
            }
            else
            {
                double m = (dx * 1.0) / dy;
                int yInc = (dy < 0 ? -1 : 1);
                double xInc = m * yInc;

                int y = line.first.y + yInc;
                double x = line.first.x + xInc;
                while (y != line.second.y)
                {
                    list.Add(new Point2D((int)Math.Round(x), y));
                    y += yInc;
                    x += xInc;
                }
            }

            // Add end Point and return list
            list.Add(line.second);
            return list;
        }
        protected override List<Point2D> GeneratePoint2DList(Ellipse ellipse)
        {
            List<Point2D> list = new List<Point2D>();

            int a2 = ellipse.a * ellipse.a;
            int b2 = ellipse.b * ellipse.b;

            // First part of the curve
            int x = 0;
            int y = ellipse.b;
            while((double)b2 / a2 * x <= y)
            {
                list.Add(new Point2D(x, y));

                x++;
                y = (int)Math.Round(Math.Sqrt(b2 - (double)x * x * b2 / a2));
            }

            // Second part of the curve
            y = 0;
            x = ellipse.a;
            while ((double)a2 / b2 * y <= x)
            {
                list.Add(new Point2D(x, y));

                y++;
                x = (int)Math.Round(Math.Sqrt(a2 - (double)y * y * a2 / b2));
            }

            return Generate4SymmetricPoint2D(list, ellipse.center);
        }
        protected override List<Point2D> GeneratePoint2DList(Circle circle)
        {
            List<Point2D> list = new List<Point2D>();

            int x = 0;
            int y = circle.radius;
            long r2 = circle.radius * circle.radius;
            while(x <= y)
            {
                list.Add(new Point2D(x, y));

                x += 1;
                y = (int)Math.Round(Math.Sqrt(r2 - x * x));
            }

            return Generate8SymmetricPoint2D(list, circle.center);
        }
        protected override List<Point2D> GeneratePoint2DList(Parabola para)
        {
            List<Point2D> list = new List<Point2D>();

            int a = para.a;
            int b = para.b;
            int aSign = a < 0 ? -1 : 1;
            int bSign = b < 0 ? -1 : 1;
            int x = 0;
            int y = 0;
            int xprev = x;
            int yprev = y;

            int yInc = -aSign * bSign;
            // First half
            while (Math.Abs(2 * a * x) < Math.Abs(b))
            {
                if (para.orientation == Orientation.Vertical)
                    list.Add(new Point2D(x, y));
                else
                    list.Add(new Point2D(y, -x));
                xprev = x;
                yprev = y;

                x += 1;
                y = (int)Math.Round(-a * x * x / (double)b);
            }
            // Second half
            x = xprev;
            y = yprev;
            while (y * yInc <= para.limit * yInc)
            {
                if (para.orientation == Orientation.Vertical)
                    list.Add(new Point2D(x, y));
                else
                    list.Add(new Point2D(y, -x));

                y += yInc;
                x = (int)Math.Round(Math.Sqrt(-y * b * 1.0 / a));
            }

            if (para.orientation == Orientation.Vertical)
                list = GenerateOnYSymmetricPoint2D(list, para.pivot);
            else
                list = GenerateOnXSymmetricPoint2D(list, para.pivot);

            return list;
        }
        protected override List<Point2D> GeneratePoint2DList(Hyperbola hyper)
        {
            List<Point2D> list = new List<Point2D>();

            int x = hyper.a;
            int y = 0;
            int a = hyper.a;
            int b = hyper.b;
            int a2 = a * a;
            int b2 = b * b;
            int xprev = x;
            int yprev = y;
            // First Half
            while(Math.Abs(x * b2) >= Math.Abs(y * a2) && x <= hyper.limit)
            {
                if (hyper.orientation == Orientation.Horizontal)
                    list.Add(new Point2D(x, y));
                else
                    list.Add(new Point2D(-y, x));
                xprev = x;
                yprev = y;

                y += 1;
                x = (int)Math.Round(Math.Sqrt((double)a2 * y * y / b2 + a2));
            }
            // Second Half
            x = xprev;
            y = yprev;
            while(x <= hyper.limit)
            {
                if (hyper.orientation == Orientation.Horizontal)
                    list.Add(new Point2D(x, y));
                else
                    list.Add(new Point2D(-y, x));
                x += 1;
                y = (int)Math.Round(Math.Sqrt((double)b2 * x * x / a2 - b2));
            }

            return Generate4SymmetricPoint2D(list, hyper.center);
        }
    }

    public class Bresenham: Algorithm
    {
        protected override List<Point2D> GeneratePoint2DList(Line line)
        {
            List<Point2D> list = new List<Point2D>();

            // Add start Point
            list.Add(line.first);
            // If end Point is the same as start Point
            // Return list immediately
            if (line.first == line.second)
                return list;

            int dx = line.second.x - line.first.x;
            int dy = line.second.y - line.first.y;

            // Add middle Points
            if (Math.Abs(dx) > Math.Abs(dy))
            {
                int dxSign = (dx < 0 ? -1 : 1);
                int dySign = (dy < 0 ? -1 : 1);

                int p = dySign * dxSign * 2 * dy - dx;
                int c1 = dySign * dxSign * 2 * dy - 2 * dx;
                int c2 = dySign * dxSign * 2 * dy;
                
                int x = line.first.x + dxSign;
                int y = line.first.y;
                while(x != line.second.x)
                {
                    if ((p > 0 && dx > 0) || (p < 0 && dx < 0))
                    {
                        y += dySign;
                        p += c1;
                    }
                    else
                    {
                        p += c2;
                    }
                    list.Add(new Point2D(x, y));

                    x += dxSign;
                }    
            }
            else
            {
                int dySign = (dy < 0 ? -1 : 1);
                int dxSign = (dx < 0 ? -1 : 1);

                int p = dxSign * dySign * 2 * dx - dy;
                int c1 = dxSign * dySign * 2 * dx - 2 * dy;
                int c2 = dxSign * dySign * 2 * dx;

                int y = line.first.y + dySign;
                int x = line.first.x;
                while (y != line.second.y)
                {
                    if ((p > 0 && dy > 0) || (p < 0 && dy < 0))
                    {
                        x += dxSign;
                        p += c1;
                    }
                    else
                    {
                        p += c2;
                    }
                    list.Add(new Point2D(x, y));

                    y += dySign;
                }
            }

            // Add end Point and return list
            list.Add(line.second);

            return list;
        }
        protected override List<Point2D> GeneratePoint2DList(Ellipse ellipse)
        {
            List<Point2D> list = new List<Point2D>();

            int a2 = ellipse.a * ellipse.a;
            int b2 = ellipse.b * ellipse.b;

            // First part of the curve
            int x = 0;
            int y = ellipse.b;
            long p = 2 * b2 + 2 * ellipse.b * a2 - a2;
            while (b2 * x <= y * a2)
            {
                list.Add(new Point2D(x, y));

                x++;
                if(p < 0)
                {
                    p += 2 * b2 * (2 * x + 3);
                }
                else
                {
                    p += 2 * b2 * (2 * x + 3) - 4 * y * a2;
                    y -= 1;
                }
            }

            // Second part of the curve
            y = 0;
            x = ellipse.a;
            p = 2 * a2 + 2 * ellipse.a * b2 - b2;
            while (a2 * y <= x * b2)
            {
                list.Add(new Point2D(x, y));

                y++;
                if (p < 0)
                {
                    p += 2 * a2 * (2 * y + 3);
                }
                else
                {
                    p += 2 * a2 * (2 * y + 3) - 4 * x * b2;
                    x -= 1;
                }
            }

            return Generate4SymmetricPoint2D(list, ellipse.center);
        }
        protected override List<Point2D> GeneratePoint2DList(Circle circle)
        {
            List<Point2D> list = new List<Point2D>();

            int x = 0;
            int y = circle.radius;
            long r = circle.radius;
            long p = 1 + 2 * r;
            while (x <= y)
            {
                list.Add(new Point2D(x, y));

                x++;
                if (p < 0)
                {
                    p += 2 * (2 * x + 3);
                }
                else
                {
                    p += 2 * (2 * x + 3) - 4 * y;
                    y -= 1;
                }
            }

            return Generate8SymmetricPoint2D(list, circle.center);
        }
        protected override List<Point2D> GeneratePoint2DList(Parabola para)
        {
            List<Point2D> list = new List<Point2D>();

            int a = para.a;
            int b = para.b;
            int aSign = a < 0 ? -1 : 1;
            int bSign = b < 0 ? -1 : 1;
            int x = 0;
            int y = 0;
            int xprev = x;
            int yprev = y;

            int yInc = -aSign * bSign;
            // First half
            int p = -2 * yInc * a - b;
            while (Math.Abs(2 * a * x) < Math.Abs(b))
            {
                if (para.orientation == Orientation.Vertical)
                    list.Add(new Point2D(x, y));
                else
                    list.Add(new Point2D(y, -x));
                xprev = x;
                yprev = y;

                if ((b > 0 && p > 0) || (b < 0 && p < 0))
                {
                    p += yInc * -2 * a * (2 * x + 3) - 2 * b;
                    y += yInc;
                }
                else
                {
                    p += yInc * -2 * a * (2 * x + 3);
                }
                x += 1;
            }
            // Second half
            x = xprev;
            y = yprev;
            p = -2 * b * (y + yInc) - a * (2 * x * x + 2 * x + 1);
            while (y * yInc <= para.limit * yInc)
            {
                if (para.orientation == Orientation.Vertical)
                    list.Add(new Point2D(x, y));
                else
                    list.Add(new Point2D(y, -x));

                if ((a > 0 && p > 0) || (a < 0 && p < 0))
                {
                    p += -2 * b * yInc - 2 * a * (2 * x + 2);
                    x += 1;
                }
                else
                {
                    p += -2 * b * yInc;
                }
                y += yInc;
            }

            if (para.orientation == Orientation.Vertical)
                list = GenerateOnYSymmetricPoint2D(list, para.pivot);
            else
                list = GenerateOnXSymmetricPoint2D(list, para.pivot);

            return list;
        }
        protected override List<Point2D> GeneratePoint2DList(Hyperbola hyper)
        {
            List<Point2D> list = new List<Point2D>();

            int x = hyper.a;
            int y = 0;
            int a = hyper.a;
            int b = hyper.b;
            int a2 = a * a;
            int b2 = b * b;
            long p = 2 * a2 - 2 * a * b2 - b2;
            int xprev = x;
            int yprev = y;
            // First Half
            while (Math.Abs(x * b2) >= Math.Abs(y * a2) && x <= hyper.limit)
            {
                if (hyper.orientation == Orientation.Horizontal)
                    list.Add(new Point2D(x, y));
                else
                    list.Add(new Point2D(-y, x));
                xprev = x;
                yprev = y;

                if (p > 0)
                {
                    p += 2 * a2 * (2 * y + 3) - 2 * b2 * (2 * x + 2);
                    x += 1;
                }
                else
                {
                    p += 2 * a2 * (2 * y + 3);
                }
                y += 1;
            }
            // Second Half
            x = xprev;
            y = yprev;
            p = 2 * b2 * (x + 1) * (x + 1) - 2 * a2 * (y * y + y) - a2 * (2 * b2 + 1);
            while (x <= hyper.limit)
            {
                if (hyper.orientation == Orientation.Horizontal)
                    list.Add(new Point2D(x, y));
                else
                    list.Add(new Point2D(-y, x));

                if (p > 0)
                {
                    p += 2 * b2 * (2 * x + 3) - 2 * a2 * (2 * y + 2);
                    y += 1;
                }
                else
                {
                    p += 2 * b2 * (2 * x + 3);
                }
                x += 1;
            }

            return Generate4SymmetricPoint2D(list, hyper.center);
        }
    }

    public class MidPoint: Algorithm
    {
        protected override List<Point2D> GeneratePoint2DList(Line line)
        {
            List<Point2D> list = new List<Point2D>();

            // Add start Point
            list.Add(line.first);
            // If end Point is the same as start Point
            // Return list immediately
            if (line.first == line.second)
                return list;

            int dx = line.second.x - line.first.x;
            int dy = line.second.y - line.first.y;

            // Add middle Points
            if (Math.Abs(dx) > Math.Abs(dy))
            {
                int dxSign = (dx < 0 ? -1 : 1);
                int dySign = (dy < 0 ? -1 : 1);

                int p = dySign * dxSign * 2 * dy - dx;
                int c1 = dySign * dxSign * 2 * dy - 2 * dx;
                int c2 = dySign * dxSign * 2 * dy;

                int x = line.first.x + dxSign;
                int y = line.first.y;
                while (x != line.second.x)
                {
                    if ((p > 0 && dx > 0) || (p < 0 && dx < 0))
                    {
                        y += dySign;
                        p += c1;
                    }
                    else
                    {
                        p += c2;
                    }
                    list.Add(new Point2D(x, y));

                    x += dxSign;
                }
            }
            else
            {
                int dySign = (dy < 0 ? -1 : 1);
                int dxSign = (dx < 0 ? -1 : 1);

                int p = dxSign * dySign * 2 * dx - dy;
                int c1 = dxSign * dySign * 2 * dx - 2 * dy;
                int c2 = dxSign * dySign * 2 * dx;

                int y = line.first.y + dySign;
                int x = line.first.x;
                while (y != line.second.y)
                {
                    if ((p > 0 && dy > 0) || (p < 0 && dy < 0))
                    {
                        x += dxSign;
                        p += c1;
                    }
                    else
                    {
                        p += c2;
                    }
                    list.Add(new Point2D(x, y));

                    y += dySign;
                }
            }

            // Add end Point and return list
            list.Add(line.second);

            return list;
        }
        protected override List<Point2D> GeneratePoint2DList(Ellipse ellipse)
        {
            List<Point2D> list = new List<Point2D>();

            int a2 = ellipse.a * ellipse.a;
            int b2 = ellipse.b * ellipse.b;

            // First part of the curve
            int x = 0;
            int y = ellipse.b;
            long p = 4 * b2 - 4 * a2 * ellipse.b + a2;
            while (b2 * x <= y * a2)
            {
                list.Add(new Point2D(x, y));

                x++;
                if (p < 0)
                {
                    p += 4 * b2 * (2 * x + 3);
                }
                else
                {
                    p += 4 * b2 * (2 * x + 3) - 8 * a2 * (y - 1);
                    y -= 1;
                }
            }

            // Second part of the curve
            y = 0;
            x = ellipse.a;
            p = 4 * a2 - 4 * b2 * ellipse.a + b2;
            while (a2 * y <= x * b2)
            {
                list.Add(new Point2D(x, y));

                y++;
                if (p < 0)
                {
                    p += 4 * a2 * (2 * y + 3);
                }
                else
                {
                    p += 4 * a2 * (2 * y + 3) - 8 * b2 * (x - 1);
                    x -= 1;
                }
            }

            return Generate4SymmetricPoint2D(list, ellipse.center);
        }
        protected override List<Point2D> GeneratePoint2DList(Circle circle)
        {
            List<Point2D> list = new List<Point2D>();

            int x = 0;
            int y = circle.radius;
            long r = circle.radius;
            long p = 5  - 4 * r;
            while (x <= y)
            {
                list.Add(new Point2D(x, y));

                x++;
                if (p < 0)
                {
                    p += 4 * (2 * x + 3);
                }
                else
                {
                    p += 4 * (2 * x + 3) - 8 * (y - 1);
                    y -= 1;
                }
            }

            return Generate8SymmetricPoint2D(list, circle.center);
        }
        protected override List<Point2D> GeneratePoint2DList(Parabola para)
        {
            List<Point2D> list = new List<Point2D>();

            int a = para.a;
            int b = para.b;
            if (b > 0)
            {
                a = -a;
                b = -b;
            }
            int aSign = a < 0 ? -1 : 1;
            int x = 0;
            int y = 0;
            int xprev = x;
            int yprev = y;

            int yInc = aSign;
            // First half
            int p = 2 * yInc * a + b;
            while (Math.Abs(2 * a * x) < Math.Abs(b) && Math.Abs(y) < Math.Abs(para.limit))
            {
                if (para.orientation == Orientation.Vertical)
                    list.Add(new Point2D(x, y));
                else
                    list.Add(new Point2D(y, -x));

                xprev = x;
                yprev = y;

                if (p > 0)
                {
                    p += yInc * 2 * a * (2 * x + 3) + 2 * b;
                    y += yInc;
                }
                else
                {
                    p += yInc * 2 * a * (2 * x + 3);
                }
                x += 1;
            }
            // Second half
            x = xprev;
            y = yprev;
            if (a > 0)
            {
                a = -a;
                b = -b;
            }
            p = 4 * a * x * (x + 1) + a + 4 * b * (y + yInc);
            while (y * yInc <= para.limit * yInc)
            {
                if (para.orientation == Orientation.Vertical)
                    list.Add(new Point2D(x, y));
                else
                    list.Add(new Point2D(y, -x));

                if (p > 0)
                {
                    p += 4 * a * (2 * x + 2) + 4 * b * yInc;
                    x += 1;
                }
                else
                {
                    p += 4 * b * yInc;
                }
                y += yInc;
            }

            if (para.orientation == Orientation.Vertical)
                list = GenerateOnYSymmetricPoint2D(list, para.pivot);
            else
                list = GenerateOnXSymmetricPoint2D(list, para.pivot);

            return list;
        }
        protected override List<Point2D> GeneratePoint2DList(Hyperbola hyper)
        {
            List<Point2D> list = new List<Point2D>();

            int x = hyper.a;
            int y = 0;
            int a = hyper.a;
            int b = hyper.b;
            int a2 = a * a;
            int b2 = b * b;
            long p = 4 * a * b2 + b2 - 4 * a2;
            int xprev = x;
            int yprev = y;
            // First Half
            while (Math.Abs(x * b2) >= Math.Abs(y * a2) && x <= hyper.limit)
            {
                if (hyper.orientation == Orientation.Horizontal)
                    list.Add(new Point2D(x, y));
                else
                    list.Add(new Point2D(-y, x));
                xprev = x;
                yprev = y;

                if(p < 0)
                {
                    p += -4 * a2 * (2 * y + 3) + 8 * b2 * (x + 1);
                    x += 1;
                }
                else
                {
                    p += -4 * a2 * (2 * y + 3);
                }
                y += 1;
            }
            // Second Half
            x = xprev;
            y = yprev;
            p = 4 * b2 * (x + 1) * (x + 1) - a2 * (2 * y + 1) * (2 * y + 1) - 4 * a2 * b2;
            while (x <= hyper.limit)
            {
                if (hyper.orientation == Orientation.Horizontal)
                    list.Add(new Point2D(x, y));
                else
                    list.Add(new Point2D(-y, x));
                
                if(p > 0)
                {
                    p += 4 * b2 * (2 * x + 3) - 8 * a2 * (y + 1);
                    y += 1;
                }
                else
                {
                    p += 4 * b2 * (2 * x + 3);
                }
                x += 1;
            }

            return Generate4SymmetricPoint2D(list, hyper.center);
        }
    }

    public class XiaolinWu: Algorithm
    {
        protected override List<Point2D> GeneratePoint2DList(Line line)
        {
            List<Point2D> list = new List<Point2D>();

            // Add start Point
            list.Add(line.first);
            // If end Point is the same as start Point
            // Return list immediately
            if (line.first == line.second)
                return list;

            int dx = line.second.x - line.first.x;
            int dy = line.second.y - line.first.y;

            // Add middle Points
            if (Math.Abs(dx) > Math.Abs(dy))
            {
                double m = (dy * 1.0) / dx;
                int xInc = (dx < 0 ? -1 : 1);
                double yInc = m * xInc;

                int x = line.first.x + xInc;
                double y = line.first.y + yInc;
                while (x != line.second.x)
                {
                    int iy = (int)(dy < 0 ? Math.Ceiling(y) : Math.Floor(y));
                    float fy = (float)Math.Abs(y - iy);
                    list.Add(new Point2D(x, iy, 1 - fy));
                    list.Add(new Point2D(x, iy + (dy < 0 ? -1 : 1), fy));

                    x += xInc;
                    y += yInc;
                }
            }
            else
            {
                double m = (dx * 1.0) / dy;
                int yInc = (dy < 0 ? -1 : 1);
                double xInc = m * yInc;

                int y = line.first.y + yInc;
                double x = line.first.x + xInc;
                while (y != line.second.y)
                {
                    int ix = (int)(dx < 0 ? Math.Ceiling(x) : Math.Floor(x));
                    float fx = (float)Math.Abs(x - ix);
                    list.Add(new Point2D(ix, y, 1 - fx));
                    list.Add(new Point2D(ix + (dx < 0 ? -1 : 1), y, fx));

                    y += yInc;
                    x += xInc;
                }
            }

            // Add end Point and return list
            list.Add(line.second);
            return list;
        }
        protected override List<Point2D> GeneratePoint2DList(Ellipse ellipse)
        {
            throw new NotImplementedException();
        }
        protected override List<Point2D> GeneratePoint2DList(Circle circle)
        {
            throw new NotImplementedException();
        }
        protected override List<Point2D> GeneratePoint2DList(Parabola para)
        {
            throw new NotImplementedException();
        }
        protected override List<Point2D> GeneratePoint2DList(Hyperbola hyper)
        {
            throw new NotImplementedException();
        }
    }
}
