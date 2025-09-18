using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.Text;

namespace SimpleDB
{

    public class TestDatabase<T> : IDatabaseRepository<T>
    {
        private readonly string csvDirectory;
        private readonly string csvFile;
        private readonly CsvConfiguration csvConfig;

        public TestDatabase()
        {
            csvDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CHIRP", "resources");
            csvFile = Path.Combine(csvDirectory, "test_db.csv");

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
                    if (limit == null)
                    {
                        return csv.GetRecords<T>().ToList();
                    }
                    else
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

        public void Purge(bool sure)
        {
            if (sure == false) { return; }

            string header;
            using (var reader = new StreamReader(csvFile, Encoding.UTF8))
            {
                header = reader.ReadLine();
            }

            using (var writer = new StreamWriter(csvFile, false, Encoding.UTF8))
            {
                if (!string.IsNullOrEmpty(header))
                {
                    writer.WriteLine(header);
                }
            }
        }
    }
}
