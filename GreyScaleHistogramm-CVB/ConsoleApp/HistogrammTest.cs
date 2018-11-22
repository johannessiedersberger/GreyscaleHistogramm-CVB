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
    [Test]
    public void TestCreateHistogramm()
    {
      var max = byte.MaxValue + 1;
      Image image = new Image(max, max);
      image.Initialize(0);
      var access = image.Planes[0].GetLinearAccess<byte>();

      for (int x = 0; x < image.Width; x++)
      {
        for (int y = 0; y < image.Height; y++)
        {
          access[x, y] = (byte)y;
        }
      }

      foreach (int colorValue in Histogram.Create(image)[0])
        Assert.That(colorValue, Is.EqualTo(max));

      HistogrammControl histogramm = new HistogrammControl();
      
    }

  
  }
}
