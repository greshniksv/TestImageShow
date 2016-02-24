using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;

namespace WpfApplication1
{
	public static class BitmapImageExtension
	{
		public static Bitmap ToBitmap(this BitmapImage bitmapImage)
		{
			using (var outStream = new MemoryStream()) {
				BitmapEncoder enc = new BmpBitmapEncoder();
				enc.Frames.Add(BitmapFrame.Create(bitmapImage));
				enc.Save(outStream);
				var bitmap = new Bitmap(outStream);
				return new Bitmap(bitmap);
			}
		}
	}
}
