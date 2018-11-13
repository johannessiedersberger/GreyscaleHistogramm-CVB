using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Stemmer.Cvb;

namespace GreyScaleHistogrammWPF
{
    public class HistogrammViewModel
    {
        public PointCollection PointCollection { get; private set; } = CreatePointCollection();

        private static PointCollection CreatePointCollection()
        {
            List<int> greyScaleValues = new List<int>();
            CopyPixelsWithValue(Image.FromFile(@"C:\Users\jsiedersberger\Pictures\Saved Pictures\pexels-photo-285286.jpeg"), greyScaleValues);
            int max = greyScaleValues.Max();

            PointCollection points = new PointCollection();
            // first point (lower-left corner)
            points.Add(new Point(0, max));
            // middle points
            for (int i = 0; i < greyScaleValues.Count(); i++)
            {
                points.Add(new Point(i, max - greyScaleValues[i]));
            }
            // last point (lower-right corner)
            points.Add(new Point(greyScaleValues.Count() - 1, max));
            return points;
        }

        private static void CopyPixelsWithValue(Image source, List<int> values)
        {
            
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
                    if (y >= 1)
                        return;
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
