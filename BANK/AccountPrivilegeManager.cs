using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BANK
{
    public class AccountPrivilegeManager
    {
        public static Dictionary<PrivilegeType, double> dailyLimits;

        static AccountPrivilegeManager()
        {
            LoadDailyLimits();
        }

        public static void LoadDailyLimits()
        {
            try
            {
                dailyLimits = new Dictionary<PrivilegeType, double>();
                var filePath = "dailyLimits.properties";
                if (File.Exists(filePath))
                {
                    var lines = File.ReadAllLines(filePath);
                    foreach (var line in lines)
                    {
                        var keyValue = line.Split('=');
                        if (keyValue.Length != 2)
                            throw new InvalidPrivilageTypeException("Invalid privilage ");
                        if (keyValue.Length == 2 &&
                            Enum.TryParse(keyValue[0], out PrivilegeType privilegeType) &&
                            double.TryParse(keyValue[1], out double limit))
                        {
                            dailyLimits[privilegeType] = limit;
                        }
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }


        // Method to get daily limit
        public static double GetDailyLimit(PrivilegeType type)
        {
            if (!dailyLimits.ContainsKey(type))
                throw new InvalidPrivilageTypeException("Invalid privilage type ");
            return dailyLimits[type];
        }

    }
}
