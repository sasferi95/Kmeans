using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;

namespace kmeans.Paralell
{
    class ParallelLogic2
    {
        static Random rnd = new Random();
        class Center
        {
            public readonly object lockobject = new object();
            //koordináták
            public int Red { get; set; }
            public int Blue { get; set; }
            public int Green { get; set; }
            //hozzátartozó értékek
            public int SumRed { get; set; }
            public int SumBlue { get; set; }
            public int SumGreen { get; set; }
            public int SumInGroup { get; set; }
            public double SumDistance { get; set; }
            public Center(int red, int green, int blue)
            {
                Red = red;
                Blue = blue;
                Green = green;
                SumRed = 0;
                SumBlue = 0;
                SumGreen = 0;
                SumInGroup = 0;
                SumDistance = 0;
            }
            public void SetCenter(int red, int green, int blue)
            {
                Red = red;
                Blue = blue;
                Green = green;
            }
            public void Reset()
            {
                SumRed = 0;
                SumBlue = 0;
                SumGreen = 0;
                SumInGroup = 0;
                SumDistance = 0;
            }

        }
        public int Solve(int k, string path)
        {
            byte[] clustered = SolveClustering(k, path);
            return GetClusterFrameandResult(clustered, path);
        }
        //https://stackoverflow.com/questions/2016406/converting-bitmap-pixelformats-in-c-sharp
        Bitmap IsInGoodFormat(Bitmap bmp)
        {
            if (bmp.PixelFormat != System.Drawing.Imaging.PixelFormat.Format32bppArgb)
            {

                Bitmap clone = new Bitmap(bmp.Width, bmp.Height,
                    System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

                using (Graphics gr = Graphics.FromImage(clone))
                {
                    gr.DrawImage(bmp, new Rectangle(0, 0, clone.Width, clone.Height));
                }
                return clone;
            }
            return null;
        }
        

        byte[] SolveClustering(int k,string path)
        {
            Bitmap bmp = new Bitmap(path);
            bmp = IsInGoodFormat(bmp) ?? bmp;

            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            System.Drawing.Imaging.BitmapData bmpData =
            bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
            bmp.PixelFormat);

            IntPtr ptr = bmpData.Scan0;
            int bytes = Math.Abs(bmpData.Stride) * bmp.Height;
            byte[] rgbValues = new byte[bytes];
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);
            //-----------------------------------------------------
            Dictionary<int, Center> bestcenters = Clustering(k, rgbValues);

            byte[] clustered = ModifileColours(rgbValues, bestcenters, bmp.Width, bmp.Height);


            //-----------------------------------------------------
            System.Runtime.InteropServices.Marshal.Copy(clustered, 0, ptr, bytes);
            bmp.UnlockBits(bmpData);
            try
            {
                bmp.Save("pandatest0.png");
            }
            catch
            {
                bmp.Save("pandatest1.png");
            }

            return clustered; 
        }
        private int GetClusterFrameandResult(byte[] clustered,string path)
        {
            //******************************
            Bitmap secondbmp = new Bitmap(path);
            secondbmp = IsInGoodFormat(secondbmp) ?? secondbmp;

            Rectangle rect2 = new Rectangle(0, 0, secondbmp.Width, secondbmp.Height);
            System.Drawing.Imaging.BitmapData secondbmpData =
            secondbmp.LockBits(rect2, System.Drawing.Imaging.ImageLockMode.ReadWrite,
            secondbmp.PixelFormat);
            IntPtr ptr2 = secondbmpData.Scan0;
            int bytes2 = Math.Abs(secondbmpData.Stride) * secondbmp.Height;
            byte[] rgbValues2 = new byte[bytes2];
            System.Runtime.InteropServices.Marshal.Copy(ptr2, rgbValues2, 0, bytes2);
            //-----------------------------------------------------
            byte[] segmented = SetFrame(rgbValues2, clustered, secondbmp.Width, secondbmp.Height);


            //-----------------------------------------------------
            System.Runtime.InteropServices.Marshal.Copy(segmented, 0, ptr2, bytes2);
            secondbmp.UnlockBits(secondbmpData);
            try
            {
                secondbmp.Save("pandaresult0.png");
                return 0;
            }
            catch (System.Runtime.InteropServices.ExternalException e)
            {
                secondbmp.Save("pandaresult1.png");
                return 1;
            }
            //******************************



            //bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
            //bmp.PixelFormat);
            //IntPtr ptr2 = bmpData.Scan0;
            
            //System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);
            //bmp.UnlockBits(bmpData);
            //try
            //{
            //    bmp.Save("pandaresult0.png");
            //    return 0;
            //}
            //catch (System.Runtime.InteropServices.ExternalException e)
            //{
            //    bmp.Save("pandaresult1.png");
            //    return 1;
            //}
        }

