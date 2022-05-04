namespace Models.BankSystem
{
    public abstract class BankAction
    {
        private uint _id;
        private bool _accepted = false;
        private bool _reverted = false;

        public uint Id => _id;
        public bool IsAccepted => _accepted;
        public bool IsReverted => _reverted;

        public BankAction(uint id, bool isAccepted, bool isReverted) =>
            (_id, _accepted, _reverted) = (id, isAccepted, isReverted);

        public void Accept()
        {
            if (_accepted)
                return;
            
            _accepted = true;
            OnAccept();
        }

        public void Revert()
        {
            if (!_accepted || _reverted)
                return;

            _reverted = true;
            OnRevert();
        }

        public abstract string ToString();

        protected abstract void OnAccept();

        protected abstract void OnRevert();
    }
}