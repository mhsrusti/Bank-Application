using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BankDatabase;

namespace BANK
{
    // IExternalBankService Interface
    public interface IExternalBankService
    {
        bool Deposit(string accNo, double amt);
    }

    // ICICIBankService Implementation
    public class ICICIBankService : IExternalBankService
    {
        public bool Deposit(string accNo, double amt)
        {
            // ICICI Bank-specific deposit logic
            Console.WriteLine($"Deposited {amt} to {accNo} in ICICI Bank.");
            return true;
        }
    }

    // ExternalBankServiceFactory Singleton + Factory
    public class ExternalBankServiceFactory
    {
        private static ExternalBankServiceFactory _instance;
        private readonly Dictionary<string, IExternalBankService> serviceBankPool;

        private ExternalBankServiceFactory()
        {
            serviceBankPool = new Dictionary<string, IExternalBankService>();
            LoadBankServices();
        }

        public static ExternalBankServiceFactory Instance => _instance ??= new ExternalBankServiceFactory();

        private void LoadBankServices()
        {
            var properties = new Dictionary<string, string>
        {
            { "ICICI", "ICICIBankService" },
            { "CITI", "CITIBankService" }
        };

            foreach (var entry in properties)
            {
                string bankCode = entry.Key;
                string className = entry.Value;
                IExternalBankService bankObj = (IExternalBankService)Activator.CreateInstance(Type.GetType(className));
                serviceBankPool.Add(bankCode, bankObj);
            }
        }

        public IExternalBankService GetBankService(string bankCode)
        {
            if (serviceBankPool.ContainsKey(bankCode))
            {
                return serviceBankPool[bankCode];
            }

            throw new ArgumentException("Invalid bank code.");
        }
    }     
}
