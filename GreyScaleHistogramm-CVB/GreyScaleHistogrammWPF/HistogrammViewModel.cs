using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Stemmer.Cvb;

namespace GreyScaleHistogrammWPF
{
  public class HistogrammViewModel
  {
    public PointCollection PointCollection { get; private set; } = CreatePointCollection();

    private static PointCollection CreatePointCollection()
    {
      using (var image = Image.FromFile(@"C:\Users\jsiedersberger\Pictures\Saved Pictures\pexels-photo-285286.jpeg"))
      {
        var histogram = CreateHistogram(image);
      }

      return null;
    }

    private static int[] CreateHistogram(Image image)
    {
      Debug.Assert(image != null);
      Debug.Assert(image.Planes[0].DataType == DataTypes.Int8BppUnsigned);

      var histogram = new int[byte.MaxValue + 1];

      var access = image.Planes[0].GetLinearAccess<byte>();

      var size = image.Size;
      for (int y = 0; y < size.Height; y++)
      {
        for (int x = 0; x < size.Width; x++)
        {
          var pixelValue = access[x, y];
          ++histogram[pixelValue];
        }
      }

      return histogram;
    }

    private static void CopyPixelsWithValue(Image source, List<int> values)
    {

      unsafe
      {
        int width = source.Width;
        int height = source.Height;

        var linearAccess = source.Planes[0].GetLinearAccess();
        var basePtr = linearAccess.BasePtr;

        int xInc = (int)linearAccess.XInc;
        int yInc = (int)linearAccess.YInc;

        for (int y = 0; y < /*height*/1; y++)
        {
          var line = (byte*)basePtr.ToPointer() + y * yInc;
          for (int x = 0; x < width; x++)
          {
            var pixel = *(line + x * xInc);
            values.Add(pixel);
          }
        }
      }
    }
  }
}
