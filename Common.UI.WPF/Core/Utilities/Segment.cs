using System;
using System.Windows;

namespace Common.UI.WPF.Core.Utilities
{
    internal struct Segment
    {
        #region Constructors

        public Segment(Point point)
        {
            p1 = point;
            p2 = point;
            isP1Excluded = false;
            isP2Excluded = false;
        }

        public Segment(Point p1, Point p2)
        {
            this.p1 = p1;
            this.p2 = p2;
            isP1Excluded = false;
            isP2Excluded = false;
        }

        public Segment(Point p1, Point p2, bool excludeP1, bool excludeP2)
        {
            this.p1 = p1;
            this.p2 = p2;
            isP1Excluded = excludeP1;
            isP2Excluded = excludeP2;
        }

        #endregion

        #region Empty Static Properties

        public static Segment Empty
        {
            get
            {
                Segment result = new Segment(new Point(0, 0));
                result.isP1Excluded = true;
                result.isP2Excluded = true;
                return result;
            }
        }

        #endregion

        #region P1 Property

        public Point P1
        {
            get
            {
                return p1;
            }
        }

        #endregion

        #region P2 Property

        public Point P2
        {
            get
            {
                return p2;
            }
        }

        #endregion

        #region IsP1Excluded Property

        public bool IsP1Excluded
        {
            get
            {
                return isP1Excluded;
            }
        }

        #endregion

        #region IsP2Excluded Property

        public bool IsP2Excluded
        {
            get
            {
                return isP2Excluded;
            }
        }

        #endregion

        #region IsEmpty Property

        public bool IsEmpty
        {
            get
            {
                return DoubleHelper.AreVirtuallyEqual(p1, p2) && (isP1Excluded || isP2Excluded);
            }
        }

        #endregion

        #region IsPoint Property

        public bool IsPoint
        {
            get
            {
                return DoubleHelper.AreVirtuallyEqual(p1, p2);
            }
        }

        #endregion

        #region Length Property

        public double Length
        {
            get
            {
                return (this.P2 - this.P1).Length;
            }
        }

        #endregion

        #region Slope Property

        public double Slope
        {
            get
            {
                return (this.P2.X == this.P1.X) ? double.NaN : (this.P2.Y - this.P1.Y) / (this.P2.X - this.P1.X);
            }
        }

        #endregion

        public bool Contains(Point point)
        {
            if (IsEmpty)
                return false;

            // if the point is an endpoint, ensure that it is not excluded
            if (DoubleHelper.AreVirtuallyEqual(p1, point))
                return isP1Excluded;

            if (DoubleHelper.AreVirtuallyEqual(p2, point))
                return isP2Excluded;

            bool result = false;

            // ensure that a line through P1 and the point is parallel to the current segment
            if (DoubleHelper.AreVirtuallyEqual(Slope, new Segment(p1, point).Slope))
            {
                // finally, ensure that the point is between the segment's endpoints
                result = (point.X >= Math.Min(p1.X, p2.X))
                      && (point.X <= Math.Max(p1.X, p2.X))
                      && (point.Y >= Math.Min(p1.Y, p2.Y))
                      && (point.Y <= Math.Max(p1.Y, p2.Y));
            }
            return result;
        }

        public bool Contains(Segment segment)
        {
            return (segment == this.Intersection(segment));
        }

        public override bool Equals(object o)
        {
            if (!(o is Segment))
                return false;

            Segment other = (Segment)o;

            // empty segments are always considered equal
            if (this.IsEmpty)
                return other.IsEmpty;

            // segments are considered equal if
            //    1) the endpoints are equal and equally excluded
            //    2) the opposing endpoints are equal and equally excluded
            if (DoubleHelper.AreVirtuallyEqual(p1, other.p1))
            {
                return (DoubleHelper.AreVirtuallyEqual(p2, other.p2)
                     && isP1Excluded == other.isP1Excluded
                     && isP2Excluded == other.isP2Excluded);
            }
            else
            {
                return (DoubleHelper.AreVirtuallyEqual(p1, other.p2)
                     && DoubleHelper.AreVirtuallyEqual(p2, other.p1)
                     && isP1Excluded == other.isP2Excluded
                     && isP2Excluded == other.isP1Excluded);
            }
        }

        public override int GetHashCode()
        {
            return p1.GetHashCode() ^ p2.GetHashCode() ^ isP1Excluded.GetHashCode() ^ isP2Excluded.GetHashCode();
        }

