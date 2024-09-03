using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BANK
{
    public class ExternalAccount
    {
        public string AccNo { get;  set; }
        public string BankCode { get;  set; }
        public string BankName { get;  set; }

        public ExternalAccount(string accNo, string bankCode, string bankName)
        {
            this.AccNo = accNo;
            this.BankCode = bankCode;
            this.BankName = bankName;
        }
    }

}
