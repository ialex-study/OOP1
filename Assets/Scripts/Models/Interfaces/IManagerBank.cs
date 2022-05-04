namespace Models.Interfaces
{
    public interface IManagerBank: IOperatorBank
    {
        public void AcceptDuty(uint dutyId);
        public void BlockAccount(uint accountId);
        public void UnblockAccount(uint accountId);
    }
}