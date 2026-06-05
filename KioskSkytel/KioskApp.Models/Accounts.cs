using System;

namespace KioskApp.Models
{
    public enum ServiceType
    {
        SKYTEL = 1,
        SKYMEDIA = 2,
        SKYNET = 3,
        GOPLUS = 4,
        CALLY = 5
    }

    public static class ServiceTypeExtensions
    {
        public static string ToDisplayString(this ServiceType s)
        {
            return s.ToString();
        }
    }

    public class Account
    {
        public int Id { get; set; }
        public int UserId { get; set; } = 0;
        public ServiceType ServiceType { get; set; } = ServiceType.SKYTEL;
        public string AccountNumber { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public string? BundleName { get; set; }
        //public string Status { get; set; } = string.Empty;
        public int IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}