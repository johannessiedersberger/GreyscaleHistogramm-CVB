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
    public void TestArray()
    {
      Image image = new Image(256, 256);
      image.Initialize(0);
      var access = image.Planes[0].GetLinearAccess<byte>();
      access[42, 17] = 1;
  
      //int[] colorValues = CreateAllColorValues();
      //for (int x = 0; x < image.Width; x++)
      //{
      //  for (int y = 0; y < image.Height; y++)
      //  {
      //    access[x, y] = (byte)y;
      //  }
      //}
      var histogram = HistogrammViewModel.CreateHistogram(image);
    }

    private byte[] CreateAllColorValues()
    {
      byte[] imageData = new byte[256];
      for (byte i = 0; i < imageData.Length; i++)
      {
        imageData[i] = i;
      }
      return imageData;
    }
  }
}
