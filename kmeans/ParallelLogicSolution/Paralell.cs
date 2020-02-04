//A ConcurrentDictionary-s részmegoldás




//using System;
//using System.Collections.Generic;
//using System.Drawing;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Threading;
//using System.Collections.Concurrent;

//namespace kmeans.Paralell
//{
//    class ParallelLogic
//    {
//        static Random rnd = new Random();
//        class Center
//        {
//            //koordináták
//            public int Red { get; set; }
//            public int Blue { get; set; }
//            public int Green { get; set; }
//            //hozzátartozó értékek
//            public ConcurrentDictionary<int, int> SumRed { get; set; }
//            public ConcurrentDictionary<int, int> SumBlue { get; set; }
//            public ConcurrentDictionary<int, int> SumGreen { get; set; }
//            public ConcurrentDictionary<int, int>  SumInGroup { get; set; }
//            public ConcurrentDictionary<int, double> SumDistance { get; set; }
//            public Center(int red,int green,int blue)
//            {
//                Red = red;
//                Blue = blue;
//                Green = green;
//                SumRed = new ConcurrentDictionary<int, int>();
//                SumBlue = new ConcurrentDictionary<int, int>();
//                SumGreen = new ConcurrentDictionary<int, int>();
//                SumInGroup = new ConcurrentDictionary<int, int>();
//                SumDistance = new ConcurrentDictionary<int, double>();
//            }
//            public void SetCenter(int red,int green,int blue)
//            {
//                Red = red;
//                Blue = blue;
//                Green = green;
//            }
//            public void Reset()
//            {
//                SumRed = new ConcurrentDictionary<int, int>();
//                SumBlue = new ConcurrentDictionary<int, int>();
//                SumGreen = new ConcurrentDictionary<int, int>();
//                SumInGroup = new ConcurrentDictionary<int, int>();
//                SumDistance = new ConcurrentDictionary<int, double>();
//            }

//        }
//        public int Solve(int k,string path)
//        {
//            return LockUnlockBitsExample(k,path);
//        }
//        private int LockUnlockBitsExample(int k,string path)
//        {
//            Bitmap bmp = new Bitmap(path);
//            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
//            System.Drawing.Imaging.BitmapData bmpData =
//            bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
//            bmp.PixelFormat);
//            IntPtr ptr = bmpData.Scan0;
//            int bytes = Math.Abs(bmpData.Stride) * bmp.Height;
//            byte[] rgbValues = new byte[bytes];
//            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);
//            //-----------------------------------------------------
//            Dictionary<int, Center> bestcenters=Clustering(k, rgbValues);

//            byte[] clustered = ModifileColours(rgbValues, bestcenters, bmp.Width, bmp.Height);


//            //-----------------------------------------------------
//            System.Runtime.InteropServices.Marshal.Copy(clustered, 0, ptr, bytes);
//            bmp.UnlockBits(bmpData);
//            try
//            {
//                bmp.Save("pandatest0.png");
//            }
//            catch
//            {
//                bmp.Save("pandatest1.png");
//            }

//            bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
//            bmp.PixelFormat);
//            IntPtr ptr2 = bmpData.Scan0;
//            rgbValues = SetFrame(rgbValues, clustered, bmp.Width,bmp.Height);
//            System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);
//            bmp.UnlockBits(bmpData);
//            try
//            {
//                bmp.Save("pandaresult0.png");
//                return 0;
//            }
//            catch(System.Runtime.InteropServices.ExternalException e)
//            {
//                bmp.Save("pandaresult1.png");
//                return 1;
//            }
//        }

//        private Dictionary<int, Center> Clustering(int k, byte[] colours)
//        {
//            #region init
//            double mindistance = double.MaxValue;
//            Dictionary<int, Center> bestcenters = new Dictionary<int, Center>();
//            #endregion
//            //10x futtatjuk le
//            for (int i = 0; i < 10; i++)
//            {
//                Dictionary<int, Center> centerInformaitons = new Dictionary<int, Center>();//rgb+db center mozgatáshoz
//                InitDictionary(colours, centerInformaitons, k, colours.Length);

//                //jelenleg fixen 20x léptetjük a középpontot
//                for (int z = 0; z < 20; z++)
//                {
//                    ResetCentorids(centerInformaitons);
//                    Parallel.For(0, colours.Length / 4, idx =>
//                      {
//                          int oneColour = idx * 4;
//                          //legközelebbi centroidot adja vissza, és a távolságot hozzá
//                          KeyValuePair<int, double> closest = SmallestDistance(centerInformaitons, colours[oneColour + 2], colours[oneColour + 1], colours[oneColour]);
//                          int centoirdId = closest.Key;
//                          int threadID = Thread.CurrentThread.ManagedThreadId;
//                          centerInformaitons[centoirdId].SumRed.AddOrUpdate(threadID, colours[oneColour + 2], (key, v) => v+colours[oneColour + 2]);
//                          centerInformaitons[centoirdId].SumGreen.AddOrUpdate(threadID, colours[oneColour + 1], (key, v) => v+colours[oneColour + 1]);
//                          centerInformaitons[centoirdId].SumBlue.AddOrUpdate(threadID, colours[oneColour], (key, v) => v+colours[oneColour]);
//                          centerInformaitons[centoirdId].SumInGroup.AddOrUpdate(threadID, 1, (key, v) => v + 1);
//                          centerInformaitons[centoirdId].SumDistance.AddOrUpdate(threadID, closest.Value, (key, v) => v+closest.Value);
//                      });
//                    MoveCentroids(centerInformaitons);
//                    double actualdistance2 = centerInformaitons.Values.Sum(x => x.SumDistance.Values.Sum());

