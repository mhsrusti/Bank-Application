using System;
using System.Collections.Generic;
using System.Linq;

namespace BANK
{
	public static class TransactionLog
	{
		// Dictionary to store transactions categorized by account number and transaction type
		private static Dictionary<string, Dictionary<TransactionTypes, List<Transaction>>> transactions = new();

		/// <summary>
		/// Logs a transaction for a specific account and type.
		/// </summary>
		/// <param name="accNo">The account number associated with the transaction.</param>
		/// <param name="type">The type of transaction (e.g., Deposit, Withdraw, Transfer).</param>
		/// <param name="transaction">The transaction object to be logged.</param>
		public static void LogTransaction(string accNo, TransactionTypes type, Transaction transaction)
		{
			if (!transactions.ContainsKey(accNo))
			{
				transactions[accNo] = new Dictionary<TransactionTypes, List<Transaction>>();
			}

			if (!transactions[accNo].ContainsKey(type))
			{
				transactions[accNo][type] = new List<Transaction>();
			}

			transactions[accNo][type].Add(transaction);
		}

		/// <summary>
		/// Retrieves all transactions for all accounts.
		/// </summary>
		/// <returns>A dictionary containing all transactions categorized by account number and transaction type.</returns>
		public static Dictionary<string, Dictionary<TransactionTypes, List<Transaction>>> GetTransactions()
		{
			return transactions;
		}

		/// <summary>
		/// Retrieves all transactions for a specific account.
		/// </summary>
		/// <param name="accNo">The account number for which transactions are retrieved.</param>
		/// <returns>A dictionary containing all transactions for the specified account, categorized by transaction type.</returns>
		public static Dictionary<TransactionTypes, List<Transaction>> GetTransactions(string accNo)
		{
			// Return the transactions for the specified account number, if it exists
			if (transactions.ContainsKey(accNo))
			{
				return transactions[accNo];
			}
			// Return an empty dictionary if the account number does not exist
			return new Dictionary<TransactionTypes, List<Transaction>>();
		}

		/// <summary>
		/// Retrieves all transactions of a specific type for a particular account.
		/// </summary>
		/// <param name="accNo">The account number for which transactions are retrieved.</param>
		/// <param name="type">The type of transactions to retrieve (e.g., Deposit, Withdraw, Transfer).</param>
		/// <returns>A list of transactions of the specified type for the given account number. Returns an empty list if no transactions are found.</returns>
		public static List<Transaction> GetTransactions(string accNo, TransactionTypes type)
		{
			// Return the transactions of the specified type for the specified account number, if they exist
			if (transactions.ContainsKey(accNo) && transactions[accNo].ContainsKey(type))
			{
				return transactions[accNo][type];
			}
			// Return an empty list if the account number or transaction type does not exist
			return new List<Transaction>();
		}

		/// <summary>
		/// Updates the status of a specific transaction.
		/// </summary>
		/// <param name="accNo">The account number associated with the transaction.</param>
		/// <param name="transactionId">The ID of the transaction to be updated.</param>
		/// <param name="newStatus">The new status to set for the transaction.</param>
		/// <returns>Returns true if the transaction status was updated successfully, false otherwise.</returns>
		public static bool UpdateTransactionStatus(string accNo, int transactionId, TransactionStatus newStatus)
		{
			if (transactions.ContainsKey(accNo))
			{
				foreach (var transType in transactions[accNo])
				{
					var transaction = transType.Value.FirstOrDefault(t => t.TransID == transactionId);
					if (transaction != null)
					{
						transaction.Status = newStatus;
						return true;
					}
				}
			}
			return false;
		}

		/// <summary>
		/// Retrieves all transactions with a specific status.
		/// </summary>
		/// <param name="status">The transaction status to filter by (e.g., OPEN, CLOSE).</param>
		/// <returns>A list of transactions with the specified status.</returns>
		public static List<Transaction> GetTransactionsByStatus(TransactionStatus status)
		{
			var result = new List<Transaction>();

			foreach (var account in transactions.Values)
			{
				foreach (var transType in account.Values)
				{
					result.AddRange(transType.Where(t => t.Status == status));
				}
			}

			return result;
		}
	}
}
