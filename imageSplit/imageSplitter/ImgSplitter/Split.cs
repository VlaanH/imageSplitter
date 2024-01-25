using System.IO;
using Plugin.FilePicker.Abstractions;
using Xamarin.Forms;

using SkiaSharp;


namespace imageSplit.ImgSplitter
{
    public class Split
    {
        public static async void SplitAndSaveBitmap(SKBitmap sourceBitmap, int numberOfParts, string outputDirectory,string name)
        {
            int partHeight = sourceBitmap.Height / numberOfParts;

            for (int i = 0; i < numberOfParts; i++)
            {
                SKBitmap part = new SKBitmap(sourceBitmap.Width, partHeight);
                using (SKCanvas canvas = new SKCanvas(part))
                {
                    SKRect srcRect = new SKRect(0, i * partHeight, sourceBitmap.Width, (i + 1) * partHeight);
                    SKRect destRect = new SKRect(0, 0, sourceBitmap.Width, partHeight);
                    canvas.DrawBitmap(sourceBitmap, srcRect, destRect);
                }
                SaveSKImage(part,outputDirectory+i.ToString()+name, SKEncodedImageFormat.Png);
            }

           
        }
        public static void SaveSKImage(SKBitmap image, string outputPath, SKEncodedImageFormat format, int quality = 100)
        {
            using (SKData encoded = image.Encode(format, quality))
            using (Stream stream = File.OpenWrite(outputPath))
            {
                encoded.SaveTo(stream);
            }
        }
        
        public static SKBitmap ConvertFileDataToSKBitmap(FileData fileData)
        {
            using (Stream stream = new MemoryStream(fileData.DataArray))
            {
                return SKBitmap.Decode(stream);
            }
        }

        public static ImageSource DrawStripesOnImage(SKBitmap originalBitmap, int horizontalLines, int verticalLines)
        {
            SKBitmap dividedBitmap = new SKBitmap(originalBitmap.Width, originalBitmap.Height);

            using (SKCanvas canvas = new SKCanvas(dividedBitmap))
            {
                canvas.DrawBitmap(originalBitmap, new SKPoint(0, 0));

                using (SKPaint paint = new SKPaint())
                {
                    paint.Color = SKColors.Pink; // Line color

                    var lineSizePercentage = originalBitmap.Width * 0.01; //Line size in percentage
                    
                    paint.StrokeWidth = (int)lineSizePercentage; 

                    // Calculate the spacing between lines
                    float horizontalSpacing = originalBitmap.Height / (float)horizontalLines;
                    float verticalSpacing = originalBitmap.Width / (float)verticalLines;

                    // Draw horizontal lines
                    for (int i = 1; i < horizontalLines; i++)
                    {
                        float y = i * horizontalSpacing;
                        canvas.DrawLine(0, y, originalBitmap.Width, y, paint);
                    }

                    // Draw vertical lines
                    for (int i = 1; i < verticalLines; i++)
                    {
                        float x = i * verticalSpacing;
                        canvas.DrawLine(x, 0, x, originalBitmap.Height, paint);
                    }
                }
            }
            
            return ConvertToImageSource(dividedBitmap);
        }

        private static ImageSource ConvertToImageSource(SKBitmap skBitmap)
        {
            SKImage image = SKImage.FromPixels(skBitmap.PeekPixels());

            SKData encoded = image.Encode();

            Stream stream = encoded.AsStream();
            
            return ImageSource.FromStream(()=> stream);
        }
        
        
    }
    
    
    
}
   
