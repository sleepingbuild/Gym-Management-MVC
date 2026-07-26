using System.ComponentModel.DataAnnotations;

namespace GYM_MANAGEMENT_SYSTEM.ViewModels
{
    public class BookingCreateViewModel : IValidatableObject
    {
        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn huấn luyện viên")]
        public int TrainerId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày tập")]
        [DataType(DataType.Date)]
        public DateTime SessionDate { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn khung giờ")]
        public string TimeSlot { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập tuổi")]
        [Range(MinAge, MaxAge, ErrorMessage = "Tuổi phải từ " + MinAgeStr + " đến " + MaxAgeStr)]
        public int Age { get; set; }

        [StringLength(500, ErrorMessage = "Ghi chú không được quá 500 ký tự")]
        public string Notes { get; set; } = string.Empty;

        // Display properties
        public string? TrainerName { get; set; }
        public string DateDisplay => SessionDate.ToString("dd/MM/yyyy");
        public string TimeDisplay => SessionDate.ToString("HH:mm");

        // Số tháng xa nhất cho phép đặt lịch trước — dùng chung cho cả validate
        // và để View đọc lại khi set thuộc tính "max" cho input ngày.
        public const int MaxMonthsAhead = 4;

        // Giới hạn tuổi hợp lệ để đăng ký tập — dùng chung cho [Range] và View.
        // const int không cho phép nội suy trực tiếp vào ErrorMessage nên cần
        // thêm 2 hằng string tương ứng.
        public const int MinAge = 10;
        public const int MaxAge = 90;
        private const string MinAgeStr = "10";
        private const string MaxAgeStr = "90";

        // Server-side validation: chặn ngày trong quá khứ và ngày cách quá xa hiện tại.
        // Đây là lớp bảo vệ cuối cùng — validate HTML5 (min/max) + JS ở View chỉ là
        // hỗ trợ UX, không thể tin tưởng hoàn toàn vì người dùng có thể bypass.
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var today = DateTime.Today;
            var maxDate = today.AddMonths(MaxMonthsAhead);

            if (SessionDate.Date < today)
            {
                yield return new ValidationResult(
                    "Ngày tập không được là ngày trong quá khứ. Vui lòng chọn từ hôm nay trở đi.",
                    new[] { nameof(SessionDate) });
            }
            else if (SessionDate.Date > maxDate)
            {
                yield return new ValidationResult(
                    $"Ngày tập không được cách quá {MaxMonthsAhead} tháng so với hiện tại (tối đa đến {maxDate:dd/MM/yyyy}).",
                    new[] { nameof(SessionDate) });
            }

           
            var impliedBirthYear = DateTime.UtcNow.Year - Age;
            if (impliedBirthYear > DateTime.UtcNow.Year)
            {
                yield return new ValidationResult(
                    "Tuổi không hợp lệ: năm sinh suy ra từ tuổi này chưa đến (là năm tương lai).",
                    new[] { nameof(Age) });
            }
        }
    }
}