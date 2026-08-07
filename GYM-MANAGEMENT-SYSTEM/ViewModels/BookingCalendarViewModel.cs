namespace GYM_MANAGEMENT_SYSTEM.ViewModels
{
    public class BookingCalendarViewModel
    {
        public DateOnly WeekStart { get; set; }
        public DateOnly WeekEnd { get; set; }
        public List<BookingCalendarSlotViewModel> Slots { get; set; } = new();
    }

    public class BookingCalendarSlotViewModel
    {
        public DateOnly WorkDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public bool IsBooked { get; set; }

        public string TimeDisplay => $"{StartTime:HH:mm}–{EndTime:HH:mm}";
        public string TimeSlotValue => $"{StartTime:HH:mm}-{EndTime:HH:mm}";
    }
}