//                }
//                //összehasonlítani hogy jobb - e mint az korábbiak
//                double actualdistance = centerInformaitons.Values.Sum(x => x.SumDistance.Values.Sum());
//                if (actualdistance<mindistance)
//                {

//                    mindistance = actualdistance;
//                    bestcenters = centerInformaitons;
//                }
//            }
//            return bestcenters;
//        }


//        void InitDictionary(byte[] colours,Dictionary<int, Center> centerInformaitons, int k, int length)
//        {
//            for (int i = 0; i < k; i++)
//            {
//                int idx = rnd.Next(0, length)/4;
//                int  r, g, b = 0;
//                r = colours[idx * 4 + 2];
//                g = colours[idx * 4 + 1];
//                b = colours[idx * 4];
//                centerInformaitons.Add(i, new Center(r,g,b));
//            }
//        }

//        KeyValuePair<int, double> SmallestDistance(Dictionary<int, Center> centers, int r,int g, int b)
//        {
//            int min_center = 0;
//            double min_distance = double.MaxValue;

//            for (int i = 0; i < centers.Count; i++)
//            {
//                //Euklideszi távolság
//                double distance = Math.Sqrt(Math.Pow((centers[i].Red - r), 2) + Math.Pow((centers[i].Green - g), 2) + Math.Pow((centers[i].Blue - b), 2));
//                if (distance < min_distance)
//                {
//                    min_center = i;
//                    min_distance = distance;
//                }
//            }
//            return new KeyValuePair<int, double>(min_center, min_distance);
//        }
//        private void MoveCentroids( Dictionary<int, Center> centerInformaitons)
//        {
//            Parallel.For(0, centerInformaitons.Count, i => {
//                Tuple<int, int, int> avg = CalcAvg(centerInformaitons[i]);
//                centerInformaitons[i].SetCenter(avg.Item1, avg.Item2, avg.Item3);
//            });
//        }
//        void ResetCentorids(Dictionary<int, Center> centerInformaitons)
//        {
//            Parallel.For(0,centerInformaitons.Count, idx => {
//                centerInformaitons[idx].Reset();
//            });
//        }
//        Tuple<int,int,int> CalcAvg(Center center)
//        {
//            int numberInGroup = center.SumInGroup.Values.Sum();
//            if (numberInGroup != 0)
//            {
//                int red = (int)center.SumRed.Values.Sum() / numberInGroup;
//                int green = (int)center.SumGreen.Values.Sum() / numberInGroup;
//                int blue = (int)center.SumBlue.Values.Sum() / numberInGroup;
//                return new Tuple<int, int, int>(red, green, blue);
//            }
//            else
//                return new Tuple<int, int, int>(center.Red,center.Green,center.Blue);
//        }

//        byte[] ModifileColours(byte[] colours, Dictionary<int, Center> bestcenters, int width,int height)
//        {
//            int sor = 0;

//            int numberofpixels = colours.Length/4;
//            byte[] colours2 = colours.ToArray();

//            Parallel.For ( 0, numberofpixels, i=>
//            {
//                int idx = SmallestDistance(bestcenters, colours[i*4 + 2], colours[i*4 + 1], colours[i*4]).Key;
//                CopyValues(ref colours2[(i) * 4], ref colours2[((i) * 4) + 1], ref colours2[((i) * 4) + 2], ref colours2[((i) * 4) + 3],
//                         (byte)bestcenters[idx].Blue, (byte)bestcenters[idx].Green, (byte)bestcenters[idx].Red, (byte)255);
//            });
//            return colours2;
//        }

//        byte[] SetFrame(byte[] colours, byte[] colourssegmented, int width,int height)
//        {
//            int numberofpixels = colours.Length / 4;
//            byte[] withframe = colours.ToArray();

//            for (int i = 0; i < numberofpixels; i++)
//            {
//                for (int k = -1; k < 2; k++)
//                {
//                    for (int z = -1; z < 2; z++)
//                    {
//                        if (!IsSame(colourssegmented, i, k, z, width, height))
//                        {
//                            CopyValues(ref withframe[(i) * 4], ref withframe[((i) * 4) + 1], ref withframe[((i) * 4) + 2], ref withframe[((i) * 4) + 3], (byte)0);
//                            break;
//                        }
//                    }
//                }
//            }
//            return withframe;
//        }
//        bool IsSame(byte[] colours,int idx, int x,int y,int width,int heigth)
//        {

//            if (idx%width==0&&x==-1)//nincs bal szomszéd és balra nézzük
//                return true;
//            if (idx % width == width - 1 && x == 1)//nincs jobbra szomszéd és jobbra nézzük
//                return true;
//            if (idx/width==0&&y==-1)//nincs felfele szomszéd
//                return true;
//            if ((idx/ width)==heigth-1&&y==1)//alsó sorban vagyunk és lefel néznénk
//                return true;

//            int neighbourb = (idx + x) * 4 + (width*4 * y);//szomszéd ugyanaz-e

//            //if (neighbourb>-1&& neighbourb < colours.Length)
//            //{
//            if (colours[neighbourb] != colours[idx * 4])
//                return false;
//            if (colours[neighbourb + 1] != colours[idx * 4 + 1])
//                return false;
//            if (colours[neighbourb + 2] != colours[idx * 4 + 2])
//                return false;
//            //}

//            return true;
//        }
//        void CopyValues(ref byte ob,ref byte og,ref byte or,ref byte oa,byte value)//n mint new, o mint original
//        {
//            oa = 255;
//            or = value;
//            og = value;
//            ob = value;
//        }
//        void CopyValues(ref byte ob, ref byte og, ref byte or, ref byte oa, byte nb,byte ng,byte nr,byte na)//n mint new, o mint original
//        {
//            oa = na;
//            or = nr;
//            og = ng;
//            ob = nb;
//        }
//    }
//}
