using Stemmer.Cvb;
using System.Collections.Generic;
using System.Diagnostics;

namespace GreyScaleHistogrammWPF
{
  public class HistogrammViewModel
  {
    public HistogrammViewModel()
    {
      using (var image = Image.FromFile(@"C:\Users\jsiedersberger\Pictures\Saved Pictures\lamborghini.jpg"))
      {
        Histogram = CreateHistogram(image);
      }
    }

    public int[][] Histogram { get; }

    private static int[][] CreateHistogram(Image image)
    {
      Debug.Assert(image != null);
      Debug.Assert(image.Planes[0].DataType == DataTypes.Int8BppUnsigned);

      var histograms = new int[image.Planes.Count][];
   
      for (int i = 0; i < image.Planes.Count; i++)
      {
        var histogram = new int[byte.MaxValue + 1];

        var access = image.Planes[i].GetLinearAccess<byte>();     
        var size = image.Size;

        for (int y = 0; y < size.Height; y++)
        {
          for (int x = 0; x < size.Width; x++)
          {
            var pixelValue = access[x, y];
            ++histogram[pixelValue];
          }
        }
        histograms[i] = (histogram);
      }      
      return histograms;
    }
  }
}
