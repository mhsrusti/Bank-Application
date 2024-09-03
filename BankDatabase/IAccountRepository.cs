using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using BANK;
namespace BankDatabase;


public interface IAccountRepository //DataAcess Layer
{
    void Create(AccountDB accountDB);
    AccountDB GetAccount(string accNo);
    void DepositAmountUpdateBalance(AccountDB accountDB, int transID);
    void WithdrawAmountUpdateBalance(AccountDB accountDB,int transID);
    void TransferAmountUpdateBalance(AccountDB fromAccountDB, AccountDB toAccountDB,int transID,double amount);
    void Close(AccountDB accountDB,int pin);
    void CreateTransaction(Transaction transactionDB);
    void ExternalTransferUpdate(AccountDB fromAccountDB, ExternalAccountDB toExternalAccount,double amount,int pin, int transID);
  }
