using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.Text;

namespace SimpleDB
{
    public interface IDatabaseRepository<T>
    {
        public IEnumerable<T> Read(int? limit = null);
        public void Store(T record);
    }

    public sealed class CSVDatabase<T> : IDatabaseRepository<T>
    {
        private readonly string csvDirectory;
        private readonly string csvFile;
        private readonly string csvHeader = "Author,Message,Timestamp";
        private readonly CsvConfiguration csvConfig;

        public CSVDatabase() {
            csvDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CHIRP", "resources");
            csvFile = Path.Combine(csvDirectory, "csv_db.csv");

            csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ",",
                TrimOptions = TrimOptions.Trim,
                IgnoreBlankLines = true,
                Quote = '"',
                Mode = CsvMode.RFC4180,
                HasHeaderRecord = true
            };

            Initialize();
        }

        public void Initialize()
        {
            if (!File.Exists(csvFile))
            {
                Directory.CreateDirectory(csvDirectory);
                AppendHeader(csvHeader);
            }
        }

        public void AppendHeader(string header)
        {
            using (StreamWriter sw = new StreamWriter(csvFile, append: true, Encoding.UTF8))
            {
                using (var csv = new CsvWriter(sw, csvConfig))
                {
                    csv.WriteHeader<T>();
                    csv.NextRecord();
                }
            }
        }

        public IEnumerable<T> Read(int? limit = null)
        {
            using (var reader = new StreamReader(csvFile, Encoding.UTF8))
            {
                using (var csv = new CsvReader(reader, csvConfig))
                {
                    return csv.GetRecords<T>().ToList();
                }
            }
        }

        public void Store(T record)
        {
            using (var writer = new StreamWriter(csvFile, append: true, Encoding.UTF8))
            {
                using (var csv = new CsvWriter(writer, csvConfig))
                {
                    csv.WriteRecord(record);
                }
            }
        }
    }
}
