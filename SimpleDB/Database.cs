using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.Text;

namespace SimpleDB
{

    public sealed class CSVDatabase<T> : IDatabaseRepository<T>
    {
        private readonly string csvDirectory;
        private readonly string csvFile;
        private readonly CsvConfiguration csvConfig;

        public CSVDatabase() {

            var home = Environment.GetEnvironmentVariable("HOME");
            var baseDir = !string.IsNullOrEmpty(home)
                ? Path.Combine(home, "site", "data")
                : Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            csvDirectory = Path.Combine(baseDir, "CHIRP", "resources");
            csvFile = Path.Combine(csvDirectory, "csv_db.csv");

            csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ";",
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
                AppendHeader();
            }
        }

        public void AppendHeader()
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
                    if(limit == null)
                    {
                        return csv.GetRecords<T>().ToList();
                    } else
                    {
                      return csv.GetRecords<T>().Take(limit.Value).ToList();
                    }
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
                    csv.NextRecord();
                }
            }
        }
    }
}
