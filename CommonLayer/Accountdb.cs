using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace BankDatabase
{
    public class AccountDB
    {
        public string AccNo { get; set; }
        public string Name { get; set; }
        public string Pin { get; set; }
        public double Balance { get; set; }
        public DateTime DateOfOpening { get; set; }
        public string PrivilegeType { get; set; }
        public string AccountType { get;set; }
        public int Active { get; set; }

        
    }

    
}
