using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using RGBHistogrammWPF;
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

    [Test]
    public unsafe void TestPixelAverage()
    {
      //given
      using(var oldImage = CreateRampImage())
      {
        //when
        var newImage = PixelAverage.Calculate(oldImage, 1);

        //old image
        var oldSize = oldImage.Planes[0].Parent.Size;
        var oldaccess = oldImage.Planes[0].GetLinearAccess();

        var oldXInc = oldaccess.XInc.ToInt64();
        var oldYInc = oldaccess.YInc.ToInt64();
        var oldpBase = (byte*)oldaccess.BasePtr;

        //new image
        var newSize = newImage.Planes[0].Parent.Size;
        var newAccess = newImage.Planes[0].GetLinearAccess();

        var newXInc = newAccess.XInc.ToInt64();
        var newYInc = newAccess.YInc.ToInt64();
        var newPBase = (byte*)newAccess.BasePtr;
      }
    }
  }
}
