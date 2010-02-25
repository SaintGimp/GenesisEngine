using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;

namespace GenesisEngine
{
    // Based on the XNA Framework BoundingBox

    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct DoubleBoundingBox : IEquatable<DoubleBoundingBox>
    {
        public const int CornerCount = 8;
        public DoubleVector3 Min;
        public DoubleVector3 Max;
        
        public DoubleVector3[] GetCorners()
        {
            return new[]
            {
                new DoubleVector3(Min.X, Max.Y, Max.Z),
                new DoubleVector3(Max.X, Max.Y, Max.Z),
                new DoubleVector3(Max.X, Min.Y, Max.Z),
                new DoubleVector3(Min.X, Min.Y, Max.Z),
                new DoubleVector3(Min.X, Max.Y, Min.Z),
                new DoubleVector3(Max.X, Max.Y, Min.Z),
                new DoubleVector3(Max.X, Min.Y, Min.Z),
                new DoubleVector3(Min.X, Min.Y, Min.Z)
            };
        }

        public void GetCorners(DoubleVector3[] corners)
        {
            if (corners == null)
            {
                throw new ArgumentNullException("corners");
            }
            if (corners.Length < 8)
            {
                throw new ArgumentOutOfRangeException("corners", "Not enough corners.");
            }

            corners[0].X = Min.X;
            corners[0].Y = Max.Y;
            corners[0].Z = Max.Z;
            corners[1].X = Max.X;
            corners[1].Y = Max.Y;
            corners[1].Z = Max.Z;
            corners[2].X = Max.X;
            corners[2].Y = Min.Y;
            corners[2].Z = Max.Z;
            corners[3].X = Min.X;
            corners[3].Y = Min.Y;
            corners[3].Z = Max.Z;
            corners[4].X = Min.X;
            corners[4].Y = Max.Y;
            corners[4].Z = Min.Z;
            corners[5].X = Max.X;
            corners[5].Y = Max.Y;
            corners[5].Z = Min.Z;
            corners[6].X = Max.X;
            corners[6].Y = Min.Y;
            corners[6].Z = Min.Z;
            corners[7].X = Min.X;
            corners[7].Y = Min.Y;
            corners[7].Z = Min.Z;
        }

        public DoubleBoundingBox(DoubleVector3 min, DoubleVector3 max)
        {
            Min = min;
            Max = max;
        }

        public bool Equals(DoubleBoundingBox other)
        {
            return ((Min == other.Min) && (Max == other.Max));
        }

        public override bool Equals(object obj)
        {
            bool isEqual = false;
            if (obj is DoubleBoundingBox)
            {
                isEqual = Equals((DoubleBoundingBox)obj);
            }

            return isEqual;
        }

        public override int GetHashCode()
        {
            return (Min.GetHashCode() + Max.GetHashCode());
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "{{Min:{0} Max:{1}}}", Min.ToString(), Max.ToString());
        }

        public static DoubleBoundingBox CreateMerged(DoubleBoundingBox original, DoubleBoundingBox additional)
        {
            DoubleBoundingBox box;
            box.Min = DoubleVector3.Min(original.Min, additional.Min);
            box.Max = DoubleVector3.Max(original.Max, additional.Max);

            return box;
        }

        public static void CreateMerged(ref DoubleBoundingBox original, ref DoubleBoundingBox additional, out DoubleBoundingBox result)
        {
            var min = DoubleVector3.Min(original.Min, additional.Min);
            var max = DoubleVector3.Max(original.Max, additional.Max);
            result.Min = min;
            result.Max = max;
        }

