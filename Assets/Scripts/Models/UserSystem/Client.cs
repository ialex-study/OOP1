using Models.Enums;
using Models.Interfaces;

namespace Models.UserSystem
{
    public class Client
    {
        private string _name;
        private string _surname;
        private string _email;
        private string _phone;
        private Passport _passport;
        
        public string Name => _name;
        public string Surname => _surname;
        public string Email => _email;
        public string Phone => _phone;
        public Passport Passport => _passport;

        public Client(string name, string surname, string email, string phone, Passport passport) =>
            (_name, _surname, _email, _phone, _passport) = (name, surname, email, phone, passport);

        public uint CreateBankAccount(IClientBank bank, string password)
        {
            return bank.CreateBankAccount(password);
        }

        public void WithdrawCash(IClientBank bank, uint accountId, float delta)
        {
            bank.WithdrawCash(accountId, delta);
        }

        public uint MakeTransfer(IClientBank bank, uint fromAccountId, string toBankName, uint toAccountId, float cash)
        {
            return bank.MakeTransfer(fromAccountId, toBankName, toAccountId, cash);
        }

        public uint GetDuty(IClientBank bank, uint accountId, DutyPeriod period, float cash)
        {
            return bank.GetDuty(accountId, period, cash);
        }

        public uint GetCredit(IClientBank bank, uint accountId, DutyPeriod period, float cash, float percent)
        {
            return bank.GetCredit(accountId, period, cash, percent);
        }

        public void FreezeAccount(IClientBank bank, uint bankAccount)
        {
            bank.FreezeAccount(bankAccount);
        }

        public void UnfreezeAccount(IClientBank bank, uint bankAccount)
        {
            bank.UnfreezeAccount(bankAccount);
        }
    }
}