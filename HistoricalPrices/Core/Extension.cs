namespace HistoricalPrices.Core {
    public static class Extension {
        public static DateTime AddBusinessDays(this DateTime startDate, int businessDays) {
            int direction = Math.Sign(businessDays);
            if (direction == 1) {
                if (startDate.DayOfWeek == DayOfWeek.Saturday) {
                    startDate = startDate.AddDays(2);
                    businessDays = businessDays - 1;
                } else if (startDate.DayOfWeek == DayOfWeek.Sunday) {
                    startDate = startDate.AddDays(1);
                    businessDays = businessDays - 1;
                }
            } else {
                if (startDate.DayOfWeek == DayOfWeek.Saturday) {
                    startDate = startDate.AddDays(-1);
                    businessDays = businessDays + 1;
                } else if (startDate.DayOfWeek == DayOfWeek.Sunday) {
                    startDate = startDate.AddDays(-2);
                    businessDays = businessDays + 1;
                }
            }

            int initialDayOfWeek = (int)startDate.DayOfWeek;

            int weeksBase = Math.Abs(businessDays / 5);
            int addDays = Math.Abs(businessDays % 5);

            if ((direction == 1 && addDays + initialDayOfWeek > 5) ||
                 (direction == -1 && addDays >= initialDayOfWeek)) {
                addDays += 2;
            }

            int totalDays = (weeksBase * 7) + addDays;
            return startDate.AddDays(totalDays * direction);
        }
    }
}