        private Dictionary<int, Center> Clustering(int k, byte[] colours)
        {
            #region init
            double mindistance = double.MaxValue;
            Dictionary<int, Center> bestcenters = new Dictionary<int, Center>();
            #endregion
            //10x futtatjuk le
            for (int i = 0; i < 10; i++)
            {
                Dictionary<int, Center> centerInformaitons = new Dictionary<int, Center>();//rgb+db center mozgatáshoz
                InitDictionary(colours, centerInformaitons, k, colours.Length);

                //jelenleg fixen 20x léptetjük a középpontot
                for (int z = 0; z < 20; z++)
                {
                    ResetCentorids(centerInformaitons);
                    Parallel.For(0, colours.Length / 4, idx =>
                    {
                        int oneColour = idx * 4;
                        //legközelebbi centroidot adja vissza, és a távolságot hozzá
                        KeyValuePair<int, double> closest = SmallestDistance(centerInformaitons, colours[oneColour + 2], colours[oneColour + 1], colours[oneColour]);
                        int centoirdId = closest.Key;
;
                        lock (centerInformaitons[centoirdId].lockobject)
                        {
                            centerInformaitons[centoirdId].SumRed += colours[oneColour + 2];
                            centerInformaitons[centoirdId].SumGreen += colours[oneColour + 1];
                            centerInformaitons[centoirdId].SumBlue += colours[oneColour];
                            centerInformaitons[centoirdId].SumDistance += closest.Value;
                            centerInformaitons[centoirdId].SumInGroup += 1;
                        }
                        
                    });
                    MoveCentroids(centerInformaitons);

                }
                //összehasonlítani hogy jobb - e mint az korábbiak
                double actualdistance = centerInformaitons.Values.Sum(x => x.SumDistance);
                if (actualdistance < mindistance)
                {

                    mindistance = actualdistance;
                    bestcenters = centerInformaitons;
                }
            }
            return bestcenters;
        }


        void InitDictionary(byte[] colours, Dictionary<int, Center> centerInformaitons, int k, int length)
        {
            for (int i = 0; i < k; i++)
            {
                int idx = rnd.Next(0, length) / 4;
                int r, g, b = 0;
                r = colours[idx * 4 + 2];
                g = colours[idx * 4 + 1];
                b = colours[idx * 4];
                centerInformaitons.Add(i, new Center(r, g, b));
            }
        }

