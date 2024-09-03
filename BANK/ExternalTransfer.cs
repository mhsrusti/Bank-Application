using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using BankDatabase;

namespace BANK
{
    public class ExternalTransfer: Transaction
    {
        public ExternalAccountDB ToExternalAccount { get; set; }
        public int FromAccountPin { get; private set; }
        public ExternalTransfer(AccountDB fromAccount, ExternalAccountDB toExternalAccount, double amount, int fromAccountPin, int transactionId)
        : base(fromAccount, amount, transactionId, TransactionTypes.ExternalTransfer)
        {
            this.ToExternalAccount = toExternalAccount;
            this.FromAccountPin = fromAccountPin;      
            this.TransID = transactionId;
            this.Status = TransactionStatus.OPEN; // Initially set to OPEN

            try
            {
                // Perform the deposit operation
                var bankServiceFactory = ExternalBankServiceFactory.Instance;
                var externalBankService = bankServiceFactory.GetBankService(toExternalAccount.BankCode);
                externalBankService.Deposit(toExternalAccount.AccNo, amount);
            }
            catch (Exception ex)
            {
                
                throw new ExternalTransferException($"External transfer failed: {ex.Message}");
            }
        }
    }
    public class ExternalTransferException : ApplicationException
    {
        public ExternalTransferException (string message) : base(message) { }
    }
}
