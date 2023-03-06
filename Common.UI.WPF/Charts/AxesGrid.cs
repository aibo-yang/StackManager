using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace Common.UI.WPF.Charts
{
    //internal class AxesGrid : Panel
    //{
    //    private Axis m_yAxis;

    //    private Axis m_xAxis;

    //    private ElementPanel m_elementPanel;

    //    private Rect m_layoutBounds = Rect.Empty;

    //    private DataTemplate m_gridTemplate;

    //    private ChartPrimitive m_background;

    //    internal ElementPanel ElementPanel
    //    {
    //        get
    //        {
    //            return this.m_elementPanel;
    //        }
    //        set
    //        {
    //            this.m_elementPanel = value;
    //        }
    //    }

    //    public DataTemplate GridTemplate
    //    {
    //        get
    //        {
    //            return this.m_gridTemplate;
    //        }
    //        set
    //        {
    //            if (this.m_gridTemplate != value)
    //            {
    //                this.m_gridTemplate = value;
    //            }
    //        }
    //    }

    //    public Rect LayoutBounds
    //    {
    //        get
    //        {
    //            return this.m_layoutBounds;
    //        }
    //        internal set
    //        {
    //            if (this.m_layoutBounds != value)
    //            {
    //                this.m_layoutBounds = value;
    //            }
    //        }
    //    }

    //    public Axis XAxis
    //    {
    //        get
    //        {
    //            return this.m_xAxis;
    //        }
    //        set
    //        {
    //            if (this.m_xAxis != value)
    //            {
    //                this.UnsubscribeXAxis();
    //                this.m_xAxis = value;
    //                this.SubscribeXAxis();
    //            }
    //        }
    //    }

    //    public Axis YAxis
    //    {
    //        get
    //        {
    //            return this.m_yAxis;
    //        }
    //        set
    //        {
    //            if (this.m_yAxis != value)
    //            {
    //                this.UnsubscribeYAxis();
    //                this.m_yAxis = value;
    //                this.SubscribeYAxis();
    //            }
    //        }
    //    }

    //    public AxesGrid()
    //    {
    //        this.m_background = new ChartPrimitive();
    //        this.XAxis = new Axis(Orientation.Horizontal);
    //        this.YAxis = new Axis(Orientation.Vertical);
    //    }

    //    protected override Size ArrangeOverride(Size finalSize)
    //    {
    //        Rect seriesLayoutBounds = this.GetSeriesLayoutBounds();
    //        this.PerformBackgroundLayout(finalSize);
    //        this.PerformLayout();
    //        if (seriesLayoutBounds != this.GetSeriesLayoutBounds() && this.ElementPanel != null)
    //        {
    //            this.ElementPanel.InvalidateArrange();
    //        }
    //        return finalSize;
    //    }

    //    protected void CalculateLabelSpace()
    //    {
    //        bool flag;
    //        bool flag1;
    //        Rect layoutBounds = this.LayoutBounds;
    //        int num = 0;
    //        int num1 = 0;
    //        int num2 = 0;
    //        int num3 = 0;
    //        Rect rect = layoutBounds;
    //        Rect rect1 = layoutBounds;
    //        this.XAxis.PerformGraduation();
    //        this.YAxis.PerformGraduation();
    //        if (!this.YAxis.Reversed || !this.XAxis.IntersectMinValue)
    //        {
    //            flag = (this.YAxis.Reversed ? false : !this.XAxis.IntersectMinValue);
    //        }
    //        else
    //        {
    //            flag = true;
    //        }
    //        bool flag2 = flag;
    //        this.XAxis.SetTitlesDirection(this.YAxis.Range, flag2);
    //        if (!this.XAxis.Reversed || !this.YAxis.IntersectMinValue)
    //        {
    //            flag1 = (this.XAxis.Reversed ? false : !this.YAxis.IntersectMinValue);
    //        }
    //        else
    //        {
    //            flag1 = true;
    //        }
    //        flag2 = flag1;
    //        this.YAxis.SetTitlesDirection(this.XAxis.Range, flag2);
    //        for (int i = 0; i < 10; i++)
    //        {
    //            this.XAxis.LayoutAxis(rect1);
    //            this.YAxis.LayoutAxis(rect);
    //            Axis xAxis = this.XAxis;
    //            Point location = this.XAxis.Location;
    //            xAxis.Location = new Point(location.X, this.YAxis.GetRealPoint(this.YAxis.Range.Start));
    //            Axis yAxis = this.YAxis;
    //            double realPoint = this.XAxis.GetRealPoint(this.XAxis.Range.Start);
    //            Point point = this.YAxis.Location;
    //            yAxis.Location = new Point(realPoint, point.Y);
    //            num2 = num;
    //            num3 = num1;
    //            num = this.m_xAxis.PerformLabelsLayout(layoutBounds, out rect);
    //            num1 = this.m_yAxis.PerformLabelsLayout(layoutBounds, out rect1);
    //            rect.Intersect(layoutBounds);
    //            rect1.Intersect(layoutBounds);
    //            if (num2 == num && num3 == num1)
    //            {
    //                return;
    //            }
    //        }
    //    }

    //    public Point GetActualPoint(Point point)
    //    {
    //        Point point1 = new Point()
    //        {
    //            X = this.XAxis.GetRealPoint(point.X),
    //            Y = this.YAxis.GetRealPoint(point.Y)
    //        };
    //        return point1;
    //    }

    //    public Point GetActualPoint(DataPoint point)
    //    {
    //        Point point1 = new Point()
    //        {
    //            X = this.XAxis.GetRealPoint(point.X),
    //            Y = this.YAxis.GetRealPoint(point.Y)
    //        };
    //        return point1;
    //    }

    //    public Rect GetSeriesLayoutBounds()
    //    {
    //        double start = 0;
    //        double end = 0;
    //        double num = 0;
    //        double end1 = 0;
    //        Point point = new Point();
    //        Point point1 = new Point();
    //        Point actualPoint = new Point();
    //        Point actualPoint1 = new Point();
    //        Rect empty = Rect.Empty;
    //        DataRange range = this.XAxis.Range;
    //        DataRange dataRange = this.YAxis.Range;
    //        if (range.Length != 0 && dataRange.Length != 0)
    //        {
    //            if (!this.XAxis.Reversed)
    //            {
    //                start = range.Start;
    //                num = range.End;
    //            }
    //            else
    //            {
    //                start = range.End;
    //                num = range.Start;
    //            }
    //            if (!this.YAxis.Reversed)
    //            {
    //                end = dataRange.Start;
    //                end1 = dataRange.End;
    //            }
    //            else
    //            {
    //                end = dataRange.End;
    //                end1 = dataRange.Start;
    //            }
    //            point = new Point(start, end);
    //            point1 = new Point(num, end1);
    //            actualPoint = this.GetActualPoint(point);
    //            actualPoint1 = this.GetActualPoint(point1);
    //            empty = new Rect(actualPoint, actualPoint1);
    //        }
    //        return empty;
    //    }

    //    protected virtual void PerformBackgroundLayout(Size finalSize)
    //    {
    //        object info;
    //        this.m_background.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
    //        this.m_background.ContentTemplate = this.m_gridTemplate;
    //        ChartPrimitive mBackground = this.m_background;
    //        if (this.m_gridTemplate == null)
    //        {
    //            info = null;
    //        }
    //        else
    //        {
    //            info = this.m_background.Info;
    //        }
    //        mBackground.Content = info;
    //        this.m_background.Info.W = base.Width;
    //        this.m_background.Info.H = base.Height;
    //    }

    //    protected virtual void PerformLayout()
    //    {
    //        this.CalculateLabelSpace();
    //        this.XAxis.PerformLayout();
    //        this.YAxis.PerformLayout();
    //    }

    //    public void PrepareLayout(DataPointsList<DataPoint> dataPoints)
    //    {
    //        base.Children.Clear();
    //        base.Children.Add(this.m_background);
    //        Panel.SetZIndex(this.m_background, -1);
    //        this.XAxis.Content = this.XAxis.Info;
    //        this.YAxis.Content = this.YAxis.Info;
    //        this.XAxis.PrepareLayout(dataPoints);
    //        this.YAxis.PrepareLayout(dataPoints);
    //    }

    //    public void Reset()
    //    {
    //        this.XAxis.Reset();
    //        this.YAxis.Reset();
    //        base.Children.Clear();
    //    }

    //    protected virtual void SubscribeXAxis()
    //    {
    //        if (this.m_xAxis != null)
    //        {
    //            this.m_xAxis.AxesGrid = this;
    //            this.m_xAxis.Orientation = Orientation.Horizontal;
    //        }
    //    }

    //    protected virtual void SubscribeYAxis()
    //    {
    //        if (this.m_yAxis != null)
    //        {
    //            this.m_yAxis.AxesGrid = this;
    //            this.m_yAxis.Orientation = Orientation.Vertical;
    //        }
    //    }

    //    protected virtual void UnsubscribeXAxis()
    //    {
    //        if (this.m_xAxis != null)
    //        {
    //            this.m_xAxis.AxesGrid = null;
    //        }
    //    }

    //    protected virtual void UnsubscribeYAxis()
    //    {
    //        if (this.m_yAxis != null)
    //        {
    //            this.m_yAxis.AxesGrid = null;
    //        }
    //    }
    //}
}
