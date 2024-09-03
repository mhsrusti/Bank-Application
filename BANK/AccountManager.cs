using System.Net.NetworkInformation;
using System.Xml.Linq;
using System;
using BankDatabase;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Data.SqlClient;
using System.Transactions;

namespace BANK
{
    public class AccountManager
    {
        private static Dictionary<string, IAccount> accounts = new Dictionary<string, IAccount>();
        private static Dictionary<string, AccountDB> accountDB = new Dictionary<string, AccountDB>();


        public IAccount CreateAccount(string name, string pin, double balance, PrivilegeType privilegeType, AccountType accountType)
        {
           
                // Fetch the policy based on account type and privilege type
                IPolicy policy = PolicyFactory.Instance.CreatePolicy(accountType.ToString().ToUpper(), privilegeType.ToString().ToUpper());

                // Check if the initial balance meets the minimum balance criteria
                if (balance < policy.GetMinBalance())
                {
                    throw new MinBalanceNeedsToBeMaintainedException("Initial balance is less than the minimum balance required.");
                }

                // Create the account based on the account type
                IAccount account;
                if (accountType == AccountType.Savings)
                {
                    account = new SavingsAccount(name, pin, balance, privilegeType, policy);
                }
                else if (accountType == AccountType.Current)
                {
                    account = new CurrentAccount(name, pin, balance, privilegeType, policy);
                }
                else
                {
                    throw new ArgumentException("Invalid account type.");
                }

                // Check if the account was successfully created
                if (account == null)
                {
                    throw new UnableToOpenAccountException("Unable to create account.");
                }

                // Add the account to the in-memory collection
                accounts[account.AccNo] = account;

            // Create and save the account data to the database
            AccountDB accountDB = new AccountDB
            {
                AccNo = IDGenerator.GenerateAccNo(accountType.ToString()),
                Name = account.Name,
                Pin = account.Pin,
                Balance = account.Balance,
                DateOfOpening=DateTime.Now,
                PrivilegeType = account.PrivilegeType.ToString(),
                AccountType = account.AccountType.ToString(),
                Active = 1
            };

            //Save to the database
            IAccountRepository repo = new BankDBRepository();
            repo.Create(accountDB);
           
            return account; // Return the successfully created account

        }

        public void Deposit(AccountDB accountDB, double amount ,int pin)
        {
            if (accountDB == null)
            {
                throw new ArgumentNullException(nameof(accountDB), "Account cannot be null.");
            }

            if (accountDB.Active != 1)
            {
                throw new InactiveAccountException("Account is inactive.");
            }
            if (accountDB.Pin != pin.ToString())
            {
                throw new InvalidPinException("Invalid PIN.");
            }

            CheckDailyLimit(accountDB, TransactionTypes.Deposit, amount);

            accountDB.Balance += amount;

            int transID = IDGenerator.GenerateTransID();

            IAccountRepository repo = new BankDBRepository();
            repo.DepositAmountUpdateBalance(accountDB, transID);
           
            LogTransaction(accountDB, TransactionTypes.Deposit, amount, transID);
        }

        public void WithDraw(AccountDB accountDB, double amount, int pin)
        {
            if (accountDB == null)
            {
                throw new ArgumentNullException(nameof(accountDB), "Account cannot be null.");
            }

            if (accountDB.Active!=1)
            {
                throw new InactiveAccountException("Account is inactive.");
            }

            if (accountDB.Pin != pin.ToString())
            {
                throw new InvalidPinException("Invalid PIN.");
            }
                      
            IPolicy policy = PolicyFactory.Instance.CreatePolicy(accountDB.AccountType, accountDB.PrivilegeType);
           
            if (accountDB.Balance < amount || accountDB.Balance - amount < policy.GetMinBalance())
            {
                throw new MinBalanceNeedsToBeMaintainedException("Insufficient balance or below minimum balance.");
            }

            CheckDailyLimit(accountDB, TransactionTypes.Withdraw, amount);

            int transID = IDGenerator.GenerateTransID();

            accountDB.Balance -= amount;

            IAccountRepository repo = new BankDBRepository();
            repo.WithdrawAmountUpdateBalance(accountDB, transID);
           
            LogTransaction(accountDB, TransactionTypes.Withdraw, amount,transID);
        }

