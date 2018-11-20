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
  /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
  ///
  /// Step 1a) Using this custom control in a XAML file that exists in the current project.
  /// Add this XmlNamespace attribute to the root element of the markup file where it is 
  /// to be used:
  ///
  ///     xmlns:MyNamespace="clr-namespace:GreyScaleHistogrammWPF"
  ///
  ///
  /// Step 1b) Using this custom control in a XAML file that exists in a different project.
  /// Add this XmlNamespace attribute to the root element of the markup file where it is 
  /// to be used:
  ///
  ///     xmlns:MyNamespace="clr-namespace:GreyScaleHistogrammWPF;assembly=GreyScaleHistogrammWPF"
  ///
  /// You will also need to add a project reference from the project where the XAML file lives
  /// to this project and Rebuild to avoid compilation errors:
  ///
  ///     Right click on the target project in the Solution Explorer and
  ///     "Add Reference"->"Projects"->[Browse to and select this project]
  ///
  ///
  /// Step 2)
  /// Go ahead and use your control in the XAML file.
  ///
  ///     <MyNamespace:Histogramm/>
  ///
  /// </summary>
  public class Histogramm : Control
  {
    public Pen ForegroundPen
    {
      get { return (Pen)GetValue(ForegroundPenProperty); }
      set { SetValue(ForegroundPenProperty, value); }
    }

    public static readonly DependencyProperty ForegroundPenProperty;


    private static readonly Brush[] DefaultBrushes;

    private static readonly Pen DefaultPen;
    static Histogramm()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(Histogramm), new FrameworkPropertyMetadata(typeof(Histogramm)));

      DefaultBrushes = new Brush[]
      {
        new SolidColorBrush(Color.FromArgb(128, 255, 0, 0)),//red
        new SolidColorBrush(Color.FromArgb(128, 0, 255, 0)),//green
        new SolidColorBrush(Color.FromArgb(128, 0, 0, 255)),//blue
        new SolidColorBrush(Color.FromArgb(128, 0, 0, 0)),//black
      };

      DefaultPen = new Pen(Brushes.Black, 1.0);

      DataProperty = DependencyProperty.Register
      (
        nameof(Data), typeof(int[][]), typeof(Histogramm),
        new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(Data_Changed))
      );

      BrushProperty = DependencyProperty.Register
      (
          nameof(Brush), typeof(Brush[]), typeof(Histogramm),
          new FrameworkPropertyMetadata(DefaultBrushes, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(Brush_Changed))
      );

      ForegroundPenProperty = DependencyProperty.Register
      (
          nameof(ForegroundPen), typeof(Pen), typeof(Histogramm),
          new FrameworkPropertyMetadata(DefaultPen, FrameworkPropertyMetadataOptions.AffectsRender)
      );
    }

    protected override Size ArrangeOverride(Size arrangeBounds)
    {
      var newSize = base.ArrangeOverride(arrangeBounds);

      UpdateGeometries(Data, newSize);

      return newSize;
    }


    #region brush
    public Brush[] Brush
    {
      get { return (Brush[])GetValue(BrushProperty); }
      set { SetValue(BrushProperty, value); }
    }
    public static readonly DependencyProperty BrushProperty;

    private static void Brush_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      if (d is Histogramm histogramm)
      {
        var newValue = e.NewValue as Brush[];
        histogramm.UpdateBrushes(newValue.Length);
      }
    }
    #endregion

    #region Data

    public int[][] Data
    {
      get { return (int[][])GetValue(DataProperty); }
      set { SetValue(DataProperty, value); }
    }
    public static readonly DependencyProperty DataProperty;

    private static void Data_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      if (d is Histogramm histogramm)
      {
        var newValue = e.NewValue as int[][];
        histogramm.UpdateGeometries(newValue, histogramm.RenderSize);
        histogramm.UpdateBrushes(newValue?.Length ?? 0);
      }
    }

    private void UpdateGeometries(int[][] data, Size size)
    {
      if (data != null)
      {
        _streamGeometries = data.Select(plane => CreateGeometry(plane, size))
                                .ToArray();
      }
      else
      {
        _streamGeometries = null;
      }
    }

    private StreamGeometry[] _streamGeometries;

    private StreamGeometry CreateGeometry(int[] histogramData, Size maxSize)
    {
      StreamGeometry geometry = new StreamGeometry();
      geometry.FillRule = FillRule.Nonzero;

      using (StreamGeometryContext context = geometry.Open())
      {
        context.BeginFigure(new Point(0, maxSize.Height), isFilled: true, isClosed: false);

        double xMultiplier = maxSize.Width / histogramData.Length;
        double yMultiplier = (maxSize.Height / histogramData.Max());

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

    private void UpdateBrushes(int brushes)
    {
    }

    #endregion

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




  }
}
