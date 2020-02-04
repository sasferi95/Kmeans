using Microsoft.Win32;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;


namespace kmeans
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainWindowVM mvvm;
        //public static Image img { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = mvvm = new MainWindowVM();
        }

        private void Button_StartClick(object sender, RoutedEventArgs e)
        {
            Start.IsEnabled = false;
            int result = 0;

            Task clustering;
            DateTime start;
            DateTime end;


            if (mvvm.Szekvencialis)
            {
                start = DateTime.Now;
                clustering = new Task(() => result = mvvm.MWL.Solve(mvvm.K, mvvm.ImagePath), TaskCreationOptions.LongRunning);

            }
            else
            {
                start = DateTime.Now;
                clustering = new Task(() => result = mvvm.PL.Solve(mvvm.K, mvvm.ImagePath), TaskCreationOptions.LongRunning);
                
            }

            clustering.Start();
            clustering.ContinueWith((t) =>
            {
                end = DateTime.Now;
                if (result == -1)
                {
                    MessageBox.Show("Kérlek válassz olyan képet amelyen 4 csatorna van");
                }

                if (mvvm.Szekvencialis)
                    mvvm.SzekvencialisFutasIdo = end - start;
                else
                {
                    mvvm.ParhuzamosFutasIdo = end - start;
                }
                mvvm.Result = result;
                Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() => {
                    Start.IsEnabled = true;
                }));
                    
            });

        }

        private void ButtonBrowser_Click(object sender, RoutedEventArgs e)
        {
            mvvm.Reset();
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.jpg,  *.png) | *.jpg;  *.png";
            if (openFileDialog.ShowDialog() == true)
            {
                mvvm.ImagePath = openFileDialog.FileName;
            }

        }

        private void TextBox_TextChanged_3(object sender, TextChangedEventArgs e)
        {
            mvvm.Reset();
        }
    }
}
