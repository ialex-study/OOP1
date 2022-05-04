using System.Linq;
using Models.Enums;
using UnityEngine;
using ViewModels;
using ObserverSystem;
using TMPro;

namespace Views
{
    public class AuthorizerView : MonoBehaviour, IObserver
    {
        [Header("Auth by credentials")]
        [SerializeField] private GameObject authByCredsWindow;
        [SerializeField] private TMP_InputField loginField;
        [SerializeField] private TMP_InputField passwordField;
        
        [Header("Auth by passport")]
        [SerializeField] private GameObject authByPassportWindow;
        [SerializeField] private TMP_InputField passportIdField;
        
        [Space]
        [SerializeField] private GameObject firstTimeWindow;
        
        [Header("Dropdowns")]
        [SerializeField] private TMP_Dropdown authTypeDropdown;
        [SerializeField] private TMP_Dropdown companiesDropdown;

        [Header("Authorized Windows")] 
        [SerializeField] private AuthorizedView clientWindow;
        [SerializeField] private AuthorizedView adminWindow;
        [SerializeField] private AuthorizedView operatorWindow;
        [SerializeField] private AuthorizedView managerWindow;
        [SerializeField] private AuthorizedView companyWindow;

        private AuthorizerVM _authorizerVM;

        public void AuthTypeChanged(int value)
        {
            value = authTypeDropdown.value;
            
            switch (value)
            {
                case 0:
                    authByCredsWindow.SetActive(true);
                    authByPassportWindow.SetActive(false);
                    break;
                case 1:
                    authByCredsWindow.SetActive(false);
                    authByPassportWindow.SetActive(true);
                    break;
            }
        }

        public void EnterByCreds()
        {
            string login = loginField.text;
            string password = passwordField.text;
            int companyIndex = companiesDropdown.value;

            _authorizerVM.AuthorizeByCreds(login, password, companyIndex);
        }

        public void EnterByPassport()
        {
            uint passportId = uint.Parse(passportIdField.text);
            
            _authorizerVM.AuthorizeByPassport(passportId);
        }

        public void Logout()
        {
            _authorizerVM.Logout();
        }
        
        void IObserver.Update()
        {
            UpdateBanks();
            CheckAuthorization();
        }
        
        private void Start()
        {
            _authorizerVM = new AuthorizerVM();
            _authorizerVM.Subscribe(this);

            authByPassportWindow.SetActive(false);
            firstTimeWindow.SetActive(false);
            DisableWindows();

            UpdateBanks();
        }

        private void UpdateBanks()
        {
            companiesDropdown.ClearOptions();
            companiesDropdown.AddOptions((from company in _authorizerVM.Companies select company.Name).ToList());
        }

        private void CheckAuthorization()
        {
            ClientType clientType = _authorizerVM.ClientType;

            switch (clientType)
            {
                case ClientType.Client:
                    clientWindow.Login(null);
                    break;
                
                case ClientType.Operator:
                    operatorWindow.Login(_authorizerVM.Company);
                    break;
                
                case ClientType.Manager:
                    managerWindow.Login(_authorizerVM.Company);
                    break;
                
                case ClientType.Admin:
                    adminWindow.Login(_authorizerVM.Company);
                    break;

                case ClientType.Company:
                    companyWindow.Login(_authorizerVM.Company);
                    break;
                
                default:
                    DisableWindows();
                    break;
            }
        }

        private void DisableWindows()
        {
            clientWindow.Logout();
            operatorWindow.Logout();
            managerWindow.Logout();
            adminWindow.Logout();
            companyWindow.Logout();
        }
    }
}