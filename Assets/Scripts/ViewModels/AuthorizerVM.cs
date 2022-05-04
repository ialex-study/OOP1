using System.Collections.Generic;
using Models;
using Models.CompanySystem;
using Models.Enums;
using Models.Interfaces;
using Models.UserSystem;
using ObserverSystem;

namespace ViewModels
{
    public class AuthorizerVM : Observable
    {
        private List<Company> _companies;
        private IDataBase _dataBase;
        private bool _isAuthorized = false;
        private Client _client;
        private Company _company;
        private ClientType _clientType;
        
        public List<Company> Companies
        {
            get => _companies;
            private set
            {
                _companies = value;
                Notify();
            }
        }

        public bool IsAuthorized
        {
            get => _isAuthorized;
            private set
            {
                _isAuthorized = value;
                Notify();
            }
        }

        public Company Company => _company;
        public ClientType ClientType => _clientType;
        public Client Client => _client;

        public AuthorizerVM()
        {
            _dataBase = DataBase.GetInstance();

            Companies = _dataBase.GetCompanies();
        }

        public void AuthorizeByCreds(string login, string password, int companyIndex)
        {
            (Company company, ClientType clientType) = Authorizer.AuthorizeByCredentials(login, password, Companies[companyIndex]);

            if (clientType == ClientType.None)
                return;

            _clientType = clientType;
            _company = company;
            IsAuthorized = true;
        }

        public void AuthorizeByPassport(uint passportId)
        {
            Client client = Authorizer.AuthorizeByPassport(_dataBase.GetClient(passportId).Passport);

            if (client is null)
                return;

            _client = client;
            _clientType = ClientType.Client;
            IsAuthorized = true;
        }

        public void Logout()
        {
            _clientType = ClientType.None;
            IsAuthorized = false;
        }
    }
}