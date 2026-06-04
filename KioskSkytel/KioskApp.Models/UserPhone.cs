namespace KioskApp.Models
{
    public class UserPhone
    {
        public required string Id { get; set; }
        public required string UserId { get; set; }
        public required string PhoneId { get; set; }

        public required string IsUsed { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}

/*select u.*, pn.phone_number
from users u
left join user_phone up on up.user_id = u.id
left join phone_numbers pn on pn.id = up.phone_number_id
where pn.phone_number = '96950039';*/