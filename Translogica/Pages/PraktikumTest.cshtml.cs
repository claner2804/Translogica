using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class PraktikumTestModel : PageModel
{
    [BindProperty]
    public int StundenProWoche { get; set; }

    [BindProperty]
    public int AnzahlWochen { get; set; }

    [BindProperty]
    public int Jahresgehalt { get; set; }

    public int Gesamtstunden { get; set; }
    public string Ergebnis { get; set; } = string.Empty;
    public string AlertClass { get; set; } = "alert-info";

    private const int MindestPraktikumsstunden = 250;
    private const int Zuverdienstgrenze = 16455;

    public void OnPost()
    {
        Gesamtstunden = StundenProWoche * AnzahlWochen;

        bool genugErfahrung = Gesamtstunden >= MindestPraktikumsstunden;
        bool überGrenze = Jahresgehalt > Zuverdienstgrenze;

        if (genugErfahrung && überGrenze)
        {
            Ergebnis = $"👏 Du hast bereits {Gesamtstunden} Stunden gearbeitet – das reicht für eine Anrechnung! Außerdem würdest du mit einem weiteren Praktikum die Zuverdienstgrenze überschreiten. Die Anrechnung ist in deinem Fall klar sinnvoll.";
            AlertClass = "alert-success";
        }
        else if (!genugErfahrung)
        {
            Ergebnis = $"Du hast bisher {Gesamtstunden} Stunden gesammelt. Für eine Anrechnung brauchst du mindestens {MindestPraktikumsstunden}. Vielleicht fehlen noch ein paar Wochen!";
            AlertClass = "alert-warning";
        }
        else
        {
            Ergebnis = "Die Entscheidung hängt von weiteren Faktoren ab – du könntest ein Praktikum machen, aber achte auf die Zuverdienstgrenze.";
            AlertClass = "alert-secondary";
        }
    }
}
