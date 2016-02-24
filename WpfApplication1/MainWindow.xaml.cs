using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WpfApplication1
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private int _currentCanvasIndex;
		private readonly List<Canvas> _canvases = new List<Canvas>();

		public MainWindow() {
			InitializeComponent();
		}

		private void MyScrollViewer_Drop(object sender, DragEventArgs e) {
			var data = (string[])e.Data.GetData(DataFormats.FileDrop);
			foreach (var item in data) {
				AddImage(item);
			}
		}

		void canvas_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
			if (e.ClickCount == 2)
			{
				Action showAction = () => ShowFullSize((Canvas)sender);
				Dispatcher.BeginInvoke(showAction);
			}
		}

		private void btnBloor_Click(object sender, RoutedEventArgs e) {
			GaussianBlur();
		}
		
		private void showImageGrid_PreviewKeyDown(object sender, KeyEventArgs e) {
			if (e.Key == Key.Escape) {
				showImageGrid.Visibility = Visibility.Hidden;
			}
			if (e.Key == Key.Down) {
				PreviousImage();
			}
			if (e.Key == Key.Up) {
				NextImage();
			}
			if (e.Key == Key.B) {
				GaussianBlur();
			}
		}

		private void btnNext_Click(object sender, RoutedEventArgs e) {
			NextImage();
		}

		private void btnPrev_Click(object sender, RoutedEventArgs e) {
			PreviousImage();
		}

		private async void GaussianBlur() {
			var bitmapImage = (BitmapImage)((ImageBrush)(fullSizeImage).Background).ImageSource;
			Bitmap bitmap = bitmapImage.ToBitmap();
			var blured = await ImageEffects.BlurAsync(bitmap, 2);
			var bitmapImageBlur = blured.ToBitmapImage();
			var imageBrush = new ImageBrush {
				ImageSource = bitmapImageBlur
			};
			fullSizeImage.Background = imageBrush;
		}

		private void PreviousImage() {
			if (_currentCanvasIndex == -1) {
				throw new Exception("Error in canvas colletion");
			}
			if (_canvases.Count > _currentCanvasIndex + 1) {
				ShowFullSize(_canvases[_currentCanvasIndex + 1]);
			} else {
				_currentCanvasIndex = 0;
				ShowFullSize(_canvases[_currentCanvasIndex]);
			}
		}

		private void NextImage() {
			if (_currentCanvasIndex == -1) {
				throw new Exception("Error in canvas colletion");
			}
			if (_currentCanvasIndex > 0) {
				ShowFullSize(_canvases[_currentCanvasIndex - 1]);
			} else {
				_currentCanvasIndex = _canvases.Count - 1;
				ShowFullSize(_canvases[_currentCanvasIndex]);
			}
		}

		private void ShowFullSize(Canvas canvas) {
			_currentCanvasIndex = _canvases.IndexOf(canvas);
			var bitmapImage = (BitmapImage)((ImageBrush)canvas.Background).ImageSource;
			var proportionHW = bitmapImage.Height / bitmapImage.Width;
			var proportionWH = bitmapImage.Width / bitmapImage.Height;
			fullSizeImage.Background = canvas.Background;
			double width = showImageGrid.ActualWidth - 120;
			double height = width * proportionHW;

			if (height > showImageGrid.ActualHeight) {
				height = showImageGrid.ActualHeight - 10;
				width = height * proportionWH;
			}
			fullSizeImage.Width = width;
			fullSizeImage.Height = height;
			showImageGrid.Visibility = Visibility.Visible;
			showImageGrid.Focus();
		}

		private void AddImage(string path) {
			var imageBrush = new ImageBrush();
			var bitmapImage = new BitmapImage(new Uri(path, UriKind.Absolute));
			imageBrush.ImageSource = bitmapImage;
			var proportion = bitmapImage.Width / bitmapImage.Height;
			var canvas = new Canvas {
				Width = proportion * 150,
				Height = 150,
				Background = imageBrush,
				Margin = new Thickness(8),
				Cursor = Cursors.Hand
			};
			_canvases.Add(canvas);

			canvas.PreviewMouseLeftButtonDown += canvas_PreviewMouseLeftButtonDown;

			//canvas.MouseLeftButtonUp += CanvasOnMouseLeftButtonUp;
			wrapPanel.Children.Add(canvas);
		}

		

		private void Window_SizeChanged(object sender, SizeChangedEventArgs e) {
			if (_canvases.Count == 0 || showImageGrid.Visibility == Visibility.Hidden) {
				return;
			}
			Canvas canvas = _canvases[_currentCanvasIndex];
			var bitmapImage = (BitmapImage)((ImageBrush)canvas.Background).ImageSource;
			var proportionHW = bitmapImage.Height / bitmapImage.Width;
			var proportionWH = bitmapImage.Width / bitmapImage.Height;
			double width = showImageGrid.ActualWidth - 120;
			double height = width * proportionHW;

			if (height > showImageGrid.ActualHeight) {
				height = showImageGrid.ActualHeight - 10;
				width = height * proportionWH;
			}
			fullSizeImage.Width = width;
			fullSizeImage.Height = height;
		}

	}
}
