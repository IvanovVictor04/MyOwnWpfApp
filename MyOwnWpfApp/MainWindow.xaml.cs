using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MyOwnWpfApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            PopulateComboBoxWithColors();
            PopulateImagesComboBoxWithImages();
            MyColors.FontStyle = FontStyles.Italic;
        }
        private void PopulateImagesComboBoxWithImages()
        {
            ImagesComboBox.Items.Add(WallpaperImages);
            ImagesComboBox.Items.Add(Prideimages);
        }
        private void PopulateComboBoxWithColors()
        {
            foreach (PropertyInfo prop in typeof(Colors).GetProperties(BindingFlags.Static | BindingFlags.Public))
            {
              MyColors.Items.Add(prop.Name);
            }       
        }

        List<BitmapImage> WallpaperImages = new List<BitmapImage>();

        List<BitmapImage> Prideimages = new List<BitmapImage>();

        ObservableCollection<Uri> uris = new ObservableCollection<Uri>();

        private Uri currUri = null;
        private int UriIndex = 0;
        private int _currImage = 0;
        private int NewField;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (string image in Directory.GetFiles("Wallpapers-Pack-3"))
            {
                Uri imagepath = new Uri(image, UriKind.Relative);
                WallpaperImages.Add(new BitmapImage(imagepath));
            }

            foreach (string image in Directory.GetFiles("Pride-2025"))
            {
                Uri imagepath = new Uri(image, UriKind.Relative);
                Prideimages.Add(new BitmapImage(imagepath));
            }
            imageHolder.Source = Prideimages[_currImage];

            foreach (string video in Directory.GetFiles("New Videos"))
            {
                Uri imagepath = new Uri(video, UriKind.Relative);
                uris.Add(imagepath);
            }
            currUri = uris[UriIndex];
            MyMediaElement.Source = currUri;
        }

        private void btnPreviousImage_Click(object sender, RoutedEventArgs e)
        {
            if (imageHolder.Source == WallpaperImages[_currImage])
            {
                if (--_currImage < 0)
                {
                    _currImage = WallpaperImages.Count - 1;
                }
                imageHolder.Source = WallpaperImages[_currImage];
            }
            else
            {
                if (--_currImage < 0)
                {
                    _currImage = Prideimages.Count - 1;
                }
                imageHolder.Source = Prideimages[_currImage];
            }
        }

        private void btnNextImage_Click(object sender, RoutedEventArgs e)
        {
            if (imageHolder.Source == WallpaperImages[_currImage])
            {
                if (++_currImage >= WallpaperImages.Count)
                {
                    _currImage = 0;
                }
                imageHolder.Source = WallpaperImages[_currImage];
            }
            else
            {
                if (++_currImage >= Prideimages.Count)
                {
                    _currImage = 0;
                }
                imageHolder.Source = Prideimages[_currImage];
            }
        }
        private void btnDeleteImage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (imageHolder.Source == WallpaperImages[_currImage])
                {
                    WallpaperImages[_currImage] = null;
                    if (_currImage + 1 > WallpaperImages.Count - 1)
                    {
                        this.imageHolder.Source = WallpaperImages[0];
                    }
                    else
                    {
                        this.imageHolder.Source = WallpaperImages[++_currImage];
                    }
                }
                else if(imageHolder.Source == Prideimages[_currImage])
                {
                    Prideimages[_currImage] = null;
                    if (_currImage + 1 > Prideimages.Count-1)
                    {
                        this.imageHolder.Source = Prideimages[0];
                    }
                    else
                    {
                        this.imageHolder.Source = Prideimages[++_currImage];
                    }
                }
            }
            catch (Exception ex)    
            {
                Console.WriteLine(ex);
            }
        }
        private void ImagesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            List<BitmapImage> currImages = this.ImagesComboBox.SelectedItem as List<BitmapImage>;

            this.imageHolder.Source = currImages[0];
        }
        private void RadioButtonClicked(object sender, RoutedEventArgs e)
        {
            switch ((sender as RadioButton)?.Content.ToString())
            {
                case "Ink Mode":
                    this.MyInkCanvas.EditingMode = InkCanvasEditingMode.Ink;
                    break;
                case "EraseByStroke Mode":
                    this.MyInkCanvas.EditingMode = InkCanvasEditingMode.EraseByStroke;
                    break;
                case "Select Mode":
                    this.MyInkCanvas.EditingMode = InkCanvasEditingMode.Select;
                    break;
                case "EraseByPoint Mode":
                    this.MyInkCanvas.EditingMode = InkCanvasEditingMode.EraseByPoint;
                    break;
            }
        }
        private void Colors_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string colorToUse = (this.MyColors.SelectedItem.ToString());
            this.MyInkCanvas.DefaultDrawingAttributes.Color = (Color)ColorConverter.ConvertFromString(colorToUse);
            SolidColorBrush solidColorBrush = new SolidColorBrush(this.MyInkCanvas.DefaultDrawingAttributes.Color);
            MyColors.Foreground = solidColorBrush;
        }

        private void NextVideoButton_Click(object sender, RoutedEventArgs e)
        {
            if (++UriIndex >= uris.Count)
            {
                UriIndex = 0;
            }
            MyMediaElement.Source = uris[UriIndex];
        }

        private void PreviusVideoButton_Click(object sender, RoutedEventArgs e)
        {
            if (--UriIndex < 0)
            {
                UriIndex = uris.Count - 1;
            }
            MyMediaElement.Source = uris[UriIndex];
        }

        private void ReplayButton_Click(object sender, RoutedEventArgs e)
        {
          MyMediaElement.MediaEnded += Replay;
          MyMediaElement.Play();
        }
        private void Replay(object sender, RoutedEventArgs e)
        {
            MyMediaElement.Close();
            MyMediaElement.Play();
        }

        private void PausePlay_Click(object sender, RoutedEventArgs e)
        {
            MyMediaElement.Pause();
        }

        private void volumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
          MyMediaElement.Volume = (Double)volumeSlider.Value;
        }

        private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var openDlg = new OpenFileDialog { Filter = "Text Files|*.txt" };
            if (true == openDlg.ShowDialog())
            {
                string dataFromFile = File.ReadAllText(openDlg.FileName);
                txtData.Text = dataFromFile;
            }
        }
        private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var saveDlg = new SaveFileDialog { Filter = "Text Files|*.txt" };
            saveDlg.FileOk += SaveDlg_FileOk;
            if (true == saveDlg.ShowDialog())
            {
                File.WriteAllText(saveDlg.FileName, txtData.Text);
            }
        }

        private void Open_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void Save_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void SaveDlg_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            string msg = "Do you want to save this file?";
            MessageBoxResult result = MessageBox.Show(msg, "MyOwnWpfApp", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }
    }
}

