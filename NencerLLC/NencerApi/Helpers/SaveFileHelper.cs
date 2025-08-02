using NencerApi.Extentions;

namespace NencerApi.Helpers
{
    public static class SaveFileHelper
    {
        /// <summary>
        /// Tạo folder bên trong Document mặc định windown
        /// </summary>
        /// <param name="folderName">Tên folder sẽ tạo bên trong</param>
        /// <param name="isFolderWithDay">True là tạo folder con theo ngày bên trong folderName</param>
        /// <returns>Trả về 1 folder đã tạo trong Document của windown. Ví dụ KQ: ..\Documents\NencerLLC\folderName\FolderWithDay</returns>
        public static string GetPathFromDocument(string? folderName = null, bool isFolderWithDay = false)
        {
            try
            {
                var path = string.Empty;

                string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                string documentsPathDoc = documentsPath + "\\NencerLLC";

                if (!string.IsNullOrEmpty(folderName))
                {
                    path = "\\" + folderName.Trim().Trim('\\').Trim('/');
                }

                documentsPathDoc = documentsPathDoc + path;

                if (isFolderWithDay)
                {
                    documentsPathDoc += "\\" + DateTime.Now.ToString("ddMMyyyy");
                }

                // Kiểm tra xem thư mục đã tồn tại chưa
                if (!Directory.Exists(documentsPathDoc))
                {
                    // Tạo thư mục
                    Directory.CreateDirectory(documentsPathDoc);
                }
                return documentsPathDoc;
            }
            catch (Exception ex)
            {
                LogHelper.Exception("Lỗi khi GetPathFromDocument", ex);
            }

            return string.Empty;
        }

        /// <summary>
        /// Tạo folder và xoá file nếu đã tồn tại
        /// </summary>
        /// <param name="folderName"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string CreateFolderAndDelFileExists(string? folderName = null, string? fileName = null)
        {
            try
            {
                var path = string.Empty;

                string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                string documentsPathDoc = documentsPath + "\\NencerLLC";

                if (!string.IsNullOrEmpty(folderName))
                {
                    path = "\\" + folderName.Trim().Trim('\\').Trim('/');

                    if (!string.IsNullOrEmpty(fileName))
                    {
                        var linkFile = folderName + "\\" + fileName.Trim().Trim('\\').Trim('/');

                        if (File.Exists(linkFile))
                        {
                            File.Delete(linkFile);
                        }
                    }
                }

                documentsPathDoc = documentsPathDoc + path;

                // Kiểm tra xem thư mục đã tồn tại chưa
                if (!Directory.Exists(documentsPathDoc))
                {
                    // Tạo thư mục
                    Directory.CreateDirectory(documentsPathDoc);
                }
                return documentsPathDoc;
            }
            catch (Exception ex)
            {
                LogHelper.Exception("Lỗi khi CreateFolderAndDelFileExists", ex);
            }

            return string.Empty;
        }

        /// <summary>
        /// Đổi tên file trong trường hợp ghi đè hoặc copy file bị lỗi, 
        /// với quy ước link phải đúng như sau, ví dụ: C:\Users\Admin\Documents\File.txt;
        /// Nếu nó có dạng "/" thì sẽ lỗi, cần đổi lại về "\"
        /// </summary>
        /// <param name="linkFileName">link path chứa đến file</param>
        /// <returns>1 link file có tên mới</returns>
        public static string ChangeFileNameWhenError(string linkFileName)
        {
            var filePath = Path.GetDirectoryName(linkFileName);

            var fileName2 = string.Empty;

            if (string.IsNullOrEmpty(filePath))
            {
                return string.Empty;
            }

            if (CanReadOrCopyFile(linkFileName))
            {
                return linkFileName;
            }
            else
            {
                string fileName = Path.GetFileName(linkFileName);

                var splitFilenameDot = fileName.ToEmptyWhenNull().Split(".");
                var splitFilename = splitFilenameDot[0].Split("-");

                if (splitFilename.Count() == 2)
                {
                    var replaceDigit = splitFilename[1].Replace("(Add", "").Replace(")", "").Trim();

                    var digitEnd = Convert.ToInt32(replaceDigit) + 1;
                    fileName2 = splitFilename[0] + " - (Add " + digitEnd + ")";
                }
                else
                {
                    fileName2 = splitFilename[0] + " - (Add 1)";
                }

                var linkFileNew = Path.Combine(filePath, fileName2);

                ChangeFileNameWhenError(linkFileNew);
            }

            return string.Empty;
        }

