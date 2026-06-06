namespace KioskApp.Models
{
    public class Card
    {
        public enum CardCategory
        {
            SKYTEL = 1,
            GOPLUS = 2,
        }

        public required int Id { get; set; }
        public required string Title { get; set; }
        public required double Price { get; set; }
        public required string Duration { get; set; }
        public required int UnitAmount { get; set; }
        public required int DataGB { get; set; }
        public required int CardType { get; set; }
    }
    public static class CardCategoryExtensions
    {
        public static string ToDisplayString(this Card.CardCategory c)
        {
            return c switch
            {
                Card.CardCategory.SKYTEL => "Нэгж",
                Card.CardCategory.GOPLUS => "GOPlus Карт",
                _ => c.ToString(),
            };
        }

        // overload to accept integer category codes
        public static string ToDisplayString(this int categoryCode)
        {
            if (Enum.IsDefined(typeof(Card.CardCategory), categoryCode))
            {
                var c = (Card.CardCategory)categoryCode;
                return c.ToDisplayString();
            }
            return "UNKNOWN";
        }
    }
}
