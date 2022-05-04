using Models.Enums;

namespace Models.BankSystem
{
    public class Credit: Duty
    {
        private float _percent;

        public float Percent => _percent;

        public Credit(uint id, BankAccount bankAccount, float cash, DutyPeriod dutyPeriod, float percent,
            bool isAccepted = false, bool isReverted = false) :
            base(id, bankAccount, cash, dutyPeriod, isAccepted, isReverted)
        {
            _percent = percent;
        }

        public override string ToString()
        {
            return base.ToString() + $"Percent: {_percent}";
        }
    }
}