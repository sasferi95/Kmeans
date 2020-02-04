using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
//using System.Windows.Forms;
using System.Drawing;
using kmeans.Paralell;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace kmeans
{
    class MainWindowVM:INotifyPropertyChanged
    {
        string defaultpath = Environment.CurrentDirectory;
        private bool szekvencialis;
        private string imagePath;
        private int clusterNumber;
        private int result;

        public MainWindowLogic MWL { get; set; }
        public ParallelLogic2 PL { get; set; }

        public int K
        {
            get { return clusterNumber; }
            set { clusterNumber = value; }
        }
        private int iterationNumber;

        public int IterationNumber
        {
            get { return iterationNumber; }
            set { iterationNumber = value; }
        }

        public bool Szekvencialis
        {
            get { return szekvencialis; }
            set { szekvencialis = value; }
        }

        public string ImagePath
        {
            get { return imagePath; }
            set {
                imagePath = value;
                NotifyPropertyChanged("SourceImage");
            }
        }

        public BitmapImage SourceImage { get { return new BitmapImage(new Uri(imagePath)); } }

        public int Result
        {
            get { return result; }
            set
            {
                result = value;
                NotifyPropertyChanged("ClusterImage");
                NotifyPropertyChanged("SegmentedImage");
            }
        }

        public BitmapImage ClusterImage { get { return new BitmapImage(new Uri(defaultpath+ "\\pandatest"+result+".png")); } }
        public BitmapImage SegmentedImage { get { return new BitmapImage(new Uri(defaultpath + "\\pandaresult" + result + ".png")); } }


        private TimeSpan szekvencialisFutasiIdo;
        public TimeSpan SzekvencialisFutasIdo
        {
            get { return szekvencialisFutasiIdo; }
            set
            {
                szekvencialisFutasiIdo = value;
                NotifyPropertyChanged("TotalsecondsSzekvencialis");
            }
        }
        public double TotalsecondsSzekvencialis { get { return szekvencialisFutasiIdo.TotalSeconds; } }
        public double TotalsecondsParhuzamos { get { return parhuzamosFutasiIdo.TotalSeconds; } }
        TimeSpan parhuzamosFutasiIdo;
        public TimeSpan ParhuzamosFutasIdo
        {
            get { return parhuzamosFutasiIdo; }
            set
            {
                parhuzamosFutasiIdo = value;
                NotifyPropertyChanged("TotalsecondsParhuzamos");
            }
        }
        public MainWindowVM()
        {
            Szekvencialis = true;

            MWL = new MainWindowLogic();
            PL = new ParallelLogic2();

            SzekvencialisFutasIdo = new TimeSpan();
            ParhuzamosFutasIdo = new TimeSpan();
            imagePath = defaultpath+"\\banner-car.png";
            IterationNumber = 20;
            K = 5;
            result = -1;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Reset()
        {
            SzekvencialisFutasIdo = new TimeSpan();
            ParhuzamosFutasIdo = new TimeSpan();
            Result = -1;

        }
    }
}
