using Stemmer.Cvb;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Image image = Image.FromFile(@"C:\Users\jsiedersberger\Pictures\Saved Pictures\pexels-photo-285286.jpeg");
            List<int> values = new List<int>();
            Stopwatch executionWatch = new Stopwatch();
            executionWatch.Start();
            CopyPixelsWithValue(image, values);
            executionWatch.Stop();
            Console.WriteLine(executionWatch.ElapsedMilliseconds.ToString());
        }

        static void CopyPixelsWithValue(Image source, List<int> values)
        {
            unsafe
            {
                int width = source.Width;
                int height = source.Height;

                var linearAccess = source.Planes[0].GetLinearAccess();
                var basePtr = linearAccess.BasePtr;
                long greyScale = linearAccess.BasePtr.ToInt64();

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
        }
    }
}
