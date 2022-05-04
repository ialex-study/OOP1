namespace Models.BankSystem
{
    public class Transfer: BankAction
    {
        private readonly BankAccount _from;
        private readonly BankAccount _to;
        
        private readonly float _cash;

        public BankAccount From => _from;
        public BankAccount To => _to;
        public float Cash => _cash;

        public Transfer(uint id, BankAccount from, BankAccount to, float cash, bool isAccepted = false, bool isReverted = false) :
            base(id, isAccepted, isReverted)
        {
            _from = from;
            _to = to;
            _cash = cash;
        }

        public override string ToString()
        {
            return $"Type: Transfer, " +
                   $"From: {_from.Id}, " +
                   $"To: {_to.Id}, " +
                   $"Cash: {_cash}";
        }

        public void Withdraw()
        {
            _from.WithdrawCash(_cash);
        }
        
        protected override void OnAccept()
        {
            _to.TopUpCash(_cash);
        }

        protected override void OnRevert()
        {
            _from.TopUpCash(_cash);
            _to.WithdrawCash(_cash);
        }
    }
}