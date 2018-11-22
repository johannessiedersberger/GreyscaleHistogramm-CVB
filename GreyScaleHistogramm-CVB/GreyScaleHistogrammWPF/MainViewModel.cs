using Stemmer.Cvb;
using System;
using System.Collections.Generic;
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
        Data = Histogram.Create(image);
      }
    }

    /// <summary>
    /// Contains how often a color value appeard in the image
    /// </summary>
    public int[][] Data { get; private set; }
  }
}
