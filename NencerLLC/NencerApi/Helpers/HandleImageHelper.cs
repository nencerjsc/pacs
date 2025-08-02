using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Drawing;
using ZXing.QrCode;
using ZXing;

namespace NencerApi.Helpers
{
    public static class HandleImageHelper
    {
        /// <summary>
        /// Chồng 2 ảnh lên nhau
        /// </summary>
        /// <param name="imagePath1">Ảnh này nằm bên dưới ảnh đè (imagePath2)</param>
        /// <param name="imagePath2">Ảnh này nằm bên trên ảnh đè (imagePath1)</param>
        /// <returns>Trả về 1 đường dẫn ảnh đã tạo</returns>
        public static string OverlayImages(string imagePath1, string imagePath2)
        {
            using (Bitmap image1 = new Bitmap(imagePath1))
            using (Bitmap image2 = new Bitmap(imagePath2))
            {
                int maxWidth = Math.Max(image1.Width, image2.Width);
                int maxHeight = Math.Max(image1.Height, image2.Height);

                using (Bitmap resultImage = new Bitmap(maxWidth, maxHeight, PixelFormat.Format32bppArgb))
                using (Graphics graphics = Graphics.FromImage(resultImage))
                {
                    graphics.DrawImage(image1, new Rectangle(0, 0, image1.Width, image1.Height));
                    int x = (maxWidth - image2.Width) / 2;
                    int y = (maxHeight - image2.Height) / 2;
                    graphics.DrawImage(image2, new Rectangle(x, y, image2.Width, image2.Height));

                    string outputPath = Path.Combine("Shared", "HSM", "image", "layout.png");
                    CreateDirectoryIfNeeded(outputPath);

                    try
                    {
                        resultImage.Save(outputPath, ImageFormat.Png);
                    }
                    catch (Exception ex)
                    {
                        return null;
                    }

                    return outputPath;
                }
            }
        }

