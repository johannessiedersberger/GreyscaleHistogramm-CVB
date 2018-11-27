using Stemmer.Cvb;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreyScaleHistogrammWPF
{
  public static class Histogram
  {
    /// <summary>
    /// Creates an array of how often a color appeared
    /// in each plane of the image
    /// </summary>
    /// <param name="image"></param>
    /// <returns></returns>
    public static int[][] Create(Image image)
    {
      Debug.Assert(image != null);
      Debug.Assert(image.Planes[0].DataType == DataTypes.Int8BppUnsigned);

      return image.Planes
                  .Select(plane => plane.ToHistogram())
                  //.Select(plane => plane.AllPixels.ToHistogram())
                  .ToArray();
    }
  }
  
  static class HistogramHelper
  {
    public static unsafe int[] ToHistogram(this ImagePlane plane)
    {
      var histogram = new int[byte.MaxValue + 1];

      var size = plane.Parent.Size;
      var access = plane.GetLinearAccess();

      var xInc = access.XInc.ToInt64();
      var yInc = access.YInc.ToInt64();
      var pBase = (byte*)access.BasePtr;
      
      for (int y = 0; y < size.Height; y++)
      {
        for (int x = 0; x < size.Width; x++)
        {
          var pPixel = pBase + x * xInc + y * yInc;
          ++histogram[*pPixel];
        }
      }
      return histogram;
    }

    public static unsafe int[] ToHistogram(this IEnumerable<IntPtr> pixels)
    {
      var histogram = new int[byte.MaxValue + 1];

      foreach (var pixel in pixels)
      {
        ++histogram[*(byte*)pixel];
      }

      return histogram;
    }
  }
}
