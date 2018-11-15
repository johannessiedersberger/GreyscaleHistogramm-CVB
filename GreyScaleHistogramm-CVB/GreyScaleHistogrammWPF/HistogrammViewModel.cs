using Stemmer.Cvb;
using System.Diagnostics;

namespace GreyScaleHistogrammWPF
{
  public class HistogrammViewModel
  {
    public HistogrammViewModel()
    {
      using (var image = Image.FromFile(@"C: \Users\jsiedersberger\Pictures\Saved Pictures\lion.jpg"))
      {
        Histogram = CreateHistogram(image);
      }
    }

    public int[] Histogram { get; }

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
  }
}
