using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Honours_Stage_Project.Helpers
{
    public static class LineHelpers
    {
        public static double GetAngle(double x1, double y1, double x2, double y2)
        {
            return Math.Atan2(y2 - y1, x2 - x1);
        }
        public static (double x, double y) GetPointAtDistance(double x1, double y1, double angle, double distance)
        {
            return (x1 + distance * Math.Cos(angle), y1 + distance * Math.Sin(angle));
        }

        public static List<Line> DrawAngularConnection(double x1, double y1, double x2, double y2)
        {
            List<Line> backSegments = new List<Line>();
            List<Line> frontSegments = new List<Line>();

            double x3 = x1 + (x2 - x1) * 0.5;

            if (x1 > x2)
            {
                var (backLine, frontLine) = GetLines(x1, y1, x1 + 20, y1, true);
                backSegments.Add(backLine);
                frontSegments.Add(frontLine);

                double highestY = Math.Min(y1, y2);

                (backLine, frontLine) = GetLines(x1 + 20, y1, x1 + 20, highestY - 20, true);
                backSegments.Add(backLine);
                frontSegments.Add(frontLine);

                (backLine, frontLine) = GetLines(x1 + 20, highestY - 20, x2 - 20, highestY - 20, true);
                backSegments.Add(backLine);
                frontSegments.Add(frontLine);

                (backLine, frontLine) = GetLines(x2 - 20, highestY - 20, x2 - 20, y2, true);
                backSegments.Add(backLine);
                frontSegments.Add(frontLine);

                (backLine, frontLine) = GetLines(x2 - 20, y2, x2, y2, true);
                backSegments.Add(backLine);
                frontSegments.Add(frontLine);

            }
            else
            {
                var (backLine, frontLine) = GetLines(x1, y1, x3, y1);
                backSegments.Add(backLine);
                frontSegments.Add(frontLine);

                (backLine, frontLine) = GetLines(x3, y1, x3, y2);
                backSegments.Add(backLine);
                frontSegments.Add(frontLine);

                (backLine, frontLine) = GetLines(x3, y2, x2, y2);
                backSegments.Add(backLine);
                frontSegments.Add(frontLine);
            }

            List<Line> allSegments = new List<Line>();
            allSegments.AddRange(backSegments);
            allSegments.AddRange(frontSegments);

            return allSegments;
        }

        private static (Line, Line) GetLines(double x1, double y1, double x2, double y2, bool backwards = false)
        {
            Brush inStrokeColor = Brushes.LightYellow;
            Brush outStrokeColor = Brushes.Orange;
            if (backwards)
            {
                outStrokeColor = Brushes.LimeGreen;
                inStrokeColor = Brushes.LightBlue;
            }
            var backLine = new Line
            {
                Stroke = outStrokeColor,
                StrokeThickness = 5,
                X1 = x1,
                Y1 = y1,
                X2 = x2,
                Y2 = y2,
            };
            var frontLine = new Line
            {
                Stroke = inStrokeColor,
                StrokeThickness = 2,
                X1 = x1,
                Y1 = y1,
                X2 = x2,
                Y2 = y2,
            };

            return (backLine, frontLine);
        }
    }
}
