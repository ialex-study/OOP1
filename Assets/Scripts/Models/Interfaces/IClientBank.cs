using Models.Enums;

namespace Models.Interfaces
{
    public interface IClientBank
    {
        public uint CreateBankAccount(string password);
        public void WithdrawCash(uint accountId, float delta);
        public uint MakeTransfer(uint fromAccountId, string toBankName, uint toAccountId, float cash);
        public uint GetDuty(uint accountId, DutyPeriod period, float cash);
        public uint GetCredit(uint accountId, DutyPeriod period, float cash, float percent);
        public void FreezeAccount(uint accountId);
        public void UnfreezeAccount(uint accountId);
    }
}