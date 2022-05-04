using Models.CompanySystem;
using ObserverSystem;
using TMPro;
using UnityEngine;
using ViewModels;

namespace Views
{
    public class CompanyView : AuthorizedView, IObserver
    {
        [SerializeField] private TMP_InputField employeeIdField;
        [SerializeField] private TMP_InputField transferIdField;
        [SerializeField] private TMP_Text employeesView;

        private CompanyVM _companyVM;
        
        public void HireEmployee()
        {
            uint employeeId = uint.Parse(employeeIdField.text);
            
            _companyVM.HireEmployee(employeeId);
        }
        
        public void FireEmployee()
        {
            uint employeeId = uint.Parse(employeeIdField.text);
            
            _companyVM.FireEmployee(employeeId);
        }

        public void MakeTransfer()
        {
            
        }

        public void SubmitForProject()
        {
            _companyVM.SubmitForPaymentProject();
        }

        public override void Login(Company company)
        {
            _companyVM.Login(company);
            gameObject.SetActive(true);
        }

        public override void Logout()
        {
            gameObject.SetActive(false);
        }
        
        void IObserver.Update()
        {
            UpdateEmployees();
        }

        private void Awake()
        {
            _companyVM = new CompanyVM();
            _companyVM.Subscribe(this);
        }

        private void UpdateEmployees()
        {
            string employees = "";

            foreach (var employee in _companyVM.Employees)
            {
                employees += employee.BankAccount.Id + "\n";
            }

            employeesView.text = employees;
        }
    }
}