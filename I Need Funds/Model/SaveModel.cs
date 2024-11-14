namespace INeedFunds.Model
{
    public class SaveModel
    {
        public bool IsSubscribed { get; set; } = false;
        public int LoanAmount { get; set; } = 25000;
        public double LoanRate { get; set; } = 1.005;
        
    }
}