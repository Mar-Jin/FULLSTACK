namespace RevaliNow.Dtos
{
    // Een toegewezen oefening zoals de cliënt die ziet, inclusief de voortgang van vandaag
    public class OefenplanDto
    {
        public int Id { get; set; }
        public int OefeningId { get; set; }
        public string Naam { get; set; } = string.Empty;
        public string Beschrijving { get; set; } = string.Empty;
        public string? InstructieVideoUrl { get; set; }
        public int Herhalingen { get; set; }
        public int Sets { get; set; }
        public int FrequentiePerDag { get; set; }
        public DateTime StartDatum { get; set; }
        public DateTime EindDatum { get; set; }
        public string? Opmerkingen { get; set; }

        // Loopt het oefenplan vandaag?
        public bool IsActief { get; set; }

        // Aantal keer uitgevoerd vandaag (afgevinkt min ongedaan gemaakt)
        public int KeerGedaanVandaag { get; set; }

        public bool IsAfgerondVandaag => IsActief && KeerGedaanVandaag >= FrequentiePerDag;
    }

    public class AfvinkenDto
    {
        // true = uitgevoerd, false = ongedaan maken
        public bool IsAfgevinkt { get; set; }
    }
}
