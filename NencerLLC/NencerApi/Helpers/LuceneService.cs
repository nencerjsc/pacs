using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using Lucene.Net.QueryParsers.Classic;
using System.Globalization;
using System.Text;

namespace NencerApi.Helpers
{

    public class LuceneService : IDisposable
    {
        private const LuceneVersion AppLuceneVersion = LuceneVersion.LUCENE_48;
        private readonly string _indexPath;
        private readonly FSDirectory _directory;
        private readonly IndexWriter _writer;

        public LuceneService(string indexName)
        {
            var basePath = Path.Combine(@"C:\NencerLLC", "Search");
            System.IO.Directory.CreateDirectory(basePath);

            // Đường dẫn lưu trữ chỉ mục
            _indexPath = Path.Combine(basePath, indexName);

            // Mở thư mục Lucene và cấu hình IndexWriter
            _directory = FSDirectory.Open(_indexPath);

            var analyzer = new StandardAnalyzer(AppLuceneVersion);
            var indexConfig = new IndexWriterConfig(AppLuceneVersion, analyzer);
            _writer = new IndexWriter(_directory, indexConfig);
        }

        public void Commit()
        {
            _writer.Flush(triggerMerge: false, applyAllDeletes: false);
        }


        public void Dispose()
        {
            _writer?.Dispose();
            _directory?.Dispose();
        }


        public static string RemoveDiacritics(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}
