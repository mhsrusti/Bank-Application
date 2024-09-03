using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BANK
{
    public class Policy : IPolicy
    {
        private readonly double minBalance;
        private readonly double rateOfInterest;

        public Policy(double minBalance, double rateOfInterest)
        {
            this.minBalance = minBalance;
            this.rateOfInterest = rateOfInterest;
        }

        public double GetMinBalance() => minBalance;
        public double GetRateOfInterest() => rateOfInterest;
    }
}
