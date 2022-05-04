using Models.Enums;

namespace Models.BankSystem
{
    public class Duty: BankAction
    {
        private readonly BankAccount _bankAccount;
        private readonly float _cash;
        private readonly DutyPeriod _dutyPeriod;

        public BankAccount BankAccount => _bankAccount;
        public float Cash => _cash;
        public DutyPeriod DutyPeriod => _dutyPeriod;

        public Duty(uint id, BankAccount bankAccount, float cash, DutyPeriod dutyPeriod, bool isAccepted = false,
            bool isReverted = false) :
            base(id, isAccepted, isReverted)
        {
            _bankAccount = bankAccount;
            _cash = cash;
            _dutyPeriod = dutyPeriod;
        }

        public override string ToString()
        {
            return $"Type: Duty, " +
                   $"Bank account: {_bankAccount.Id}, " +
                   $"Cash: {_cash}, " +
                   $"Period: {_dutyPeriod}";
        }

        protected override void OnAccept()
        {
            _bankAccount.TopUpCash(_cash);
        }

        protected override void OnRevert()
        {
            _bankAccount.WithdrawCash(_cash);
        }
    }
}