        public void TransferFunds(AccountDB fromAccountDB, AccountDB toAccountDB, double amount, int pin, PrivilegeType privilegeType)
        {
            if (fromAccountDB == null)
            {
                throw new ArgumentNullException(nameof(fromAccountDB), "Source account cannot be null.");
            }

            if (toAccountDB == null)
            {
                throw new ArgumentNullException(nameof(toAccountDB), "Destination account cannot be null.");
            }

            if (fromAccountDB.Active != 1 || toAccountDB.Active != 1)
            {
                throw new InactiveAccountException("One or both accounts are inactive.");
            }

            if (fromAccountDB.Pin != pin.ToString())
            {
                throw new InvalidPinException("Invalid PIN.");
            }

            // Fetch policies using PolicyFactory
            IPolicy fromPolicy = PolicyFactory.Instance.CreatePolicy(fromAccountDB.AccountType, fromAccountDB.PrivilegeType);

            if (fromAccountDB.Balance < amount || fromAccountDB.Balance - amount < fromPolicy.GetMinBalance())
            {
                throw new MinBalanceNeedsToBeMaintainedException("Insufficient balance or below minimum balance.");
            }

            CheckDailyLimit(fromAccountDB, TransactionTypes.Transfer, amount);

            int transID = IDGenerator.GenerateTransID();

            // Update balances
            fromAccountDB.Balance -= amount;
            toAccountDB.Balance += amount;

            // Use repository to update the balance
            IAccountRepository repo = new BankDBRepository();
            repo.TransferAmountUpdateBalance(fromAccountDB, toAccountDB, transID, amount);

            // Create and log transactions
            var transferTransaction = new Transaction(fromAccountDB, amount, transID, TransactionTypes.Transfer);
            var depositTransaction = new Transaction(toAccountDB, amount, transID, TransactionTypes.Deposit);

            TransactionLog.LogTransaction(fromAccountDB.AccNo, TransactionTypes.Transfer, transferTransaction);
            TransactionLog.LogTransaction(toAccountDB.AccNo, TransactionTypes.Deposit, depositTransaction);
        }

        public void CloseAccount(AccountDB accountDB, int pin)
        {
            if (accountDB.Pin != pin.ToString())
            {
                throw new InvalidPinException("Invalid PIN.");
            }

            if (accountDB.Active != 1)
            {
                throw new InactiveAccountException("Account is already inactive.");
            }
            accountDB.Active = 0;
            IAccountRepository repo = new BankDBRepository();
            repo.Close(accountDB, pin);
            
        }
        public void ExternalTransfer(AccountDB fromAccountDB, ExternalAccountDB toExternalAccount, double amount, int pin)
        {
            if (fromAccountDB == null)
            {
                throw new ArgumentNullException(nameof(fromAccountDB), "Source account cannot be null.");
            }

            if (toExternalAccount == null)
            {
                throw new ArgumentNullException(nameof(toExternalAccount), "Destination external account cannot be null.");
            }

            if (fromAccountDB.Active != 1)
            {
                throw new InactiveAccountException("Source account is inactive.");
            }

            if (fromAccountDB.Pin != pin.ToString())
            {
                throw new InvalidPinException("Invalid PIN.");
            }

            IPolicy fromPolicy = PolicyFactory.Instance.CreatePolicy(fromAccountDB.AccountType, fromAccountDB.PrivilegeType);

            if (fromAccountDB.Balance < amount || fromAccountDB.Balance - amount < fromPolicy.GetMinBalance())
            {
                throw new MinBalanceNeedsToBeMaintainedException("Insufficient balance or below minimum balance.");
            }

            CheckDailyLimit(fromAccountDB, TransactionTypes.ExternalTransfer, amount);

            int transID = IDGenerator.GenerateTransID();

            fromAccountDB.Balance -= amount;

            IAccountRepository repo = new BankDBRepository();

            try
            {
                // Update balance in database
                repo.ExternalTransferUpdate(fromAccountDB, toExternalAccount, amount, pin, transID);

                // Create and log the ExternalTransfer
                var externalTransfer = new ExternalTransfer(fromAccountDB, toExternalAccount, amount, pin, transID);

                // Ensure the external transfer operation is completed successfully
                externalTransfer.Status = TransactionStatus.CLOSE; // Set status to CLOSE after success

                // Log transactions
                TransactionLog.LogTransaction(fromAccountDB.AccNo, TransactionTypes.ExternalTransfer, externalTransfer);
            }
            catch (ExternalTransferException ex)
            {
                Console.WriteLine($"Error in external transfer: {ex.Message}");
            }
        }


        private void CheckDailyLimit(AccountDB accountDB, TransactionTypes type, double amount)
        {
            if (!Enum.TryParse(accountDB.PrivilegeType, out PrivilegeType privilegeType))
            {
                throw new InvalidPrivilageTypeException("Invalid privilege type.");
            }
            double dailyLimit = AccountPrivilegeManager.GetDailyLimit(privilegeType);

            var transactionsToday = TransactionLog.GetTransactions(accountDB.AccNo, type);
            double totalAmountToday = 0;
            foreach (var transaction in transactionsToday)
            {
                if (transaction.TranDate.Date == DateTime.Now.Date)
                {
                    totalAmountToday += transaction.Amount;
                }
            }

            if (totalAmountToday + amount > dailyLimit)
            {
                throw new DailyLimitExceededException("Daily limit exceeded.");
            }
        }

        private void LogTransaction(AccountDB accountDB, TransactionTypes transactionType, double amount, int transID)
        {
            var transaction = new Transaction(accountDB, amount, transID, transactionType);

            IAccountRepository transRepo = new BankDBRepository();
            transRepo.CreateTransaction(transaction); 
        }



    }
}
