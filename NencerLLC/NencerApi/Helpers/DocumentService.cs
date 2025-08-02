using System.Reflection;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.IO;

namespace NencerApi.Helpers
{
    public class DocumentService<T> where T : class
    {

        public byte[] GenerateDoc(string templatePath, T data)
        {
            string outputPath = Path.Combine(Path.GetTempPath(), "OutPutDocument.docx");

            // Tạo bản sao của file template DOCX
            using (var templateStream = new FileStream(templatePath, FileMode.Open, FileAccess.Read))
            using (var generatedStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
            {
                templateStream.CopyTo(generatedStream);
            }

            // Mở file DOCX và thay thế các placeholder bằng dữ liệu từ đối tượng T
            using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(outputPath, true))
            {
                string docText = null;
                using (StreamReader sr = new StreamReader(wordDoc.MainDocumentPart.GetStream()))
                {
                    docText = sr.ReadToEnd();
                }

                // Duyệt qua tất cả các thuộc tính của lớp T và thay thế các placeholder
                foreach (PropertyInfo prop in typeof(T).GetProperties())
                {
                    var value = prop.GetValue(data)?.ToString() ?? string.Empty;
                    string placeholder = "{" + prop.Name.ToLower() + "}"; // Giả định placeholder là {tên_thuộc_tính}
                    docText = docText.Replace(placeholder, value);
                }

                // Ghi lại nội dung mới vào file DOCX
                using (StreamWriter sw = new StreamWriter(wordDoc.MainDocumentPart.GetStream(FileMode.Create)))
                {
                    sw.Write(docText);
                }
            }

            // Trả về file DOCX đã được sinh ra dưới dạng byte[]
            return File.ReadAllBytes(outputPath);
        }







    }
}
