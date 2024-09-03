using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BANK
{
    class AccountFactory
    {
        public static IAccount CreateAccount(string name, string pin, double balance, PrivilegeType privilegeType, AccountType accType, string pan, string aadhar, IPolicy policy)
        {
            IAccount account;
            switch (accType)
            {
                case AccountType.Savings:
                    account = new SavingsAccount(name, pin, balance, privilegeType,policy);
                    break;
                case AccountType.Current:
                    account = new CurrentAccount(name, pin, balance, privilegeType, policy);
                    break;
                default:
                    throw new ArgumentException("Invalid account type.");
            }
            return account;
        }
    }

}