        public static DoubleBoundingBox CreateFromPoints(IEnumerable<DoubleVector3> points)
        {
            if (points == null)
            {
                throw new ArgumentNullException();
            }

            bool created = false;
            var maxPoint = new DoubleVector3(double.MaxValue);
            var minPoint = new DoubleVector3(double.MinValue);

            foreach (DoubleVector3 point in points)
            {
                DoubleVector3 current = point;
                maxPoint = DoubleVector3.Min(maxPoint, current);
                minPoint = DoubleVector3.Max(minPoint, current);
                created = true;
            }

            if (!created)
            {
                throw new ArgumentException("No points supplied.");
            }

            return new DoubleBoundingBox(maxPoint, minPoint);
        }

        public void AddPoint(DoubleVector3 point)
        {
            Max = DoubleVector3.Min(Max, point);
            Min = DoubleVector3.Max(Min, point);
        }

        public bool Intersects(DoubleBoundingBox box)
        {
            if ((Max.X < box.Min.X) || (Min.X > box.Max.X))
            {
                return false;
            }
            if ((Max.Y < box.Min.Y) || (Min.Y > box.Max.Y))
            {
                return false;
            }
            return ((Max.Z >= box.Min.Z) && (Min.Z <= box.Max.Z));
        }

        public void Intersects(ref DoubleBoundingBox box, out bool result)
        {
            result = false;
            if ((((Max.X >= box.Min.X) && (Min.X <= box.Max.X)) && ((Max.Y >= box.Min.Y) && (Min.Y <= box.Max.Y))) && ((Max.Z >= box.Min.Z) && (Min.Z <= box.Max.Z)))
            {
                result = true;
            }
        }

        public ContainmentType Contains(DoubleBoundingBox box)
        {
            if ((Max.X < box.Min.X) || (Min.X > box.Max.X))
            {
                return ContainmentType.Disjoint;
            }
            if ((Max.Y < box.Min.Y) || (Min.Y > box.Max.Y))
            {
                return ContainmentType.Disjoint;
            }
            if ((Max.Z < box.Min.Z) || (Min.Z > box.Max.Z))
            {
                return ContainmentType.Disjoint;
            }
            if ((((Min.X <= box.Min.X) && (box.Max.X <= Max.X)) && ((Min.Y <= box.Min.Y) && (box.Max.Y <= Max.Y))) && ((Min.Z <= box.Min.Z) && (box.Max.Z <= Max.Z)))
            {
                return ContainmentType.Contains;
            }
            return ContainmentType.Intersects;
        }

        public void Contains(ref DoubleBoundingBox box, out ContainmentType result)
        {
            result = ContainmentType.Disjoint;
            if ((((Max.X >= box.Min.X) && (Min.X <= box.Max.X)) && ((Max.Y >= box.Min.Y) && (Min.Y <= box.Max.Y))) && ((Max.Z >= box.Min.Z) && (Min.Z <= box.Max.Z)))
            {
                result = ((((Min.X <= box.Min.X) && (box.Max.X <= Max.X)) && ((Min.Y <= box.Min.Y) && (box.Max.Y <= Max.Y))) && ((Min.Z <= box.Min.Z) && (box.Max.Z <= Max.Z))) ? ContainmentType.Contains : ContainmentType.Intersects;
            }
        }

        public ContainmentType Contains(DoubleVector3 point)
        {
            if ((((Min.X <= point.X) && (point.X <= Max.X)) && ((Min.Y <= point.Y) && (point.Y <= Max.Y))) && ((Min.Z <= point.Z) && (point.Z <= Max.Z)))
            {
                return ContainmentType.Contains;
            }
            return ContainmentType.Disjoint;
        }

        public void Contains(ref DoubleVector3 point, out ContainmentType result)
        {
            result = ((((Min.X <= point.X) && (point.X <= Max.X)) && ((Min.Y <= point.Y) && (point.Y <= Max.Y))) && ((Min.Z <= point.Z) && (point.Z <= Max.Z))) ? ContainmentType.Contains : ContainmentType.Disjoint;
        }

        public static bool operator ==(DoubleBoundingBox a, DoubleBoundingBox b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(DoubleBoundingBox a, DoubleBoundingBox b)
        {
            return !a.Equals(b);
        }
    }
}
