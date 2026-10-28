namespace RevaliNow.Dtos
{
    public class AfspraakDto
    {
        public int Id { get; set; }
        public DateTime DatumTijdAfspraak { get; set; }
        public int DuurMinuten { get; set; }
        public string TypeAfspraak { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Revalidatiearts { get; set; } = string.Empty;
        public bool IsToekomstig { get; set; }
    }
}
