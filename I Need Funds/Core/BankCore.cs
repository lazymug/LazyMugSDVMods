using INeedFunds.Model;
using StardewValley;

namespace INeedFunds.Core
{
    public static class BankCore
    {
        public static string DepositFunds(BankAccount bankAccount, int amount, Farmer player)
        {
            if (amount > player.Money)
            {
                return ModEntry.Translation("inf.feedback.noFundsInWallet");
            }

            if (amount < 0)
            {
                return ModEntry.Translation("inf.feedback.invalidAmount");
            }
            player.Money -= amount;
            bankAccount.BankBalance += amount;
            bankAccount.TransactionHistory.Add(string.Format(ModEntry.Translation("inf.history.transactionDeposit"), amount, bankAccount.BankBalance));
            return string.Format(ModEntry.Translation("inf.feedback.successToDeposit"), amount);
        }
        
        public static string WithdrawFunds(BankAccount bankAccount, int amount, Farmer player)
        {
            if (amount > bankAccount.BankBalance)
            {
                return ModEntry.Translation("inf.feedback.noFundsInBankAccount");
            }
            if (amount < 0)
            {
                return ModEntry.Translation("inf.feedback.invalidAmount");
            }
            player.Money += amount;
            bankAccount.BankBalance -= amount;
            bankAccount.TransactionHistory.Add(string.Format(ModEntry.Translation("inf.history.transactionWithdraw"), amount, bankAccount.BankBalance));
            return string.Format(ModEntry.Translation("inf.feedback.successToWithdraw"), amount);
        }
        
        public static string TakeLoan(BankAccount bankAccount, int amount, Farmer player)
        {
            if (amount < 0)
            {
                return ModEntry.Translation("inf.feedback.invalidAmount");
            }
            bankAccount.LoanBalance += amount;
            player.Money += amount;
            bankAccount.TransactionHistory.Add(string.Format(ModEntry.Translation("inf.history.transactionTakeLoan"), amount, bankAccount.LoanBalance));
            return string.Format(ModEntry.Translation("inf.feedback.successToTakeLoan"), amount);
        }
        
        public static string PayLoan(BankAccount bankAccount, int amount, Farmer player)
        {
            if (amount > player.Money)
            {
                return ModEntry.Translation("inf.feedback.noFundsToRepay");
            }
            if (amount < 0)
            {
                return ModEntry.Translation("inf.feedback.invalidAmount");
            }

            if (amount > bankAccount.LoanBalance)
            {
                amount = bankAccount.LoanBalance;
            }
            player.Money -= amount;
            bankAccount.LoanBalance -= amount;
            bankAccount.TransactionHistory.Add(string.Format(ModEntry.Translation("inf.history.transactionPayLoan"), amount, bankAccount.LoanBalance));
            return string.Format(ModEntry.Translation("inf.feedback.successToPayLoan"), amount);
        }
        
        /**
         * 
         */
        public static void ApplyDailyInterest(BankAccount account)
        {
            // Calculate interest for bank balance
            double depositInterest = account.BankBalance * account.DepositInterestRate;
            account.BankBalance += (int)depositInterest;

            // Calculate interest for loan balance
            double loanInterest = account.LoanBalance * account.LoanInterestRate;
            account.LoanBalance += (int)loanInterest;
        }
        
        public static string GetTransactionHistory(BankAccount account, int limit)
        {
            var history = string.Join("\n", account.TransactionHistory.TakeLast(limit));
            return history;
        }


    }
}