using Stemmer.Cvb;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GreyScaleHistogrammWPF
{
  /// <summary>
  /// The MainWindow of the Application
  /// </summary>
  public class MainViewModel : ViewModelBase, INotifyPropertyChanged
  {
    /// <summary>
    /// Calculates the Histogramm and 
    /// sets the button property 
    /// </summary>
    public MainViewModel()
    {
      Calculate = new DelegateAction(CalculateHistogramm);

      Image = Image.FromFile(@"C:\Users\jsiedersberger\Pictures\Saved Pictures\lamborghini.jpg");
      CalculateHistogramm();
    }

    private void CalculateHistogramm()
    {
      var elapsedTime = Measure(() =>
      {
        Data = Histogram.Create(Image);
      });

      Time = $"{elapsedTime.TotalMilliseconds}ms";
    }

    private static TimeSpan Measure(Action action)
    {
      Stopwatch stopwatch = new Stopwatch();
      stopwatch.Start();
      action();
      stopwatch.Stop();
      return TimeSpan.FromMilliseconds(stopwatch.ElapsedMilliseconds);
    }

    /// <summary>
    /// The image to display.
    /// </summary>
    public Image Image { get; private set; }

    /// <summary>
    /// Contains how often a color value appeard in the image
    /// </summary>
    public int[][] Data { get; private set; }

    /// <summary>
    /// The duration of the histogramm calculation
    /// </summary>
    public string Time
    {
      get => _time;
      set
      {
        _time = value;
        FirePropertyChanged();
      }
    }
    private string _time;

    public ICommand Calculate { get; set; }
  }
}
