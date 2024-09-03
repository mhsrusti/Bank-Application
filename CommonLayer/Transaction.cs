using System;
using BankDatabase;


namespace BANK
{
    public class Transaction
    {
        public int TransID { get; set; }
        public TransactionTypes TransactionType { get; private set; }
        public AccountDB FromAccount { get; private set; }
        public DateTime TranDate { get; private set; }
        public double Amount { get; private set; }
        public TransactionStatus Status { get; set; }

        // Constructor to initialize the transaction
        public Transaction(AccountDB fromAccountDB, double amount, int transID, TransactionTypes transactionType)
        {
            TransID = transID;
            TransactionType = transactionType;
            FromAccount = fromAccountDB;
            TranDate = DateTime.Now;
            Amount = amount;
            //this.Status = TransactionStatus.CLOSE;
        }
    }
}
