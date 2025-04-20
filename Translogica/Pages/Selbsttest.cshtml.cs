using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Npgsql;
using System;

namespace Translogica.Pages
{
    public class SelbsttestModel : PageModel
    {
        [BindProperty]
        public InputModel Input { get; set; }

        public string ErgebnisText { get; set; }

        public class InputModel
        {
            public string Name { get; set; }
            public int Years { get; set; }
            public int Months { get; set; }
            public int HoursPerWeek { get; set; }
            public bool UsesTools { get; set; }
            public bool HasStipendium { get; set; }
        }

        public void OnGet()
        {
            Input = new InputModel(); // für erste Initialisierung
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid) return Page();

            int totalWeeks = Input.Years * 52 + Input.Months * 4;
            int totalHours = totalWeeks * Input.HoursPerWeek;

            string result;
            if (totalHours >= 500 && Input.UsesTools && Input.HasStipendium)
            {
                result = $"{Input.Name}, du hast {totalHours} Stunden gesammelt (mehr als 500), verwendest relevante Tools und liegst an der Zuverdienstgrenze. ---> Eine Anrechnung des Pflichtpraktikums ist sinnvoll.";
            }
            else if (totalHours >= 500 && Input.UsesTools)
            {
                result = $"{Input.Name}, du hast {totalHours} Stunden gesammelt (mehr als 500) und verwendest bereits relevante Tools. ---> Eine Anrechnung des Pflichtpraktikums ist sinnvoll.";
            }
            else if (totalHours >= 500)
            {
                result = $"{Input.Name}, du hast {totalHours} Stunden gesammelt (mehr als 500), aber du verwendest keine relevanten Tools. ---> Eine Absolvierung des Pflichtpraktikums ist sinnvoll.";
            }
            else
            {
                result = $"{Input.Name}, du hast nur {totalHours} Stunden gesammelt – für eine Anrechnung brauchst du mindestens 500.";
            }

            ErgebnisText = result;

            // In PostgreSQL speichern
            SaveResultToDatabase(Input.Name, totalHours, Input.UsesTools, Input.HasStipendium, result);

            return Page();
        }

        private void SaveResultToDatabase(string name, int totalHours, bool usesTools, bool stipendLimit, string result)
        {
            string connectionString = "Host=gondola.proxy.rlwy.net;Port=57980;Username=postgres;Password=WhVtLzEAnkYeHErdZqigXyIGOxpHBSZg;Database=railway;SSL Mode=Require;Trust Server Certificate=true";

            string query = @"
                INSERT INTO Ergebnisse (Timestamp, Name, Gesamtstunden, VerwendetTools, AnZuverdienstgrenze, Ergebnistest)
                VALUES (@Timestamp, @Name, @Stunden, @Tools, @Stipendium, @Ergebnis);";

            try
            {
                using var conn = new NpgsqlConnection(connectionString);
                using var cmd = new NpgsqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@Timestamp", DateTime.Now);
                cmd.Parameters.AddWithValue("@Name", name);
                cmd.Parameters.AddWithValue("@Stunden", totalHours);
                cmd.Parameters.AddWithValue("@Tools", usesTools);
                cmd.Parameters.AddWithValue("@Stipendium", stipendLimit);
                cmd.Parameters.AddWithValue("@Ergebnis", result);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Speichern: {ex.Message}");
            }
        }
    }
}
