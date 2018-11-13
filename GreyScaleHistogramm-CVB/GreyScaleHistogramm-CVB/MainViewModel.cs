using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stemmer.Cvb;

namespace GreyScaleHistogramm_CVB
{
    public class MainViewModel
    {

        public int[] ChartValues { get; set; } = CopyPixelsWithValue(Image.FromFile(_path));

        private static string _path = @"C:\Users\jsiedersberger\Pictures\Saved Pictures\pexels-photo-285286.jpeg";
        public static int[] CopyPixelsWithValue(Image source)
        {
            List<int> values = new List<int>();
            unsafe
            {
                int width = source.Width;
                int height = source.Height;

                var linearAccess = source.Planes[0].GetLinearAccess();
                var basePtr = linearAccess.BasePtr;

                int xInc = (int)linearAccess.XInc;
                int yInc = (int)linearAccess.YInc;


                for (int y = 0; y < height; y++)
                {
                    var line = (byte*)basePtr.ToPointer() + y * yInc;
                    for (int x = 0; x < width; x++)
                    {
                        var pixel = *(line + x * xInc);
                        values.Add(pixel);
                    }
                }
            }
            return values.ToArray();
        }
    }
}
