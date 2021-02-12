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
using System.ComponentModel;

namespace Crack_the_Code
{
    /// <summary>
    /// Interaction logic for Body.xaml
    /// </summary>
    public partial class Body : Page
    {
        MainWindow mainwindow;
        string code;

        
        System.ComponentModel.BackgroundWorker CountdownClock;
        System.ComponentModel.BackgroundWorker glitchEffect;
        int minutes, seconds, miliseconds;
        bool workerdone = false;

        public Body(MainWindow mainwindow, string code)
        {
            InitializeComponent();
            this.mainwindow = mainwindow;
            this.code = code;

            // Disable Controls
            mainwindow.DisableControls(true);


            minutes = 5;
            seconds = 0;
            miliseconds = 0;

            timer.Text = minutes.ToString("0") + " : " + seconds.ToString("00") + " : " + miliseconds.ToString("00");
        }

        private void btn_Start(object sender, RoutedEventArgs e)
        {
            grid1.Visibility = Visibility.Hidden;
            grid2.Visibility = Visibility.Visible;

            tb1.Focus();

            CountdownClock = new BackgroundWorker();
            CountdownClock.WorkerReportsProgress = true;
            CountdownClock.DoWork += CountdownClock_DoWork;
            CountdownClock.ProgressChanged += CountdownClock_ProgressChanged;
            CountdownClock.RunWorkerCompleted += CountdownClock_RunWorkerCompleted;
            CountdownClock.RunWorkerAsync();
        }

        private void CountdownClock_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            while (!workerdone && (minutes != 0 || seconds != 0 || miliseconds != 0))
            {
                if (miliseconds == 0)
                {
                    if (seconds == 0)
                    {
                        if (minutes != 0)
                        {
                            minutes--;
                            seconds = 59;
                            miliseconds = 59;
                        }
                    }
                    else
                    {
                        seconds--;
                        miliseconds = 59;
                    }
                }
                else
                {
                    miliseconds--;
                }

                CountdownClock.ReportProgress(1, minutes.ToString("0") + " : " + seconds.ToString("00") + " : " + miliseconds.ToString("00"));
                System.Threading.Thread.Sleep(17);
            }
        }

        private void CountdownClock_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            timer.Text = e.UserState.ToString();
        }

        private void CountdownClock_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            string Header, Message, Time;
            if (tries > 0 && (minutes !=0 || seconds != 0 || miliseconds != 0))
            {
                Header = "ACCESS GRANTED";
                Message = "You  completed the task.";
            } else
            {
                Header = "ACCESS DENIED";
                Message = "You have failed in saving the community.";
            }
            Time = "Time: " + timer.Text;
            mainwindow.main.Content = new Results(mainwindow, Header, Message, Time);
        }

        private void btn_Enter(object sender, RoutedEventArgs e)
        {
            CheckCode();
        }

        int tries = 2;
        private void CheckCode()
        {
            if (code.Equals(tb1.Text + "+" + tb2.Text + "-" + tb3.Text + "*" + tb4.Text))
            {
                workerdone = true;
            } else
            {
                tries--;
                if (tries == 0)
                {
                    workerdone = true;
                } else
                {
                    tb1.Text = "";
                    tb2.Text = "";
                    tb3.Text = "";
                    tb4.Text = "";

                    tb1.Focus();

                    beginGlitch();
                }
            }
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

        private void glitchEffect_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            Random r = new Random();
            for (int i = 0; i < 3; i++)
            {
                glitchEffect.ReportProgress(1, r.NextDouble());
                System.Threading.Thread.Sleep(r.Next(25, 250));
                glitchEffect.ReportProgress(1, 1);
                System.Threading.Thread.Sleep(r.Next(25, 250));
            }
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

        private void glitchEffect_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            mainwindow.grid.Background.Opacity = Double.Parse(e.UserState.ToString());
        }

        private void glitchEffect_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            mainwindow.grid.Background.Opacity = 1;
        }
    }
}
