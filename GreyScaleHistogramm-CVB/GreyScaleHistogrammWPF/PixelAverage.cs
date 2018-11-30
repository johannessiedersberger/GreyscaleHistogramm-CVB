using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Stemmer.Cvb;

namespace RGBHistogrammWPF
{
  public unsafe class PixelHelper
  {
    public Size2D Size;
    public LinearAccessData Access;
    public long XInc;
    public long YInc;
    public byte* PBase;

    public unsafe PixelHelper(ImagePlane plane)
    {
      Size = plane.Parent.Size;
      Access = plane.GetLinearAccess();

      XInc = Access.XInc.ToInt64();
      YInc = Access.YInc.ToInt64();
      PBase = (byte*)Access.BasePtr;
    }
  }

  public static class PixelAverage
  {
    public static Image Calculate(Image image, int fieldSize)
    {
      return CreateNewImage(image, fieldSize);
    }

    private static Image CreateNewImage(Image image, int fieldSize)
    {
      Stopwatch watch = new Stopwatch();
      watch.Start();

      var size = image.Size;
      Image newImage = new Image(size.Width, size.Height);
      var access = newImage.Planes[0].GetLinearAccess<byte>();

      PixelHelper pixelHelper = new PixelHelper(image.Planes[0]);

      for (int y = 0; y < size.Height; y++)
      {
        for (int x = 0; x < size.Width; x++)
        {
          access[x, y] = (byte)PixelsInNeighborhod(pixelHelper, x, y, fieldSize);
        }
      }
      
      watch.Stop();
      MessageBox.Show(watch.ElapsedMilliseconds.ToString());


      return newImage;
    }

    private static unsafe int PixelsInNeighborhod(PixelHelper pixelHelper, int x, int y, int fieldSize)
    {
      int pixelValueSum = 0;
      int numberOfPixels = 0;

      for (int kernelY = y-fieldSize; kernelY <= y+fieldSize; kernelY++)
      {
        var ypart = kernelY * pixelHelper.YInc + pixelHelper.PBase;
        for (int kernelX = x-fieldSize; kernelX <= x+fieldSize; kernelX++)
        {
          if(IsInsideImage(pixelHelper.Size, kernelX, kernelY))
          {
            numberOfPixels++;
            pixelValueSum += *(kernelX * pixelHelper.XInc+ ypart);
          }
        }
      }
      return pixelValueSum / numberOfPixels;
    }

    private static bool IsInsideImage(Size2D size, int x, int y)
    {
      return IsInRange(x, 0, size.Width) && IsInRange(y, 0, size.Height);
    }

    private static bool IsInRange(int test, int min, int max)
    {
      return test >= min && test < max;
    }
  }


}
