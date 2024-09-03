using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BANK
{
    public class MinBalanceNeedsToBeMaintainedException : Exception
    {
        public MinBalanceNeedsToBeMaintainedException(string message) : base(message) { }
    }

    public class UnableToOpenAccountException : Exception
    {
        public UnableToOpenAccountException(string message) : base(message) { }
    }

    public class AccountDoesNotExistsException : ApplicationException
    {
        public AccountDoesNotExistsException(string message) : base(message) { }
    }

    public class InvalidPinException : ApplicationException
    {
        public InvalidPinException(string message) : base(message) { }
    }

    public class InsufficientBalanceException : ApplicationException
    {
        public InsufficientBalanceException(string message) : base(message) { }
    }

    public class InactiveAccountException : ApplicationException
    {
        public InactiveAccountException(string message) : base(message) { }
    }

    public class InvalidTransactionException : ApplicationException
    {
        public InvalidTransactionException(string message) : base(message) { }
    }
    public class TransactionNotFoundException : ApplicationException
    {
        public TransactionNotFoundException(string message) : base(message) { }
    }

    public class InvalidTransactionTypeException : ApplicationException
    {
        public InvalidTransactionTypeException(string message) : base(message) { }
    }
    public class DailyLimitExceededException : ApplicationException
    {
        public DailyLimitExceededException(string message) : base(message) { }
    }
    public class InvalidPrivilageTypeException : ApplicationException
    {
        public InvalidPrivilageTypeException(string message) : base(message) { }
    }
    public class InvalidPolicyTypeException : ApplicationException
    {
        public InvalidPolicyTypeException(string message) : base(message) { }
    }
}
