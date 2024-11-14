namespace INeedFunds.Mail
{
    public static class Ids
    {
        public const string SubscribeAgriculturalFundsId = "INeedFunds.SubscribeAgriculturalFundsId";
        
        public static string InsufficientFundsForParcelPaymentId(int week, int month, int year)
        {
            var _week = week;
            var _month = month;
            var _year = year;

            return $"INeedFunds.NoFundsForRentPayment_{_week}_{_month}_{_year}";
        }
    }
}