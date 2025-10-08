using System;
using Microsoft.EntityFrameworkCore;

public class CheepRepository : DbContext
{
	public DbSet<Cheep> cheeps { get; set; }
	public DbSet<Author> authors { get; set; }

	public string dbPath { get; }


	public CheepRepository()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        dbPath = System.IO.Path.Join(path, "chirp.db");
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
