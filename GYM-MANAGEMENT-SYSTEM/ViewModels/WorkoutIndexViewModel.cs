namespace GYM_MANAGEMENT_SYSTEM.ViewModels
{
    public class WorkoutIndexViewModel
    {
        public int Id { get; set; }
        public DateTime RecordedAt { get; set; }
        public double Weight { get; set; }
        public double Height { get; set; }
        public double BodyFatPercentage { get; set; }
        public double MuscleMass { get; set; }
        public double WaistCircumference { get; set; }
        public string Notes { get; set; } = string.Empty;

        public double BMI => Height > 0 ? Math.Round(Weight / ((Height / 100) * (Height / 100)), 1) : 0;
        public string BMICategory => BMI switch
        {
            < 18.5 => "Thiếu cân",
            < 25 => "Bình thường",
            < 30 => "Thừa cân",
            _ => "Béo phì"
        };
        public string BMIStatus => BMI switch
        {
            < 18.5 => "warning",
            < 25 => "success",
            < 30 => "warning",
            _ => "danger"
        };
        public string DateDisplay => RecordedAt.ToString("dd/MM/yyyy HH:mm");
        public string WeightDisplay => $"{Weight:F1} kg";
        public string BodyFatDisplay => $"{BodyFatPercentage:F1}%";
    }
}