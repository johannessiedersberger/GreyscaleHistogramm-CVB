using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stemmer.Cvb;

namespace RGBHistogrammWPF
{
  public static class PixelAverage
  {
    public static Image Calculate(Image image, int fieldSize)
    {
      return CreateNewImage(image, fieldSize);
    }

    private static Image CreateNewImage(Image image, int fieldSize)
    {
      var size = image.Size;
      Image newImage = new Image(size.Width, size.Height);
      var access = newImage.Planes[0].GetLinearAccess<byte>();

      for (int y = 0; y < size.Height; y++)
      {
        for (int x = 0; x < size.Width; x++)
        {
          access[x, y] = (byte)PixelsInNeighborhod(image.Planes[0], x, y, fieldSize);
        }
      }
      return newImage;
    }

    private static unsafe int PixelsInNeighborhod(ImagePlane plane, int x, int y, int fieldSize)
    {
      int pixelValueSum = 0;
      int numberOfPixels = 0;

      var size = plane.Parent.Size;
      var access = plane.GetLinearAccess();

      var xInc = access.XInc.ToInt64();
      var yInc = access.YInc.ToInt64();
      var pBase = (byte*)access.BasePtr;

      for (int kernelY = y-fieldSize; kernelY <= y+fieldSize; kernelY++)
      {
        var ypart = kernelY * yInc + pBase;
        for (int kernelX = x-fieldSize; kernelX <= x+fieldSize; kernelX++)
        {
          if(IsInsideImage(plane.Parent.Size, kernelX, kernelY))
          {
            numberOfPixels++;
            pixelValueSum += *(kernelX * xInc + ypart);
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
