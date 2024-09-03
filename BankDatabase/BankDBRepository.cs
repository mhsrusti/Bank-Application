using System;
using System.Data;
using System.Data.Common;
using System.Configuration;
using System.Data.SqlClient;
using System.Security.Principal;
using BANK;


namespace BankDatabase
{
    public class BankDBRepository : IAccountRepository
    {
        private IDbConnection GetConnection()
        {
            string dbProvider = ConfigurationManager.ConnectionStrings["default"].ProviderName;
            if (dbProvider == null)
            {
                throw new InvalidOperationException("Failedll to create a database connection.");
            }
            DbProviderFactories.RegisterFactory(dbProvider, SqlClientFactory.Instance);
            DbProviderFactory factory = DbProviderFactories.GetFactory(dbProvider);
            IDbConnection conn = factory.CreateConnection();
            if (conn == null)
            {
                throw new InvalidOperationException("Failed to create a database connection.");
            }
            string connStr = ConfigurationManager.ConnectionStrings["default"].ConnectionString;
            conn.ConnectionString = connStr;
            return conn;
        }

        public void Create(AccountDB accountDB)
        {
            using (IDbConnection conn = GetConnection())
            {
                string sqlInsert = $"insert into accounts values (@accNo,@name, @pin,@active,@dtOfOpening, @balance, @privilegeType, @accType)"; //parsing

                IDbCommand cmd = conn.CreateCommand();
                IDbDataParameter p1 = cmd.CreateParameter();
                p1.ParameterName = "@accNo";
                p1.Value = accountDB.AccNo;
                cmd.Parameters.Add(p1);


                IDbDataParameter p2 = cmd.CreateParameter();
                p2.ParameterName = "@name";
                p2.Value = accountDB.Name;
                cmd.Parameters.Add(p2);


                IDbDataParameter p3 = cmd.CreateParameter();
                p3.ParameterName = "@pin";
                p3.Value = accountDB.Pin;
                cmd.Parameters.Add(p3);

                IDbDataParameter p4 = cmd.CreateParameter();
                p4.ParameterName = "@active";
                p4.Value = accountDB.Active;
                cmd.Parameters.Add(p4);

                IDbDataParameter p5 = cmd.CreateParameter();
                p5.ParameterName = "@dtOfOpening";
                p5.Value = accountDB.DateOfOpening;
                cmd.Parameters.Add(p5);

                IDbDataParameter p6 = cmd.CreateParameter();
                p6.ParameterName = "@balance";
                p6.Value = accountDB.Balance;
                cmd.Parameters.Add(p6);

                IDbDataParameter p7 = cmd.CreateParameter();
                p7.ParameterName = "@privilegeType";
                p7.Value = accountDB.PrivilegeType;
                cmd.Parameters.Add(p7);

                IDbDataParameter p8 = cmd.CreateParameter();
                p8.ParameterName = "@accType";
                p8.Value = accountDB.AccountType;
                cmd.Parameters.Add(p8);


                cmd.CommandText = sqlInsert;
                cmd.Connection = conn;
                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in DBRepository: {ex.Message}");
                }
                finally
                {
                    conn.Close();
                }

            }
        }
        public AccountDB GetAccount(string accNo)
        {
            using (IDbConnection conn = GetConnection())
            {
                string sqlSelect = "SELECT * FROM Accounts WHERE accNo = @accNo";

                IDbCommand cmd = conn.CreateCommand();
                cmd.CommandText = sqlSelect;

                IDbDataParameter p1 = cmd.CreateParameter();
                p1.ParameterName = "@accNo";
                p1.Value = accNo;
                cmd.Parameters.Add(p1);

                try
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var account = new AccountDB
                            {
                                AccNo = reader["accNo"].ToString(),
                                Name = reader["name"].ToString(),
                                Pin = reader["pin"].ToString(),
                                Active = Convert.ToInt32(reader["active"]),
                                DateOfOpening = Convert.ToDateTime(reader["dtOfOpening"]),
                                Balance = Convert.ToDouble(reader["balance"]),
                                PrivilegeType = reader["privilegeType"].ToString(),
                                AccountType = reader["accType"].ToString()
                            };

                            return account;
                        }
                        else
                        {
                            throw new AccountDoesNotExistsException("Account not found.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in GetAccount: {ex.Message}");
                    return null; // or rethrow the exception 
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public void DepositAmountUpdateBalance(AccountDB accountDB,int transID)
        {
            using (IDbConnection conn = GetConnection())
            {
                string sqlUpdate = @"UPDATE Accounts SET balance = @balance WHERE accNo = @accNo";

                IDbCommand cmd = conn.CreateCommand();
                cmd.CommandText = sqlUpdate;

                IDbDataParameter p1 = cmd.CreateParameter();
                p1.ParameterName = "@accNo";
                p1.Value = accountDB.AccNo;
                cmd.Parameters.Add(p1);

                IDbDataParameter p2 = cmd.CreateParameter();
                p2.ParameterName = "@balance";
                p2.Value = accountDB.Balance;
                cmd.Parameters.Add(p2);

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new DepositAmountUpdateBalanceError($"Error depositing amount to account: {ex.Message}");

                }
                finally
                {
                    conn.Close();
                }
            }
        }
        public void WithdrawAmountUpdateBalance(AccountDB accountDB, int transID)
        {
            using (IDbConnection conn = GetConnection())
            {
                string sqlUpdate = @"UPDATE Accounts SET balance = @balance WHERE accNo = @accNo";

                IDbCommand cmd = conn.CreateCommand();
                cmd.CommandText = sqlUpdate;

                IDbDataParameter p1 = cmd.CreateParameter();
                p1.ParameterName = "@accNo";
                p1.Value = accountDB.AccNo;
                cmd.Parameters.Add(p1);

                IDbDataParameter p2 = cmd.CreateParameter();
                p2.ParameterName = "@balance";
                p2.Value = accountDB.Balance;
                cmd.Parameters.Add(p2);

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new WithdrawAmountUpdateBalanceError($"Error withdrawing amount from account: {ex.Message}");

                }
                finally
                {
                    conn.Close();
                }
            }
        }
        public void TransferAmountUpdateBalance(AccountDB fromAccountDB, AccountDB toAccountDB, int transID,double amount)
        {
            using (IDbConnection conn = GetConnection())
            {
               
                string withdrawQuery = "UPDATE Accounts SET balance = @balance WHERE accNo = @fromAccNo";
                string depositQuery = "UPDATE Accounts SET balance = @balance WHERE accNo = @toAccNo";
                conn.Open();

                using (IDbTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        
                        IDbCommand cmd1 = conn.CreateCommand();
                        cmd1.Transaction = transaction;
                        cmd1.CommandText = withdrawQuery;

                        IDbDataParameter p1 = cmd1.CreateParameter();
                        p1.ParameterName = "@fromAccNo";
                        p1.Value = fromAccountDB.AccNo;
                        cmd1.Parameters.Add(p1);

                        IDbDataParameter p2 = cmd1.CreateParameter();
                        p2.ParameterName = "@balance";
                        p2.Value = fromAccountDB.Balance;
                        cmd1.Parameters.Add(p2);

                        cmd1.ExecuteNonQuery();

                        IDbCommand cmd2 = conn.CreateCommand();
                        cmd2.Transaction = transaction;
                        cmd2.CommandText = depositQuery;

                        IDbDataParameter p3 = cmd2.CreateParameter();
                        p3.ParameterName = "@toAccNo";
                        p3.Value = toAccountDB.AccNo;
                        cmd2.Parameters.Add(p3);

                        IDbDataParameter p4 = cmd2.CreateParameter();
                        p4.ParameterName = "@balance";
                        p4.Value = toAccountDB.Balance;
                        cmd2.Parameters.Add(p4);
                        
                        cmd2.ExecuteNonQuery();
                        transaction.Commit();
                    }

                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new TransferAmountUpdateBalanceError($"Transaction is rolled back due to: {ex.Message}");
                        
                    }
                    finally
                    { conn.Close(); }
                }
            }
        }
        public void Close(AccountDB accountDB,int pin) 
        {
            using (IDbConnection conn = GetConnection())
            {
                string sqlUpdate = @"UPDATE Accounts SET active = @active WHERE accNo = @accNo";

                IDbCommand cmd = conn.CreateCommand();
                cmd.CommandText = sqlUpdate;

                IDbDataParameter p1 = cmd.CreateParameter();
                p1.ParameterName = "@active";
                p1.Value = accountDB.Active;
                cmd.Parameters.Add(p1);

                IDbDataParameter p2 = cmd.CreateParameter();
                p2.ParameterName = "@accNo";
                p2.Value = accountDB.AccNo;
                cmd.Parameters.Add(p2);

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new CloseAccountError($"Error in closing account: {ex.Message}");

                }
                finally
                {
                    conn.Close();
                }
            }

        }
        public void CreateTransaction(Transaction transactionDB)
        {
            using (IDbConnection conn = GetConnection())
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // Prepare the SQL command to insert the transaction into the database
                        string insertQuery = @"INSERT INTO Transactions (TransId,accNo, TransactionType, TransDate, Amount) VALUES 
                        (@TransId, @accNo, @TransactionType, @TransDate, @Amount)";

                        using (IDbCommand cmd = conn.CreateCommand())
                        {
                            cmd.Transaction = transaction;
                            cmd.CommandText = insertQuery;

                            // Add parameters
                            IDbDataParameter paramTransID = cmd.CreateParameter();
                            paramTransID.ParameterName = "@TransId";
                            paramTransID.Value = transactionDB.TransID;
                            cmd.Parameters.Add(paramTransID);

                            IDbDataParameter paramAccountNo = cmd.CreateParameter();
                            paramAccountNo.ParameterName = "@accNo";
                            paramAccountNo.Value = transactionDB.FromAccount.AccNo;
                            cmd.Parameters.Add(paramAccountNo);

                            IDbDataParameter paramTransactionType = cmd.CreateParameter();
                            paramTransactionType.ParameterName = "@TransactionType";
                            paramTransactionType.Value = transactionDB.TransactionType.ToString();
                            cmd.Parameters.Add(paramTransactionType);

                            IDbDataParameter paramTranDate = cmd.CreateParameter();
                            paramTranDate.ParameterName = "@TransDate";
                            paramTranDate.Value = transactionDB.TranDate;
                            cmd.Parameters.Add(paramTranDate);

                            IDbDataParameter paramAmount = cmd.CreateParameter();
                            paramAmount.ParameterName = "@Amount";
                            paramAmount.Value = transactionDB.Amount;
                            cmd.Parameters.Add(paramAmount);

                            // Execute the command
                            cmd.ExecuteNonQuery();
                        }

                        // Commit the transaction if everything is successful
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        // Rollback the transaction if there's an error
                        transaction.Rollback();
                        Console.WriteLine("Transaction creation failed and was rolled back.");
                        throw;
                    }
                }
            }
        }




        public void ExternalTransferUpdate(AccountDB fromAccount, ExternalAccountDB toExternalAccount, double amount, int pin, int transID)
        {
            
            using (IDbConnection conn = GetConnection())
            {
                
                conn.Open(); 
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        //Step 1: Withdraw from the source account
                        string withdrawQuery = "UPDATE Accounts SET balance = balance - @Amount WHERE accNo = @fromAccNo";
                        
                        IDbCommand cmd2 = conn.CreateCommand();
                        cmd2.Transaction = transaction;
                        cmd2.CommandText = withdrawQuery;
                        
                        IDbDataParameter p5 = cmd2.CreateParameter();
						p5.ParameterName = "@Amount";
						p5.Value = amount;
                        cmd2.Parameters.Add(p5);

                        IDbDataParameter p6 = cmd2.CreateParameter();
						p6.ParameterName = "@fromAccNo";
						p6.Value = fromAccount.AccNo;
                        cmd2.Parameters.Add(p6);

                        cmd2.ExecuteNonQuery();
                      
                        // Step 2: Deposit into the external bank's account
                        string depositQuery = $"insert into {toExternalAccount.BankName}BANK values (@externalAccNo,@Amount)";
                        
                        
                        IDbCommand cmd3 = conn.CreateCommand();
                        cmd3.Transaction = transaction;
                        cmd3.CommandText = depositQuery;

                        IDbDataParameter p7 = cmd3.CreateParameter();
						p7.ParameterName = "@externalAccNo";
						p7.Value = toExternalAccount.AccNo;
                        cmd3.Parameters.Add(p7);
                        
                       

                        IDbDataParameter p8 = cmd3.CreateParameter();
                        p8.ParameterName = "@Amount";
                        p8.Value = amount;
                        cmd3.Parameters.Add(p8);

                        cmd3.ExecuteNonQuery();
                        
                        // Commit the transaction
                        transaction.Commit();
                       
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine("External transfer failed and transaction was rolled back.");
                        throw;
                    }
                }
            }
        }

    }
    public class AccountDoesNotExistsException : ApplicationException
    {
        public AccountDoesNotExistsException(string message) : base(message) { }
    }
    public class DepositAmountUpdateBalanceError : ApplicationException
    {
        public DepositAmountUpdateBalanceError(string message) : base(message) { }
    }
    public class WithdrawAmountUpdateBalanceError : ApplicationException
    {
        public WithdrawAmountUpdateBalanceError(string message) : base(message) { }
    }
    public class TransferAmountUpdateBalanceError : ApplicationException
    {
        public TransferAmountUpdateBalanceError(string message) : base(message) { }
    }
    public class CloseAccountError : ApplicationException
    {
        public CloseAccountError(string message) : base(message) { }
    }
    
}
