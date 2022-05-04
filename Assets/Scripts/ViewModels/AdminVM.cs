using Models;
using Models.CompanySystem;
using Models.Interfaces;
using ObserverSystem;

namespace ViewModels
{
    public class AdminVM : Observable, IObserver
    {
        private IAdminBank _bank;

        public IAdminBank Bank
        {
            get => _bank;
            private set
            {
                _bank = value;
                Notify();
            }
        }

        public void Login(Company bankCompany)
        {
            Bank = DataBase.GetInstance().GetBank(bankCompany.Name);
        }

        public void Logout()
        {
            _bank = null;
        }
        
        public void Update()
        {
            
        }
    }
}