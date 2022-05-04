using System.Linq;
using Models.CompanySystem;
using Models.Enums;
using ObserverSystem;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using ViewModels;

namespace Views
{
    public class ClientView : AuthorizedView, IObserver
    {
        [Header("Login Window")] 
        [SerializeField] private GameObject loginWindow;
        [SerializeField] private TMP_InputField accountIdField;
        [SerializeField] private TMP_InputField passwordField;
        [SerializeField] private TMP_Dropdown banksDropdown;

        [Header("New Account Window")] 
        [SerializeField] private GameObject newAccountWindow;

        [Header("Menu")] 
        [SerializeField] private GameObject menuWindow;
        [SerializeField] private TMP_Dropdown functionsDropdown;
        [SerializeField] private TMP_Text cashText;
        [SerializeField] private Toggle freezeToogle;

        [Header("Functions Windows")] 
        [SerializeField] private GameObject withdrawWindow;
        [SerializeField] private GameObject transferWindow;
        [SerializeField] private GameObject dutyWindow;
        [SerializeField] private GameObject creditWindow;

        [Header("Withdraw")] 
        [SerializeField] private TMP_InputField withdrawCashField;

        [Header("Transfer")] 
        [SerializeField] private TMP_InputField toAccoutIdField;
        [SerializeField] private TMP_InputField transferCashField;
        [SerializeField] private TMP_Dropdown transferBanksDropdown;

        [Header("Duty")] 
        [SerializeField] private TMP_InputField dutyCashField;
        [SerializeField] private TMP_Dropdown dutyPeriodDropdown;

        [Header("Credit")] 
        [SerializeField] private TMP_InputField creditCashField;
        [SerializeField] private TMP_InputField creditPercentField;
        [SerializeField] private TMP_Dropdown creditPeriodDropdown;

        private ClientVM _clientVM;

        public void FreezeToogled(bool isFreezed)
        {
            if(_clientVM.IsLogged)
                _clientVM.IsFreezed = freezeToogle.isOn;
        }
        
        public void UpdateWindows()
        {
            newAccountWindow.SetActive(false);
            menuWindow.SetActive(false);
            loginWindow.SetActive(false);
            
            if (_clientVM.IsLogged)
                menuWindow.SetActive(true);
            else
                loginWindow.SetActive(true);
            
            withdrawWindow.SetActive(false);
            transferWindow.SetActive(false);
            dutyWindow.SetActive(false);
            creditWindow.SetActive(false);

            int currentFunction = functionsDropdown.value;

            switch (currentFunction)
            {
                case 0:
                    withdrawWindow.SetActive(true);
                    break;
                case 1:
                    transferWindow.SetActive(true);
                    break;
                case 2:
                    dutyWindow.SetActive(true);
                    break;
                case 3:
                    creditWindow.SetActive(true);
                    break;
            }
        }
        
        public void Login()
        {
            int bankId = banksDropdown.value;
            uint login = uint.Parse(accountIdField.text);
            string password = passwordField.text;
            
            _clientVM.Login(_clientVM.Banks[bankId].Name, login, password);
        }

        public void Withdraw()
        {
            float cash = float.Parse(withdrawCashField.text);

            _clientVM.Withdraw(cash);
        }

        public void MakeTransfer()
        {
            int bankId = transferBanksDropdown.value;
            uint toAccountId = uint.Parse(toAccoutIdField.text);
            float cash = float.Parse(transferCashField.text);

            _clientVM.MakeTransfer(_clientVM.Banks[bankId], toAccountId, cash);
        }

        public void GetDuty()
        {
            float cash = float.Parse(dutyCashField.text);
            DutyPeriod period = (DutyPeriod) dutyPeriodDropdown.value;

            _clientVM.GetDuty(cash, period);
        }

        public void GetCredit()
        {
            float cash = float.Parse(creditCashField.text);
            float percent = float.Parse(creditPercentField.text);
            DutyPeriod period = (DutyPeriod) creditPeriodDropdown.value;

            _clientVM.GetCredit(cash, percent, period);
        }
        
        public override void Login(Company company)
        {
            gameObject.SetActive(true);
        }

        public override void Logout()
        {
            _clientVM.Logout();
            gameObject.SetActive(false);
        }

        void IObserver.Update()
        {
            cashText.text = "Cash: " + _clientVM.Cash;

            UpdateWindows();
        }
        
        private void Start()
        {
            _clientVM = new ClientVM();
            _clientVM.Subscribe(this);
            
            UpdateDropdowns();
            UpdateWindows();
        }

        private void UpdateDropdowns()
        {
            banksDropdown.ClearOptions();
            transferBanksDropdown.ClearOptions();
            banksDropdown.AddOptions((from bank in _clientVM.Banks select bank.Name).ToList());
            transferBanksDropdown.AddOptions((from bank in _clientVM.Banks select bank.Name).ToList());
        }
    }
}