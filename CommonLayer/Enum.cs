using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BANK
{
    public enum AccountType
    {
        Savings,
        Current
    }

    public enum PrivilegeType
    {
        Regular,
        Gold,
        Premium
    }

    public enum TransactionTypes
    {
        Deposit,
        Withdraw,
        Transfer,
        ExternalTransfer
    }
    public enum TransactionStatus
    {
        OPEN,
        CLOSE
    }
}
