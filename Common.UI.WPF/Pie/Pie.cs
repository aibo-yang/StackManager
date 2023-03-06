using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Media;
using Common.UI.WPF.Core;
using Common.UI.WPF.Core.Utilities;
using Common.UI.WPF.Primitives;

namespace Common.UI.WPF
{
    public sealed class Pie : ShapeBase
    {
        static Pie()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Pie), new FrameworkPropertyMetadata(typeof(Pie)));
            Pie.StretchProperty.OverrideMetadata(typeof(Pie), new FrameworkPropertyMetadata(Stretch.Fill));
            Pie.StrokeLineJoinProperty.OverrideMetadata(typeof(Pie), new FrameworkPropertyMetadata(PenLineJoin.Round));
        }

        public Pie() : base()
        {
        }

        private enum CacheBits
        {
            IsUpdatingEndAngle = 0x00000001,
            IsUpdatingMode = 0x00000002,
            IsUpdatingSlice = 0x00000004,
            IsUpdatingStartAngle = 0x00000008,
            IsUpdatingSweepDirection = 0x00000010,
        }

        private Rect rect = Rect.Empty;
        private BitVector32 cacheBits = new BitVector32(0);

        private bool IsUpdatingEndAngle
        {
            get
            {
                return cacheBits[(int)CacheBits.IsUpdatingEndAngle];
            }
            set
            {
                cacheBits[(int)CacheBits.IsUpdatingEndAngle] = value;
            }
        }

        private bool IsUpdatingMode
        {
            get
            {
                return cacheBits[(int)CacheBits.IsUpdatingMode];
            }
            set
            {
                cacheBits[(int)CacheBits.IsUpdatingMode] = value;
            }
        }

        private bool IsUpdatingSlice
        {
            get
            {
                return cacheBits[(int)CacheBits.IsUpdatingSlice];
            }
            set
            {
                cacheBits[(int)CacheBits.IsUpdatingSlice] = value;
            }
        }

        private bool IsUpdatingStartAngle
        {
            get
            {
                return cacheBits[(int)CacheBits.IsUpdatingStartAngle];
            }
            set
            {
                cacheBits[(int)CacheBits.IsUpdatingStartAngle] = value;
            }
        }

        private bool IsUpdatingSweepDirection
        {
            get
            {
                return cacheBits[(int)CacheBits.IsUpdatingSweepDirection];
            }
            set
            {
                cacheBits[(int)CacheBits.IsUpdatingSweepDirection] = value;
            }
        }

        #region StartAngle

        public static readonly DependencyProperty StartAngleProperty =
          DependencyProperty.Register("StartAngle", typeof(double), typeof(Pie),
            new FrameworkPropertyMetadata(360d, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
              new PropertyChangedCallback(Pie.OnStartAngleChanged)));

        public double StartAngle
        {
            get
            {
                return (double)this.GetValue(Pie.StartAngleProperty);
            }
            set
            {
                this.SetValue(Pie.StartAngleProperty, value);
            }
        }

        private static void OnStartAngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Pie)d).OnStartAngleChanged(e);
        }

        private void OnStartAngleChanged(DependencyPropertyChangedEventArgs e)
        {
            // avoid re-entrancy
            if (this.IsUpdatingStartAngle)
                return;

            // StartAngle, Slice, and SweepDirection are interrelated and must be kept in sync
            this.IsUpdatingStartAngle = true;
            try
            {
                switch (Mode)
                {
                    case PieMode.Manual:
                        this.CoerceValue(Pie.SliceProperty);
                        break;

                    case PieMode.EndAngle:
                        this.CoerceValue(Pie.SweepDirectionProperty);
                        this.CoerceValue(Pie.SliceProperty);
                        break;

                    case PieMode.Slice:
                        this.CoerceValue(Pie.EndAngleProperty);
                        break;
                }
            }
            finally
            {
                this.IsUpdatingStartAngle = false;
            }
        }

        #endregion

        #region EndAngle
        public static readonly DependencyProperty EndAngleProperty =
            DependencyProperty.Register("EndAngle", typeof(double), typeof(Pie),
                new FrameworkPropertyMetadata(360d, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    new PropertyChangedCallback(Pie.OnEndAngleChanged), new CoerceValueCallback(Pie.CoerceEndAngleValue)));

        public double EndAngle
        {
            get
            {
                return (double)this.GetValue(Pie.EndAngleProperty);
            }
            set
            {
                this.SetValue(Pie.EndAngleProperty, value);
            }
        }

        private static void OnEndAngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Pie)d).OnEndAngleChanged(e);
        }

        private void OnEndAngleChanged(DependencyPropertyChangedEventArgs e)
        {
            // avoid re-entrancy
            if (this.IsUpdatingEndAngle)
                return;

            if (!(this.IsUpdatingStartAngle || this.IsUpdatingSlice || this.IsUpdatingSweepDirection))
            {
                switch (this.Mode)
                {
                    case PieMode.Slice:
                        throw new InvalidOperationException(ErrorMessages.GetMessage("EndAngleCannotBeSetDirectlyInSlice"));
                }
            }

            // EndAngle, Slice, and SweepDirection are interrelated and must be kept in sync
            this.IsUpdatingEndAngle = true;
            try
            {
                if (this.Mode == PieMode.EndAngle)
                {
                    this.CoerceValue(Pie.SweepDirectionProperty);
                }
                this.CoerceValue(Pie.SliceProperty);
            }
            finally
            {
                this.IsUpdatingEndAngle = false;
            }
        }

        private static object CoerceEndAngleValue(DependencyObject d, object value)
        {
            // keep EndAngle in sync with Slice and SweepDirection
            Pie pie = (Pie)d;

            if (pie.IsUpdatingSlice || pie.IsUpdatingSweepDirection || (pie.IsUpdatingStartAngle && pie.Mode == PieMode.Slice))
            {
                double newValue = pie.StartAngle + ((pie.SweepDirection == SweepDirection.Clockwise) ? 1.0 : -1.0) * pie.Slice * 360;
                if (!DoubleHelper.AreVirtuallyEqual((double)value, newValue))
                {
                    value = newValue;
                }
            }
            return value;
        }
        #endregion

        #region Mode
        public static readonly DependencyProperty ModeProperty =
            DependencyProperty.Register("Mode", typeof(PieMode), typeof(Pie),
                new FrameworkPropertyMetadata(PieMode.Manual, new PropertyChangedCallback(Pie.OnModeChanged)));

        public PieMode Mode
        {
            get
            {
                return (PieMode)this.GetValue(Pie.ModeProperty);
            }
            set
            {
                this.SetValue(Pie.ModeProperty, value);
            }
        }

        private static void OnModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Pie)d).OnModeChanged(e);
        }

        private void OnModeChanged(DependencyPropertyChangedEventArgs e)
        {
            // disallow reentrancy
            if (this.IsUpdatingMode)
                return;

            this.IsUpdatingMode = true;
            try
            {
                if (this.Mode == PieMode.EndAngle)
                {
                    this.CoerceValue(Pie.SweepDirectionProperty);
                }
            }
            finally
            {
                this.IsUpdatingMode = false;
            }
        }
        #endregion

        #region Slice
        public static readonly DependencyProperty SliceProperty =
            DependencyProperty.Register("Slice", typeof(double), typeof(Pie),
                new FrameworkPropertyMetadata(1d, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    new PropertyChangedCallback(Pie.OnSliceChanged), new CoerceValueCallback(Pie.CoerceSliceValue)), new ValidateValueCallback(Pie.ValidateSlice));

        public double Slice
        {
            get
            {
                return (double)this.GetValue(Pie.SliceProperty);
            }
            set
            {
                this.SetValue(Pie.SliceProperty, value);
            }
        }

        private static void OnSliceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Pie)d).OnSliceChanged(e);
        }

        private void OnSliceChanged(DependencyPropertyChangedEventArgs e)
        {
            // avoid re-entrancy
            if (this.IsUpdatingSlice)
            {
                return;
            }

            if (!(this.IsUpdatingStartAngle || this.IsUpdatingEndAngle || this.IsUpdatingSweepDirection))
            {
                if (this.Mode == PieMode.EndAngle)
                { 
                    throw new InvalidOperationException(ErrorMessages.GetMessage("SliceCannotBeSetDirectlyInEndAngle"));
                }
            }

            // EndAngle and Slice are interrelated and must be kept in sync
            this.IsUpdatingSlice = true;
            try
            {
                if (!(this.IsUpdatingStartAngle || this.IsUpdatingEndAngle || (this.Mode == PieMode.Manual && this.IsUpdatingSweepDirection)))
                {
                    this.CoerceValue(Pie.EndAngleProperty);
                }
            }
            finally
            {
                this.IsUpdatingSlice = false;
            }
        }

        private static object CoerceSliceValue(DependencyObject d, object value)
        {
            // keep Slice in sync with EndAngle, StartAngle, and SweepDirection
            Pie pie = (Pie)d;
            if (pie.IsUpdatingEndAngle || pie.IsUpdatingStartAngle || pie.IsUpdatingSweepDirection)
            {

                double slice = Math.Max(-360.0, Math.Min(360.0, (pie.EndAngle - pie.StartAngle))) / ((pie.SweepDirection == SweepDirection.Clockwise) ? 360.0 : -360.0);
                double newValue = DoubleHelper.AreVirtuallyEqual(slice, 0) ? 0 : (slice < 0) ? slice + 1 : slice;
                if (!DoubleHelper.AreVirtuallyEqual((double)value, newValue))
                {
                    value = newValue;
                }
            }
            return value;
        }

        private static bool ValidateSlice(object value)
        {
            double newValue = (double)value;
            if (newValue < 0 || newValue > 1 || DoubleHelper.IsNaN(newValue))
                throw new ArgumentException(ErrorMessages.GetMessage("SliceOOR"));

            return true;
        }
        #endregion

        #region SweepDirection

        public static readonly DependencyProperty SweepDirectionProperty =
          DependencyProperty.Register("SweepDirection", typeof(SweepDirection), typeof(Pie),
            new FrameworkPropertyMetadata((SweepDirection)SweepDirection.Clockwise, FrameworkPropertyMetadataOptions.AffectsRender,
              new PropertyChangedCallback(Pie.OnSweepDirectionChanged), new CoerceValueCallback(Pie.CoerceSweepDirectionValue)));

        public SweepDirection SweepDirection
        {
            get
            {
                return (SweepDirection)this.GetValue(Pie.SweepDirectionProperty);
            }
            set
            {
                this.SetValue(Pie.SweepDirectionProperty, value);
            }
        }

        private static void OnSweepDirectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Pie)d).OnSweepDirectionChanged(e);
        }

        private void OnSweepDirectionChanged(DependencyPropertyChangedEventArgs e)
        {
            // avoid re-entrancy
            if (this.IsUpdatingSweepDirection)
                return;

            // EndAngle, Slice, and SweepDirection are interrelated and must be kept in sync
            this.IsUpdatingSweepDirection = true;
            try
            {
                switch (Mode)
                {
                    case PieMode.Slice:
                        this.CoerceValue(Pie.EndAngleProperty);
                        break;

                    default:
                        this.CoerceValue(Pie.SliceProperty);
                        break;
                }
            }
            finally
            {
                this.IsUpdatingSweepDirection = false;
            }
        }

        private static object CoerceSweepDirectionValue(DependencyObject d, object value)
        {
            // keep SweepDirection in sync with EndAngle and StartAngle
            Pie pie = (Pie)d;
            if (pie.IsUpdatingEndAngle || pie.IsUpdatingStartAngle || pie.IsUpdatingMode)
            {
                if (DoubleHelper.AreVirtuallyEqual(pie.StartAngle, pie.EndAngle))
                {
                    // if the values are equal, use previously coerced value
                    value = pie.SweepDirection;
                }
                else
                {
                    value = (pie.EndAngle < pie.StartAngle) ? SweepDirection.Counterclockwise : SweepDirection.Clockwise;
                }
            }
            return value;
        }

        #endregion

        #region GeometryTransform

        public override Transform GeometryTransform
        {
            get
            {
                return Transform.Identity;
            }
        }

        #endregion

        #region RenderedGeometry
        public override Geometry RenderedGeometry
        {
            get
            {
                // for a Pie, the RenderedGeometry is the same as the DefiningGeometry
                return this.DefiningGeometry;
            }
        }

        #endregion

        protected override Geometry DefiningGeometry
        {
            get
            {
                double slice = Slice;
                if (rect.IsEmpty || slice <= 0)
                {
                    return Geometry.Empty;
                }

                if (slice >= 1)
                {
                    return new EllipseGeometry(rect);
                }

                double directionalFactor = (this.SweepDirection == SweepDirection.Clockwise) ? 1.0 : -1.0;
                double startAngle = StartAngle;

                Point pointA = EllipseHelper.PointOfRadialIntersection(rect, startAngle);
                Point pointB = EllipseHelper.PointOfRadialIntersection(rect, startAngle + directionalFactor * slice * 360);
                
                var segments = new PathSegmentCollection
                {
                    new LineSegment(pointA, true)
                };

                var arc = new ArcSegment
                {
                    Point = pointB,
                    Size = new Size(rect.Width / 2, rect.Height / 2),
                    IsLargeArc = slice > 0.5,
                    SweepDirection = SweepDirection
                };

                segments.Add(arc);

                var figures = new PathFigureCollection
                {
                    new PathFigure(RectHelper.Center(rect), segments, true)
                };

                return new PathGeometry(figures);
            }
        }

        internal override Size GetNaturalSize()
        {
            double strokeThickness = this.GetStrokeThickness();
            return new Size(strokeThickness, strokeThickness);
        }

        internal override Rect GetDefiningGeometryBounds()
        {
            return rect;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double penThickness = this.GetStrokeThickness();
            double margin = penThickness / 2;

            rect = new Rect(margin, margin,
                Math.Max(0, finalSize.Width - penThickness),
                Math.Max(0, finalSize.Height - penThickness));

            switch (Stretch)
            {
                case Stretch.None:
                    // empty rectangle
                    rect.Width = rect.Height = 0;
                    break;

                case Stretch.Fill:
                    // already initialized for Fill
                    break;

                case Stretch.Uniform:
                    // largest square that fits in the final size
                    if (rect.Width > rect.Height)
                    {
                        rect.Width = rect.Height;
                    }
                    else
                    {
                        rect.Height = rect.Width;
                    }
                    break;

                case Stretch.UniformToFill:
                    // smallest square that fills the final size
                    if (rect.Width < rect.Height)
                    {
                        rect.Width = rect.Height;
                    }
                    else
                    {
                        rect.Height = rect.Width;
                    }
                    break;
            }

            return finalSize;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            if (this.Stretch == Stretch.UniformToFill)
            {
                double width = constraint.Width;
                double height = constraint.Height;

                if (Double.IsInfinity(width) && Double.IsInfinity(height))
                {
                    return this.GetNaturalSize();
                }
                else if (Double.IsInfinity(width) || Double.IsInfinity(height))
                {
                    width = Math.Min(width, height);
                }
                else
                {
                    width = Math.Max(width, height);
                }

                return new Size(width, width);
            }

            return this.GetNaturalSize();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            if (!rect.IsEmpty)
            {
                Pen pen = this.GetPen();
                drawingContext.DrawGeometry(this.Fill, pen, this.RenderedGeometry);
            }
        }
    }
}
