using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using Common.UI.WPF.Core.Utilities;

namespace Common.UI.WPF.Primitives
{
    public abstract class ShapeBase : Shape
    {
        private Pen pen = null;

        internal bool IsPenEmptyOrUndefined
        {
            get
            {
                double strokeThickness = this.StrokeThickness;
                return (this.Stroke == null) || DoubleHelper.IsNaN(strokeThickness) || DoubleHelper.AreVirtuallyEqual(0, strokeThickness);
            }
        }

        protected abstract override Geometry DefiningGeometry
        {
            get;
        }

        static ShapeBase()
        {
            ShapeBase.StrokeDashArrayProperty.OverrideMetadata(typeof(ShapeBase), new FrameworkPropertyMetadata(new PropertyChangedCallback(ShapeBase.OnStrokeChanged)));
            ShapeBase.StrokeDashCapProperty.OverrideMetadata(typeof(ShapeBase), new FrameworkPropertyMetadata(new PropertyChangedCallback(ShapeBase.OnStrokeChanged)));
            ShapeBase.StrokeDashOffsetProperty.OverrideMetadata(typeof(ShapeBase), new FrameworkPropertyMetadata(new PropertyChangedCallback(ShapeBase.OnStrokeChanged)));
            ShapeBase.StrokeEndLineCapProperty.OverrideMetadata(typeof(ShapeBase), new FrameworkPropertyMetadata(new PropertyChangedCallback(ShapeBase.OnStrokeChanged)));
            ShapeBase.StrokeLineJoinProperty.OverrideMetadata(typeof(ShapeBase), new FrameworkPropertyMetadata(new PropertyChangedCallback(ShapeBase.OnStrokeChanged)));
            ShapeBase.StrokeMiterLimitProperty.OverrideMetadata(typeof(ShapeBase), new FrameworkPropertyMetadata(new PropertyChangedCallback(ShapeBase.OnStrokeChanged)));
            ShapeBase.StrokeProperty.OverrideMetadata(typeof(ShapeBase), new FrameworkPropertyMetadata(new PropertyChangedCallback(ShapeBase.OnStrokeChanged)));
            ShapeBase.StrokeStartLineCapProperty.OverrideMetadata(typeof(ShapeBase), new FrameworkPropertyMetadata(new PropertyChangedCallback(ShapeBase.OnStrokeChanged)));
            ShapeBase.StrokeThicknessProperty.OverrideMetadata(typeof(ShapeBase), new FrameworkPropertyMetadata(new PropertyChangedCallback(ShapeBase.OnStrokeChanged)));
        }

        private static void OnStrokeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ShapeBase)d).pen = null;
        }

        internal virtual Rect GetDefiningGeometryBounds()
        {
            Geometry geometry = this.DefiningGeometry;
            Debug.Assert(geometry != null);
            return geometry.Bounds;
        }

        internal virtual Size GetNaturalSize()
        {
            Geometry geometry = this.DefiningGeometry;
            Debug.Assert(geometry != null);
            Rect bounds = geometry.GetRenderBounds(GetPen());
            return new Size(Math.Max(bounds.Right, 0), Math.Max(bounds.Bottom, 0));
        }

        internal Pen GetPen()
        {
            if (this.IsPenEmptyOrUndefined)
                return null;

            if (pen == null)
            {
                pen = this.MakePen();
            }

            return pen;
        }

        internal double GetStrokeThickness()
        {
            if (this.IsPenEmptyOrUndefined)
                return 0d;

            return Math.Abs(this.StrokeThickness);
        }

        internal bool IsSizeEmptyOrUndefined(Size size)
        {
            return (DoubleHelper.IsNaN(size.Width) || DoubleHelper.IsNaN(size.Height) || size.IsEmpty);
        }

        private Pen MakePen()
        {
            var pen = new Pen
            {
                Brush = this.Stroke,
                DashCap = this.StrokeDashCap
            };

            if (this.StrokeDashArray != null || this.StrokeDashOffset != 0.0)
            {
                pen.DashStyle = new DashStyle(this.StrokeDashArray, this.StrokeDashOffset);
            }

            pen.EndLineCap = this.StrokeEndLineCap;
            pen.LineJoin = this.StrokeLineJoin;
            pen.MiterLimit = this.StrokeMiterLimit;
            pen.StartLineCap = this.StrokeStartLineCap;
            pen.Thickness = Math.Abs(this.StrokeThickness);

            return pen;
        }
    }
}
