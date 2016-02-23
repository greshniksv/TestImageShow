using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
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
using Color = System.Windows.Media.Color;
using Rectangle = System.Windows.Shapes.Rectangle;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MyScrollViewer_Drop(object sender, DragEventArgs e)
        {
            var data = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (var item in data)
            {
                AddImage(item);
            }
        }

        private async void AddImage(string path)
        {
            ImageBrush imageBrush = new ImageBrush();
            var bitmapImage = new BitmapImage(new Uri(path, UriKind.Absolute));
            imageBrush.ImageSource = bitmapImage;
            var proportion = bitmapImage.Width / bitmapImage.Height;
            var canvas = new Canvas
            {
                Width = proportion * 150,
                Height = 150,
                Background = imageBrush,
                Margin = new Thickness(8),
                Cursor = Cursors.Hand
            };

            canvas.MouseLeftButtonUp += CanvasOnMouseLeftButtonUp;

            /*canvas.MouseLeftButtonUp += (sender, args) =>
            {
                Bitmap bitmap = BitmapImage2Bitmap(bitmapImage);
                var blured = Blur(bitmap, 3);
                var bitmapImageBlur = await ToBitmapImage(blured);
                ImageBrush imageBrush1 = new ImageBrush();
                imageBrush1.ImageSource = bitmapImageBlur;
                fullSizeImage.Background = imageBrush1;
                fullSizeImage.Height = fullSizeImage.Width * proportion;
                showImageGrid.Visibility = Visibility.Visible;
            };*/
            wrapPanel.Children.Add(canvas);
        }

        private async void CanvasOnMouseLeftButtonUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            BitmapImage bitmapImage = (BitmapImage)((ImageBrush)((Canvas)sender).Background).ImageSource;
            var proportion = bitmapImage.Height / bitmapImage.Width;
            Bitmap bitmap = BitmapImage2Bitmap(bitmapImage);
            var blured = await Blur(bitmap, 3);
            var bitmapImageBlur = ToBitmapImage(blured);
            ImageBrush imageBrush1 = new ImageBrush();
            imageBrush1.ImageSource = bitmapImageBlur;
            fullSizeImage.Background = imageBrush1;
            fullSizeImage.Height = fullSizeImage.ActualWidth * proportion;
            showImageGrid.Visibility = Visibility.Visible;
        }


        private Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
        {
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                Bitmap bitmap = new Bitmap(outStream);
                return new Bitmap(bitmap);
            }
        }

        public static BitmapImage ToBitmapImage(Bitmap bitmap)
        {
            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Bmp);
                memory.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                return bitmapImage;
            }
        }

        private void btnBloor_Click(object sender, RoutedEventArgs e)
        {
            showImageGrid.Visibility = Visibility.Hidden;
        }

        private static async Task<System.Drawing.Bitmap> Blur(Bitmap image, Int32 blurSize)
        {
            Bitmap blurred = new Bitmap(image.Width, image.Height);
            await Task.Factory.StartNew(() =>
            {
                var rectangle = new System.Drawing.Rectangle(0, 0, image.Width, image.Height);
                // make an exact copy of the bitmap provided
                using (Graphics graphics = Graphics.FromImage(blurred))
                    graphics.DrawImage(image, new System.Drawing.Rectangle(0, 0, image.Width, image.Height),
                        new System.Drawing.Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);

                // look at every pixel in the blur rectangle
                for (Int32 xx = rectangle.X; xx < rectangle.X + rectangle.Width; xx++)
                {
                    for (Int32 yy = rectangle.Y; yy < rectangle.Y + rectangle.Height; yy++)
                    {
                        Int32 avgR = 0, avgG = 0, avgB = 0;
                        Int32 blurPixelCount = 0;

                        // average the color of the red, green and blue for each pixel in the
                        // blur size while making sure you don't go outside the image bounds
                        for (Int32 x = xx; (x < xx + blurSize && x < image.Width); x++)
                        {
                            for (Int32 y = yy; (y < yy + blurSize && y < image.Height); y++)
                            {
                                System.Drawing.Color pixel = blurred.GetPixel(x, y);

                                avgR += pixel.R;
                                avgG += pixel.G;
                                avgB += pixel.B;

                                blurPixelCount++;
                            }
                        }

                        avgR = avgR / blurPixelCount;
                        avgG = avgG / blurPixelCount;
                        avgB = avgB / blurPixelCount;

                        // now that we know the average for the blur size, set each pixel to that color
                        for (Int32 x = xx; x < xx + blurSize && x < image.Width && x < rectangle.Width; x++)
                            for (Int32 y = yy; y < yy + blurSize && y < image.Height && y < rectangle.Height; y++)
                                blurred.SetPixel(x, y, System.Drawing.Color.FromArgb(avgR, avgG, avgB));
                    }
                }

            }
                );

            return blurred;

        }


    }
}
