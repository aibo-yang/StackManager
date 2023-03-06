using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows;

namespace Common.UI.WPF.Charts
{
    //public class Area : Panel
    //{
    //    private const string c_sAreaTag = "Area";

    //    private static int m_id;

    //    private bool m_bIsCreatingVisualChildren;

    //    private SeriesList<Series> m_series = new SeriesList<Series>();

    //    private AxesGrid m_axesGrid;

    //    private double m_xStep;

    //    private double m_yStep;

    //    private ElementPanel m_elementPanel;

    //    private DataPointsList<DataPoint> m_allPoints = new DataPointsList<DataPoint>();

    //    /// <summary>Identifies the Title dependency property.</summary>
    //    public readonly static DependencyProperty TitleProperty;

    //    /// <summary>Identifies the BackgroundTemplate dependency property.</summary>
    //    public readonly static DependencyProperty BackgroundTemplateProperty;

    //    internal AxesGrid AxesGrid
    //    {
    //        get
    //        {
    //            return this.m_axesGrid;
    //        }
    //    }

    //    /// <summary>Gets or sets the data template for grid representation.</summary>
    //    public DataTemplate BackgroundTemplate
    //    {
    //        get
    //        {
    //            return (DataTemplate)base.GetValue(Area.BackgroundTemplateProperty);
    //        }
    //        set
    //        {
    //            base.SetValue(Area.BackgroundTemplateProperty, value);
    //        }
    //    }

    //    /// <summary>Gets the layout bounds for the interior of a Series.</summary>
    //    public Rect LayoutBounds
    //    {
    //        get
    //        {
    //            if (this.m_axesGrid == null)
    //            {
    //                return Rect.Empty;
    //            }
    //            return this.m_axesGrid.LayoutBounds;
    //        }
    //    }

    //    /// <summary>Gets a list of Series instances contained in the <see cref="Xceed.Wpf.Toolkit~Xceed.Wpf.Toolkit.Chart.Area.html">Area</see>.</summary>
    //    public SeriesList<Series> Series
    //    {
    //        get
    //        {
    //            return this.m_series;
    //        }
    //    }

    //    /// <summary>Gets or sets the Area title.</summary>
    //    public string Title
    //    {
    //        get
    //        {
    //            return (string)base.GetValue(Area.TitleProperty);
    //        }
    //        set
    //        {
    //            base.SetValue(Area.TitleProperty, value);
    //        }
    //    }

    //    /// <summary>Gets or sets the x-axis of the Area.</summary>
    //    public Axis XAxis
    //    {
    //        get
    //        {
    //            return this.AxesGrid.XAxis;
    //        }
    //        set
    //        {
    //            if (this.AxesGrid.XAxis != value)
    //            {
    //                this.AxesGrid.XAxis.PropertyChanged -= new PropertyChangedEventHandler(this.Axis_PropertyChanged);
    //                this.AxesGrid.XAxis = value;
    //                this.AxesGrid.XAxis.PropertyChanged += new PropertyChangedEventHandler(this.Axis_PropertyChanged);
    //                this.Invalidate();
    //            }
    //        }
    //    }

    //    /// <summary>Gets or sets the step for the x-axis.</summary>
    //    protected internal double XStep
    //    {
    //        get
    //        {
    //            return this.m_xStep;
    //        }
    //        set
    //        {
    //            if (this.m_xStep != value)
    //            {
    //                this.m_xStep = value;
    //            }
    //            this.m_xStep = (this.m_xStep == 0 ? 1 : this.m_xStep);
    //        }
    //    }

    //    /// <summary>Gets or sets the y-axis of the Area.</summary>
    //    public Axis YAxis
    //    {
    //        get
    //        {
    //            return this.AxesGrid.YAxis;
    //        }
    //        set
    //        {
    //            if (this.AxesGrid.YAxis != value)
    //            {
    //                this.AxesGrid.YAxis.PropertyChanged -= new PropertyChangedEventHandler(this.Axis_PropertyChanged);
    //                this.AxesGrid.YAxis = value;
    //                this.AxesGrid.YAxis.PropertyChanged += new PropertyChangedEventHandler(this.Axis_PropertyChanged);
    //                this.Invalidate();
    //            }
    //        }
    //    }

