﻿using Models;
using Models.CompanySystem;
using Models.Interfaces;
using ObserverSystem;

namespace ViewModels
{
    public class ManagerVM : Observable, IObserver
    {
        private IManagerBank _bank;

        public IManagerBank Bank
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