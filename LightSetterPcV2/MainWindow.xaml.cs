using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Haley.WPF.Controls;
using System.Windows.Media.Media3D;
using Haley.Utils;
using System.Collections;

namespace LightSetterPcV2
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer lightDT = new DispatcherTimer();
        DispatcherTimer countDT = new DispatcherTimer();
        DispatcherTimer startDT = new DispatcherTimer();
        DispatcherTimer colorDT = new DispatcherTimer();
        bool colorPick = false;
        bool lightPick = false;
        bool countPick = false;
        bool startPick = false;
        enum Themes
        {
            white,
            black
        }
        public MainWindow()
        {

            lightDT.Tick += LightDT_Tick;
            countDT.Tick += CountDT_Tick;
            startDT.Tick += StartDT_Tick;
            colorDT.Tick += ColorDT_Tick;
            InitializeComponent();
            GetActualValues();

            lightSlider.ValueChanged += lightSlider_ValueChanged;
            countSlider.ValueChanged += countSlider_ValueChanged;
            startSlider.ValueChanged += startSlider_ValueChanged;
            color.SelectedBrushChanged += color_SelectedBrushChanged;

            colorDT.Interval = new TimeSpan(0, 0, 1);
            countDT.Interval = new TimeSpan(0, 0, 1);
            startDT.Interval = new TimeSpan(0, 0, 1);
            lightDT.Interval = new TimeSpan(0,0,1);

            if (!themeCheck.IsChecked.Value)
                SetTheme(Themes.black);
            else
                SetTheme(Themes.white);
        }
        void GetActualValues()
        {
            string[] rez = GET("getvalues").Split(' ');
            lightSlider.Value = Convert.ToInt32(rez[0]);
            countSlider.Value = Convert.ToInt32(rez[1]);
            startSlider.Value = Convert.ToInt32(rez[2]);
            turnOffCheck.IsChecked = Convert.ToBoolean(int.Parse(rez[3]));
            string[] colors = rez[4].Split(';');
            countSlider.Maximum = int.Parse(rez[5]);
            startSlider.Maximum = int.Parse(rez[5])-1;

            countLb.Content = countSlider.Value;
            startLb.Content = startSlider.Value;
            lightLb.Content = lightSlider.Value;
            activeColor.Background = new SolidColorBrush(Color.FromRgb(byte.Parse(colors[0]), byte.Parse(colors[1]), byte.Parse(colors[2])));
        }
        void SetTheme(Themes theme)
        {
            switch (theme)
            {
                case Themes.white:
                    {
                        settingsGrid.Background = new SolidColorBrush(Colors.White);
                        appSettings.Background = new SolidColorBrush(Colors.White);
                        back.Background = new SolidColorBrush(Colors.White);

                        lightTitleLb.Foreground = new SolidColorBrush(Colors.Black);
                        countTitleLb.Foreground = new SolidColorBrush(Colors.Black);
                        startTitleLb.Foreground = new SolidColorBrush(Colors.Black);

                        lightLb.Foreground = new SolidColorBrush(Colors.Black);
                        countLb.Foreground = new SolidColorBrush(Colors.Black);
                        startLb.Foreground = new SolidColorBrush(Colors.Black);

                        controlGrid.Children.OfType<Border>().ToList().ForEach(x => x.BorderBrush = new SolidColorBrush(Colors.Black));
                    }
                    break;
                case Themes.black:
                    {
                        back.Background = new SolidColorBrush(Color.FromRgb(29, 29, 29));
                        settingsGrid.Background = new SolidColorBrush(Color.FromRgb(29, 29, 29));
                        appSettings.Background = new SolidColorBrush(Color.FromRgb(29, 29, 29));

                        lightTitleLb.Foreground = new SolidColorBrush(Colors.White);
                        countTitleLb.Foreground = new SolidColorBrush(Colors.White);
                        startTitleLb.Foreground = new SolidColorBrush(Colors.White);

                        lightLb.Foreground = new SolidColorBrush(Colors.White);
                        countLb.Foreground = new SolidColorBrush(Colors.White);
                        startLb.Foreground = new SolidColorBrush(Colors.White);

                        controlGrid.Children.OfType<Border>().ToList().ForEach(x => x.BorderBrush = new SolidColorBrush(Colors.White));
                    }
                    break;
                default:
                    break;
            }
        }
        private void ColorDT_Tick(object sender, EventArgs e)
        {
            if (colorPick)
            {
                string query = "setColor?r=" + color.SelectedBrush.Color.R + "&g=" + color.SelectedBrush.Color.G +"&b=" + color.SelectedBrush.Color.B;
                Get(query);
                colorPick = false;
                colorDT.Stop();
            }
        }
        private void StartDT_Tick(object sender, EventArgs e)
        {
            if (startPick)
            {
                Get("setStart?start=" + startSlider.Value);
                startPick = false;
                startDT.Stop();
            }
        }
        private void CountDT_Tick(object sender, EventArgs e)
        {
            if (countPick)
            {
                Get("setCount?count=" + countSlider.Value);
                startPick = false;
                startDT.Stop();
            }
        }
        private void LightDT_Tick(object sender, EventArgs e)
        {
            if (lightPick)
            {
                Get("setBrightnes?light=" + lightSlider.Value);
                startPick = false;
                startDT.Stop();
            }
        }
        private static HttpClient httpClient = new HttpClient()
        {
            BaseAddress = new Uri("http://192.168.1.4/"),
        };
        private string GET(string Data)
        {
            WebRequest req = WebRequest.Create(httpClient.BaseAddress + Data);
            WebResponse resp = req.GetResponse();
            Stream stream = resp.GetResponseStream();
            StreamReader sr = new StreamReader(stream);
            string Out = sr.ReadToEnd();
            sr.Close();
            return Out;
        }
        static async void Get(string value)
        {
            var todos = await httpClient.GetAsync(value);
        }
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            SetTheme(Themes.white);
        }
        private void themeCheck_Unchecked(object sender, RoutedEventArgs e)
        {
            SetTheme(Themes.black);
        }
        private void OpenMouseDown(object sender, MouseButtonEventArgs e)
        {
            Border button = sender as Border;
            ThicknessAnimation animation = new ThicknessAnimation();
            animation.Duration = TimeSpan.FromSeconds(0.5);
            Thickness thickness = new Thickness(0, button.Margin.Top, 0, 0);
            thickness.Right = button.Margin.Right == 0 ? 298 : 0;
            animation.To = thickness;
            button.BeginAnimation(MarginProperty, animation);

            thickness.Top = 0;
            thickness.Right = button.Margin.Right == 0 ? 0 : -300;
            animation.To = thickness;

            switch (button.Name)
            {
                case "appBtn":
                    {
                        appSettings.BeginAnimation(MarginProperty, animation);
                    }
                    break;
                case "settingsBtn":
                    {
                        settingsGrid.BeginAnimation(MarginProperty, animation);
                    }
                    break;
                default:
                    break;
            }
        }
        private void color_SelectedBrushChanged(object sender, RoutedEventArgs e)
        {
            if (!colorPick)
            {
                colorPick = true;
                colorDT.Start();
            }
            activeColor.Background = color.SelectedBrush;
        }
        private void lightSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            lightLb.Content = (int)lightSlider.Value;
            if (!lightPick)
            {
                lightPick = true;
                lightDT.Start();
            }
        }
        private void countSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            countLb.Content = (int)countSlider.Value;
            if (!countPick)
            {
                countPick = true;
                countDT.Start();
            }
        }
        private void startSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            startLb.Content = (int)startSlider.Value;
            if (!startPick)
            {
                startPick = true;
                startDT.Start();
            }
        }
        private void Border_MouseEnter(object sender, MouseEventArgs e)
        {
            int change = 4;
            Border border = sender as Border;
            border.BeginAnimation(HeightProperty, new DoubleAnimation()
            {
                To = border.Height + change,
                Duration = TimeSpan.FromSeconds(0.1),

            });
            border.BeginAnimation(WidthProperty, new DoubleAnimation()
            {
                To = border.Width + change,
                Duration = TimeSpan.FromSeconds(0.1),

            });
            border.BeginAnimation(MarginProperty, new ThicknessAnimation()
            {
                To = new Thickness(border.Margin.Left- change/2, border.Margin.Top- change / 2, 0,0),
                Duration = TimeSpan.FromSeconds(0.1),

            });
        }
        private void Border_MouseLeave(object sender, MouseEventArgs e)
        {
            int change = 4;
            Border border = sender as Border;
            border.BeginAnimation(HeightProperty, new DoubleAnimation()
            {
                To = border.Height - change,
                Duration = TimeSpan.FromSeconds(0.1),

            });
            border.BeginAnimation(WidthProperty, new DoubleAnimation()
            {
                To = border.Width - change,
                Duration = TimeSpan.FromSeconds(0.1),

            });
            border.BeginAnimation(MarginProperty, new ThicknessAnimation()
            {
                To = new Thickness(border.Margin.Left + change/2, border.Margin.Top + change/2, 0, 0),
                Duration = TimeSpan.FromSeconds(0.1),

            });
        }
        private void WhiteColorClick(object sender, MouseButtonEventArgs e)
        {
            string query = "setColor?r=" + 255 + "&g=" + 255 +"&b=" + 255;
            Get(query);
            activeColor.Background = new SolidColorBrush(Colors.White);
        }
        private void RedColorClick(object sender, MouseButtonEventArgs e)
        {
            string query = "setColor?r=" + 255 + "&g=" + 0 + "&b=" + 0;
            Get(query);
            activeColor.Background = new SolidColorBrush(Colors.Red);
        }
        private void BlueColorClick(object sender, MouseButtonEventArgs e)
        {
            string query = "setColor?r=" + 0 + "&g=" + 0 + "&b=" + 255;
            Get(query);
            activeColor.Background = new SolidColorBrush(Colors.Blue);
        }
        private void GreenColorClick(object sender, MouseButtonEventArgs e)
        {
            string query = "setColor?r=" + 0 + "&g=" + 255 + "&b=" + 0;
            Get(query);
            activeColor.Background = new SolidColorBrush(Colors.Green);
        }
        private void PurpleColorPick(object sender, MouseButtonEventArgs e)
        {
            string query = "setColor?r=" + 255 + "&g=" + 0 + "&b=" + 164;
            Get(query);
            activeColor.Background = new SolidColorBrush(Color.FromRgb(255,0,164));
        }
        private void YelowColorPick(object sender, MouseButtonEventArgs e)
        {
            string query = "setColor?r=" + 255 + "&g=" + 255 + "&b=" + 0;
            Get(query);
            activeColor.Background = new SolidColorBrush(Color.FromRgb(255, 255, 0));
        }
        private void BlackColorPick(object sender, MouseButtonEventArgs e)
        {
            string query = "setColor?r=" + 0 + "&g=" + 0 + "&b=" + 0;
            Get(query);
            activeColor.Background = new SolidColorBrush(Colors.Black);
        }
        private void turnOffCheck_Checked(object sender, RoutedEventArgs e)
        {
            Get("/turnOn");
        }
        private void turnOffCheck_Unchecked(object sender, RoutedEventArgs e)
        {
            Get("/turnOff");
        }
        private void setRainbow(object sender, MouseButtonEventArgs e)
        {
            Get("/setMode?mode=rainbow");
        }
        private void setSolid(object sender, MouseButtonEventArgs e)
        {
            Get("/setMode?mode=solid");
        }
    }
}
