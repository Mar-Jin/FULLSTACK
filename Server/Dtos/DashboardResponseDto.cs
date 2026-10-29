using RevaliNow.Models;

namespace RevaliNow.Dtos
{
    public class DashboardResponseDto
    {
        public Patient User { get; set; } = null!;
        public List<PijnIndicatie> RecentPains { get; set; } = [];
        public List<PijnIndicatie> PainData { get; set; } = [];
        public DashboardSamenvattingDto Samenvatting { get; set; } = new();
    }

    // Resultaat van sp_DashboardSamenvatting
    public class DashboardSamenvattingDto
    {
        public int? LaatstePijnScore { get; set; }
        public DateTime? LaatstePijnDatum { get; set; }
        public decimal? GemiddeldePijn7Dagen { get; set; }
        public int AantalMetingen7Dagen { get; set; }
        public DateTime? VolgendeAfspraakDatum { get; set; }
        public int? VolgendeAfspraakDuur { get; set; }
        public string? VolgendeAfspraakType { get; set; }
        public string? VolgendeAfspraakStatus { get; set; }
        public string? VolgendeAfspraakArts { get; set; }
    }

    // Resultaat van sp_PijnverloopPerWeek
    public class PijnWeekDto
    {
        public DateOnly WeekStart { get; set; }
        public decimal GemiddeldePijn { get; set; }
        public int LaagstePijn { get; set; }
        public int HoogstePijn { get; set; }
        public int AantalMetingen { get; set; }
    }
}
