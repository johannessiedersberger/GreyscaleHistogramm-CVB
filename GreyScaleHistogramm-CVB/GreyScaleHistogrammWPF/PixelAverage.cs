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
      return CreateImage(CalculateAverage(image.Planes[0], fieldSize));
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

    private static int[,] CalculateAverage(ImagePlane plane, int fieldSize)
    {
      var size = plane.Parent.Size;
      int[,] imageValues = new int[size.Width, size.Height];

      for (int y = 0; y < size.Height; y++)
      {
        for (int x = 0; x < size.Width; x++)
        {
          imageValues[x,y] = PixelsInNeighborhod(plane, x, y, fieldSize);
        }
      }
      return imageValues;
    }

    private static int PixelsInNeighborhod(ImagePlane plane, int x, int y, int fieldSize)
    {
      int pixelValueSum = 0;
      int numberOfPixels = 0;

      fieldSize /= 2;

      for (int kernelY = y-fieldSize; kernelY <= y+fieldSize; kernelY++)
      {
        for (int kernelX = x-fieldSize; kernelX <= x+fieldSize; kernelX++)
        {
          if(IsInsideImage(plane, kernelX, kernelY))
          {
            numberOfPixels++;
            pixelValueSum += GetPixelValue(plane, kernelX, kernelY);
          }
        }
      }

      return pixelValueSum / numberOfPixels;
    }

    private static bool IsInsideImage(ImagePlane plane, int x, int y)
    {
      var size = plane.Parent.Size;
      return IsInRange(x, 0, size.Width) && IsInRange(y, 0, size.Height);
    }

    private static bool IsInRange(int test, int min, int max)
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
