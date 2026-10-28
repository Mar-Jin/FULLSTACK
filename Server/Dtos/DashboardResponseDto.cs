using RevaliNow.Models;

namespace RevaliNow.Dtos
{
    public class DashboardResponseDto
    {
        public Patient User { get; set; } = null!;
        public List<PijnIndicatie> RecentPains { get; set; } = [];
        public List<PijnIndicatie> PainData { get; set; } = [];
        public List<Oefenplan> UpcomingExercises { get; set; } = [];
        public Afspraak? NextAppointment { get; set; }
        public List<Oefenplan> Exercises { get; set; } = [];
    }
}
