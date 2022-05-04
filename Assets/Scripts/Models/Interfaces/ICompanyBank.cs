using Models.BankSystem;
using Models.CompanySystem;

namespace Models.Interfaces
{
    public interface ICompanyBank
    {
        public uint CreateBankAccount(string password, Company company);
        public uint SubmitForPaymentProject(Company company);
        public uint MakeTransfer(Company company, string toBankName, uint toAccountId, float cash);
    }
}