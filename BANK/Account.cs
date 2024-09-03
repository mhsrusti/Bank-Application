using System;
using System.Collections.Generic;
using System.IO;
using BankDatabase;

namespace BANK
{
    public interface IAccount
    {
        string AccNo { get; }
        string Name { get; }
        string Pin { get; set; }
        double Balance { get; set; }
        PrivilegeType PrivilegeType { get; }
        AccountType AccountType { get; }
        bool Active { get; set; }
        void Open();
        IPolicy Policy { get; }
        IPolicy GetPolicy();
    }

    public abstract class Account : IAccount
    {
        public string AccNo { get; private set; }
        public string Name { get; private set; }
        public string Pin { get; set; }
        public double Balance { get;  set; }
        public PrivilegeType PrivilegeType { get; private set; }
        public AccountType AccountType { get; private set; }
        public bool Active { get;  set; }
        public IPolicy Policy { get; private set; }

        protected IPolicy policy;

        public IPolicy GetPolicy()
        {
            return policy;
        }

        public void SetPolicy(IPolicy policy)
        {
            this.policy = policy;
        }

        protected Account(string name, string pin, double balance, PrivilegeType privilegeType, AccountType accountType, IPolicy policy)
        {
            AccNo = IDGenerator.GenerateAccNo(accountType.ToString());
            Name = name;
            Pin = pin;
            Balance = balance;
            PrivilegeType = privilegeType;
            AccountType = accountType;
            Active = false;
            Policy = policy;
            this.policy = policy;
        }

        public void Open()
        {
            Active = true;
        }
    }
    //class IDGenerator
    //{
    //    private static int idCounter = 1000;

    //    public static int IDNumber()
    //    {
    //        return idCounter++;
    //    }
    //    public static string GenerateAccountNumber(AccountType accountType)
    //    {
    //        string prefix = accountType == AccountType.Savings ? "SAV" : "CUR";
    //        return $"{prefix}-{IDGenerator.IDNumber()}";
    //    }
    //}

public static class IDGenerator
    {
        private static readonly string filePath = "CurrentID.txt";
        private static readonly string transIDFilePath = "CurrentTransID.txt";

        // Generates a consistent account number based on account type
        public static string GenerateAccNo(string accType)
        {
            int newID = GenerateID(filePath);
            return $"{accType.Substring(0, 3).ToUpper()}{newID:D4}";
        }

        // Generates a new unique ID from the file and increments it
        public static int GenerateID(string filePath)
        {
            int currentID = ReadCurrentID(filePath);
            int newID = currentID + 1;
            WriteNewID(filePath, newID);
            return newID;
        }

        // Generates a new transaction ID
        public static int GenerateTransID()
        {
            return GenerateID(transIDFilePath);
        }

        // Reads the current ID from the file
        private static int ReadCurrentID(string filePath)
        {
            if (!File.Exists(filePath))
            {
                File.WriteAllText(filePath, "1000"); // Initialize with starting point
                return 1000; // Return the initial ID
            }
            string idStr = File.ReadAllText(filePath);
            if (int.TryParse(idStr, out int currentID))
            {
                return currentID;
            }
            else
            {
                throw new Exception("Invalid ID in file.");
            }
        }

        // Writes the new ID to the file
        private static void WriteNewID(string filePath, int newID)
        {
            File.WriteAllText(filePath, newID.ToString());
        }
    }

    public class SavingsAccount : Account
    {
        public SavingsAccount(string name, string pin, double balance, PrivilegeType privilegeType, IPolicy policy)
            : base(name, pin, balance, privilegeType, AccountType.Savings, policy)
        {
        }
    }

    public class CurrentAccount : Account
    {
        public CurrentAccount(string name, string pin, double balance, PrivilegeType privilegeType, IPolicy policy)
            : base(name, pin, balance, privilegeType, AccountType.Current, policy)
        {
        }
    }
}