    //    /// <summary>Gets or sets the step for the y-axis.</summary>
    //    protected internal double YStep
    //    {
    //        get
    //        {
    //            return this.m_yStep;
    //        }
    //        set
    //        {
    //            if (this.m_yStep != value)
    //            {
    //                this.m_yStep = value;
    //            }
    //        }
    //    }

    //    static Area()
    //    {
    //        Area.m_id = -1;
    //        Area.TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(Area), new UIPropertyMetadata("", new PropertyChangedCallback(Area.TitlePropertyChanged)));
    //        Area.BackgroundTemplateProperty = DependencyProperty.Register("BackgroundTemplate", typeof(DataTemplate), typeof(Area), new PropertyMetadata(null, new PropertyChangedCallback(Area.BackgroundTemplatePropertyChanged)));
    //        FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(Area), new FrameworkPropertyMetadata(typeof(Area)));
    //    }

    //    /// <summary>Initializes a new instance of the Area class.</summary>
    //    public Area()
    //    {
    //        this.m_axesGrid = new AxesGrid();
    //        this.m_elementPanel = new ElementPanel(this.m_axesGrid);
    //        this.m_axesGrid.ElementPanel = this.m_elementPanel;
    //        base.SetCurrentValue(Area.TitleProperty, Area.GenerateUniqueName());
    //        base.Loaded += new RoutedEventHandler(this.Area_Loaded);
    //        this.Series.ListChanged += new ListChangedEventHandler<Series>(this.Series_ListChanged);
    //    }

    //    private void Area_Loaded(object sender, RoutedEventArgs e)
    //    {
    //        this.CreateVisualChildren();
    //    }

    //    /// <summary>Arranges and sizes the content of an Area object.</summary>
    //    /// <returns>The size of the content.</returns>
    //    /// <param name="arrangeSize">The computed size that is used to arrange the content.</param>
    //    protected override Size ArrangeOverride(Size arrangeSize)
    //    {
    //        if (base.InternalChildren != null && base.InternalChildren.Count > 0 && base.IsLoaded)
    //        {
    //            Rect rect = new Rect(new Point(0, 0), arrangeSize);
    //            this.m_axesGrid.LayoutBounds = rect;
    //            this.m_axesGrid.Arrange(rect);
    //            this.m_elementPanel.Arrange(rect);
    //        }
    //        return arrangeSize;
    //    }

    //    private void Axis_PropertyChanged(object sender, PropertyChangedEventArgs e)
    //    {
    //        this.Invalidate();
    //    }

    //    private static void BackgroundTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    //    {
    //        Area area = (Area)d;
    //        if (area != null)
    //        {
    //            area.OnBackgroundTemplateChanged((DataTemplate)e.OldValue, (DataTemplate)e.NewValue);
    //        }
    //    }

    //    private void CalculateStep(DataPointsList<DataPoint> lstDataPoints)
    //    {
    //        double x = 0;
    //        double y = 0;
    //        if (lstDataPoints != null && lstDataPoints.Count > 0)
    //        {
    //            int count = lstDataPoints.Count;
    //            DataPoint[] dataPointArray = new DataPoint[count];
    //            for (int i = 0; i < count; i++)
    //            {
    //                DataPoint item = lstDataPoints[i];
    //                if (item != null)
    //                {
    //                    dataPointArray[i] = item;
    //                }
    //            }
    //            Array.Sort(dataPointArray, new DataPointComparer());
    //            for (int j = 0; j < count; j++)
    //            {
    //                if (j + 1 < count)
    //                {
    //                    DataPoint dataPoint = dataPointArray[j];
    //                    DataPoint dataPoint1 = dataPointArray[j + 1];
    //                    if ((x > dataPoint1.X - dataPoint.X || x == 0) && dataPoint1.X != dataPoint.X)
    //                    {
    //                        x = dataPoint1.X - dataPoint.X;
    //                    }
    //                    if (y > dataPoint1.Y - dataPoint.Y || y == 0)
    //                    {
    //                        y = dataPoint1.Y - dataPoint.Y;
    //                    }
    //                }
    //            }
    //            this.XStep = x;
    //            this.YStep = y;
    //        }
    //    }

