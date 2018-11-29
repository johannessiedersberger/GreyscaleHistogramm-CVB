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
    public static Image Calculate(Image image)
    {
      return CreateImage(CalculateAverage(image.Planes[0]));
    }

    private static Image CreateImage(int[,] imageValues)
    {
      Image image = new Image(imageValues.GetLength(0), imageValues.GetLength(1));
      var access = image.Planes[0].GetLinearAccess<byte>();

      var size = image.Size;
      for (int y = 0; y < size.Height; y++)
      {
        for (int x = 0; x < size.Width; x++)
        {
          access[x, y] = (byte)imageValues[x, y];
        }
      }
      return image;
    }

    private static int[,] CalculateAverage(ImagePlane plane)
    {
      var size = plane.Parent.Size;
      int[,] imageValues = new int[size.Width, size.Height];

      for (int y = 0; y < size.Height; y++)
      {
        for (int x = 0; x < size.Width; x++)
        {
          imageValues[x,y] = PixelsInNeighborhod(plane, x, y);
        }
      }
      return imageValues;
    }

    private static int PixelsInNeighborhod(ImagePlane plane, int x, int y)
    {
      int pixelValueSum = 0;
      int numberOfPixels = 0;

      for (int kernelY = y-1; kernelY <= y+1; kernelY++)
      {
        for (int kernelX = x-1; kernelX <= x+1; kernelX++)
        {
          if(IsInsideImage(plane, x, y))
          {
            numberOfPixels++;
            pixelValueSum += GetPixelValue(plane, x, y);
          }
        }
      }

      return pixelValueSum / numberOfPixels;
    }

    private static bool IsInsideImage(ImagePlane plane, int x, int y)
    {
      var size = plane.Parent.Size;
      return 
        IsInArea(x, 0, size.Width) && IsInArea(y, 0, size.Height);
    }

    private static bool IsInArea(int test, int min, int max)
    {
      return test >= min && test < max;
    }

    private static unsafe byte GetPixelValue(ImagePlane plane, int x, int y)
    {
      Debug.Assert(x >= 0 && x < plane.Parent.Width);
      Debug.Assert(y >= 0 && y < plane.Parent.Height);

      var size = plane.Parent.Size;
      var access = plane.GetLinearAccess();

      var xInc = access.XInc.ToInt64();
      var yInc = access.YInc.ToInt64();
      var pBase = (byte*)access.BasePtr;

      var pPixel = pBase + x * xInc + y * yInc;
      return *pPixel;

    }
  }
}
