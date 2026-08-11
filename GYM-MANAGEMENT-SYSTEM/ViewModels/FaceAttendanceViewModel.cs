namespace GYM_MANAGEMENT_SYSTEM.ViewModels
{
    public class FaceProfileSaveViewModel
    {
        public float[] Descriptor { get; set; } = Array.Empty<float>();
    }

    public class AdminFaceProfileSaveViewModel
    {
        public string UserId { get; set; } = string.Empty;
        public float[] Descriptor { get; set; } = Array.Empty<float>();
    }

    public class FaceEnrollableUserViewModel
    {
        public string UserId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty; // "Admin" | "Trainer" | "Member" | "Member, Trainer"...
        public bool HasFaceProfile { get; set; }
    }

    public class KioskFaceProfileViewModel
    {
        public string UserId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public float[] Descriptor { get; set; } = Array.Empty<float>();
    }
    public class FaceScanRequestViewModel
    {
        public float[] Descriptor { get; set; } = Array.Empty<float>();
    }

    public class FaceCheckInRequestViewModel
    {
        public string UserId { get; set; } = string.Empty;
    }

    public class FaceCheckInResultViewModel
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? FullName { get; set; }

        public string? Role { get; set; }

        public string? Action { get; set; }
        public DateTime? Time { get; set; }
    }
}