        KeyValuePair<int, double> SmallestDistance(Dictionary<int, Center> centers, int r, int g, int b)
        {
            int min_center = 0;
            double min_distance = double.MaxValue;

            for (int i = 0; i < centers.Count; i++)
            {
                //Euklideszi távolság
                double distance = Math.Sqrt(Math.Pow((centers[i].Red - r), 2) + Math.Pow((centers[i].Green - g), 2) + Math.Pow((centers[i].Blue - b), 2));
                if (distance < min_distance)
                {
                    min_center = i;
                    min_distance = distance;
                }
            }
            return new KeyValuePair<int, double>(min_center, min_distance);
        }
        private void MoveCentroids(Dictionary<int, Center> centerInformaitons)
        {
            Parallel.For(0, centerInformaitons.Count, i => {
                Tuple<int, int, int> avg = CalcAvg(centerInformaitons[i]);
                centerInformaitons[i].SetCenter(avg.Item1, avg.Item2, avg.Item3);
            });
        }
        void ResetCentorids(Dictionary<int, Center> centerInformaitons)
        {
            Parallel.For(0, centerInformaitons.Count, idx => {
                centerInformaitons[idx].Reset();
            });
        }
        Tuple<int, int, int> CalcAvg(Center center)
        {
            int numberInGroup = center.SumInGroup;
            if (numberInGroup != 0)
            {
                int red = (int)center.SumRed / numberInGroup;
                int green = (int)center.SumGreen / numberInGroup;
                int blue = (int)center.SumBlue / numberInGroup;
                return new Tuple<int, int, int>(red, green, blue);
            }
            else
                return new Tuple<int, int, int>(center.Red, center.Green, center.Blue);
        }

        byte[] ModifileColours(byte[] colours, Dictionary<int, Center> bestcenters, int width, int height)
        {
            int sor = 0;

            int numberofpixels = colours.Length / 4;
            byte[] colours2 = colours.ToArray();

            Parallel.For(0, numberofpixels, i =>
            {
                int idx = SmallestDistance(bestcenters, colours[i * 4 + 2], colours[i * 4 + 1], colours[i * 4]).Key;
                CopyValues(ref colours2[(i) * 4], ref colours2[((i) * 4) + 1], ref colours2[((i) * 4) + 2], ref colours2[((i) * 4) + 3],
                         (byte)bestcenters[idx].Blue, (byte)bestcenters[idx].Green, (byte)bestcenters[idx].Red, (byte)255);
            });
            return colours2;
        }

        byte[] SetFrame(byte[] colours, byte[] colourssegmented, int width, int height)
        {
            int numberofpixels = colours.Length / 4;
            byte[] withframe = colours.ToArray();


            Parallel.For(0, height, sor =>
            {
                for (int i = (sor * width) % numberofpixels; i < ((sor + 1) * width) % numberofpixels; i++)
                {
                    for (int k = -1; k < 2; k++)
                    {
                        for (int z = -1; z < 2; z++)
                        {
                            if (!IsSame(colourssegmented, i, k, z, width, height))
                            {
                                CopyValues(ref withframe[(i) * 4], ref withframe[((i) * 4) + 1], ref withframe[((i) * 4) + 2], ref withframe[((i) * 4) + 3], (byte)0);
                                //break;
                            }
                        }
                    }
                }


            });
            
            return withframe;
        }
        bool IsSame(byte[] colours, int idx, int x, int y, int width, int heigth)
        {
            if (idx % width == 0 && x == -1)//nincs bal szomszéd és balra nézzük
                return true;
            if (idx % width == width - 1 && x == 1)//nincs jobbra szomszéd és jobbra nézzük
                return true;
            if (idx / width == 0 && y == -1)//nincs felfele szomszéd
                return true;
            if ((idx / width) == heigth - 1 && y == 1)//alsó sorban vagyunk és lefel néznénk
                return true;

            int neighbourb = (idx + x) * 4 + (width * 4 * y);//szomszéd ugyanaz-e

            if (neighbourb > -1 && neighbourb < colours.Length)
            {
                if (colours[neighbourb] != colours[idx * 4])
                    return false;
                if (colours[neighbourb + 1] != colours[idx * 4 + 1])
                    return false;
                if (colours[neighbourb + 2] != colours[idx * 4 + 2])
                    return false;
            }

            return true;
        }
        void CopyValues(ref byte ob, ref byte og, ref byte or, ref byte oa, byte value)//n mint new, o mint original
        {
            oa = 255;
            or = value;
            og = value;
            ob = value;
        }
        void CopyValues(ref byte ob, ref byte og, ref byte or, ref byte oa, byte nb, byte ng, byte nr, byte na)//n mint new, o mint original
        {
            oa = 255;
            or = nr;
            og = ng;
            ob = nb;
        }
    }
}
