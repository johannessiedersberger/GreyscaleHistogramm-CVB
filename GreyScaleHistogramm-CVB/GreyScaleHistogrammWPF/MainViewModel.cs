using Stemmer.Cvb;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreyScaleHistogrammWPF
{
  public class MainViewModel
  {
    public MainViewModel()
    {
      using (var image = Image.FromFile(@"C:\Users\jsiedersberger\Pictures\Saved Pictures\lamborghini.jpg"))
      {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        Data = Histogram.Create(image);
        stopwatch.Stop();
        Time = stopwatch.ElapsedMilliseconds.ToString() + "ms";
      }
    }

    /// <summary>
    /// Contains how often a color value appeard in the image
    /// </summary>
    public int[][] Data { get; private set; }

    public string Time { get; set; }
  }
}
