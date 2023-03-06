using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Common.UI.WPF.Charts
{
//    public class Series : DependencyObject
//    {
//        internal const string c_series = "Series";

//        private DataRange m_yRange = new DataRange();
//        private DataRange m_xRange = new DataRange();
//        private static int m_id;

//        private BindingsList<BindingInfo> m_dataPointBindings = new BindingsList<BindingInfo>();

//        private DataPointsList<DataPoint> m_lstDataPoints = new DataPointsList<DataPoint>();

//        private ChartPrimitivesList<ChartPrimitive> m_lstLayoutPrimitives = new ChartPrimitivesList<ChartPrimitive>();

//        private Area m_area;

//        private SolidColorBrush m_defaultInterior;

//        private ChartPrimitivesList<ChartPrimitive> m_lstLayoutHints = new ChartPrimitivesList<ChartPrimitive>();

//        /// <summary>Identifies the  dependency property.</summary>
//        public readonly static DependencyProperty IsOwnerHighlightProperty;

//        /// <summary>Identifies the  dependency property.</summary>
//        public readonly static DependencyProperty MarkerTemplateProperty;

//        /// <summary>Identifies the  dependency property.</summary>
//        public readonly static DependencyProperty TemplateProperty;

//        /// <summary>Identifies the  dependency property.</summary>
//        public readonly static DependencyProperty DataPointsSourceProperty;

//        /// <summary>Identifies the  dependency property.</summary>
//        public readonly static DependencyProperty SpacingProperty;

//        /// <summary>Identifies the  dependency property.</summary>
//        public readonly static DependencyProperty LayoutProperty;

//        /// <summary>Identifies the  dependency property.</summary>
//        public readonly static DependencyProperty TitleProperty;

//        /// <summary>Identifies the  dependency property.</summary>
//        public readonly static DependencyProperty ShowPointsInLegendProperty;

//        /// <summary>Identifies the  dependency property.</summary>
//        public readonly static DependencyProperty HintLabelTemplateProperty;

//        /// <summary>Identifies the  dependency property.</summary>
//        public readonly static DependencyProperty HintLineTemplateProperty;

//        /// <summary>Identifies the  dependency property.</summary>
//        public readonly static DependencyProperty HintLineLengthProperty;

//        /// <summary>Identifies the  dependency property.</summary>
//        public readonly static DependencyProperty ShowHintLabelsProperty;

//        /// <summary>Gets the parent  of the series.</summary>
//        public Area Area
//        {
//            get
//            {
//                return this.m_area;
//            }
//            internal set
//            {
//                if (this.m_area != value)
//                {
//                    this.m_area = value;
//}
//}
//        }

//        /// <summary>Gets the list of  objects.</summary>
//        public BindingsList<BindingInfo> DataPointBindings
//        {
//            get
//            {
//                return this.m_dataPointBindings;
//            }
//        }

//        /// <summary>Gets or sets the list of  objects.</summary>
//        public DataPointsList<DataPoint> DataPoints
//        {
//            get
//            {
//                return this.m_lstDataPoints;
//            }
//        }

//        /// <summary>Gets or sets the binding source for DataPoints.</summary>
//        public IEnumerable DataPointsSource
//        {
//            get
//            {
//                return (IEnumerable)base.GetValue(Series.DataPointsSourceProperty);
//            }
//            set
//            {
//                base.SetValue(Series.DataPointsSourceProperty, value);
//            }
//        }

//        /// <summary>Gets or sets the default interior color for series elements.</summary>
//        public SolidColorBrush DefaultInterior
//        {
//            get
//            {
//                return this.m_defaultInterior;
//            }
//            set
//            {
//                if (this.m_defaultInterior != value)
//                {
//                    this.m_defaultInterior = value;
//                    this.Refresh();
//                }
//            }
//        }

//        /// <summary>Gets or sets the hint label template for elements of this series.</summary>
//        public DataTemplate HintLabelTemplate
//        {
//            get
//            {
//                return (DataTemplate)base.GetValue(Series.HintLabelTemplateProperty);
//            }
//            set
//            {
//                base.SetValue(Series.HintLabelTemplateProperty, value);
//            }
//        }

//        /// <summary>Gets or sets the hint line length.</summary>
//        public double HintLineLength
//        {
//            get
//            {
//                return (double)base.GetValue(Series.HintLineLengthProperty);
//            }
//            set
//            {
//                base.SetValue(Series.HintLineLengthProperty, value);
//            }
//        }

//        /// <summary>Gets or sets the hint line template for elements of this series.</summary>
//        public DataTemplate HintLineTemplate
//        {
//            get
//            {
//                return (DataTemplate)base.GetValue(Series.HintLineTemplateProperty);
//            }
//            set
//            {
//                base.SetValue(Series.HintLineTemplateProperty, value);
//            }
//        }

//        /// <summary>Gets or sets whether this series performs highlighting of its child elements manually.</summary>
//        public bool IsOwnerHighlight
//        {
//            get
//            {
//                return (bool)base.GetValue(Series.IsOwnerHighlightProperty);
//            }
//            set
//            {
//                base.SetValue(Series.IsOwnerHighlightProperty, value);
//            }
//        }

//        /// <summary>Gets or sets the LayoutEngine-derived layout engine used for this Series.</summary>
//        public LayoutEngine Layout
//        {
//            get
//            {
//                return (LayoutEngine)base.GetValue(Series.LayoutProperty);
//            }
//            set
//            {
//                base.SetValue(Series.LayoutProperty, value);
//            }
//        }

//        /// <summary>Gets a list of hint primitives.</summary>
//        public ChartPrimitivesList<ChartPrimitive> LayoutHints
//        {
//            get
//            {
//                return this.m_lstLayoutHints;
//            }
//        }

//        /// <summary>Gets a list of chart primitives of the series.</summary>
//        public ChartPrimitivesList<ChartPrimitive> LayoutPrimitives
//        {
//            get
//            {
//                return this.m_lstLayoutPrimitives;
//            }
//        }

//        /// <summary>Gets or sets the DataTemplate for line markers.</summary>
//        public DataTemplate MarkerTemplate
//        {
//            get
//            {
//                return (DataTemplate)base.GetValue(Series.MarkerTemplateProperty);
//            }
//            set
//            {
//                base.SetValue(Series.MarkerTemplateProperty, value);
//            }
//        }

//        /// <summary>Gets or sets whether hint labels for the series are displayed.</summary>
//        public bool ShowHintLabels
//        {
//            get
//            {
//                return (bool)base.GetValue(Series.ShowHintLabelsProperty);
//            }
//            set
//            {
//                base.SetValue(Series.ShowHintLabelsProperty, value);
//            }
//        }

//        /// <summary>
//        ///   <para>Gets or sets whether datapoints from the series are displayed in the Legend.</para>
//        /// </summary>
//        public bool ShowPointsInLegend
//        {
//            get
//            {
//                return (bool)base.GetValue(Series.ShowPointsInLegendProperty);
//            }
//            set
//            {
//                base.SetValue(Series.ShowPointsInLegendProperty, value);
//            }
//        }

//        /// <summary>Gets or sets the spacing between series elements.</summary>
//        public double Spacing
//        {
//            get
//            {
//                return (double)base.GetValue(Series.SpacingProperty);
//            }
//            set
//            {
//                base.SetValue(Series.SpacingProperty, value);
//            }
//        }

//        /// <summary>Gets or sets the DataTemplate for the elements of this Series.</summary>
//        public DataTemplate Template
//        {
//            get
//            {
//                return (DataTemplate)base.GetValue(Series.TemplateProperty);
//            }
//            set
//            {
//                base.SetValue(Series.TemplateProperty, value);
//            }
//        }

//        /// <summary>Gets or Sets the title of the series.</summary>
//        public string Title
//        {
//            get
//            {
//                return (string)base.GetValue(Series.TitleProperty);
//            }
//            set
//            {
//                base.SetValue(Series.TitleProperty, value);
//            }
//        }

//        /// <summary>Gets or sets the range this series occupies on the x-axis.</summary>
//        protected internal DataRange XRange
//        {
//            get
//            {
//                return this.m_xRange;
//            }
//            set
//            {
//                if (this.m_xRange != value)
//                {
//                    this.m_xRange = value;
//                }
//            }
//        }

//        /// <summary>Gets or sets the range this series occupies on the y-axis.</summary>
//        protected internal DataRange YRange
//        {
//            get
//            {
//                return this.m_yRange;
//            }
//            set
//            {
//                if (this.m_yRange != value)
//                {
//                    this.m_yRange = value;
//                }
//            }
//        }

//        static Series()
//        {
//            Series.m_id = -1;
//            Series.IsOwnerHighlightProperty = DependencyProperty.Register("IsOwnerHighlight", typeof(bool), typeof(Series), new PropertyMetadata(new PropertyChangedCallback(Series.IsOwnerHighlightPropertyChanged)));
//            Series.MarkerTemplateProperty = DependencyProperty.Register("MarkerTemplate", typeof(DataTemplate), typeof(Series), new PropertyMetadata(new PropertyChangedCallback(Series.MarkerDataTemplateChanged)));
//            Series.TemplateProperty = DependencyProperty.Register("Template", typeof(DataTemplate), typeof(Series), new PropertyMetadata(ColumnLayout.DefaultTemplate, new PropertyChangedCallback(Series.DataTemplateChanged)));
//            Series.DataPointsSourceProperty = DependencyProperty.Register("DataPointsSource", typeof(IEnumerable), typeof(Series), new PropertyMetadata(new PropertyChangedCallback(Series.OnDataPointsSourceChanged)));
//            Series.SpacingProperty = DependencyProperty.Register("Spacing", typeof(double), typeof(Series), new UIPropertyMetadata((object)20, new PropertyChangedCallback(Series.SpacingPropertyChanged)));
//            Series.LayoutProperty = DependencyProperty.Register("Layout", typeof(LayoutEngine), typeof(Series), new UIPropertyMetadata(new ColumnLayout(), new PropertyChangedCallback(Series.LayoutPropertyChanged)));
//            Series.TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(Series), new UIPropertyMetadata(string.Empty, new PropertyChangedCallback(Series.TitlePropertyChanged)));
//            Series.ShowPointsInLegendProperty = DependencyProperty.Register("ShowPointsInLegend", typeof(bool), typeof(Series), new UIPropertyMetadata(false, new PropertyChangedCallback(Series.ShowPointsInLegendPropertyChanged)));
//            Series.HintLabelTemplateProperty = DependencyProperty.Register("HintLabelTemplate", typeof(DataTemplate), typeof(Series), new PropertyMetadata(LayoutEngine.DefaultHintLabelTemplate, new PropertyChangedCallback(Series.HintLabelTemplatePropertyChanged)));
//            Series.HintLineTemplateProperty = DependencyProperty.Register("HintLineTemplate", typeof(DataTemplate), typeof(Series), new PropertyMetadata(LayoutEngine.DefaultHintLineTemplate, new PropertyChangedCallback(Series.HintLineTemplatePropertyChanged)));
//            Series.HintLineLengthProperty = DependencyProperty.Register("HintLineLength", typeof(double), typeof(Series), new PropertyMetadata((object)20, new PropertyChangedCallback(Series.HintLineLengthPropertyChanged)));
//            Series.ShowHintLabelsProperty = DependencyProperty.Register("ShowHintLabels", typeof(bool), typeof(Series), new PropertyMetadata(false, new PropertyChangedCallback(Series.ShowHintLabelsPropertyChanged)));
//        }

//        /// <summary>Initializes a new instance of the Series class.</summary>
//        public Series()
//        {
//            this.Title = Series.GenerateUniqueName();
//            this.m_lstDataPoints.ListChanged += new ListChangedEventHandler<DataPoint>(this.OnDataPointsChanged);
//            this.m_lstDataPoints.CollectionChanged += new NotifyCollectionChangedEventHandler(this.DataBoundCollectionChanged);
//            this.m_dataPointBindings.ListChanged += new ListChangedEventHandler<BindingInfo>(this.m_dataPointBindings_ListChanged);
//            this.m_lstLayoutPrimitives.ListChanged += new ListChangedEventHandler<ChartPrimitive>(this.m_layoutPrimitives_ListChanged);
//        }

//        /// <summary>Applies the default DataTemplates to series elements.</summary>
//        public void ApplyDefaultTemplate()
//        {
//            if (this.Layout != null)
//            {
//                this.Layout.ApplyDefaultTemplate(this);
//            }
//        }

//        /// <summary>Prepares the layout for the drawing of the chart's primitives, depending on the type of series involved.</summary>
//        /// <returns>A list of the  instances.</returns>
//        protected internal ChartPrimitivesList<ChartPrimitive> CreateElements()
//        {
//            this.Layout.CreateElements(this, this.m_lstLayoutPrimitives);
//            return this.m_lstLayoutPrimitives;
//        }

//        /// <summary>Prepares the layout for the drawing of the chart's hint primitives, depending on the type of series involved.</summary>
//        /// <returns>A list of the  instances.</returns>
//        protected internal ChartPrimitivesList<ChartPrimitive> CreateHints()
//        {
//            this.Layout.CreateHints(this, this.m_lstLayoutHints);
//            return this.m_lstLayoutHints;
//        }

//        private void DataBoundCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
//        {
//            if (this.Area != null && this.Area.IsLoaded)
//            {
//                this.Area.CreateVisualChildren();
//                this.Area.InvalidateVisual();
//            }
//        }

//        private static void DataTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
//        {
//            Series series = (Series)d;
//            if (series != null)
//            {
//                series.OnDataTemplateChanged((DataTemplate)e.OldValue, (DataTemplate)e.NewValue);
//            }
//        }

//        private static void DefaultInteriorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
//        {
//            Series series = (Series)d;
//            if (series != null)
//            {
//                series.OnDefaultInteriorChanged((SolidColorBrush)e.OldValue, (SolidColorBrush)e.NewValue);
//            }
//        }

//        private static string GenerateUniqueName()
//        {
//            int mId = Series.m_id + 1;
//            Series.m_id = mId;
//            return string.Format("{0}{1}", "Series", mId);
//        }

//        /// <summary>Calculates the x-axis and y-axis offsets for additional series decorators, such as hint lines.</summary>
//        /// <returns>The x-axis and y-axis offsets for additional series decorators.</returns>
//        protected internal Point GetHintOffset()
//        {
//            double num = 0;
//            double num1 = 0;
//            foreach (ChartPrimitive mLstLayoutHint in this.m_lstLayoutHints)
//            {
//                BaseTitleLabel baseTitleLabel = mLstLayoutHint as BaseTitleLabel;
//                if (baseTitleLabel == null)
//                {
//                    continue;
//                }
//                baseTitleLabel.Measure(new Size(2147483647, 2147483647));
//                Size desiredSize = baseTitleLabel.DesiredSize;
//                num = Math.Max(desiredSize.Width / 2, num);
//                num1 = Math.Max(baseTitleLabel.DesiredSize.Height, num1);
//            }
//            return new Point(num, this.HintLineLength + num1);
//        }

//        /// <summary>Returns an array of DataPoints contained in this series, sorted if needed.</summary>
//        /// <returns>Array of DataPoints contained in this series, sorted if needed.</returns>
//        public DataPoint[] GetSortedPoints()
//        {
//            DataPoint[] item = new DataPoint[this.DataPoints.Count];
//            int num = 0;
//            int count = this.DataPoints.Count;
//            while (num < count)
//            {
//                item[num] = this.DataPoints[num];
//                num++;
//            }
//            Array.Sort(item, new DataPointComparer());
//            return item;
//        }

//        private static void HintLabelTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
//        {
//            Series series = (Series)d;
//            if (series != null)
//            {
//                series.OnHintLabelTemplateChanged((DataTemplate)e.OldValue, (DataTemplate)e.NewValue);
//            }
//        }

//        private static void HintLineLengthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
//        {
//            Series series = (Series)d;
//            if (series != null)
//            {
//                series.OnHintLineLengthChanged((double)e.OldValue, (double)e.NewValue);
//            }
//        }

//        private static void HintLineTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
//        {
//            Series series = (Series)d;
//            if (series != null)
//            {
//                series.OnHintLineTemplateChanged((DataTemplate)e.OldValue, (DataTemplate)e.NewValue);
//            }
//        }

//        private static void IsOwnerHighlightPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
//        {
//            Series series = (Series)d;
//            if (series != null)
//            {
//                series.OnIsOwnerHighlightChanged((bool)e.OldValue, (bool)e.NewValue);
//            }
//        }

//        private bool IsUsingDefaultTemplate()
//        {
//            if (this.Template == LineLayout.DefaultTemplate || this.Template == ColumnLayout.DefaultTemplate || this.Template == PieLayout.DefaultTemplate)
//            {
//                return true;
//            }
//            return this.Template == AreaLayout.DefaultTemplate;
//        }

//        private static void LayoutPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
//        {
//            Series series = (Series)d;
//            if (series != null)
//            {
//                series.OnLayoutChanged((LayoutEngine)e.OldValue, (LayoutEngine)e.NewValue);
//            }
//        }

//        private void m_dataPointBindings_ListChanged(object sender, ListChangedEventArgs<BindingInfo> e)
//        {
//            this.DataPoints.SetBindings(this.DataPointBindings);
//        }

//        private void m_layoutPrimitives_ListChanged(object sender, ListChangedEventArgs<ChartPrimitive> e)
//        {
//            switch (e.ActionType)
//            {
//                case ListActionType.Insert:
//                case ListActionType.Set:
//                    {
//                        this.ProcessVisuals(e.OldItems, false);
//                        this.ProcessVisuals(e.NewItems, true);
//                        return;
//                    }
//                case ListActionType.Remove:
//                case ListActionType.Clear:
//                    {
//                        this.ProcessVisuals(e.OldItems, false);
//                        return;
//                    }
//                default:
//                    {
//                        return;
//                    }
//            }
//        }

//        private static void MarkerDataTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
//        {
//            Series series = (Series)d;
//            if (series != null)
//            {
//                series.OnMarkerDataTemplateChanged((DataTemplate)e.OldValue, (DataTemplate)e.NewValue);
//            }
//        }

//        private void OnDataPointChanged(object sender, PropertyChangedEventArgs e)
//        {
//            this.Refresh();
//        }

//        private void OnDataPointsChanged(object sender, ListChangedEventArgs<DataPoint> e)
//        {
//            switch (e.ActionType)
//            {
//                case ListActionType.Insert:
//                    {
//                        this.ProcessDataPoints(e.NewItems, true);
//                        return;
//                    }
//                case ListActionType.Remove:
//                case ListActionType.Clear:
//                    {
//                        this.ProcessDataPoints(e.OldItems, false);
//                        return;
//                    }
//                case ListActionType.Set:
//                    {
//                        this.ProcessDataPoints(e.OldItems, false);
//                        this.ProcessDataPoints(e.NewItems, true);
//                        return;
//                    }
//                default:
//                    {
//                        return;
//                    }
//            }
//        }

//        private static void OnDataPointsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
//        {
//            Series series = (Series)d;
//            series.OnDataPointsSourceChanged((IEnumerable)e.OldValue, (IEnumerable)e.NewValue);
//        }

//        /// <summary>Called when the  property is changed.</summary>
//        /// <param name="oldValue">Old value.</param>
//        /// <param name="newValue">New value.</param>
//        protected virtual void OnDataPointsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
//        {
//            if (newValue == null)
//            {
//                this.DataPoints.ClearItemsSource();
//                return;
//            }
//            this.DataPoints.SetItemsSource(newValue);
//        }

//        /// <summary>Called when the DataTemplate property is changed.</summary>
//        /// <param name="oldValue">The old value.</param>
//        /// <param name="newValue">The new value.</param>
//        protected virtual void OnDataTemplateChanged(DataTemplate oldValue, DataTemplate newValue)
//        {
//            this.Refresh();
//        }

//        /// <summary>Called when the DefaultInterior property changes.</summary>
//        /// <param name="oldValue">The old value.</param>
//        /// <param name="newValue">The new value.</param>
//        protected virtual void OnDefaultInteriorChanged(SolidColorBrush oldValue, SolidColorBrush newValue)
//        {
//            this.Refresh();
//        }

//        /// <summary>Called when the HintLabelTemplate changes.</summary>
//        protected virtual void OnHintLabelTemplateChanged(DataTemplate oldValue, DataTemplate newValue)
//        {
//            this.Refresh();
//        }

//        /// <summary>Called when the HintLineLength property changes.</summary>
//        /// <param name="oldValue">The old value.</param>
//        /// <param name="newValue">The new value.</param>
//        protected virtual void OnHintLineLengthChanged(double oldValue, double newValue)
//        {
//            this.Refresh();
//        }

//        /// <summary>Called when the HintLineTemplate changes.</summary>
//        /// <param name="oldValue">The old value</param>
//        /// <param name="newValue">The new value.</param>
//        protected virtual void OnHintLineTemplateChanged(DataTemplate oldValue, DataTemplate newValue)
//        {
//            this.Refresh();
//        }

//        /// <summary>Called when the IsOwnerHighlight property changes.</summary>
//        protected virtual void OnIsOwnerHighlightChanged(bool oldValue, bool newValue)
//        {
//            this.Refresh();
//        }

//        /// <summary>Called when the Layout property changes.</summary>
//        /// <param name="oldValue">The old value.</param>
//        /// <param name="newValue">The new value.</param>
//        protected virtual void OnLayoutChanged(LayoutEngine oldValue, LayoutEngine newValue)
//        {
//            if (this.IsUsingDefaultTemplate())
//            {
//                this.ApplyDefaultTemplate();
//            }
//        }

//        /// <summary>Called when the MarkerDataTemplate changes.</summary>
//        /// <param name="oldValue">The old value.</param>
//        /// <param name="newValue">The new value.</param>
//        protected virtual void OnMarkerDataTemplateChanged(DataTemplate oldValue, DataTemplate newValue)
//        {
//            this.Refresh();
//        }

//        /// <summary>Called when the ShowHintLabels property changes.</summary>
//        /// <param name="oldValue">The old value.</param>
//        /// <param name="newValue">The new value.</param>
//        protected virtual void OnShowHintLabelsChanged(bool oldValue, bool newValue)
//        {
//            this.Refresh();
//        }

//        /// <summary>Called when the ShowPointsInLegend changes.</summary>
//        /// <param name="oldValue">The old value.</param>
//        /// <param name="newValue">The new value.</param>
//        protected virtual void OnShowPointsInLegendChanged(bool oldValue, bool newValue)
//        {
//            this.Refresh();
//        }

//        /// <summary>Called when the Spacing property changes.</summary>
//        /// <param name="oldValue">The old value.</param>
//        /// <param name="newValue">The new value.</param>
//        protected virtual void OnSpacingChanged(double oldValue, double newValue)
//        {
//            this.Refresh();
//        }

//        /// <summary>Called when the Title property changes.</summary>
//        /// <param name="oldValue">The old value.</param>
//        /// <param name="newValue">The new value.</param>
//        protected virtual void OnTitleChanged(string oldValue, string newValue)
//        {
//            this.Refresh();
//        }

//        private void primitive_HighlightEnter(object sender, EventArgs e)
//        {
//            Series series;
//            if (this.Area != null)
//            {
//                Area area = this.Area;
//                if (this.IsOwnerHighlight)
//                {
//                    series = null;
//                }
//                else
//                {
//                    series = this;
//                }
//                area.HighlightSeries(series, true);
//                if (this.IsOwnerHighlight)
//                {
//                    foreach (ChartPrimitive mLstLayoutPrimitive in this.m_lstLayoutPrimitives)
//                    {
//                        if (mLstLayoutPrimitive == (ChartPrimitive)sender)
//                        {
//                            mLstLayoutPrimitive.IsHighlighted = true;
//                            mLstLayoutPrimitive.IsCovered = false;
//                        }
//                        else
//                        {
//                            mLstLayoutPrimitive.IsCovered = true;
//                        }
//                    }
//                }
//            }
//        }

//        private void primitive_HighlightLeave(object sender, EventArgs e)
//        {
//            Series series;
//            if (this.Area != null)
//            {
//                Area area = this.Area;
//                if (this.IsOwnerHighlight)
//                {
//                    series = null;
//                }
//                else
//                {
//                    series = this;
//                }
//                area.HighlightSeries(series, false);
//                if (this.IsOwnerHighlight)
//                {
//                    foreach (ChartPrimitive mLstLayoutPrimitive in this.m_lstLayoutPrimitives)
//                    {
//                        mLstLayoutPrimitive.IsHighlighted = false;
//                        mLstLayoutPrimitive.IsCovered = false;
//                    }
//                }
//            }
//        }

//        private void ProcessDataPoints(DataPoint[] arrPoints, bool bAdding)
//        {
//            if (arrPoints != null && (int)arrPoints.Length > 0)
//            {
//                DataPoint dataPoint = null;
//                int num = 0;
//                int length = (int)arrPoints.Length;
//                while (num < length)
//                {
//                    dataPoint = arrPoints[num];
//                    if (dataPoint != null)
//                    {
//                        if (!bAdding)
//                        {
//                            dataPoint.PropertyChanged -= new PropertyChangedEventHandler(this.OnDataPointChanged);
//                        }
//                        else
//                        {
//                            dataPoint.PropertyChanged += new PropertyChangedEventHandler(this.OnDataPointChanged);
//                        }
//                    }
//                    num++;
//                }
//            }
//            this.Refresh();
//        }

//        private void ProcessVisuals(ChartPrimitive[] arrPrimitives, bool bAdding)
//        {
//            if (arrPrimitives != null && (int)arrPrimitives.Length > 0)
//            {
//                ChartPrimitive chartPrimitive = null;
//                int num = 0;
//                int length = (int)arrPrimitives.Length;
//                while (num < length)
//                {
//                    chartPrimitive = arrPrimitives[num];
//                    if (chartPrimitive != null)
//                    {
//                        if (!bAdding)
//                        {
//                            chartPrimitive.HighlightEnter -= new EventHandler(this.primitive_HighlightEnter);
//                            chartPrimitive.HighlightLeave -= new EventHandler(this.primitive_HighlightLeave);
//                        }
//                        else
//                        {
//                            chartPrimitive.HighlightEnter += new EventHandler(this.primitive_HighlightEnter);
//                            chartPrimitive.HighlightLeave += new EventHandler(this.primitive_HighlightLeave);
//                        }
//                    }
//                    num++;
//                }
//            }
//        }

//        private void Refresh()
//        {
//            if (this.Area != null)
//            {
//                this.Area.Invalidate();
//            }
//        }

//        /// <summary>Clears the list of chart-specific and hint primitives.</summary>
//        public void Reset()
//        {
//            this.m_lstLayoutPrimitives.Clear();
//            this.m_lstLayoutHints.Clear();
//        }

//        /// <summary>Sets the covered state of the primitives.</summary>
//        /// <param name="isCovered">Covered state to set.</param>
//        protected internal void SetCovered(bool isCovered)
//        {
//            foreach (ChartPrimitive mLstLayoutPrimitive in this.m_lstLayoutPrimitives)
//            {
//                mLstLayoutPrimitive.IsCovered = isCovered;
//            }
//        }

//        /// <summary>Sets the highlighted state of primitives.</summary>
//        /// <param name="isHighlighted">Highlighted state to set.</param>
//        protected internal void SetHighlighted(bool isHighlighted)
//        {
//            foreach (ChartPrimitive mLstLayoutPrimitive in this.m_lstLayoutPrimitives)
//            {
//                mLstLayoutPrimitive.IsHighlighted = isHighlighted;
//            }
//        }

//        private static void ShowHintLabelsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
//        {
//            Series series = (Series)d;
//            if (series != null)
//            {
//                series.OnShowHintLabelsChanged((bool)e.OldValue, (bool)e.NewValue);
//            }
//        }

//        private static void ShowPointsInLegendPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
//        {
//            Series series = (Series)d;
//            if (series != null)
//            {
//                series.OnShowPointsInLegendChanged((bool)e.OldValue, (bool)e.NewValue);
//            }
//        }

//        private static void SpacingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
//        {
//            Series series = (Series)d;
//            if (series != null)
//            {
//                series.OnSpacingChanged((double)e.OldValue, (double)e.NewValue);
//            }
//        }

//        private static void TitlePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
//        {
//            Series series = (Series)d;
//            if (series != null)
//            {
//                series.OnTitleChanged((string)e.OldValue, (string)e.NewValue);
//            }
//        }
//    }
}
