using System.Collections.Generic;
using Models;
using Models.CompanySystem;
using Models.Interfaces;
using ObserverSystem;

namespace ViewModels
{
    public class CompanyVM : Observable, IObserver
    {
        private Company _company;
        private List<Employee> _employees;
        private ICompanyBank _bank;

        public ICompanyBank Bank => _bank;

        public List<Employee> Employees
        {
            get => _employees;
            private set
            {
                _employees = value;
                Notify();
            }
        }


        public void Login(Company company)
        {
            _company = company;
            _bank = DataBase.GetInstance().GetBank(company.BankName);
            Employees = company.Employees;
        }

        public void HireEmployee(uint accountId)
        {
            _company.HireEmployee(DataBase.GetInstance().GetEmployee(_company.Name, accountId));
        }

        public void FireEmployee(uint accountId)
        {
            _company.FireEmployee(accountId);
        }

        public void SubmitForPaymentProject()
        {
            Bank.SubmitForPaymentProject(_company);
        }

        public void Update()
        {
            
        }
    }
}