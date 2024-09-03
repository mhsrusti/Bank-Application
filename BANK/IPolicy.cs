using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BANK
{
    public interface IPolicy
    {
        double GetMinBalance();
        double GetRateOfInterest();
    }

}
