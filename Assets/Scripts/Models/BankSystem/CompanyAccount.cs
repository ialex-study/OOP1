using Models.CompanySystem;

namespace Models.BankSystem
{
    public class CompanyAccount: BankAccount
    {
        public string CompanyName { get; }
        
        public CompanyAccount(uint id, string password, string companyName) :
            base(id, password) =>
            CompanyName = companyName;
    }
}