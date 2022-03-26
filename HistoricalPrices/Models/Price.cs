using System.Text.Json.Serialization;

namespace HistoricalPrices.Models {
    public class Price : IComparable<Price> {

        public DateTime Date { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Open { get; set; }
        public decimal Close { get; set; }
        public int CompareTo(Price? other) => Close.CompareTo(other?.Close);

        /* Comment out because I cannot ignore this field from csv import
        [JsonIgnore]
        public string? Commparing { get; set; }

        public int CompareTo(Price? other) {
            if (other == null) {
                return 0;
            } else if (string.IsNullOrEmpty(Commparing)) {
                return Date.CompareTo(other.Date);
            } else if (Commparing.ToLower() == nameof(High).ToLower()) { 
                return High.CompareTo(other.High);
            } else if (Commparing.ToLower() == nameof(Low).ToLower()) {
                return Low.CompareTo(other.Low);
            } else if (Commparing.ToLower() == nameof(Open).ToLower()) {
                return Open.CompareTo(other.Open);
            } else {
                return Close.CompareTo(other.Close);
            }
        }
        */
    }
}
