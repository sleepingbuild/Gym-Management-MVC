namespace GYM_MANAGEMENT_SYSTEM.ViewModels
{
    public class WorkoutStatisticsViewModel
    {
        public int TotalRecords { get; set; }
        public double LatestWeight { get; set; }
        public double LatestBMI { get; set; }
        public double LatestBodyFat { get; set; }
        public double LatestMuscleMass { get; set; }
        public double LatestWaist { get; set; }
        public double StartWeight { get; set; }
        public double StartBMI { get; set; }
        public double StartBodyFat { get; set; }
        public double WeightChange { get; set; }
        public double BMIChange { get; set; }
        public double BodyFatChange { get; set; }
        public int RecentRecords { get; set; }
        public DateTime LastUpdated { get; set; }

        public string LatestWeightDisplay => $"{LatestWeight:F1} kg";
        public string LatestBMIDisplay => $"{LatestBMI:F1}";
        public string LatestBodyFatDisplay => $"{LatestBodyFat:F1}%";
        public string WeightChangeDisplay => WeightChange >= 0 ? $"+{WeightChange:F1} kg" : $"{WeightChange:F1} kg";
        public string BMIChangeDisplay => BMIChange >= 0 ? $"+{BMIChange:F1}" : $"{BMIChange:F1}";
        public string BodyFatChangeDisplay => BodyFatChange >= 0 ? $"+{BodyFatChange:F1}%" : $"{BodyFatChange:F1}%";
        public string WeightChangeColor => WeightChange >= 0 ? "text-danger" : "text-success";
        public string BMIChangeColor => BMIChange >= 0 ? "text-danger" : "text-success";
        public string BodyFatChangeColor => BodyFatChange >= 0 ? "text-danger" : "text-success";
        public string LastUpdatedDisplay => LastUpdated.ToString("dd/MM/yyyy HH:mm");
    }
}