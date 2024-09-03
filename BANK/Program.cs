using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using BankDatabase;

namespace BANK
{
    public class Program 
    {
        private static AccountManager accountManager = new AccountManager();
        private static Bank bank = new Bank { Name = "BankOfPratianAppln" };


        /// <summary>
        /// The main entry point of the application.
        /// </summary>
        /// <param name="args">Command line arguments</param>
        static void Main(string[] args)
        {
            // Menu to perform operations
            while (true)
            {
                Console.WriteLine("Welcome to Bank of Pratian");
                Console.WriteLine("1. Create Account");
                Console.WriteLine("2. Deposit");
                Console.WriteLine("3. Withdraw");
                Console.WriteLine("4. Transfer Funds");
                Console.WriteLine("5. Close Account");
                Console.WriteLine("6. Check All Transactions");
                Console.WriteLine("7. Check Transactions for an Account");
                Console.WriteLine("8. Check Transactions for an Account by Type");
                Console.WriteLine("9. Exit");
                Console.WriteLine("10. Check External Transfer Status");

                Console.Write("Select an option: ");

                if (!int.TryParse(Console.ReadLine(), out var option))
                {
                    Console.WriteLine("Invalid option. Please enter a numeric value.");
                    continue;
                }

                switch (option)
                {
                    case 1:
                        CreateAccount();
                        break;
                    case 2:
                        Deposit();
                        break;
                    case 3:
                        Withdraw();
                        break;
                    case 4:
                        TransferFunds();
                        break;
                    case 5:
                        CloseAccount();
                        break;
                    case 6:
                        CheckAllTransactions(); 
                        break;
                    case 7:
                        CheckTransactionsForAccount(); 
                        break;
                    case 8:
                        CheckTransactionsForAccountByType(); 
                        break;
                    case 9:
                        return;
                    case 10:
                        CheckExternalTransferStatus();
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please select a valid option.");
                        break;
                }
            }
        }

        /// <summary>
        /// Prompts the user to create a new account.
        /// </summary>
        private static void CreateAccount()
        {
            Console.Write("Enter account holder's name: ");
            var name = Console.ReadLine();

            Console.Write("Enter PIN: ");
            var pin = Console.ReadLine();

            Console.Write("Enter initial balance: ");
            if (!double.TryParse(Console.ReadLine(), out var balance))
            {
                Console.WriteLine("Invalid balance. Please enter a numeric value.");
                return;
            }

            Console.Write("Enter Privilege Type (Regular, Gold, Premium): ");
            if (!Enum.TryParse<PrivilegeType>(Console.ReadLine(), true, out var privilegeType))
            {
                Console.WriteLine("Invalid privilege type.");
                return;
            }

            Console.Write("Enter Account Type (Savings, Current): ");
            if (!Enum.TryParse<AccountType>(Console.ReadLine(), true, out var accountType))
            {
                Console.WriteLine("Invalid account type.");
                return;
            }

            //Console.Write("Enter PAN: ");
            //var pan = Console.ReadLine();

            //Console.Write("Enter Aadhar: ");
            //var aadhar = Console.ReadLine();

            try
            {
                var account = accountManager.CreateAccount(name, pin, balance, privilegeType, accountType);
                Console.WriteLine($"Account created successfully with account number: {account.AccNo}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Creating acc :{ex.Message}");
            }
        }

        /// <summary>
        /// Prompts the user to deposit an amount into an account.
        /// </summary>
        private static void Deposit()
        {
            Console.Write("Enter account number: ");
            var accNo = Console.ReadLine();

            Console.Write("Enter amount to deposit: ");
            if (!double.TryParse(Console.ReadLine(), out var amount))
            {
                Console.WriteLine("Invalid amount. Please enter a numeric value.");
                return;
            }

            Console.Write("Enter PIN: ");
            if (!int.TryParse(Console.ReadLine(), out var pin))
            {
                Console.WriteLine("Invalid PIN. Please enter a numeric value.");
                return;
            }

            try
            {
                IAccountRepository repo = new BankDBRepository();

                var account = repo.GetAccount(accNo);

                // Ensure account is not null
                if (account == null)
                {
                    Console.WriteLine("Account not found.");
                    return;
                }
                if (account.Active!=1)
                {
                    Console.WriteLine("Cannot deposit: Account is inactive.");
                    return;
                }

                accountManager.Deposit(account, amount,pin);
                Console.WriteLine($"Deposit successful to {account.AccNo}.");
                LogTransaction(account, TransactionTypes.Deposit, amount);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in depositing amount: {ex.Message}");
            }
        }


