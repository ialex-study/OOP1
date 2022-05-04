using System;
using System.Collections.Generic;
using ObserverSystem;

namespace Models.BankSystem
{
    public class BankAccount : Observable
    {
        private uint _id;
        private string _password;
        private float _cash;
        private bool _isFreezed = false;
        private bool _isBlocked = false;

        public uint Id => _id;
        public string Password => _password;
        public float Cash => _cash;

        public bool IsFreezed
        {
            get => _isFreezed;
            set
            {
                _isFreezed = value;
                Notify();
            }
        }

        public bool IsBlocked
        {
            get => _isBlocked;
            set
            {
                _isBlocked = value;
                Notify();
            }
        }

        public BankAccount(uint id, string password, float cash = 0) =>
            (_id, _password, _cash) = (id, password, cash);

        public void TopUpCash(float delta)
        {
            _cash += delta;
            Notify();
        }

        public void WithdrawCash(float delta)
        {
            _cash -= delta;
            Notify();
        }
    }
}