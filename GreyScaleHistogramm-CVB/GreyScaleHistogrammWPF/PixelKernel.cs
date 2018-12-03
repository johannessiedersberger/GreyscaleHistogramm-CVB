using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stemmer.Cvb;

namespace RGBHistogrammWPF
{
  public static class PixelKernel
  {
    public static Image Calculate(Image image)
    {
      return CreateNewImage(CreateNewImage(image, new int[3, 3]
      {
        {-1,-1,-1},
        {0, 0, 0},
        {1, 1, 1},
      }), new int[3, 3]{
         {-1,-1,-1},
        {0, 0, 0},
        {1, 1, 1},
       
      });
    }

    private static Image CreateNewImage(Image image, int[,] kernel)
    {
      var size = image.Size;
      Image newImage = new Image(size.Width, size.Height);
      var access = newImage.Planes[0].GetLinearAccess<byte>();

      PixelHelper pixelHelper = new PixelHelper(image.Planes[0]);

      for (int y = 0; y < size.Height; y++)
      {
        for (int x = 0; x < size.Width; x++)
        {
          access[x, y] = (byte)KernelOperation(pixelHelper, x, y, kernel);
        }
      }
      return newImage;
    }

    private static unsafe int KernelOperation(PixelHelper pixelHelper, int x, int y, int[,] kernel)
    {
      int valueSum = 0;

      int yArray = 0;
      for (int kernelY = y - 1; kernelY <= y + 1; kernelY++)
      {
        var ypart = kernelY * pixelHelper.YInc + pixelHelper.PBase;
        int xArray = 0;
        for (int kernelX = x - 1; kernelX <= x + 1; kernelX++)
        {
          if (IsInsideImage(pixelHelper.Size, kernelX, kernelY))
          {
            var pixelValue = *(kernelX * pixelHelper.XInc + ypart);
            valueSum += pixelValue * kernel[yArray,xArray];
          }
          xArray++;
        }
        yArray++;
      }
      return valueSum / kernel.Length;
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
