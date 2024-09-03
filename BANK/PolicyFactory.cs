using System;
using System.Collections.Generic;
using System.IO;

namespace BANK
{
    public class PolicyFactory
    {
        private static PolicyFactory instance;
        private readonly Dictionary<string, Dictionary<string, (double minBalance, double rateOfInterest)>> policies = new();

        private PolicyFactory()
        {
            try
            {
                string[] lines = File.ReadAllLines("Policies.properties");
                foreach (string line in lines)
                {
                    var parts = line.Split(':');
                    if (parts.Length == 2)
                    {
                        var keyParts = parts[0].Split('-');
                        if (keyParts.Length == 2)
                        {
                            string accountType = keyParts[0];
                            string privilegeType = keyParts[1];
                            var values = parts[1].Split(',');
                            if (values.Length == 2 && double.TryParse(values[0], out double minBalance) && double.TryParse(values[1], out double rateOfInterest))
                            {
                                if (!policies.ContainsKey(accountType))
                                {
                                    policies[accountType] = new Dictionary<string, (double minBalance, double rateOfInterest)>();
                                }
                                policies[accountType][privilegeType] = (minBalance, rateOfInterest);
                            }
                            else
                            {
                                Console.WriteLine($"Invalid format for values: {line}");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Invalid format for key: {line}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Invalid format for line: {line}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading Policies.properties file: {ex.Message}");
            }
        }

        public static PolicyFactory Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new PolicyFactory();
                }
                return instance;
            }
        }

        public IPolicy CreatePolicy(string accountType, string privilegeType)
        {
            accountType = accountType.ToUpper();
            privilegeType = privilegeType.ToUpper();
            if (policies.TryGetValue(accountType, out var privileges) && privileges.TryGetValue(privilegeType, out var policyData))
            {
                return new Policy(policyData.minBalance, policyData.rateOfInterest);
            }
            throw new ArgumentException("Invalid account type or privilege type.");
        }
    }

}
