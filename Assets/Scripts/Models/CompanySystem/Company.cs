using System.Collections.Generic;
using Models.BankSystem;
using Models.Exceptions;
using Models.Interfaces;

namespace Models.CompanySystem
{
    public class Company
    {
        protected string _name;
        private uint _id;
        private string _type;
        private string _address;
        private List<Employee> _employees;
        private readonly IDataBase _dataBase = DataBase.GetInstance();
        
        public string Type => _type;
        public string Name => _name;
        public string Address => _address;
        public List<Employee> Employees => _employees;
        public string BankName { get; }

        public Company(string type, string name, string address, string bankName, List<Employee> employees = null)
        {
            (_type, _name, _address, BankName) = (type, name, address, bankName);

            _employees = employees ?? new List<Employee>();
        }

        public void HireEmployee(Employee employee)
        {
            _employees.Add(employee);
            
            _dataBase.SaveCompany(this);
        }

        public void FireEmployee(uint employeeAccountId)
        {
            _employees.Remove(_employees.Find((employee) => employee.BankAccount.Id == employeeAccountId));
            
            _dataBase.SaveCompany(this);
        }
    }
}