    //    private void CorrectDataPointsByLabels(DataPointsList<DataPoint> lstDataPoints)
    //    {
    //        if (lstDataPoints != null && lstDataPoints.Count > 0 && this.XAxis.LabelsType == LabelsType.Labels)
    //        {
    //            List<string> strs = new List<string>();
    //            foreach (DataPoint lstDataPoint in lstDataPoints)
    //            {
    //                if (strs.Contains(lstDataPoint.Label))
    //                {
    //                    continue;
    //                }
    //                strs.Add(lstDataPoint.Label);
    //            }
    //            strs = new List<string>(strs.ToArray());
    //            foreach (DataPoint dataPoint in lstDataPoints)
    //            {
    //                double num = (double)strs.IndexOf(dataPoint.Label);
    //                if (num <= -1)
    //                {
    //                    continue;
    //                }
    //                dataPoint.X = num + 1;
    //            }
    //        }
    //    }

    //    /// <summary>Creates visual children for all Area elements.</summary>
    //    protected internal void CreateVisualChildren()
    //    {
    //        if (this.m_bIsCreatingVisualChildren)
    //        {
    //            return;
    //        }
    //        this.m_bIsCreatingVisualChildren = true;
    //        base.Children.Clear();
    //        DataPointsList<DataPoint> dataPoints = this.GetDataPoints();
    //        if (dataPoints != null && dataPoints.Count > 0)
    //        {
    //            this.CorrectDataPointsByLabels(dataPoints);
    //            this.CalculateStep(dataPoints);
    //            this.AxesGrid.Reset();
    //            base.InternalChildren.Add(this.AxesGrid);
    //            base.InternalChildren.Add(this.m_elementPanel);
    //            this.m_elementPanel.AddVisualChildren(this.Series);
    //            this.LayoutSeries();
    //            if (this.Series.Count > 0)
    //            {
    //                double num = 0;
    //                double num1 = 0;
    //                foreach (Series series in this.Series)
    //                {
    //                    if (!series.ShowHintLabels || series.Layout is PieLayout)
    //                    {
    //                        continue;
    //                    }
    //                    Point hintOffset = series.GetHintOffset();
    //                    num1 = Math.Max(hintOffset.X, num1);
    //                    num = Math.Max(hintOffset.Y, num);
    //                }
    //                if (this.YAxis != null)
    //                {
    //                    this.XAxis.Offset = num1;
    //                    this.YAxis.Offset = num;
    //                }
    //            }
    //            this.m_axesGrid.PrepareLayout(dataPoints);
    //            this.m_axesGrid.GridTemplate = this.BackgroundTemplate;
    //            this.UniteWithAxisRange(this.XAxis);
    //            this.UniteWithAxisRange(this.YAxis);
    //        }
    //        this.RaiseLegendRefresh();
    //        this.m_bIsCreatingVisualChildren = false;
    //    }

    //    private void DataPoint_PropertyChanged(object sender, PropertyChangedEventArgs e)
    //    {
    //        this.Invalidate();
    //    }

    //    private static string GenerateUniqueName()
    //    {
    //        int mId = Area.m_id + 1;
    //        Area.m_id = mId;
    //        return string.Format("{0}{1}", "Area", mId);
    //    }

    //    /// <summary>Converts point in user logic coordinates into actual pixel point.</summary>
    //    /// <returns>Point in actual pixels.</returns>
    //    /// <param name="point">Point in user logic coordinates.</param>
    //    public Point GetActualPoint(Point point)
    //    {
    //        return this.m_axesGrid.GetActualPoint(point);
    //    }

    //    /// <summary>Converts DataPoint into actual pixel point.</summary>
    //    /// <returns>Point in actual pixels.</returns>
    //    /// <param name="point">The DataPoint to convert.</param>
    //    public Point GetActualPoint(DataPoint point)
    //    {
    //        return this.m_axesGrid.GetActualPoint(point);
    //    }

