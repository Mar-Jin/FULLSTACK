using RevaliNow.Models;

namespace RevaliNow.Dtos
{
    public class DashboardResponseDto
    {
        public Patient User { get; set; } = null!;
        public List<PijnIndicatie> RecentPains { get; set; } = [];
        public List<PijnIndicatie> PainData { get; set; } = [];
        public List<OefenplanDto> UpcomingExercises { get; set; } = [];
        public AfspraakDto? NextAppointment { get; set; }
    }
}
