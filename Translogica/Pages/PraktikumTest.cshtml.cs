using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using Npgsql;

public class ErgebnisEintrag
{
    public DateTime Timestamp { get; set; }
    public int Gesamtstunden { get; set; }
    public string Ergebnistest { get; set; }

    public string Name { get; set; }
}

public class PraktikumTestModel : PageModel
{
    public List<ErgebnisEintrag> LetzteErgebnisse { get; set; } = new();

    public void OnGet()
    {
        string connectionString = "Host=gondola.proxy.rlwy.net;Port=57980;Username=postgres;Password=WhVtLzEAnkYeHErdZqigXyIGOxpHBSZg;Database=railway;SSL Mode=Require;Trust Server Certificate=true";

        string query = @"
    SELECT timestamp, gesamtstunden, ergebnistest,name
    FROM ergebnisse
    ORDER BY timestamp DESC
    LIMIT 5;";

        try
        {
            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();

            using var cmd = new NpgsqlCommand(query, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                LetzteErgebnisse.Add(new ErgebnisEintrag
                {
                    Timestamp = reader.GetDateTime(0),
                    Gesamtstunden = reader.GetInt32(1),
                    Ergebnistest = reader.GetString(2),
                    Name = reader.GetString(3),
                });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Fehler beim Abrufen: " + ex.Message);
        }
    }
}