        private static bool CanReadOrCopyFile(string filePath)
        {
            try
            {
                // Thử mở file với quyền truy cập đọc
                using (FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    // Nếu không có lỗi, file không đang được mở độc quyền và có thể đọc hoặc copy
                    return true;
                }
            }
            catch (IOException)
            {
                // Nếu có lỗi, có thể do file đang được mở không cho phép truy cập đọc hoặc vấn đề khác
                return false;
            }
        }

        /// <summary>
        /// Kiểm tra link file định tạo có lỗi không, nếu lỗi tạo file name mới
        /// </summary>
        /// <param name="folderName"></param>
        /// <param name="fileName"></param>
        /// <param name="ext"></param>
        /// <returns>link file path mới</returns>
        public static string CreateLinkFileNotError(string folderName, bool isFolderWithDay = false, string? fileName = null, string ext = ".pdf")
        {

            bool isFileUsable = false;
            int addNumber = 0; // Khởi tạo số để tăng tên file nếu file cũ bị lỗi

            string folderDoc = GetPathFromDocument(folderName, isFolderWithDay);

            if (string.IsNullOrEmpty(fileName))
            {
                var fileNameRandom = Guid.NewGuid().ToString() + ext;
                return Path.Combine(folderDoc, fileNameRandom);
            }

            // Tạo đường dẫn file dựa trên số thứ tự hiện tại
            string linkFileOld = Path.Combine(folderDoc, fileName);

            string linkFileNew = string.Empty;

            string fileName2 = fileName;

            do
            {
                // Tạo đường dẫn file dựa trên số thứ tự hiện tại
                linkFileOld = Path.Combine(folderDoc, fileName2);

                if (!File.Exists(linkFileOld))
                {
                    // Nếu file không tồn tại, đánh dấu là có thể sử dụng và sử dụng đường dẫn này
                    isFileUsable = true;
                    linkFileNew = linkFileOld;

                }
                else
                {
                    try
                    {
                        // Thử mở file để kiểm tra xem nó có bị lỗi không
                        using (FileStream fs = File.Open(linkFileOld, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                        {
                            isFileUsable = true; // File có thể mở => không bị lỗi

                            linkFileNew = linkFileOld;
                        }
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Exception("File đang được sử dụng", ex);
                        // File bị lỗi, cần tạo tên file mới
                        addNumber++; // Tăng số lên để tạo tên file mới
                        fileName2 = GenerateNewFileName(fileName, addNumber, ext);
                        // Không cần đặt isFileUsable = false; ở đây vì vòng lặp sẽ tiếp tục
                    }
                }
            } while (!isFileUsable); // Lặp lại cho đến khi tìm được file không bị lỗi

            return linkFileNew; // Cập nhật FileName mới không bị lỗi
        }

        private static string GenerateNewFileName(string originalFileName, int addition, string ext = ".pdf")
        {
            var splitFilenameDot = originalFileName.Split('.');
            var baseName = splitFilenameDot[0];
            var extension = splitFilenameDot.Length > 1 ? $".{splitFilenameDot[1]}" : ext; // Xử lý trường hợp file không có extension

            string newFileName = $"{baseName}_Add{addition}{extension}";

            return newFileName;
        }

        /// Tạo folder bên trong Document mặc định windown
        /// </summary>
        /// <param name="folderName">Tên folder sẽ tạo bên trong</param>
        /// <param name="isFolderWithDay">True là tạo folder con theo ngày bên trong folderName</param>
        /// <returns>Trả về 1 folder đã tạo trong Document của windown. Ví dụ KQ: ..\Documents\NencerLLC\folderName\FolderWithDay</returns>
        public static string SaveFileInRoot(string folderName, bool isFolderWithDay = false)
        {
            try
            {
                var path = folderName;

                if (isFolderWithDay)
                {
                    path = Path.Combine(path, DateTime.Now.ToString("ddMMyyyy"));
                }

                // Kiểm tra xem thư mục đã tồn tại chưa
                if (!Directory.Exists(path))
                {
                    // Tạo thư mục
                    Directory.CreateDirectory(path);
                }
                return path;
            }
            catch (Exception ex)
            {
                LogHelper.Exception("Có lỗi xảy ra khi lấy đường dẫn folder", ex);
            }

            return string.Empty;
        }
    }
}
