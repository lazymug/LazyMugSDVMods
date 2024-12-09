namespace INeedFunds.Model
{
    public class BankAccount
    {
        public int BankBalance { get; set; } = 0; // Gold in the bank account
        public int LoanBalance { get; set; } = 0; // Outstanding loan
        public double DepositInterestRate { get; set; } = 0.01; // % interest
        public double LoanInterestRate { get; set; } = 0.02; // % interest
        public int DailyPayment { get; set; } = 100; // Minimum daily loan payment
        public List<string> TransactionHistory { get; set; } = new(); // Log of transactions
    }
}