    //    /// <summary>Gets a list of DataPoint objects for all <see cref="Xceed.Wpf.Toolkit~Xceed.Wpf.Toolkit.Chart.Series.html">Series</see> instances in this Area.</summary>
    //    /// <returns>A list of DataPoint objects for all Series instances in this Area.</returns>
    //    protected internal DataPointsList<DataPoint> GetDataPoints()
    //    {
    //        if (this.m_allPoints.Count > 0)
    //        {
    //            foreach (DataPoint mAllPoint in this.m_allPoints)
    //            {
    //                mAllPoint.PropertyChanged -= new PropertyChangedEventHandler(this.DataPoint_PropertyChanged);
    //            }
    //        }
    //        this.m_allPoints.Clear();
    //        if (this.Series != null)
    //        {
    //            int num = 0;
    //            int count = this.Series.Count;
    //            while (num < count)
    //            {
    //                DataPointsList<DataPoint> dataPoints = this.Series[num].DataPoints;
    //                if (dataPoints != null)
    //                {
    //                    int num1 = 0;
    //                    int count1 = dataPoints.Count;
    //                    while (num1 < count1)
    //                    {
    //                        dataPoints[num1].PropertyChanged += new PropertyChangedEventHandler(this.DataPoint_PropertyChanged);
    //                        this.m_allPoints.Add(dataPoints[num1]);
    //                        num1++;
    //                    }
    //                }
    //                num++;
    //            }
    //        }
    //        return this.m_allPoints;
    //    }

    //    private void GetSeriesInfo(Series series, out int count, out int position)
    //    {
    //        count = 0;
    //        position = -1;
    //        Series item = null;
    //        int num = 0;
    //        int num1 = series.Area.Series.Count;
    //        while (num < num1)
    //        {
    //            item = series.Area.Series[num];
    //            if (item != null)
    //            {
    //                LayoutEngine layout = item.Layout;
    //                if (layout != null && layout.LayoutType == series.Layout.LayoutType)
    //                {
    //                    count++;
    //                    if (series == item)
    //                    {
    //                        position = count - 1;
    //                    }
    //                }
    //            }
    //            num++;
    //        }
    //    }

    //    /// <summary>Calculates the layout bounds without tick and title labels.</summary>
    //    /// <returns>The calculated bounds.</returns>
    //    public Rect GetSeriesLayoutBounds()
    //    {
    //        if (this.m_axesGrid == null)
    //        {
    //            return Rect.Empty;
    //        }
    //        return this.m_axesGrid.GetSeriesLayoutBounds();
    //    }

    //    ///       <summary>Used to calculate position and size by x-axis for certain Series relative to other
    //    /// side-by-side Series in Series list.</summary>
    //    ///       <returns>DataPoint with offset (X value means location and Y value means width for one DataPoint to display).</returns>
    //    ///       <param name="series">The series for which the relative position will be calculated.</param>
    //    protected internal virtual DataPoint GetSideBySideSeriesInfo(Series series)
    //    {
    //        DataPoint dataPoint = null;
    //        if (series == null)
    //        {
    //            throw new ArgumentNullException("series");
    //        }
    //        double spacing = 1 - series.Spacing / 100;
    //        int num = -1;
    //        int num1 = -1;
    //        this.GetSeriesInfo(series, out num, out num1);
    //        double xStep = spacing / (double)num;
    //        xStep *= this.XStep;
    //        double num2 = xStep * (double)num1;
    //        dataPoint = (series.Layout.LayoutMode != LayoutMode.Stacked ? new DataPoint(spacing * this.XStep / 2 - num2, xStep) : new DataPoint(spacing * this.XStep / 2, spacing * this.XStep));
    //        return dataPoint;
    //    }

    //    /// <summary>Sets highlighted state for the specified Series.</summary>
    //    /// <param name="series">Series to highlight.</param>
    //    /// <param name="highlighted">Highlight the series if true; otherwise, remove highlight.</param>
    //    public void HighlightSeries(Series series, bool highlighted)
    //    {
    //        foreach (Series series1 in this.Series)
    //        {
    //            if (series1 == null)
    //            {
    //                continue;
    //            }
    //            if (!highlighted)
    //            {
    //                series1.SetHighlighted(false);
    //                series1.SetCovered(false);
    //            }
    //            else if (series != series1 || series == null)
    //            {
    //                series1.SetHighlighted(false);
    //                series1.SetCovered(true);
    //            }
    //            else
    //            {
    //                series1.SetHighlighted(highlighted);
    //                series1.SetCovered(false);
    //            }
    //        }
    //    }

    //    /// <summary>Invalidates Area and all its children.</summary>
    //    public void Invalidate()
    //    {
    //        if (base.IsLoaded)
    //        {
    //            this.CreateVisualChildren();
    //            base.InvalidateArrange();
    //        }
    //    }

