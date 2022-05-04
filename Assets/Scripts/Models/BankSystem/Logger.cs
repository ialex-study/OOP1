using System.Collections.Generic;
using Models.BankSystem;

namespace Models.BankSystem
{
    public class Logger
    {
        private List<string> _logs = new List<string>();

        public void Log(Dictionary<uint, BankAction> logs)
        {
            uint logKey = (uint) _logs.Count + 1;
            
            foreach(var pair in logs)
            {
                string log = $"{logKey}. {pair.Value.ToString()}";
                _logs.Add(log);

                logKey++;
            }
        }

        public string GetLogs()
        {
            string result = "";

            foreach (string log in _logs)
            {
                result += log + "\n";
            }

            return result;
        }
    }
}