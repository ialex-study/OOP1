using System.Collections.Generic;
using Models.BankSystem;
using Models.CompanySystem;
using Models.Enums;
using Models.UserSystem;

namespace Models.Interfaces
{
    public interface IDataBase
    {
        public void SaveBank(Bank bank);
        public void SaveBankAccount(string bankName, BankAccount bankAccount);
        public void SaveDuty(string bankName, Duty duty);
        public void SaveCredit(string bankName, Credit credit);
        public void SaveTransfer(string bankName, Transfer transfer);
        public void SaveClient(Client client);
        public void SaveCompany(Company company);
        public void SaveEmployee(string companyName, Employee employee);
        public void SaveProject(string bankName, PaymentProject project);
        public void SaveUser(string login, string password, Company company, ClientType type);
        
        public Bank GetBank(string name);
        public BankAccount GetBankAccount(string bankName, uint id);
        public Duty GetDuty(string bankName, uint id);
        public Credit GetCredit(string bankName, uint id);
        public Transfer GetTransfer(string bankName, uint id);
        public Client GetClient(uint id);
        public Company GetCompany(string name);
        public Employee GetEmployee(string companyName, uint id);
        public PaymentProject GetProject(string bankName, uint id);
        public (string, ClientType) GetUser(string login, Company company);
        public List<Bank> GetBanks();
        public List<Company> GetCompanies();
    }
}