        private static void CreateDirectoryIfNeeded(string filePath)
        {
            string directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        /// <summary>
        /// Lấy kích thước ảnh
        /// </summary>
        /// <param name="path">Link đến vị trí ảnh</param>
        /// <returns>(width, height)</returns>
        public static (int, int) GetSizeImage(string path)
        {
            try
            {
                if (!File.Exists(path))
                {
                    return (0, 0);
                }

                using (Image image = Image.FromFile(path))
                {
                    int width = image.Width;
                    int height = image.Height;
                    return (width, height);
                }
            }
            catch (Exception ex)
            {
            }

            return (0, 0);
        }

        /// <summary>
        /// DÙng để thay đổi kích thước cân đối cho ảnh
        /// </summary>
        /// <param name="image">Ảnh truyền vào</param>
        /// <param name="width">Chiều rộng mong muốn</param>
        /// <param name="height">Chiều cao mong muốn</param>
        /// <returns>Trả về 1 bitmap ảnh</returns>
        public static Bitmap ResizeImageHighQuality(Image image, int width, int height)
        {
            try
            {
                var destRect = new Rectangle(0, 0, width, height);
                var destImage = new Bitmap(width, height, PixelFormat.Format32bppArgb);

                destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

                using (var graphics = Graphics.FromImage(destImage))
                {
                    graphics.CompositingMode = CompositingMode.SourceCopy;
                    graphics.CompositingQuality = CompositingQuality.HighQuality;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.SmoothingMode = SmoothingMode.HighQuality;
                    graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                    using (var wrapMode = new ImageAttributes())
                    {
                        wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                        graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                    }
                }

                return destImage;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Chuyển đổi ảnh thành dạng Base64
        /// </summary>
        /// <param name="imagePath">Link ảnh</param>
        /// <returns>Trả về 1 string Base64</returns>
        public static string ImageToBase64(string? imagePath)
        {
            if (string.IsNullOrEmpty(imagePath)) return null;

            using (Image image = Image.FromFile(imagePath))
            {
                using (MemoryStream m = new MemoryStream())
                {
                    image.Save(m, ImageFormat.Jpeg);
                    byte[] imageBytes = m.ToArray();
                    string base64String = Convert.ToBase64String(imageBytes);
                    return base64String;
                }
            }
        }

        /// <summary>
        /// Chuyển đổi lại string từ Base64 về dạng ảnh
        /// </summary>
        /// <param name="base64String">string Base64</param>
        /// <returns>Trả về 1 ảnh</returns>
        public static Image? Base64ToImage(string? base64String)
        {
            if (string.IsNullOrEmpty(base64String)) return null;

            byte[] imageBytes = Convert.FromBase64String(base64String);
            using (var ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
            {
                Image image = Image.FromStream(ms, true);
                return image;
            }
        }

        public static byte[]? ByteToImage(string? base64String)
        {
            if (string.IsNullOrEmpty(base64String)) return null;

            byte[] imageBytes = Convert.FromBase64String(base64String);
            return imageBytes;
        }

        public static byte[]? CreateByteToImage(string? linkImage)
        {
            if (string.IsNullOrEmpty(linkImage)) return null;

            if (File.Exists(linkImage))
            {
                byte[] imageBytes = File.ReadAllBytes(linkImage);
                return imageBytes;
            }

            return null;
        }



        /// <summary>
        /// Resize lại kích thước tiêu chuẩn
        /// </summary>
        /// <param name="linkImage">Link đến vị trí ảnh</param>
        /// <param name="fixedHeight">Chiều cao của ảnh cần lấy</param>
        /// <returns>(width, height)</returns>
        public static (int, int) GetSizeResizeImage(string linkImage, int fixedHeight = 80)
        {
            if (!File.Exists(linkImage))
            {
                return (0, 0);
            }

            var sizeImg = GetSizeImage(linkImage);
            int w = sizeImg.Item1;
            int h = sizeImg.Item2;

            if (sizeImg.Item2 > fixedHeight)
            {
                w = (int)(w * ((double)fixedHeight / h));
                h = fixedHeight;
            }

            return (w, h);
        }

        /// <summary>
        /// Tạo hình ảnh có độ rộng tương ứng với văn bản
        /// </summary>
        /// <param name="textLine"></param>
        /// <param name="imageBitmap"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static MemoryStream? CreateImageHasTextAutoWidth(string textLine, Bitmap? imageBitmap = null, int height = 20)
        {
            Font font = new Font("Times New Roman", 20, FontStyle.Bold);
            Color backgroundColor = Color.White; // Màu nền
            Color textColor = Color.Black; // Màu chữ

            // Tạo đối tượng Graphics từ Bitmap để đo kích thước của text
            using (var tmpImg = new Bitmap(1, 1))
            {
                using (var tmpG = Graphics.FromImage(tmpImg))
                {
                    tmpG.TextRenderingHint = TextRenderingHint.AntiAlias;

                    // Đo kích thước của text
                    SizeF textSize = tmpG.MeasureString(textLine, font);

                    // Tính toán chiều rộng dựa trên chiều dài của text + padding
                    int width = (int)Math.Ceiling(textSize.Width) + 20; // Thêm padding để không cắt text

                    using (Bitmap image = new Bitmap(width, height, PixelFormat.Format32bppArgb))
                    {
                        using (Graphics graphics = Graphics.FromImage(image))
                        {
                            graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
                            graphics.Clear(backgroundColor);

                            Brush textBrush = new SolidBrush(textColor);

                            // Vì đã thêm padding, text sẽ được căn giữa một cách tự nhiên
                            PointF textPosition = new PointF(10, (height - textSize.Height) / 2); // Thêm padding bên trái là 10

                            graphics.DrawString(textLine, font, textBrush, textPosition);

                            if (imageBitmap != null)
                            {
                                // Xử lý thêm ảnh bitmap nếu cần
                                return OverlayImages(imageBitmap, image);
                            }

                            //string outputPath = "D:\\text_auto_width.png";
                            //try
                            //{
                            //    image.Save(outputPath, ImageFormat.Png);
                            //    return outputPath;
                            //}
                            //catch (Exception ex)
                            //{
                            //    Console.WriteLine($"Lỗi khi lưu ảnh: {ex.Message}");
                            //    return string.Empty;
                            //}
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Chồng 2 bitmap lên nhau
        /// </summary>
        /// <param name="bitMap1">Ảnh bipmat này nằm bên dưới ảnh đè (bipMap2)</param>
        /// <param name="bitMap2">Ảnh bitmap này nằm bên trên ảnh đè bipMap1</param>
        /// <returns>Trả về 1 đường dẫn ảnh đã tạo</returns>
        public static MemoryStream? OverlayImages(Bitmap bitMap1, Bitmap bitMap2)
        {
            // Độ rộng giữ nguyên như bitMap1
            int maxWidth = bitMap1.Width;
            // Chiều cao mới bằng tổng chiều cao của bitMap1 và thêm 14 pixels
            int additionalHeight = 14; // chiều cao bottom thêm
            int maxHeight = bitMap1.Height + additionalHeight;
            Color backgroundColor = Color.White; // Màu nền

            Bitmap resultImage = new Bitmap(maxWidth, maxHeight, PixelFormat.Format32bppArgb);

            using (Graphics graphics = Graphics.FromImage(resultImage))
            {
                // Cài đặt chất lượng cao cho việc vẽ
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                graphics.Clear(backgroundColor);
                // Vẽ bitMap1 vào resultImage
                graphics.DrawImage(bitMap1, 0, 0);

                //resultImage.Save("D:\\overlayed_image33388888_77.png", ImageFormat.Png);

                // Tính lại kích thước cho bitMap2 để vừa vặn với chiều rộng của bitMap1 và chiều cao thêm vào
                // additionalHeight - 2 để tạo thêm 2pixel padding-top , ví dụ Tạo độ cao bottom là 14 thì chỉ lấy 12
                using (Bitmap resizedBitMap2 = new Bitmap(bitMap2, maxWidth, additionalHeight - 2))
                {
                    // Vẽ bitMap2 đã điều chỉnh vào phần dưới cùng của resultImage
                    // bitMap1.Height + 2 tạo thêm 2 pixel padding-bottom, ví dụ độ cao 20 + `2` là `2` là thêm vào để tạo padding-bottom
                    graphics.DrawImage(resizedBitMap2, 0, bitMap1.Height + 2);
                    //resizedBitMap2.Save("D:\\overlayed_image33388888_7766.png", ImageFormat.Png);
                }

                //// Lưu ảnh kết quả
                //string outputPath = "D:\\overlayed_image33388888kkkk.png";
                //try
                //{
                //    resultImage.Save(outputPath, ImageFormat.Png);
                //}
                //catch (Exception ex)
                //{
                //    // Xử lý ngoại lệ
                //    Console.WriteLine("Lỗi khi lưu ảnh chồng bitmap: " + ex.Message);
                //    return null;
                //}

                //return outputPath;
            }

            // Lưu hình ảnh vào một tệp
            var ms = new MemoryStream();

            resultImage.Save(ms, ImageFormat.Png);

            // Đảm bảo con trỏ MemoryStream quay trở lại đầu stream trước khi trả về
            ms.Position = 0;
            return ms;
        }


        /// <summary>
        /// Tạo ảnh byte QRCode 
        /// </summary>
        /// <param name="content"></param>
        /// <param name="digit">Số hiện dưới bottom QR code</param>
        /// <param name="height"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        public static byte[]? CreateByteQRCodeImage(string content, string digit, int? height = null, int? width = null)
        {
            byte[]? bytes = null;
            var qrcodeStream = CreateQRCodeImageMemory(content, digit, true, height, width);
            try
            {
                if (qrcodeStream != null)
                {
                    bytes = qrcodeStream.ToArray();
                }
            }
            finally
            {
                qrcodeStream?.Dispose(); // Đảm bảo dispose MemoryStream sau khi sử dụng xong
            }
            return bytes;

        }

        //tạo file ảnh tạm để chèn cho 1 textParam
        public static string CreateTempImageForParamFromBase64(string textParam, string? base64Image, int? fixedHeight = null)
        {
            try
            {
                string folderBarCode = SaveFileHelper.GetPathFromDocument("ChuKy");

                if (!Directory.Exists(folderBarCode))
                {
                    // Nếu thư mục không tồn tại, tạo thư mục mới
                    Directory.CreateDirectory(folderBarCode);
                }

                var imageByte = HandleImageHelper.ByteToImage(base64Image);

                var linkImg = textParam + ".png";

                if (imageByte != null)
                {
                    linkImg = SaveFileHelper.CreateLinkFileNotError("ChuKy", false, linkImg, ".JPEG");

                    SaveImageToFile(linkImg, imageByte);
                }

                return linkImg;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static void SaveImageToFile(string filePath, byte[] imageBytes)
        {
            // Sử dụng MemoryStream để tạo Image từ mảng byte
            using (MemoryStream ms = new MemoryStream(imageBytes))
            {
                // Tạo Image từ MemoryStream
                using (Image image = Image.FromStream(ms))
                {
                    // Đảm bảo thư mục tồn tại
                    var directory = Path.GetDirectoryName(filePath);
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    // Lưu hình ảnh
                    try
                    {
                        // Lưu Image vào file
                        image.Save(filePath, ImageFormat.Jpeg); // Hoặc định dạng khác tùy theo yêu cầu
                    }
                    catch (Exception ex)
                    {
                        // Xử lý hoặc ghi log lỗi tại đây
                        LogHelper.Exception("Không thể lưu hình ảnh: " + ex.Message, ex);
                    }

                }
            }
        }

        private static MemoryStream? CreateQRCodeImageMemory(string content, string digit, bool isShowNumberBacode = false, int? height = null, int? width = null)
        {
            var writer = new BarcodeWriterPixelData()
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new QrCodeEncodingOptions
                {
                    Height = height ?? 80, // Đặt chiều cao mặc định nếu không được cung cấp
                    Width = width ?? 80, // Đặt chiều rộng mặc định nếu không được cung cấp
                    Margin = 0,
                    PureBarcode = true
                }
            };

            var pixelData = writer.Write(content);

            var bitmap = new Bitmap(pixelData.Width, pixelData.Height, System.Drawing.Imaging.PixelFormat.Format32bppRgb);

            // Điền pixelData vào bitmap
            var bitmapData = bitmap.LockBits(new Rectangle(0, 0, pixelData.Width, pixelData.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            try
            {
                System.Runtime.InteropServices.Marshal.Copy(pixelData.Pixels, 0, bitmapData.Scan0, pixelData.Pixels.Length);
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);
            }

            if (!isShowNumberBacode)
            {
                var ms = new MemoryStream();

                // Lưu bitmap vào MemoryStream
                bitmap.Save(ms, ImageFormat.Jpeg);

                // Đảm bảo con trỏ MemoryStream quay trở lại đầu stream trước khi trả về
                ms.Position = 0;

                return ms;
            }

            // Tính toán kích thước mới cho bitmap để thêm text
            var msNumber = CreateImageHasTextAutoWidth(digit, bitmap);

            return msNumber;

        }

        /// <summary>
        /// Tạo ảnh có nội dung
        /// </summary>
        /// <param name="textLines">Là 1 list text dùng để tạo nhiều câu và xuống dòng dựa vào từng phần tử con</param>
        /// <param name="linkForderSaveImage">Lưu ảnh vào đường dẫn chỉ định</param>
        /// <param name="linkImageSign">Đường dẫn đến ảnh cần chèn</param>
        /// <param name="width">Chiều rộng của ảnh</param>
        /// <param name="height">Chiều cao của ảnh</param>
        /// <param name="isNew">Có tạo mới ảnh hay không, true là có</param>
        /// <returns>Trả về 1 đường dẫn ảnh đã tạo</returns>
        public static string CreateImageSignature(string[] textLines, string linkForderSaveImage, string linkImageSign, int width = 580, int height = 120, bool isNew = false)
        {
            var dateToday = DateTime.Now.ToString("ddMMyyyy");
            var dateOld = DateTime.Now.AddDays(-1).ToString("ddMMyyyy");

            string outputPath = Path.Combine(linkForderSaveImage, $"layout_{dateToday}.png");
            string outputPathOld = Path.Combine(linkForderSaveImage, $"layout_{dateOld}.png");
            //if(File.Exists(outputPath))
            //{
            //    return outputPath;
            //}
            try
            {
                if (File.Exists(outputPathOld))
                {
                    File.Delete(outputPathOld);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Exception("Lỗi xoá ảnh layout cũ", ex);
            }

            CreateDirectoryIfNeeded(linkForderSaveImage);

            using (Bitmap image = new Bitmap(width, height, PixelFormat.Format32bppArgb))
            {
                using (Graphics graphics = Graphics.FromImage(image))
                {
                    graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
                    Font font = new Font("Times New Roman", 18);
                    Brush textBrush = new SolidBrush(Color.Blue);
                    PointF textPosition = new PointF(20, 20);
                    float lineHeight = font.GetHeight() + 0;

                    foreach (string line in textLines)
                    {
                        graphics.DrawString(line, font, textBrush, textPosition);
                        textPosition.Y += lineHeight;
                    }
                }

                try
                {
                    OverlayImagesV3(outputPath, linkImageSign, image);
                }
                catch (Exception ex)
                {
                    LogHelper.Exception("Lỗi khi lưu ảnh: ", ex);
                    return null;
                }
            }

            return outputPath;
        }

        /// <summary>
        /// Chồng 2 ảnh lên nhau
        /// </summary>
        /// <param name="linkFileSave">Link dẫn đến ảnh để lưu ảnh, nếu có ảnh thì ghi đè</param>
        /// <param name="imagePath1">Ảnh này nằm bên dưới ảnh đè bitmap</param>
        /// <param name="bitmapImage2">Ảnh bitmap này nằm bên trên ảnh đè imagePath1</param>
        /// <returns>Trả về 1 đường dẫn ảnh đã tạo</returns>
        public static string OverlayImagesV3(string linkFileSave, string imagePath1, Bitmap bitmapImage2)
        {
            using (Bitmap image1 = new Bitmap(imagePath1))
            {
                int maxWidth = Math.Max(image1.Width, bitmapImage2.Width);
                int maxHeight = Math.Max(image1.Height, bitmapImage2.Height);

                using (Bitmap resultImage = new Bitmap(maxWidth, maxHeight, PixelFormat.Format32bppArgb))
                using (Graphics graphics = Graphics.FromImage(resultImage))
                {
                    // Tính toán vị trí để đảm bảo image1 nằm ở giữa
                    int x1 = (maxWidth - image1.Width) / 2;
                    int y1 = (maxHeight - image1.Height) / 2;

                    // Tính toán vị trí để đảm bảo bitmapImage2 nằm ở giữa
                    int x2 = (maxWidth - bitmapImage2.Width) / 2;
                    int y2 = (maxHeight - bitmapImage2.Height) / 2;

                    graphics.DrawImage(image1, new Rectangle(x1, y1, image1.Width, image1.Height));
                    graphics.DrawImage(bitmapImage2, new Rectangle(x2, y2, bitmapImage2.Width, bitmapImage2.Height));

                    try
                    {
                        resultImage.Save(linkFileSave, ImageFormat.Png);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Exception("Lỗi khi lưu ảnh chồng: ", ex);
                        return null;
                    }

                    return linkFileSave;
                }
            }
        }
    }
}
