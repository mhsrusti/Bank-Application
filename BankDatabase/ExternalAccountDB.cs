using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDatabase
{
    public class ExternalAccountDB
    {
        public string AccNo { get; private set; }
        public string BankCode { get; private set; }
        public string BankName { get; private set; }

        public ExternalAccountDB(string accNo, string bankCode, string bankName)
        {
            AccNo = accNo;
            BankCode = bankCode;
            BankName = bankName;
        }
    }
}
