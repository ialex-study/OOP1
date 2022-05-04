using System.Collections.Generic;
using Models;
using Models.BankSystem;
using Models.Enums;
using Models.Interfaces;
using ObserverSystem;

namespace ViewModels
{
    public class ClientVM : Observable, IObserver
    {
        private IClientBank _selectedBank;
        private BankAccount _currentAccount;
        private List<Bank> _banks;
        private bool _isLogged;
        private IDataBase _dataBase = DataBase.GetInstance();

        public float Cash
        {
            get => _currentAccount?.Cash ?? 0;
        }

        public bool IsFreezed
        {
            get => _currentAccount.IsFreezed;
            set
            {
                _currentAccount.IsFreezed = value;
            }
        }

        public bool IsBlocked
        {
            get => _currentAccount.IsBlocked;
        }

        public BankAccount BankAccount
        {
            get => _currentAccount;
            private set
            {
                _currentAccount = value;
                Notify();
            }
        }

        public List<Bank> Banks
        {
            get => _banks;
            private set
            {
                _banks = value;
                Notify();
            }
        }

        public bool IsLogged
        {
            get => _isLogged;
            private set
            {
                _isLogged = value;
                Notify();
            }
        }

        public ClientVM()
        {
            _banks = DataBase.GetInstance().GetBanks();
        }

        public void Login(string bankName, uint accountId, string password)
        {
            _banks = DataBase.GetInstance().GetBanks();
            Bank bank = DataBase.GetInstance().GetBank(bankName);
            BankAccount account = bank.BankAccounts[accountId];

            if (account.Password != password)
                return;
            
            _currentAccount?.Unsubscribe(this);

            _selectedBank = bank;
            _currentAccount = account;
            
            _currentAccount.Subscribe(this);
            
            IsLogged = true;
        }

        public void Logout()
        {
            _selectedBank = null;
            _currentAccount = null;
            IsLogged = false;
        }

        public void Withdraw(float delta)
        {
            _selectedBank.WithdrawCash(_currentAccount.Id, delta);
        }

        public uint MakeTransfer(Bank toBank, uint toAccountId, float cash)
        {
            return _selectedBank.MakeTransfer(_currentAccount.Id, toBank.Name, toAccountId, cash);
        }

        public uint GetDuty(float cash, DutyPeriod period)
        {
            return _selectedBank.GetDuty(_currentAccount.Id, period, cash);
        }

        public uint GetCredit(float cash, float percent, DutyPeriod period)
        {
            return _selectedBank.GetCredit(_currentAccount.Id, period, cash, percent);
        }

        public void Update()
        {
            Notify();
        }
    }
}