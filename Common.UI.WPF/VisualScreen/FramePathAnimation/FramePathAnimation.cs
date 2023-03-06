using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Common.UI.WPF.Core.Utilities;
using Common.UI.WPF.VisualScreen.Converters;

namespace Common.UI.WPF.VisualScreen
{
    public class FramePathAnimation : Control
    {
        static FramePathAnimation()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FramePathAnimation), new FrameworkPropertyMetadata(typeof(FramePathAnimation)));
        }

        //public event PropertyChangedEventHandler PropertyChanged;
        //private void OnPropertyChanged(string info)
        //{
        //    PropertyChangedEventHandler handler = PropertyChanged;
        //    if (handler != null)
        //    {
        //        handler(this, new PropertyChangedEventArgs(info));
        //    }
        //}

        #region Path
        public static readonly DependencyProperty PathDataProperty = 
            DependencyProperty.Register("PathData", typeof(Geometry), typeof(FramePathAnimation), new PropertyMetadata(Geometry.Parse("M 10,100 C 35,0 135,0 160,100 180,190 285,200 310,100")));
       
        public Geometry PathData
        {
            get { return (Geometry)GetValue(PathDataProperty); }
            set { SetValue(PathDataProperty, value); }
        }

        public static readonly DependencyProperty PathStrokeProperty =
            DependencyProperty.Register("PathStroke", typeof(Brush), typeof(FramePathAnimation), new PropertyMetadata(Brushes.Black));

        public Brush PathStroke
        {
            get { return (Brush)GetValue(PathStrokeProperty); }
            set { SetValue(PathStrokeProperty, value); }
        }

        public static readonly DependencyProperty PathStrokeThicknessProperty =
            DependencyProperty.Register("PathStrokeThickness", typeof(double), typeof(FramePathAnimation), new PropertyMetadata(1.0));

        public double PathStrokeThickness
        {
            get { return (double)GetValue(PathStrokeThicknessProperty); }
            set { SetValue(PathStrokeThicknessProperty, value); }
        }
        #endregion

        #region Arrow
        public static readonly DependencyProperty ArrowDataProperty =
            DependencyProperty.Register("ArrowData", typeof(Geometry), typeof(FramePathAnimation), new PropertyMetadata(Geometry.Parse("M 0,0 L 10,0 M 5,-5 L 10,0 L 5,5")));

        public Geometry ArrowData
        {
            get { return (Geometry)GetValue(ArrowDataProperty); }
            set { SetValue(ArrowDataProperty, value); }
        }

        public static readonly DependencyProperty ArrowCountProperty =
            DependencyProperty.Register("ArrowCount", typeof(int), typeof(FramePathAnimation), new FrameworkPropertyMetadata(1, new PropertyChangedCallback(OnArrowChanged)));

        public int ArrowCount
        {
            get { return (int)GetValue(ArrowCountProperty); }
            set { SetValue(ArrowCountProperty, value); }
        }

        public static readonly DependencyProperty ArrowStrokeProperty =
            DependencyProperty.Register("ArrowStroke", typeof(Brush), typeof(FramePathAnimation), new PropertyMetadata(Brushes.Red));

        public Brush ArrowStroke
        {
            get { return (Brush)GetValue(ArrowStrokeProperty); }
            set { SetValue(ArrowStrokeProperty, value); }
        }

        public static readonly DependencyProperty ArrowFillProperty =
            DependencyProperty.Register("ArrowFill", typeof(Brush), typeof(FramePathAnimation), new PropertyMetadata(Brushes.Red));

        public Brush ArrowFill
        {
            get { return (Brush)GetValue(ArrowFillProperty); }
            set { SetValue(ArrowFillProperty, value); }
        }

        public static readonly DependencyProperty ArrowHeightProperty =
            DependencyProperty.Register("ArrowHeight", typeof(double), typeof(FramePathAnimation), new PropertyMetadata(16.0));

        public double ArrowHeight
        {
            get { return (double)GetValue(ArrowHeightProperty); }
            set { SetValue(ArrowHeightProperty, value); }
        }

        public static readonly DependencyProperty ArrowWidthProperty =
            DependencyProperty.Register("ArrowWidth", typeof(double), typeof(FramePathAnimation), new PropertyMetadata(16.0));

        public double ArrowWidth
        {
            get { return (double)GetValue(ArrowWidthProperty); }
            set { SetValue(ArrowWidthProperty, value); }
        }

        public static readonly DependencyProperty ArrowStrokeThicknessProperty =
            DependencyProperty.Register("ArrowStrokeThickness", typeof(double), typeof(FramePathAnimation), new PropertyMetadata(2.0));

        public double ArrowStrokeThickness
        {
            get { return (double)GetValue(ArrowStrokeThicknessProperty); }
            set { SetValue(ArrowStrokeThicknessProperty, value); }
        }

        public static readonly DependencyProperty ArrowDurationProperty =
            DependencyProperty.Register("ArrowDuration", typeof(Duration), typeof(FramePathAnimation), new PropertyMetadata(new Duration(TimeSpan.FromSeconds(5)), new PropertyChangedCallback(OnArrowChanged)));

        public Duration ArrowDuration
        {
            get { return (Duration)GetValue(ArrowDurationProperty); }
            set { SetValue(ArrowDurationProperty, value); }
        }

        #endregion

        private static void OnArrowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not FramePathAnimation pathAnimation)
            {
                return;
            }

            if (pathAnimation.IsLoaded)
            {
                BuildPathAnimation(pathAnimation);
            }
            else
            {
                pathAnimation.Loaded += PathAnimation_Loaded;
                // pathAnimation.SizeChanged += (s, e) => BuildPathAnimation(pathAnimation);
            }
        }

        private static void PathAnimation_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is not FramePathAnimation pathAnimation)
            {
                return;
            }

            pathAnimation.Loaded -= PathAnimation_Loaded;
            BuildPathAnimation(pathAnimation);
        }

        private static void BuildPathAnimation(FramePathAnimation pathAnimation)
        {
            var rootCanvas = UIHelper.FindVisualChild<Canvas>(pathAnimation, "PART_RootCanvas");
            if (rootCanvas == null)
            {
                return;
            }

            rootCanvas.Children.Clear();
            NameScope.SetNameScope(rootCanvas, new NameScope());
            Storyboard storyboard = new();

            for (int i = 0; i < pathAnimation.ArrowCount; i++)
            {
                var transformGroup = new TransformGroup();
                transformGroup.Children.Add(new TranslateTransform(0, -1));
                transformGroup.Children.Add(new MatrixTransform(matrix: new Matrix()));

                var path = new Path
                {
                    StrokeLineJoin = PenLineJoin.Miter,
                    Name = $"Arrow_{i}",
                    // Data = PathGeometry.CreateFromGeometry(pathAnimation.ArrowData),
                    RenderTransform = transformGroup
                };

                BindingOperations.SetBinding(path, Path.DataProperty, new Binding("ArrowData")
                {
                    Source = pathAnimation,
                });

                BindingOperations.SetBinding(path, Path.StrokeProperty, new Binding("ArrowStroke")
                {
                    Source = pathAnimation
                });

                BindingOperations.SetBinding(path, Path.FillProperty, new Binding("ArrowFill")
                {
                    Source = pathAnimation
                });

                BindingOperations.SetBinding(path, Path.WidthProperty, new Binding("ArrowWidth")
                {
                    Source = pathAnimation
                });

                BindingOperations.SetBinding(path, Path.HeightProperty, new Binding("ArrowHeight")
                {
                    Source = pathAnimation
                });

                BindingOperations.SetBinding(path, Path.StrokeThicknessProperty, new Binding("ArrowStrokeThickness")
                {
                    Source = pathAnimation
                });
                
                rootCanvas.Children.Add(path);
                rootCanvas.RegisterName(path.Name, path);

                var matrixAnimation = new MatrixAnimationUsingPath
                {
                    // PathGeometry = PathGeometry.CreateFromGeometry(pathAnimation.PathData),
                    // Duration = new Duration(TimeSpan.FromSeconds(20)),
                    BeginTime = TimeSpan.FromSeconds(i),
                    DoesRotateWithTangent = true,
                    RepeatBehavior = RepeatBehavior.Forever,
                };

                BindingOperations.SetBinding(matrixAnimation, MatrixAnimationUsingPath.PathGeometryProperty, new Binding("PathData")
                {
                    Source = pathAnimation,
                    // Mode = BindingMode.TwoWay,
                    Converter = new GeometryToPathConverter(),
                    ConverterParameter = pathAnimation.PathData
                }) ;

                BindingOperations.SetBinding(matrixAnimation, MatrixAnimationUsingPath.DurationProperty, new Binding("ArrowDuration")
                {
                    Source = pathAnimation
                });

                //BindingOperations.SetBinding(matrixAnimation, MatrixAnimationUsingPath.BeginTimeProperty, new Binding("ArrowBeginTime")
                //{
                //    Source = pathAnimation
                //});

                Storyboard.SetTargetName(matrixAnimation, path.Name);
                Storyboard.SetTargetProperty(matrixAnimation, new PropertyPath("RenderTransform.Children[1].Matrix"));

                storyboard.Children.Add(matrixAnimation);

                path.Loaded += (sender, e) =>
                {
                    storyboard.Begin(path);
                };
            }
        }
    }
}

