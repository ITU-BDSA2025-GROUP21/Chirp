using System;
using Microsoft.EntityFrameworkCore;

public class CheepRepository : DbContext
{
	public DbSet<Cheep> cheeps { get; set; }
	public DbSet<Author> authors { get; set; }

	public string dbPath { get; }


	public CheepRepository(string dbPath = Environment.GetEnvironmentVariable("CHIRPDBPATH");)
    {
        string tempPath = Path.Combine(Path.GetTempPath(), "chirp.db");

        if (string.IsNullOrWhiteSpace(dbPath))
        {
	        dbPath = tempPath;
        }

        if (!dbPath.EndsWith(".db", StringComparison.OrdinalIgnoreCase))
        {
	        string directory = Path.GetDirectoryName(dbPath);
	        if (string.IsNullOrEmpty(directory))
	        {
		        dbPath = tempPath;
	        }
	        else
	        {
		        dbPath = Path.Combine(directory, 'chirp.db');
	        }
        }
    }
	
}

public Author()
{
	public string email { get; set; }
	public string name { get; set; }
    public List<Cheep> cheeps { get; set; }

}

public Cheep()
{
	public string text { get; set;}
	public DateTime timestamp { get; set; }
	public Author author { get; set; }
}
