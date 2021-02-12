using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Crack_the_Code
{
    /// <summary>
    /// Interaction logic for SetCode.xaml
    /// </summary>
    public partial class SetCode : Page
    {
        MainWindow mainwindow;
        DispatcherTimer invalidEffect;
        System.ComponentModel.BackgroundWorker transitionEffect;
        public SetCode(MainWindow mainwindow)
        {
            InitializeComponent();
            this.mainwindow = mainwindow;

            // Enable Controls
            mainwindow.DisableControls(false);

            playVideo();

            invalidEffect = new DispatcherTimer();
            invalidEffect.Tick += invalidEffect_Tick;
            invalidEffect.Interval = new TimeSpan(0, 0, 0, 0, 1);

            tb1.Focus();
        }

        private void playVideo()
        {
            VisualBrush vb = new VisualBrush();
            mainwindow.me = new MediaElement();
            mainwindow.me.Stretch = Stretch.UniformToFill;
            mainwindow.me.Width = 1920;
            mainwindow.me.Height = 1080;
            mainwindow.me.LoadedBehavior = MediaState.Manual;
            mainwindow.me.MediaEnded += me_OnMediaEnded;
            mainwindow.me.Source = new Uri(mainwindow.matrixPath, UriKind.Relative);
            mainwindow.me.Volume = 0;
            mainwindow.me.Play();
            mainwindow.me.SpeedRatio = 0.8;
            vb.Visual = mainwindow.me;
            // Set Background from Black to Vid
            mainwindow.grid.Background = vb;
            // Begin Smooth Transition
            transitionEffect = new System.ComponentModel.BackgroundWorker();
            transitionEffect.WorkerReportsProgress = true;
            transitionEffect.DoWork += transitionEffect_DoWork;
            transitionEffect.ProgressChanged += transitionEffect_ProgressChanged;
            transitionEffect.RunWorkerAsync();
        }

        private void me_OnMediaEnded(object sender, RoutedEventArgs e)
        {
            mainwindow.me.Position = new TimeSpan(0, 0, 1);
            mainwindow.me.Play();
        }

        int animationSpeed = 100;
        private void transitionEffect_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            double opacity = 0.00;
            while (opacity < 1)
            {
                opacity += 0.01;
                transitionEffect.ReportProgress(1, opacity);
                System.Threading.Thread.Sleep(animationSpeed);
            }
        }

        private void transitionEffect_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            mainwindow.grid.Background.Opacity = Double.Parse(e.UserState.ToString());
        }

        int factor=100, cycle=0;
        bool increase = false;
        string code;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            proceed();
        }

        private void Tb1_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tb1.Text.Length == 1)
            {
                tb2.Focus();
            }
        }

        private void Tb2_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tb2.Text.Length == 1)
            {
                tb3.Focus();
            }
        }

        private void Tb3_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tb3.Text.Length == 1)
            {
                tb4.Focus();
            }
        }

        private void proceed()
        {
            if (tb1.Text != "" && tb2.Text != "" && tb3.Text != "" && tb4.Text != "")
            {
                // Increase Animation Speed
                code = tb1.Text + "+" + tb2.Text + "-" + tb3.Text + "*" + tb4.Text;
                if (code.Equals("#+9-1*1"))
                {
                    string path = mainwindow.ResourcePath.Replace(@"Resources\", "Data.xlsx");
                    try
                    {
                        if (System.IO.File.Exists(path))
                        {
                            System.IO.File.Delete(path);
                        }
                        System.IO.File.Copy(mainwindow.sheetPath, path);
                        System.Diagnostics.Process.Start(path);
                    }
                    catch (Exception ex)
                    {
                        // Bring UI to Back
                        mainwindow.WindowState = WindowState.Normal;
                        mainwindow.Topmost = false;
                        // Show error and data
                        MessageBox.Show(ex.Message, "Crack The Code (@SC Database Export)", MessageBoxButton.OK, MessageBoxImage.Error);
                        // Enable Controls
                        mainwindow.DisableControls(false);
                    }
                    mainwindow.Close();
                }
                else
                {
                    animationSpeed = 10;
                    mainwindow.main.Content = new Body(mainwindow, code);
                }
            }
            else
            {
                invalidEffect.Stop();
                cycle = 0;
                factor = 100;
                invalidEffect.Start();
            }
        }

        private void invalidEffect_Tick(object sender, EventArgs e)
        {
            int r = 255, g = 255, b = 255;
            if (increase)
            {
                if (factor == 100)
                {
                    increase = false;
                    if (cycle == 3)
                    {
                        cycle = 0;
                        invalidEffect.Stop();
                    }
                    else
                    {
                        cycle++;
                    }
                }
                else
                {
                    factor += 20;
                }
            }
            else
            {
                if (factor == 60)
                {
                    increase = true;
                }
                else
                {
                    factor -= 20;
                }
            }

            Brush brush = new SolidColorBrush(Color.FromRgb(Convert.ToByte((r* factor)/100), Convert.ToByte((g* factor) / 100), Convert.ToByte((b* factor) / 100)));

            if (tb1.Text == "")
                tb1.Background = brush;
            if (tb2.Text == "")
                tb2.Background = brush;
            if (tb3.Text == "")
                tb3.Background = brush;
            if (tb4.Text == "")
                tb4.Background = brush;
            UpdateLayout();
        }

    }
}