    //    /// <summary>Performs Series layout and prepares all series for arrangement.</summary>
    //    protected void LayoutSeries()
    //    {
    //        SeriesList<Series> series = this.Series;
    //        if (series.Count > 0)
    //        {
    //            int num = 0;
    //            int count = series.Count;
    //            while (num < count)
    //            {
    //                Series item = this.Series[num];
    //                if (item != null && item.Layout != null)
    //                {
    //                    item.XRange.Reset();
    //                    item.YRange.Reset();
    //                    item.Layout.PerformLayout(item);
    //                }
    //                num++;
    //            }
    //        }
    //    }

    //    /// <summary>Called when BackgroundTemplate changes.</summary>
    //    /// <param name="oldValue">The old DataTemplate.</param>
    //    /// <param name="newValue">The new DataTemplate.</param>
    //    protected virtual void OnBackgroundTemplateChanged(DataTemplate oldValue, DataTemplate newValue)
    //    {
    //        this.Invalidate();
    //    }

    //    protected override AutomationPeer OnCreateAutomationPeer()
    //    {
    //        return new GenericAutomationPeer(this);
    //    }

    //    protected override void OnStyleChanged(Style oldStyle, Style newStyle)
    //    {
    //        base.OnStyleChanged(oldStyle, newStyle);
    //        this.CreateVisualChildren();
    //    }

    //    /// <summary>Called when Title changes.</summary>
    //    protected virtual void OnTitleChanged(string oldValue, string newValue)
    //    {
    //        this.Invalidate();
    //    }

    //    /// <summary>Raises the LegendRefresh event.</summary>
    //    protected void RaiseLegendRefresh()
    //    {
    //        if (this.LegendRefresh != null)
    //        {
    //            this.LegendRefresh(this, EventArgs.Empty);
    //        }
    //    }

    //    private void Series_ListChanged(object sender, ListChangedEventArgs<Series> e)
    //    {
    //        switch (e.ActionType)
    //        {
    //            case ListActionType.Insert:
    //            case ListActionType.Set:
    //                {
    //                    if (e.NewItems == null || (int)e.NewItems.Length <= 0)
    //                    {
    //                        break;
    //                    }
    //                    Series[] newItems = e.NewItems;
    //                    for (int i = 0; i < (int)newItems.Length; i++)
    //                    {
    //                        newItems[i].Area = this;
    //                    }
    //                    break;
    //                }
    //            case ListActionType.Remove:
    //            case ListActionType.Clear:
    //                {
    //                    Series[] oldItems = e.OldItems;
    //                    for (int j = 0; j < (int)oldItems.Length; j++)
    //                    {
    //                        oldItems[j].Area = null;
    //                    }
    //                    break;
    //                }
    //        }
    //        this.Invalidate();
    //    }

    //    private static void TitlePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    //    {
    //        Area area = (Area)d;
    //        if (area != null)
    //        {
    //            area.OnTitleChanged((string)e.OldValue, (string)e.NewValue);
    //        }
    //    }

    //    /// <summary>Goes through the Series instances contained in the <see cref="Xceed.Wpf.Toolkit~Xceed.Wpf.Toolkit.Chart.Area.html">Area</see> and uses the range of each one to calculate the needed total range for an <see cref="Xceed.Wpf.Toolkit~Xceed.Wpf.Toolkit.Chart.Axis.html">Axis</see>.</summary>
    //    /// <param name="axis">The Axis whose total range will be calculated.</param>
    //    protected void UniteWithAxisRange(Axis axis)
    //    {
    //        if (axis == null)
    //        {
    //            throw new ArgumentNullException("axis");
    //        }
    //        if (axis.ScaleMode != AxisScaleMode.Automatic)
    //        {
    //            axis.PerformCustomRangeGraduation();
    //        }
    //        else
    //        {
    //            SeriesList<Series> series = this.Series;
    //            if (series.Count > 0)
    //            {
    //                DataRange dataRange = new DataRange();
    //                int num = 0;
    //                int count = series.Count;
    //                while (num < count)
    //                {
    //                    Series item = series[num];
    //                    dataRange = dataRange + (axis.Orientation == Orientation.Horizontal ? item.XRange : item.YRange);
    //                    num++;
    //                }
    //                if (dataRange.IsEmpty)
    //                {
    //                    dataRange += new DataRange(0, 10);
    //                }
    //                axis.Range = dataRange;
    //                return;
    //            }
    //        }
    //    }

    //    /// <summary>Raised when legend items need to be refreshed.</summary>
    //    protected internal event EventHandler LegendRefresh;
    //}
}
