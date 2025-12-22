using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Formats.Tiff;
using SixLabors.ImageSharp.Processing;
using System.IO;
using System.Windows.Forms;
using SkiaSharp;
using Image = SixLabors.ImageSharp.Image;
using Size = SixLabors.ImageSharp.Size;

namespace MySpyImageResizer;

internal static class ImageHelper
{
    public static void ResizeImage(string inputFilePath, string outputPath, int width, int height,
        ImageFormat outputFormat, bool keepAspectRatio = false, bool keepDimensions = false)
    {
        var outputFilePath = EnsureNewExtension(Path.Combine(outputPath, Path.GetFileName(inputFilePath)), outputFormat);
        using var image = Image.Load(inputFilePath);

        if (!keepDimensions)
        {
            if (keepAspectRatio)
            {
                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Mode = ResizeMode.Max,     // keep aspect ratio
                    Size = new Size(width, height),
                    Sampler = KnownResamplers.Lanczos3,
                    Compand = true // 
                }));
            }
            else
                image.Mutate(x => x.Resize(width, height));
        }

        // Used SkiaSharp because ImageSharp don't support PDF export
        if (outputFormat == ImageFormat.Pdf)
        {
            SaveAsPdf(image, outputFilePath);
            return;
        }

        var encoder = GetEncoder(outputFormat);
        image.Save(outputFilePath, encoder);
    }

    private static void SaveAsPdf(Image image, string outputFilePath)
    {
        // ImageSharp → SKBitmap
        using var ms = new MemoryStream();
        image.SaveAsPng(ms);
        ms.Position = 0;

        using var skBitmap = SKBitmap.Decode(ms);
        using var stream = File.OpenWrite(outputFilePath);
        using var doc = SKDocument.CreatePdf(stream);

        #region test to up quality - not working as expected
        //using var skiaImage = SKImage.FromEncodedData(ms);
        //var skiaInfo = new SKImageInfo(skiaImage.Width, skiaImage.Height);
        //var targetDpi = 300;
        //var scale = 72f / targetDpi;
        //var pdfWidth = skiaImage.Width * scale;
        //var pdfHeight = skiaImage.Height * scale;
        //using (var canvas = doc.BeginPage(pdfWidth, pdfHeight))
        //{
        //    var paint = new SKPaint
        //    {
        //        FilterQuality = SKFilterQuality.High,
        //        IsAntialias = true
        //    };

        //    var dest = new SKRect(0, 0, pdfWidth, pdfHeight);
        //    canvas.DrawImage(skiaImage, dest, paint);

        //    doc.EndPage();
        //}
        #endregion

        var page = doc.BeginPage(skBitmap.Width, skBitmap.Height);
        page.DrawBitmap(skBitmap, 0, 0);
        doc.EndPage();

        doc.Close();
    }

    private static IImageEncoder GetEncoder(ImageFormat format)
    {
        return format switch
        {
            ImageFormat.Jpeg => new JpegEncoder { Quality = 100, ColorType = JpegEncodingColor.YCbCrRatio444, Interleaved = true },
            ImageFormat.Png => new PngEncoder { CompressionLevel = PngCompressionLevel.BestCompression, },
            ImageFormat.Bmp => new BmpEncoder(),
            ImageFormat.Gif => new GifEncoder(),
            ImageFormat.Webp => new WebpEncoder() { Quality = 100, FileFormat = WebpFileFormatType.Lossless },
            ImageFormat.Tiff => new TiffEncoder(),
            _ => throw new ArgumentOutOfRangeException(nameof(format), "Invalid format.")
        };
    }

    private static string EnsureNewExtension(string path, ImageFormat format)
    {
        var newExt = format switch
        {
            ImageFormat.Jpeg => ".jpg",
            ImageFormat.Png => ".png",
            ImageFormat.Bmp => ".bmp",
            ImageFormat.Gif => ".gif",
            ImageFormat.Webp => ".webp",
            ImageFormat.Tiff => ".tiff",
            ImageFormat.Pdf => ".pdf",
            _ => throw new ArgumentOutOfRangeException()
        };

        return Path.ChangeExtension(path, newExt);
    }
}