        /// <summary>
        /// Prompts the user to withdraw an amount from an account.
        /// </summary>
        private static void Withdraw()
        {
            Console.Write("Enter account number: ");
            var accNo = Console.ReadLine();

            Console.Write("Enter amount to withdraw: ");
            if (!double.TryParse(Console.ReadLine(), out var amount))
            {
                Console.WriteLine("Invalid amount. Please enter a numeric value.");
                return;
            }

            Console.Write("Enter PIN: ");
            if (!int.TryParse(Console.ReadLine(), out var pin))
            {
                Console.WriteLine("Invalid PIN. Please enter a numeric value.");
                return;
            }

            try
            {
                IAccountRepository repo = new BankDBRepository();

                var account = repo.GetAccount(accNo);

                // Ensure account is not null
                if (account == null)
                {
                    Console.WriteLine("Account not found.");
                    return;
                }
                if (account.Active != 1)
                {
                    Console.WriteLine("Cannot withdraw: Account is inactive.");
                    return;
                }

                accountManager.WithDraw(account, amount, pin);
                Console.WriteLine($"Withdraw successful from {account.AccNo}.");
                //LogTransaction(account, TransactionTypes.Deposit, amount);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in withdrawing amount: {ex.Message}");
            }
        }

        /// <summary>
        /// Prompts the user to transfer funds between two accounts.
        /// </summary>
        public static void TransferFunds()
        {
            
            Console.WriteLine("Select Transfer Type:");
            Console.WriteLine("1. Internal Transfer");
            Console.WriteLine("2. External Transfer");
            Console.Write("Enter your choice: ");
            var transferType = Console.ReadLine();

            if (transferType == "1")
            {
                InternalTransfer();
            }
            else if (transferType == "2")
            {
                ExternalTransfer();
            }
            else
            {
                Console.WriteLine("Invalid option selected.");
            }
            

            static void InternalTransfer()
            {
                Console.Write("Enter source account number: ");
                var fromAccNo = Console.ReadLine();

                Console.Write("Enter destination account number: ");
                var toAccNo = Console.ReadLine();

                Console.Write("Enter amount to transfer: ");
                if (!double.TryParse(Console.ReadLine(), out var amount))
                {
                    Console.WriteLine("Invalid amount. Please enter a numeric value.");
                    return;
                }

                Console.Write("Enter source account PIN: ");
                if (!int.TryParse(Console.ReadLine(), out var pin))
                {
                    Console.WriteLine("Invalid PIN. Please enter a numeric value.");
                    return;
                }

                Console.Write("Enter Privilege Type (Regular, Gold, Premium): ");
                if (!Enum.TryParse<PrivilegeType>(Console.ReadLine(), true, out var privilegeType))
                {
                    Console.WriteLine("Invalid privilege type.");
                    return;
                }

                try
                {
                    IAccountRepository repo = new BankDBRepository();
                    var fromAccountDB = repo.GetAccount(fromAccNo);
                    var toAccountDB = repo.GetAccount(toAccNo);

                    accountManager.TransferFunds(fromAccountDB, toAccountDB, amount, pin, privilegeType);
                    Console.WriteLine("Transfer successful.");
                    LogTransaction(fromAccountDB, TransactionTypes.Transfer, amount);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in transferring amount: {ex.Message}");
                }
            }

            static void ExternalTransfer()
            {
                Console.Write("Enter source account number: ");
                var fromAccNo = Console.ReadLine();

                //Console.Write("Enter external account number: ");
                //var externalAccNo = Console.ReadLine();

                //Console.Write("Enter bank name: ");
                //var bankName = Console.ReadLine();

                Console.Write("Enter amount to transfer: ");
                if (!double.TryParse(Console.ReadLine(), out var amount))
                {
                    Console.WriteLine("Invalid amount. Please enter a numeric value.");
                    return;
                }

                Console.Write("Enter your PIN: ");
                if (!int.TryParse(Console.ReadLine(), out var pin))
                {
                    Console.WriteLine("Invalid PIN. Please enter a numeric value.");
                    return;
                }
                try
                {
                    IAccountRepository repo = new BankDBRepository();
                    var fromAccountDB = repo.GetAccount(fromAccNo);
                   
                    ExternalAccountDB externalAccount = new ExternalAccountDB("111", "5", "HCL");
                    Console.WriteLine($"{externalAccount.BankName}BANK");
                    accountManager.ExternalTransfer(fromAccountDB, externalAccount, amount, pin);
                    Console.WriteLine("Transfer successful.");
                    LogTransaction(fromAccountDB, TransactionTypes.ExternalTransfer, amount);
                }
                catch (Exception ex) 
                {
                    Console.WriteLine("External transfer failed.");
                }
            }

            
        }