#region Backup
/** 
var streamGeometry = new StreamGeometry();
using (StreamGeometryContext ctx = streamGeometry.Open())
{
    ctx.BeginFigure(new Point(0, 0), true, false);
    ctx.LineTo(new Point(20, 0), true, false);

    ctx.BeginFigure(new Point(10, -10), true, false);
    ctx.LineTo(new Point(20, 0), true, false);
    ctx.LineTo(new Point(10, 10), true, false);
}
streamGeometry.Freeze();
**/

/**
<Canvas Width="1600" Height="1000">
    <Path
        Data="{StaticResource FlowlinePath}"
        Stroke="Yellow"
        StrokeThickness="2" />
    <Path
        x:Name="Arrow0"
        Width="30"
        Height="30"
        Data="M 0,0 L 20,0 M 10,-10 L 20,0 L 10,10"
        Stroke="Blue"
        StrokeLineJoin="Miter"
        StrokeThickness="5">
        <Path.RenderTransform>
            <TransformGroup>
                <TranslateTransform />
                <MatrixTransform>
                    <MatrixTransform.Matrix>
                        <Matrix />
                    </MatrixTransform.Matrix>
                </MatrixTransform>
            </TransformGroup>
        </Path.RenderTransform>
        <Path.Triggers>
            <EventTrigger RoutedEvent="Path.Loaded">
                <BeginStoryboard>
                    <Storyboard>
                        <MatrixAnimationUsingPath
                            BeginTime="0:0:0"
                            DoesRotateWithTangent="True"
                            PathGeometry="{StaticResource FlowlinePath}"
                            RepeatBehavior="Forever"
                            Storyboard.TargetName="Arrow0"
                            Storyboard.TargetProperty="RenderTransform.Children[1].Matrix"
                            Duration="0:0:20" />
                    </Storyboard>
                </BeginStoryboard>
            </EventTrigger>
        </Path.Triggers>
    </Path>
    <Path
        x:Name="Arrow1"
        Width="30"
        Height="30"
        Data="M 0,0 L 20,0 M 10,-10 L 20,0 L 10,10"
        Stroke="Blue"
        StrokeLineJoin="Miter"
        StrokeThickness="5">
        <Path.RenderTransform>
            <TransformGroup>
                <TranslateTransform />
                <MatrixTransform>
                    <MatrixTransform.Matrix>
                        <Matrix />
                    </MatrixTransform.Matrix>
                </MatrixTransform>
            </TransformGroup>
        </Path.RenderTransform>
        <Path.Triggers>
            <EventTrigger RoutedEvent="Path.Loaded">
                <BeginStoryboard>
                    <Storyboard>
                        <MatrixAnimationUsingPath
                            BeginTime="0:0:5"
                            DoesRotateWithTangent="True"
                            PathGeometry="{StaticResource FlowlinePath}"
                            RepeatBehavior="Forever"
                            Storyboard.TargetName="Arrow1"
                            Storyboard.TargetProperty="RenderTransform.Children[1].Matrix"
                            Duration="0:0:20" />
                    </Storyboard>
                </BeginStoryboard>
            </EventTrigger>
        </Path.Triggers>
    </Path>
    <Path
        x:Name="Arrow2"
        Width="30"
        Height="30"
        Data="M 0,0 L 20,0 M 10,-10 L 20,0 L 10,10"
        Stroke="Blue"
        StrokeLineJoin="Miter"
        StrokeThickness="5">
        <Path.RenderTransform>
            <TransformGroup>
                <TranslateTransform />
                <MatrixTransform>
                    <MatrixTransform.Matrix>
                        <Matrix />
                    </MatrixTransform.Matrix>
                </MatrixTransform>
            </TransformGroup>
        </Path.RenderTransform>
        <Path.Triggers>
            <EventTrigger RoutedEvent="Path.Loaded">
                <BeginStoryboard>
                    <Storyboard>
                        <MatrixAnimationUsingPath
                            BeginTime="0:0:10"
                            DoesRotateWithTangent="True"
                            PathGeometry="{StaticResource FlowlinePath}"
                            RepeatBehavior="Forever"
                            Storyboard.TargetName="Arrow2"
                            Storyboard.TargetProperty="RenderTransform.Children[1].Matrix"
                            Duration="0:0:20" />
                    </Storyboard>
                </BeginStoryboard>
            </EventTrigger>
        </Path.Triggers>
    </Path>
    <Path
        x:Name="Arrow3"
        Width="30"
        Height="30"
        Data="M 0,0 L 20,0 M 10,-10 L 20,0 L 10,10"
        Stroke="Blue"
        StrokeLineJoin="Miter"
        StrokeThickness="5">
        <Path.RenderTransform>
            <TransformGroup>
                <TranslateTransform />
                <MatrixTransform>
                    <MatrixTransform.Matrix>
                        <Matrix />
                    </MatrixTransform.Matrix>
                </MatrixTransform>
            </TransformGroup>
        </Path.RenderTransform>
        <Path.Triggers>
            <EventTrigger RoutedEvent="Path.Loaded">
                <BeginStoryboard>
                    <Storyboard>
                        <MatrixAnimationUsingPath
                            BeginTime="0:0:15"
                            DoesRotateWithTangent="True"
                            PathGeometry="{StaticResource FlowlinePath}"
                            RepeatBehavior="Forever"
                            Storyboard.TargetName="Arrow3"
                            Storyboard.TargetProperty="RenderTransform.Children[1].Matrix"
                            Duration="0:0:20" />
                    </Storyboard>
                </BeginStoryboard>
            </EventTrigger>
        </Path.Triggers>
    </Path>
    <Path
        x:Name="Arrow4"
        Width="30"
        Height="30"
        Data="M 0,0 L 20,0 M 10,-10 L 20,0 L 10,10"
        Stroke="Blue"
        StrokeLineJoin="Miter"
        StrokeThickness="5">
        <Path.RenderTransform>
            <TransformGroup>
                <TranslateTransform />
                <MatrixTransform>
                    <MatrixTransform.Matrix>
                        <Matrix />
                    </MatrixTransform.Matrix>
                </MatrixTransform>
            </TransformGroup>
        </Path.RenderTransform>
        <Path.Triggers>
            <EventTrigger RoutedEvent="Path.Loaded">
                <BeginStoryboard>
                    <Storyboard>
                        <MatrixAnimationUsingPath
                            BeginTime="0:0:20"
                            DoesRotateWithTangent="True"
                            PathGeometry="{StaticResource FlowlinePath}"
                            RepeatBehavior="Forever"
                            Storyboard.TargetName="Arrow4"
                            Storyboard.TargetProperty="RenderTransform.Children[1].Matrix"
                            Duration="0:0:20" />
                    </Storyboard>
                </BeginStoryboard>
            </EventTrigger>
        </Path.Triggers>
    </Path>
    </Canvas>
**/
#endregion