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
using System.IO;
using OfficeOpenXml;

namespace Crack_the_Code
{
    /// <summary>
    /// Interaction logic for Results.xaml
    /// </summary>
    public partial class Results : Page
    {
        MainWindow mainwindow;
        System.ComponentModel.BackgroundWorker transitionEffect, glitchEffect;
        bool granted = false;
        double currentOpacity = 1;

        public Results(MainWindow mainwindow, string rheader, string rmessage, string rtime)
        {
            InitializeComponent();

            this.mainwindow = mainwindow;
            header.Text = rheader;
            message.Text = rmessage;
            time.Text = rtime;

            if (rheader.ToLower().Contains("granted"))
            {
                granted = true;
                header.Foreground = Brushes.LimeGreen;
                playHackedVideo();
            } else
            {
                granted = false;
                header.Foreground = Brushes.Red;
                ImageBrush imgBrush = new ImageBrush();
                imgBrush.ImageSource = new BitmapImage(new Uri(mainwindow.scarePath, UriKind.Relative));
                mainwindow.grid.Background = imgBrush;

                beginGlitch();
            }

            name.Focus();
        }

        private void playHackedVideo()
        {
            currentOpacity = 0.5;
            VisualBrush vb = new VisualBrush();
            mainwindow.me = new MediaElement();
            mainwindow.me.Stretch = Stretch.UniformToFill;
            mainwindow.me.Width = 1920;
            mainwindow.me.Height = 1080;
            mainwindow.me.LoadedBehavior = MediaState.Manual;
            mainwindow.me.MediaEnded += me_OnMediaEnded;
            mainwindow.me.Source = new Uri(mainwindow.hackedPath, UriKind.Relative);
            mainwindow.me.Volume = 0;
            mainwindow.me.Play();
            mainwindow.me.SpeedRatio = 3;
            vb.Visual = mainwindow.me;
            // Set Background from Black to Vid
            mainwindow.grid.Background = vb;
            mainwindow.grid.Background.Opacity = currentOpacity;
        }

        private void me_OnMediaEnded(object sender, RoutedEventArgs e)
        {
            mainwindow.me.Position = new TimeSpan(0, 0, 1);
            mainwindow.me.Play();
        }

        private void beginTransition()
        {
            transitionEffect = new System.ComponentModel.BackgroundWorker();
            transitionEffect.WorkerReportsProgress = true;
            transitionEffect.DoWork += transitionEffect_DoWork;
            transitionEffect.ProgressChanged += transitionEffect_ProgressChanged;
            transitionEffect.RunWorkerCompleted += transitionEffect_RunWorkerCompleted;
            transitionEffect.RunWorkerAsync();
        }

        private void beginGlitch()
        {
            glitchEffect = new System.ComponentModel.BackgroundWorker();
            glitchEffect.WorkerReportsProgress = true;
            glitchEffect.DoWork += glitchEffect_DoWork;
            glitchEffect.ProgressChanged += glitchEffect_ProgressChanged;
            glitchEffect.RunWorkerCompleted += glitchEffect_RunWorkerCompleted;
            glitchEffect.RunWorkerAsync();
        }

        private void btn_Continue(object sender, RoutedEventArgs e)
        {
            if (name.Text != "")
            {
                // Save to Database
                string msg = "\nUnsaved Data:";
                try
                {
                    FileInfo spreadsheetInfo = new FileInfo(mainwindow.sheetPath);
                    ExcelPackage package = new ExcelPackage(spreadsheetInfo);
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.First();
                    int ID = worksheet.Dimension.End.Row;
                    int row = ID + 1;
                    worksheet.Cells[row, 1].Value = ID;
                    msg += "\n\tID: " + worksheet.Cells[row, 1].Value;
                    worksheet.Cells[row, 2].Value = name.Text;
                    msg += "\n\tName: " + worksheet.Cells[row, 2].Value;
                    worksheet.Cells[row, 3].Value = granted ? "Passed" : "Failed";
                    msg += "\n\tRemarks: " + worksheet.Cells[row, 3].Value;
                    worksheet.Cells[row, 4].Value = time.Text;
                    msg += "\n\tTime Remaining: " + worksheet.Cells[row, 4].Value;
                    worksheet.Cells[row, 5].Value = DateTime.Now.ToString();
                    msg += "\n\tDate: " + worksheet.Cells[row, 5].Value;
                    package.Save();
                } catch (Exception ex)
                {
                    // Bring UI to Back
                    mainwindow.WindowState = WindowState.Normal;
                    mainwindow.Topmost = false;
                    // Show error and data
                    MessageBox.Show(ex.Message + Environment.NewLine + msg, "Crack The Code (@R Database Update)", MessageBoxButton.OK, MessageBoxImage.Error);
                    // Enable Controls
                    mainwindow.DisableControls(false);
                    // Close UI
                    mainwindow.Close();
                }
                if (granted)
                {
                    beginTransition();
                }
                else
                {
                    granted = true;
                }
                btn_Cont.IsEnabled = false;
            }
        }

        private void transitionEffect_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            double opacity = currentOpacity;
            while (opacity > 0)
            {
                opacity -= 0.01;
                transitionEffect.ReportProgress(1, opacity);
                System.Threading.Thread.Sleep(10);
            }
            System.Threading.Thread.Sleep(1000);
            transitionEffect.ReportProgress(2);
            while (opacity < 1)
            {
                opacity += 0.01;
                transitionEffect.ReportProgress(3, opacity);
                System.Threading.Thread.Sleep(10);
            }
            System.Threading.Thread.Sleep(2500);
            while (opacity > 0)
            {
                opacity -= 0.01;
                transitionEffect.ReportProgress(3, opacity);
                System.Threading.Thread.Sleep(10);
            }
        }

        private void transitionEffect_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == 1)
            {
                currentOpacity = Double.Parse(e.UserState.ToString());
                mainwindow.grid.Background.Opacity = currentOpacity;
                content.Opacity = currentOpacity;
            } else if (e.ProgressPercentage == 2)
            {
                content.Visibility = Visibility.Hidden;
                splashscreen.Visibility = Visibility.Visible;
            } else if (e.ProgressPercentage == 3)
            {
                currentOpacity = Double.Parse(e.UserState.ToString());
                splashscreen.Opacity = currentOpacity;
            }
        }

        private void transitionEffect_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            mainwindow.main.Content = new SetCode(mainwindow);
        }

        private void glitchEffect_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            Random r = new Random();
            while (!granted)
            {
                glitchEffect.ReportProgress(1, r.NextDouble());
                System.Threading.Thread.Sleep(100);
                glitchEffect.ReportProgress(1, 1);
                System.Threading.Thread.Sleep(r.Next(25, 500));
            }
        }

        private void glitchEffect_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            currentOpacity = Double.Parse(e.UserState.ToString());
            mainwindow.grid.Background.Opacity = currentOpacity;
        }

        private void glitchEffect_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            beginTransition();
        }
    }
}
