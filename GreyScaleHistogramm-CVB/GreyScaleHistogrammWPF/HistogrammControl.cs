using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GreyScaleHistogrammWPF
{
  /// <summary>
  /// Control for the Histogramm of a MonoPlane or Multiplane Image
  /// </summary>
  public class HistogrammControl : Control
  {
    private static readonly Brush[] DefaultMultiPlaneBrushes;

    private static readonly Brush[] DefaultMonoPlaneBrushes;

    private static readonly Pen DefaultPen;

    static HistogrammControl()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(HistogrammControl), new FrameworkPropertyMetadata(typeof(HistogrammControl)));

      DefaultMultiPlaneBrushes = new Brush[]
      {
        new SolidColorBrush(Color.FromArgb(128, 255, 0, 0)),//red
        new SolidColorBrush(Color.FromArgb(128, 0, 255, 0)),//green
        new SolidColorBrush(Color.FromArgb(128, 0, 0, 255)),//blue
      };

      DefaultMonoPlaneBrushes = new Brush[]
      {
        new SolidColorBrush(Color.FromArgb(128, 0, 0, 0))//black
      };

      DefaultPen = new Pen(Brushes.Black, 1.0);
 
      DataProperty = DependencyProperty.Register
      (
        nameof(Data), typeof(int[][]), typeof(HistogrammControl),
        new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(Data_Changed))
      );

      BrushProperty = DependencyProperty.Register
      (
        nameof(Brush), typeof(Brush[]), typeof(HistogrammControl),
        new FrameworkPropertyMetadata(DefaultMultiPlaneBrushes, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(Brush_Changed))
      );

      ForegroundPenProperty = DependencyProperty.Register
      (
        nameof(ForegroundPen), typeof(Pen), typeof(HistogrammControl),
        new FrameworkPropertyMetadata(DefaultPen, FrameworkPropertyMetadataOptions.AffectsRender)
      );

      MonoPlaneBrushesProperty = DependencyProperty.Register
      (
        nameof(MonoPlaneBrushes), typeof(Brush[]), typeof(HistogrammControl),
        new FrameworkPropertyMetadata(DefaultMonoPlaneBrushes, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(Brush_Changed))
      );

      MultiPlaneBrushesProperty = DependencyProperty.Register
      (
        nameof(MultiPlaneBrushes), typeof(Brush[]), typeof(HistogrammControl),
        new FrameworkPropertyMetadata(DefaultMultiPlaneBrushes, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(Brush_Changed))
      );
    }

    #region brush
    /// <summary>
    /// The Colors that are used to display the different histograms
    /// </summary>
    private Brush[] Brush
    {
      get { return (Brush[])GetValue(BrushProperty); }
      set { SetValue(BrushProperty, value); }
    }
    private static readonly DependencyProperty BrushProperty;

    private static void Brush_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      if (d is HistogrammControl histogramm)
      {
        var newValue = e.NewValue as Brush[];
        histogramm.UpdateBrushes();
      }
    }

    /// <summary>
    /// The Color that is used to display a MonoPlane Histogramm
    /// </summary>
    public Brush[] MonoPlaneBrushes
    {
      get { return (Brush[])GetValue(MonoPlaneBrushesProperty); }
      set { SetValue(MonoPlaneBrushesProperty, value); }
    }
    private static readonly DependencyProperty MonoPlaneBrushesProperty;

    /// <summary>
    /// The Colors that are used to display a MultiPlane Histogramm
    /// </summary>
    public Brush[] MultiPlaneBrushes
    {
      get { return (Brush[])GetValue(MultiPlaneBrushesProperty); }
      set { SetValue(MultiPlaneBrushesProperty, value); }
    }
    private static readonly DependencyProperty MultiPlaneBrushesProperty;
    #endregion

    #region pen
    /// <summary>
    /// The pen that is used to draw the histogramm
    /// </summary>
    public Pen ForegroundPen
    {
      get { return (Pen)GetValue(ForegroundPenProperty); }
      set { SetValue(ForegroundPenProperty, value); }
    }
    private static readonly DependencyProperty ForegroundPenProperty;
    #endregion

    #region Data

    /// <summary>
    /// the number of times each color value has been used
    /// in the picture
    /// </summary>
    public int[][] Data
    {
      get { return (int[][])GetValue(DataProperty); }
      set { SetValue(DataProperty, value); }
    }
    private static readonly DependencyProperty DataProperty;

    private static void Data_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      if (d is HistogrammControl histogramm)
      {
        var newValue = e.NewValue as int[][];
        histogramm.UpdateGeometries(newValue, histogramm.RenderSize);
        histogramm.UpdateBrushes();
      }
    }
    #endregion

    #region updateHistogram

    private void UpdateGeometries(int[][] data, Size size)
    {
      var commonMax = data.Select(histogram => histogram.Max())
                          .Max();

      if (data != null)
      {
        _streamGeometries = data.Select(plane => CreateGeometry(plane, commonMax, size))
                                .ToArray();
      }
      else
      {
        _streamGeometries = null;
      }
    }

    private StreamGeometry CreateGeometry(int[] histogramData, int maxValue, Size maxSize)
    {
      StreamGeometry geometry = new StreamGeometry();
      geometry.FillRule = FillRule.Nonzero;

      using (StreamGeometryContext context = geometry.Open())
      {
        context.BeginFigure(new Point(0, maxSize.Height), isFilled: true, isClosed: false);

        double xMultiplier = maxSize.Width / histogramData.Length;
        double yMultiplier = maxSize.Height / maxValue;

        for (int i = 0; i < histogramData.Length; i++)
        {
          Point point = (new Point(i * xMultiplier, maxSize.Height - histogramData[i] * yMultiplier));
          context.LineTo(point, true, true);
        }
        context.LineTo(new Point(maxSize.Width, maxSize.Height), isStroked: true, isSmoothJoin: true);
        geometry.Freeze();
      }
      return geometry;
    }

    private void UpdateBrushes()
    {
      if (Data.Length == 1)
      {
        if (MonoPlaneBrushes.Length < Data.Length)
          MonoPlaneBrushes = FillBrushesUp(MonoPlaneBrushes, Data.Length);

        Brush = MonoPlaneBrushes;
      }
      else if (Data.Length > 1)
      {
        if (MultiPlaneBrushes.Length < Data.Length)
          MultiPlaneBrushes = FillBrushesUp(MultiPlaneBrushes, Data.Length);

        Brush = MultiPlaneBrushes;
      }
    }

    private Brush[] FillBrushesUp(Brush[] brush, int planes)
    {
      int missingBrushes = planes - brush.Length;
      List<Brush> brushesList = brush.ToList();
      for (int i = 0; i < missingBrushes; i++)
      {
        brushesList.Add(DefaultBrush);
      }
      return brushesList.ToArray();
    }

    private static readonly Brush DefaultBrush = new SolidColorBrush(Color.FromArgb(128, 0, 0, 0));

    #endregion

    protected override Size ArrangeOverride(Size arrangeBounds)
    {
      var newSize = base.ArrangeOverride(arrangeBounds);

      UpdateGeometries(Data, newSize);

      return newSize;
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
      base.OnRender(drawingContext);

      DrawGeometries(drawingContext);
    }

    private void DrawGeometries(DrawingContext drawingContext)
    {
      if (_streamGeometries == null)
        return;

      foreach (var toDraw in _streamGeometries.Zip(Brush, (geometry, brush) => new { geometry, brush }))
      {
        drawingContext.DrawGeometry(toDraw.brush, ForegroundPen, toDraw.geometry);
      }
    }
   
    private StreamGeometry[] _streamGeometries;
  
  }
}
