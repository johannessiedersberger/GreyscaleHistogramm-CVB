using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using GreyScaleHistogrammWPF;
using Stemmer.Cvb;

namespace ConsoleApp
{
  public class HistogrammTest
  {
    private const int MaxCount = byte.MaxValue + 1;

    /// <summary>
    /// Checks if the Histogramm is created correctly
    /// </summary>
    [Test]
    public void TestCreateHistogramm()
    {
      // given
      using (var image = CreateRampImage())
      {
        // when
        var histogram = Histogram.Create(image)[0];

        // then
        Assert.That(histogram, Is.EquivalentTo(Enumerable.Repeat(MaxCount, 256)));
      }
    }

    private static Image CreateRampImage()
    {
      var image = new Image(MaxCount, MaxCount);
      var access = image.Planes[0].GetLinearAccess<byte>();

      var size = image.Size;
      for (int y = 0; y < size.Height; y++)
      {
        for (int x = 0; x < size.Width; x++)
        {
          access[x, y] = (byte)y;
        }
      }

      return image;
    }
  }
}
