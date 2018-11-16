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
    private static readonly Pen ForegroundPen = new Pen(Brushes.Black, 1.0);

    #region Data

    public int[][] Data
    {
      get { return (int[][])GetValue(DataProperty); }
      set { SetValue(DataProperty, value); }
    }

    // Using a DependencyProperty as the backing store for GreyScaleHistogramm.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty DataProperty;

    #endregion

    static Histogramm()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(Histogramm), new FrameworkPropertyMetadata(typeof(Histogramm)));

      DataProperty = DependencyProperty.Register
      (
        nameof(Data), typeof(int[][]), typeof(Histogramm),
        new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender)
      );
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
      base.OnRender(drawingContext);

      if (Data == null)
        return;

      StreamGeometry[] geometries = new StreamGeometry[Data.Length];

      for (int plane = 0; plane < geometries.Length; plane++)
        geometries[plane] = CreateGeometries(Data[plane]);    

      for (int plane = 0; plane < geometries.Length; plane++)   
        drawingContext.DrawGeometry(GetBrush(Data.Length, plane),ForegroundPen , geometries[plane]);
    }

    private StreamGeometry CreateGeometries(int[] histogramData)
    {
      StreamGeometry geometry = new StreamGeometry();
      geometry.FillRule = FillRule.Nonzero;
      var maxSize = RenderSize;

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

    private Brush GetBrush(int numberOfPlanes, int currentPlane)
    {
      if (numberOfPlanes == 1) //Mono
        return BackgroundBrush[3];
      if (numberOfPlanes >= 3) //RGB + Alpha
        return BackgroundBrush[currentPlane];
      throw new ArgumentException("No Background Brush selected");
    }

    private static readonly List<Brush> BackgroundBrush = new List<Brush>() {
      new SolidColorBrush(Color.FromArgb(128,255,0,0)),
      new SolidColorBrush(Color.FromArgb(128,0,255,0)),
      new SolidColorBrush(Color.FromArgb(128,0,0,255)),
      new SolidColorBrush(Color.FromArgb(128,0,0,0)),
      };
  }
}