        public Segment Intersection(Segment segment)
        {
            // if either segment is empty, the intersection is also empty
            if (this.IsEmpty || segment.IsEmpty)
                return Segment.Empty;

            // if the segments are equal, just return a new equal segment
            if (this == segment)
                return new Segment(this.p1, this.p2, this.isP1Excluded, this.isP2Excluded);

            // if either segment is a Point, just see if the point is contained in the other segment
            if (this.IsPoint)
                return segment.Contains(this.p1) ? new Segment(this.p1) : Segment.Empty;

            if (segment.IsPoint)
                return this.Contains(segment.p1) ? new Segment(segment.p1) : Segment.Empty;

            // okay, no easy answer, so let's do the math...
            Point p1 = this.p1;
            Vector v1 = this.p2 - this.p1;
            Point p2 = segment.p1;
            Vector v2 = segment.p2 - segment.p1;
            Vector endpointVector = p2 - p1;

            double xProd = Vector.CrossProduct(v1, v2);

            // if segments are not parallel, then look for intersection on each segment
            if (!DoubleHelper.AreVirtuallyEqual(Slope, segment.Slope))
            {
                // check for intersection on other segment
                double s = (Vector.CrossProduct(endpointVector, v1)) / xProd;
                if (s < 0 || s > 1)
                    return Segment.Empty;

                // check for intersection on this segment
                s = (Vector.CrossProduct(endpointVector, v2)) / xProd;
                if (s < 0 || s > 1)
                    return Segment.Empty;

                // intersection of segments is a point
                return new Segment(p1 + s * v1);
            }

            // segments are parallel
            xProd = Vector.CrossProduct(endpointVector, v1);
            if (xProd * xProd > 1.0e-06 * v1.LengthSquared * endpointVector.LengthSquared)
            {
                // segments do not intersect
                return Segment.Empty;
            }

            // intersection is overlapping segment
            Segment result = new Segment();

            // to determine the overlapping segment, create reference segments where the endpoints are *not* excluded
            Segment refThis = new Segment(this.p1, this.p2);
            Segment refSegment = new Segment(segment.p1, segment.p2);

            // check whether this segment is contained in the other segment
            bool includeThisP1 = refSegment.Contains(refThis.p1);
            bool includeThisP2 = refSegment.Contains(refThis.p2);
            if (includeThisP1 && includeThisP2)
            {
                result.p1 = this.p1;
                result.p2 = this.p2;
                result.isP1Excluded = this.isP1Excluded || !segment.Contains(this.p1);
                result.isP2Excluded = this.isP2Excluded || !segment.Contains(this.p2);
                return result;
            }

            // check whether the other segment is contained in this segment
            bool includeSegmentP1 = refThis.Contains(refSegment.p1);
            bool includeSegmentP2 = refThis.Contains(refSegment.p2);
            if (includeSegmentP1 && includeSegmentP2)
            {
                result.p1 = segment.p1;
                result.p2 = segment.p2;
                result.isP1Excluded = segment.isP1Excluded || !this.Contains(segment.p1);
                result.isP2Excluded = segment.isP2Excluded || !this.Contains(segment.p2);
                return result;
            }

            // the intersection must include one endpoint from this segment and one endpoint from the other segment
            if (includeThisP1)
            {
                result.p1 = this.p1;
                result.isP1Excluded = this.isP1Excluded || !segment.Contains(this.p1);
            }
            else
            {
                result.p1 = this.p2;
                result.isP1Excluded = this.isP2Excluded || !segment.Contains(this.p2);
            }
            if (includeSegmentP1)
            {
                result.p2 = segment.p1;
                result.isP2Excluded = segment.isP1Excluded || !this.Contains(segment.p1);
            }
            else
            {
                result.p2 = segment.p2;
                result.isP2Excluded = segment.isP2Excluded || !this.Contains(segment.p2);
            }
            return result;
        }

        public override string ToString()
        {
            string s = base.ToString();

            if (this.IsEmpty)
            {
                s = s + ": {Empty}";
            }
            else if (this.IsPoint)
            {
                s = s + ", Point: " + p1.ToString();
            }
            else
            {
                s = s + ": " + p1.ToString() + (isP1Excluded ? " (excl)" : " (incl)")
                    + " to " + p2.ToString() + (isP2Excluded ? " (excl)" : " (incl)");
            }

            return s;
        }

        #region Operators Methods

        public static bool operator ==(Segment s1, Segment s2)
        {
            if ((object)s1 == null)
                return (object)s2 == null;

            if ((object)s2 == null)
                return (object)s1 == null;

            return s1.Equals(s2);
        }

        public static bool operator !=(Segment s1, Segment s2)
        {
            return !(s1 == s2);
        }

        #endregion

        #region Private Fields

        private bool isP1Excluded;
        private bool isP2Excluded;
        private Point p1;
        private Point p2;

        #endregion
    }
}
