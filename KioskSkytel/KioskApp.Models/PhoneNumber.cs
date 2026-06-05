namespace KioskApp.Models
{
    public class PhoneNumber
    {
        public enum PhoneCategory
        {
            GOLDEN = 1,
            SILVER = 2,
            PLATINUM = 3,
            REGULAR = 4
        }

        public required int Id { get; set; }
        public required string Number { get; set; }
        // store category as integer (DB uses integer codes)
        public required int Category { get; set; }
        public required int isActive { get; set; }
        public required DateTime CreatedAt { get; set; }
    }

    public static class PhoneCategoryExtensions
    {
        public static string ToDisplayString(this PhoneNumber.PhoneCategory c)
        {
            return c.ToString();
        }

        // overload to accept integer category codes
        public static string ToDisplayString(this int categoryCode)
        {
            if (Enum.IsDefined(typeof(PhoneNumber.PhoneCategory), categoryCode))
            {
                var c = (PhoneNumber.PhoneCategory)categoryCode;
                return c.ToString();
            }
            return "UNKNOWN";
        }
    }
}