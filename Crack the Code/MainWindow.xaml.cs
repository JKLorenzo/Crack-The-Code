using System;
using System.Linq;
using System.Windows;
using System.IO;
using System.Windows.Controls;
using System.Collections.Generic;
using System.ComponentModel;

using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Diagnostics;
using System.Reflection;

namespace Crack_the_Code
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Startup();
        }

        BackgroundWorker resourceUnloader;
        BackgroundWorker progressbarAnimator;

        public MediaElement me;
        public string AppPath, ResourcePath, sheetPath, scarePath, matrixPath, hackedPath, EPPlusPath;
        public bool isClosable = true, isInitialized = false;

        private int progressbarmaxvalue = 0;

        private LowLevelKeyboardProcDelegate LLKeyboardProc;
        private static int intLLKey;

        private void Startup()
        {
            // Link Resources to Physical Copy
            AppPath = Environment.CurrentDirectory;
            ResourcePath = Path.Combine(AppPath, @"Resources\");
            sheetPath = Path.Combine(ResourcePath, "Res1.jklorenzo");
            scarePath = Path.Combine(ResourcePath, "Res2.jklorenzo");
            matrixPath = Path.Combine(ResourcePath, "Res3.jklorenzo");
            hackedPath = Path.Combine(ResourcePath, "Res4.jklorenzo");
            EPPlusPath = AppPath + @"\EPPlus.dll";

            Console.WriteLine(AppPath);
            Console.WriteLine(ResourcePath);
            Console.WriteLine(sheetPath);
            Console.WriteLine(scarePath);
            Console.WriteLine(matrixPath);
            Console.WriteLine(hackedPath);
            Console.WriteLine(EPPlusPath);

            // Create Animator Instance
            progressbarAnimator = new BackgroundWorker();
            progressbarAnimator.WorkerReportsProgress = true;
            progressbarAnimator.DoWork += progressbarAnimator_DoWork;
            progressbarAnimator.ProgressChanged += progressbarAnimator_ProgressChanged;

            // Unload Resources
            resourceUnloader = new BackgroundWorker();
            resourceUnloader.WorkerReportsProgress = true;
            resourceUnloader.DoWork += resourceUnloader_DoWork;
            resourceUnloader.ProgressChanged += resourceUnloader_ProgressChanged;
            resourceUnloader.RunWorkerCompleted += resourceUnloader_RunWorkerCompleted;
            resourceUnloader.RunWorkerAsync();

            // Start Animator
            progressbarAnimator.RunWorkerAsync();

            // Set Hooks
            LLKeyboardProc = LowLevelKeyboardProc;
            intLLKey = SetWindowsHookEx(WH_KEYBOARD_LL, LLKeyboardProc, Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().GetModules()[0]).ToInt32(), 0);
        }
        

        private void progressbarAnimator_DoWork(object sender, DoWorkEventArgs e)
        {
            int currentprogressvalue = 0;
            while (currentprogressvalue != 1000)
            {
                if (currentprogressvalue < progressbarmaxvalue)
                {
                    try
                    {
                        currentprogressvalue += (int) (((1 - (currentprogressvalue / progressbarmaxvalue)) * 0.25) * 10);
                    }
                    catch
                    {

                    }
                    progressbarAnimator.ReportProgress(1, currentprogressvalue);
                    Console.WriteLine(currentprogressvalue + " -> " + progressbarmaxvalue);
                    System.Threading.Thread.Sleep(10);
                }
            }
            Console.WriteLine("PBDone");
        }

        private void progressbarAnimator_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ResourcePB.Value = Double.Parse(e.UserState.ToString());
        }


        private void resourceUnloader_DoWork(object sender, DoWorkEventArgs e)
        {
            if (!Directory.Exists(ResourcePath))
            {
                Directory.CreateDirectory(ResourcePath);
            }
            else
            {
                System.Threading.Thread.Sleep(500);
            }
            resourceUnloader.ReportProgress(100);
            if (!File.Exists(EPPlusPath))
            {
                File.WriteAllBytes(EPPlusPath, Properties.Resources.EPPlus);
            }
            else
            {
                System.Threading.Thread.Sleep(500);
            }
            resourceUnloader.ReportProgress(200);
            if (!File.Exists(sheetPath))
            {
                File.WriteAllBytes(sheetPath, Properties.Resources.sheet);
            }
            else
            {
                System.Threading.Thread.Sleep(500);
            }
            resourceUnloader.ReportProgress(400);
            if (!File.Exists(scarePath))
            {
                resourceUnloader.ReportProgress(911);
                return;
            }
            else
            {
                System.Threading.Thread.Sleep(500);
            }
            resourceUnloader.ReportProgress(600);


            if (!File.Exists(matrixPath))
            {
                if (File.Exists(matrixPath + ".mkv"))
                {
                    matrixPath += ".mkv";
                }
                else
                {
                    resourceUnloader.ReportProgress(911);
                    return;
                }
            }
            else
            {
                File.Move(matrixPath, matrixPath + ".mkv");
                matrixPath += ".mkv";
            }
            System.Threading.Thread.Sleep(500);
            resourceUnloader.ReportProgress(800);


            if (!File.Exists(hackedPath))
            {
                if (File.Exists(hackedPath + ".mkv"))
                {
                    hackedPath += ".mkv";
                } else
                {
                    resourceUnloader.ReportProgress(911);
                    return;
                }
            }
            else
            {
                File.Move(hackedPath, hackedPath + ".mkv");
                hackedPath += ".mkv";
            }
            System.Threading.Thread.Sleep(500);
            resourceUnloader.ReportProgress(1000);


            System.Threading.Thread.Sleep(5000);
            while (progressbarAnimator.IsBusy)
            {
                System.Threading.Thread.Sleep(1000);
            }
        }

        bool hasMissingResources = false;
        private void resourceUnloader_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == 911)
                hasMissingResources = true;
            progressbarmaxvalue = e.ProgressPercentage;
        }

        private void resourceUnloader_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if(e.Error == null)
            {
                main.Content = new SetCode(this);
                splashscreen.Visibility = Visibility.Hidden;
                main.Visibility = Visibility.Visible;
                isInitialized = true;
            }
            else if (hasMissingResources)
            {
                MessageBox.Show("Some resource files are missing. Reinstallation required.", "Crack The Code (@W1MW)", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
            else
            {
                MessageBox.Show(e.Error.Message, "Crack The Code (@W1MW)", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        }

        

        [DllImport("user32", EntryPoint = "SetWindowsHookExA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        public static extern int SetWindowsHookEx(int idHook, LowLevelKeyboardProcDelegate lpfn, int hMod, int dwThreadId);
        [DllImport("user32", EntryPoint = "UnhookWindowsHookEx", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        public static extern int UnhookWindowsHookEx(int hHook);
        public delegate int LowLevelKeyboardProcDelegate(int nCode, int wParam, ref KBDLLHOOKSTRUCT lParam);
        [DllImport("user32", EntryPoint = "CallNextHookEx", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        public static extern int CallNextHookEx(int hHook, int nCode, int wParam, ref KBDLLHOOKSTRUCT lParam);
        public const int WH_KEYBOARD_LL = 13;

        /*code needed to disable start menu*/
        [DllImport("user32.dll")]
        private static extern int FindWindow(string className, string windowText);
        [DllImport("user32.dll")]
        private static extern int ShowWindow(int hwnd, int command);

        private const int SW_HIDE = 0;
        private const int SW_SHOW = 1;
        public struct KBDLLHOOKSTRUCT
        {
            public int vkCode;
            public int scanCode;
            public int flags;
            public int time;
            public int dwExtraInfo;
        }

        private int LowLevelKeyboardProc(int nCode, int wParam, ref KBDLLHOOKSTRUCT lParam)
        {
            bool blnEat = false;
            switch (wParam)
            {
                case 256:
                case 257:
                case 260:
                case 261:
                    blnEat =
                        ((lParam.vkCode == 9) && (lParam.flags == 32)) | //Alt Tab
                        ((lParam.vkCode == 27) && (lParam.flags == 32)) | // Alt Esc
                        ((lParam.vkCode == 27) && (lParam.flags == 0)) | // Ctrl Esc
                        ((lParam.vkCode == 91) && (lParam.flags == 1)) | // Win Key L
                        ((lParam.vkCode == 92) && (lParam.flags == 1)); // Win Key R
                    break;
            }

            if (blnEat == true)
            {
                Console.WriteLine("true:\t" + lParam.vkCode + "\t" + lParam.flags);
                return 1;
            }
            else
            {
                Console.WriteLine("false:\t" + lParam.vkCode + "\t" + lParam.flags);
                return CallNextHookEx(0, nCode, wParam, ref lParam);
            }
        }
        private void DisableStartMenu()
        {
            int hwnd = FindWindow("Shell_TrayWnd", "");
            ShowWindow(hwnd, SW_HIDE);
        }
        private static void EnableStartMenu()
        {
            int hwnd = FindWindow("Shell_TrayWnd", "");
            ShowWindow(hwnd, SW_SHOW);
        }
        private void DisableTaskManager()
        {
            RegistryKey regkey;
            string keyValueInt = "1";
            string subKey = @"Software\Microsoft\Windows\CurrentVersion\Policies\System";
            try
            {
                regkey = Registry.CurrentUser.CreateSubKey(subKey);
                regkey.SetValue("DisableTaskMgr", keyValueInt);
                regkey.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private static void EnableTaskManager()
        {
            try
            {
                string subKey = @"Software\Microsoft\Windows\CurrentVersion\Policies\System";
                RegistryKey rk = Registry.CurrentUser;
                RegistryKey sk1 = rk.OpenSubKey(subKey);
                if (sk1 != null)
                    rk.DeleteSubKeyTree(subKey);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public void DisableControls(bool disable)
        {
            if (disable) {
                DisableTaskManager();
                DisableStartMenu();
                isClosable = false;
            }
            else
            {
                EnableTaskManager();
                EnableStartMenu();
                isClosable = true;
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (isClosable)
            {
                if (isInitialized)
                {
                    me.Close();
                }
                if (hackedPath.Contains(".mkv") && File.Exists(hackedPath))
                    File.Move(hackedPath, hackedPath.Replace(".mkv", ""));
                if (matrixPath.Contains(".mkv") && File.Exists(matrixPath))
                    File.Move(matrixPath, matrixPath.Replace(".mkv", ""));
                UnhookWindowsHookEx(intLLKey);
                DisableControls(false);
            } else
            {
                e.Cancel = true;
            }
        }
    }
}


/*
<Window x:Class="Crack_the_Code.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
        xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
        xmlns:local="clr-namespace:Crack_the_Code"
        mc:Ignorable="d"
        Title="Crack The Code" Height="1080" Width="1920" WindowState="Maximized" WindowStartupLocation="CenterScreen" Topmost="True" WindowStyle="None" MinWidth="1366" MinHeight="768" ResizeMode="NoResize" Background="Black" FontFamily="Verdana" Closing="Window_Closing">
    <Grid x:Name="grid">
        <Frame x:Name="main" NavigationUIVisibility="Hidden" Visibility="Hidden"/>

        <Grid x:Name="splashscreen" Visibility="Visible">
            <Grid.Background>
                <ImageBrush ImageSource="Resources/background.jpg" TileMode="Tile" Stretch="UniformToFill"/>
            </Grid.Background>
            <Grid.RowDefinitions>
                <RowDefinition Height="9*"/>
                <RowDefinition Height="*"/>
            </Grid.RowDefinitions>

            <Grid Grid.Row="0">
                <Grid.RowDefinitions>
                    <RowDefinition Height="1.5*"/>
                    <RowDefinition Height="*"/>
                    <RowDefinition Height="2*"/>
                </Grid.RowDefinitions>

                <Grid Grid.Row="0" VerticalAlignment="Center">
                    <Grid.RowDefinitions>
                        <RowDefinition Height="*"/>
                        <RowDefinition Height="*"/>
                    </Grid.RowDefinitions>
                    <TextBlock x:Name="h1" Grid.Row="0" Text="Institute of Computer Engineers" FontWeight="UltraBold" FontSize="100" Foreground="White" HorizontalAlignment="Center"/>
                    <TextBlock x:Name="h2" Grid.Row="1" Text="of the Philippines: SE USLS" FontSize="100" FontWeight="UltraBold" Foreground="White" HorizontalAlignment="Center"/>
                </Grid>

                <Grid Grid.Row="1" Margin="300 0 300 0" VerticalAlignment="Center">
                    <Grid.RowDefinitions>
                        <RowDefinition Height="*"/>
                        <RowDefinition Height="*"/>
                    </Grid.RowDefinitions>

                    <Grid Grid.Row="0">
                        <Grid.RowDefinitions>
                            <RowDefinition Height="*"/>
                            <RowDefinition Height="*"/>
                        </Grid.RowDefinitions>

                        <TextBlock x:Name="h3" Grid.Row="0" Text="Developer:" FontWeight="Bold" FontSize="30" Foreground="White" HorizontalAlignment="Center"/>
                        <TextBlock x:Name="h4" Grid.Row="1" Text="Juruel Keanu Lorenzo" FontSize="30" Foreground="White" HorizontalAlignment="Center"/>
                    </Grid>

                    <Grid Grid.Row="1" Margin="0 10 0 0">
                        <Grid.ColumnDefinitions>
                            <ColumnDefinition Width="*"/>
                            <ColumnDefinition Width="*"/>
                        </Grid.ColumnDefinitions>

                        <Grid Grid.Column="0">
                            <Grid.RowDefinitions>
                                <RowDefinition Height="*"/>
                                <RowDefinition Height="*"/>
                            </Grid.RowDefinitions>

                            <TextBlock Grid.Row="0" Text="GUI Designer:" FontWeight="Bold" FontSize="30" Foreground="White" HorizontalAlignment="Center"/>
                            <TextBlock Grid.Row="1" Text="John Wilbert Absalon" FontSize="30" Foreground="White" HorizontalAlignment="Center"/>
                        </Grid>

                        <Grid Grid.Column="1">
                            <Grid.RowDefinitions>
                                <RowDefinition Height="*"/>
                                <RowDefinition Height="*"/>
                            </Grid.RowDefinitions>

                            <TextBlock Grid.Row="0" Text="Software Analyst:" FontWeight="Bold" FontSize="30" Foreground="White" HorizontalAlignment="Center"/>
                            <TextBlock Grid.Row="1" Text="Jesserie Feleciano Pinuela" FontSize="30" Foreground="White" HorizontalAlignment="Center"/>
                        </Grid>
                    </Grid>
                </Grid>

                <Grid Grid.Row="2">
                    <Grid.RowDefinitions>
                        <RowDefinition Height="*"/>
                        <RowDefinition Height="2*"/>
                        <RowDefinition Height="*"/>
                    </Grid.RowDefinitions>

                    <TextBlock Grid.Row="0" Text="Special thanks to:" Foreground="White" FontWeight="Bold"  FontSize="30" HorizontalAlignment="Center" VerticalAlignment="Center"/>
                    <Grid Grid.Row="1" Margin="100 0 100 0">
                        <Grid.ColumnDefinitions>
                            <ColumnDefinition Width="*"/>
                            <ColumnDefinition Width="*"/>
                            <ColumnDefinition Width="*"/>
                        </Grid.ColumnDefinitions>

                        <Grid  Grid.Column="0" Margin="10" Background="White">
                            <Image Source="Resources/stackoverflow.png"/>
                        </Grid>

                        <Grid  Grid.Column="1" Margin="10" Background="White">
                            <Image Source="Resources/codeproject.png"/>
                        </Grid>

                        <Grid  Grid.Column="2" Margin="10" Background="White">
                            <Image Source="Resources/msdn.png"/>
                        </Grid>
                    </Grid>

                    <Grid Grid.Row="3" Width="800">
                        <Grid.ColumnDefinitions>
                            <ColumnDefinition Width="*"/>
                            <ColumnDefinition Width="*"/>
                        </Grid.ColumnDefinitions>

                        <Grid Grid.Column="0" Margin="10" HorizontalAlignment="Center">
                            <Grid.ColumnDefinitions>
                                <ColumnDefinition Width="*"/>
                                <ColumnDefinition Width="*"/>
                            </Grid.ColumnDefinitions>

                            <Image Grid.Column="0" Source="Resources/youtube.png"/>
                            <TextBlock Grid.Column="1" Text=" AA VFX" Foreground="White" FontSize="30" HorizontalAlignment="Left" VerticalAlignment="Center"/>
                        </Grid>

                        <Grid Grid.Column="1" Margin="10" HorizontalAlignment="Center">
                            <Grid.ColumnDefinitions>
                                <ColumnDefinition Width="*"/>
                                <ColumnDefinition Width="*"/>
                            </Grid.ColumnDefinitions>

                            <Image Grid.Column="0" Source="Resources/youtube.png"/>
                            <TextBlock Grid.Column="1" Text=" Hash Life" Foreground="White" FontSize="30" HorizontalAlignment="Left" VerticalAlignment="Center"/>
                        </Grid>
                    </Grid>
                </Grid>
            </Grid>
            <Grid Grid.Row="1">
                <Grid.RowDefinitions>
                    <RowDefinition Height="9*"/>
                    <RowDefinition Height="6*"/>
                    <RowDefinition Height="*"/>
                </Grid.RowDefinitions>

                <TextBlock Grid.Row="0" Text="Checking Resources" FontSize="30" Foreground="White" HorizontalAlignment="Center" VerticalAlignment="Bottom"/>
                <ProgressBar Grid.Row="1" x:Name="ResourcePB" Value="0" Height="10" Margin="10 10 10 0" VerticalAlignment="Top"/>
            </Grid>
        </Grid>
    </Grid>
</Window>
*/