        /// <summary>
        /// Prompts the user to close an account.
        /// </summary>
        private static void CloseAccount()
        {
            Console.Write("Enter account number: ");
            string accNo = Console.ReadLine();
            Console.Write("Enter PIN: ");
            int pin = int.Parse(Console.ReadLine());

            try
            {
                IAccountRepository repo = new BankDBRepository();

                var account = repo.GetAccount(accNo);

                // Ensure account is not null
                if (account == null)
                {
                    Console.WriteLine("Account not found.");
                    return;
                }
                if (account.Active != 1)
                {
                    Console.WriteLine("Cannot deposit: Account is inactive.");
                    return;
                }
                accountManager.CloseAccount(account,pin);
                Console.WriteLine("Account closed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in closing account: {ex.Message}");
            }
        }

            
        private static void CheckAllTransactions()
        {
            var allTransactions = TransactionLog.GetTransactions();
            if (allTransactions.Count == 0)
            {
                Console.WriteLine($"No transactions.");
                return;

            }
            foreach (var accountTransactions in allTransactions)
            {
                Console.WriteLine($"Account: {accountTransactions.Key}");
                foreach (var transactionType in accountTransactions.Value)
                {
                    Console.WriteLine($"  Transaction Type: {transactionType.Key}");
                    foreach (var transaction in transactionType.Value)
                    {
                        Console.WriteLine($"    Transaction ID: {transaction.TransID}, Amount: {transaction.Amount}, Date: {transaction.TranDate}");
                    }
                }
            }
        }

        private static void CheckTransactionsForAccount()
        {
            Console.Write("Enter account number: ");
            var accNo = Console.ReadLine();

            var accountTransactions = TransactionLog.GetTransactions(accNo);
            if (accountTransactions.Count == 0)
            {
                Console.WriteLine($"No transactions found for account {accNo}.");
                return;
            }
            foreach (var transactionType in accountTransactions)
            {
                Console.WriteLine($"Transaction Type: {transactionType.Key}");
                foreach (var transaction in transactionType.Value)
                {
                    Console.WriteLine($"  Transaction ID: {transaction.TransID}, Amount: {transaction.Amount}, Date: {transaction.TranDate}");
                }
            }
        }

        private static void CheckTransactionsForAccountByType()
        {
            Console.Write("Enter account number: ");
            var accNo = Console.ReadLine();

            Console.Write("Enter transaction type (Deposit, Withdraw, Transfer): ");
            if (!Enum.TryParse<TransactionTypes>(Console.ReadLine(), true, out var type))
            {
                Console.WriteLine("Invalid transaction type.");
                return;
            }

            var transactions = TransactionLog.GetTransactions(accNo, type);
            if (transactions.Count == 0)
            {
                Console.WriteLine($"No transactions found for account {accNo} of type {type}.");
                return;
            }

            Console.WriteLine($"Transactions for account {accNo} of type {type}:");
            foreach (var transaction in transactions)
            {
                Console.WriteLine($"Transaction ID: {transaction.TransID}, Amount: {transaction.Amount}, Date: {transaction.TranDate}");
            }
        }
        private static void CheckExternalTransferStatus()
        {
            Console.Write("Enter account number: ");
            var accNo = Console.ReadLine();

            var transactions = TransactionLog.GetTransactions(accNo, TransactionTypes.ExternalTransfer);

            if (transactions.Count == 0)
            {
                Console.WriteLine($"No external transfer transactions found for account {accNo}.");
                return;
            }

            Console.WriteLine($"External Transfer Transactions for account {accNo}:");
            foreach (var transaction in transactions)
            {
                Console.WriteLine($"Transaction ID: {transaction.TransID}, Amount: {transaction.Amount}, Date: {transaction.TranDate}, Status: {transaction.Status}");
            }
        }


        /// <summary>
        /// Logs a transaction for a given account.
        /// </summary>
        /// <param name="account">The account associated with the transaction</param>
        /// <param name="type">The type of transaction</param>
        /// <param name="amount">The transaction amount</param>
        private static void LogTransaction(AccountDB accountDB, TransactionTypes transactionType, double amount)
        {
            var transaction = new Transaction(accountDB, amount, IDGenerator.GenerateTransID(), transactionType);
            TransactionLog.LogTransaction(accountDB.AccNo, transactionType, transaction);
        }
    }
}
