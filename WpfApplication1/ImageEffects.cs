using System;
using System.Drawing;
using System.Threading.Tasks;

namespace WpfApplication1
{
	public  static class ImageEffects
	{
		public static async Task<Bitmap> BlurAsync(Bitmap image, Int32 blurSize) {
			Bitmap blurred = null;
			await Task.Factory.StartNew(() => blurred = Blur(image, blurSize));
			return blurred;
		}

		public static Bitmap Blur(Bitmap image, Int32 blurSize)
		{
			var blurred = new Bitmap(image.Width, image.Height);
			var rectangle = new Rectangle(0, 0, image.Width, image.Height);
			using (Graphics graphics = Graphics.FromImage(blurred))
				graphics.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height),
					new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);
			for (Int32 xx = rectangle.X; xx < rectangle.X + rectangle.Width; xx++) {
				for (Int32 yy = rectangle.Y; yy < rectangle.Y + rectangle.Height; yy++) {
					Int32 avgR = 0, avgG = 0, avgB = 0;
					Int32 blurPixelCount = 0;
					for (Int32 x = xx; (x < xx + blurSize && x < image.Width); x++) {
						for (Int32 y = yy; (y < yy + blurSize && y < image.Height); y++) {
							Color pixel = blurred.GetPixel(x, y);
							avgR += pixel.R;
							avgG += pixel.G;
							avgB += pixel.B;
							blurPixelCount++;
						}
					}
					avgR = avgR / blurPixelCount;
					avgG = avgG / blurPixelCount;
					avgB = avgB / blurPixelCount;
					for (Int32 x = xx; x < xx + blurSize && x < image.Width && x < rectangle.Width; x++)
						for (Int32 y = yy; y < yy + blurSize && y < image.Height && y < rectangle.Height; y++)
							blurred.SetPixel(x, y, Color.FromArgb(avgR, avgG, avgB));
				}
			}
			return blurred;
		}

	}
}
