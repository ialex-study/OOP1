using Models.BankSystem;
using Models.UserSystem;

namespace Models.CompanySystem
{
    public class Employee: Client
    {
        private BankAccount _bankAccount;

        public BankAccount BankAccount => _bankAccount;

        public Employee(string name, string surname, string email, string phone, Passport passport,
            BankAccount account) :
            base(name, surname, email, phone, passport) =>
            _bankAccount = account;
    }
}