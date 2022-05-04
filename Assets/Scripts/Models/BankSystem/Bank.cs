using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Models.CompanySystem;
using Models.Enums;
using Models.Interfaces;
using Models.Exceptions;

namespace Models.BankSystem
{
    public class Bank: Company, IClientBank, IOperatorBank, IManagerBank, ICompanyBank, IAdminBank
    {
        private readonly Dictionary<uint, BankAccount> _bankAccounts;
        private readonly Dictionary<uint, PaymentProject> _paymentProjects;
        private readonly Dictionary<uint, BankAction> _actions;
        private readonly Logger _logger = new Logger();
        private readonly IDataBase _dataBase = DataBase.GetInstance();

        public Dictionary<uint, BankAccount> BankAccounts => _bankAccounts;
        public Dictionary<uint, PaymentProject> PaymentProjects => _paymentProjects;
        public Dictionary<uint, BankAction> BankActions => _actions;
        public Logger Logger => _logger;

        public Bank(string name, string address, Dictionary<uint, BankAccount> bankAccounts,
            Dictionary<uint, PaymentProject> paymentProjects, Dictionary<uint, BankAction> actions, Logger logger) :
            base("Банк", name, address, null)
        {
            _bankAccounts = bankAccounts ?? new Dictionary<uint, BankAccount>();
            _paymentProjects = paymentProjects ?? new Dictionary<uint, PaymentProject>();
            _actions = actions ?? new Dictionary<uint, BankAction>();
            _logger = logger ?? new Logger();
        }

        // CLIENT BANK FUNCTIONALITY
        public uint CreateBankAccount(string password)
        {
            uint id = _bankAccounts.Keys.Max() + 1;
            BankAccount newBankAccount = new BankAccount(id, password);
            _bankAccounts.Add(id, newBankAccount);
            
            _dataBase.SaveBankAccount(Name, newBankAccount);
            
            return id;
        }

        public void WithdrawCash(uint accountId, float cash)
        {
            BankAccount bankAccount = TryGetBankAccount(accountId);

            if (bankAccount.Cash < cash)
                throw new NotEnoughCashException();
            
            bankAccount.WithdrawCash(cash);
            _dataBase.SaveBankAccount(Name, bankAccount);
        }

        public uint MakeTransfer(uint fromAccountId, string toBankName, uint toAccountId, float cash)
        {
            Bank toBank = DataBase.GetInstance().GetBank(toBankName);

            if (toBank is null)
                throw new NoSuchBankException();
            
            BankAccount fromAccount = TryGetBankAccount(fromAccountId);
            BankAccount toAccount = toBank.TryGetBankAccount(toAccountId);

            if (cash <= 0)
                throw new InvalidExpressionException();
            if (fromAccount.Cash < cash)
                throw new NotEnoughCashException();

            uint transferId = GenerateActionId();
            Transfer transfer = new Transfer(transferId, fromAccount, toAccount, cash);
            transfer.Withdraw();
            _actions.Add(transferId, transfer);
            
            _dataBase.SaveTransfer(Name, transfer);

            return transferId;
        }

        public uint GetDuty(uint accountId, DutyPeriod period, float cash)
        {
            BankAccount bankAccount = TryGetBankAccount(accountId);

            uint dutyId = GenerateActionId();
            Duty duty = new Duty(dutyId, bankAccount, cash, period);
            _actions.Add(dutyId, duty);
            
            _dataBase.SaveDuty(Name, duty);

            return dutyId;
        }

        public uint GetCredit(uint accountId, DutyPeriod period, float cash, float percent)
        {
            BankAccount bankAccount = TryGetBankAccount(accountId);

            uint creditId = GenerateActionId();
            Credit credit = new Credit(creditId, bankAccount, cash, period, percent);
            _actions.Add(creditId, credit);
            
            _dataBase.SaveCredit(Name, credit);

            return creditId;
        }

        public void FreezeAccount(uint accountId)
        {
            BankAccount account = TryGetBankAccount(accountId);
            
            account.IsFreezed = true;
            _dataBase.SaveBankAccount(Name, account);
        }

        public void UnfreezeAccount(uint accountId)
        {
            BankAccount account = TryGetBankAccount(accountId);
            
            account.IsFreezed = false;
            _dataBase.SaveBankAccount(Name, account);
        }
        // END CLIENT BANK FUNCTIONALITY
        
        // COMPANY BANK FUNCTIONALITY
        public uint CreateBankAccount(string password, Company company)
        {
            uint id = _bankAccounts.Keys.Max() + 1;
            BankAccount newBankAccount = new CompanyAccount(id, password, company.Name);
            _bankAccounts.Add(id, newBankAccount);
            
            _dataBase.SaveBankAccount(Name, newBankAccount);
            
            return id;
        }

        public uint SubmitForPaymentProject(Company company)
        {
            uint projectId = _paymentProjects.Count == 0 ? 1 : _paymentProjects.Keys.Max() + 1;
            PaymentProject paymentProject = new PaymentProject(company);
            _paymentProjects.Add(projectId, paymentProject);
            
            _dataBase.SaveProject(Name, paymentProject);

            return projectId;
        }

        public uint MakeTransfer(Company company, string toBankName, uint toAccountId, float cash)
        {
            CompanyAccount fromAccount = TryGetCompanyAccount(company);
            
            Bank toBank = DataBase.GetInstance().GetBank(toBankName);

            if (toBank is null)
                throw new NoSuchBankException();
            
            BankAccount toAccount = toBank.TryGetBankAccount(toAccountId);

            if (cash <= 0)
                throw new InvalidExpressionException();

            uint transferId = GenerateActionId();
            Transfer transfer = new Transfer(transferId, fromAccount, toAccount, cash);
            transfer.Withdraw();
            _actions.Add(transferId, transfer);
            
            _dataBase.SaveTransfer(Name, transfer);

            return transferId;
        }
        // END COMPANY BANK FUNCTIONALITY

        // ADMIN BANK FUNCTIONALITY
        public string GetLogs()
        {
            return _logger.GetLogs();
        }

        void IAdminBank.RevertAction(uint actionId)
        {
            if (!IsActionExist(actionId))
                throw new NoSuchActionException();

            BankAction action = _actions[actionId];
            
            action.Revert();
            
            Type actionType = action.GetType();

            if(actionType == typeof(Credit))
                _dataBase.SaveCredit(Name, (Credit) action);
            else if(actionType == typeof(Duty))
                _dataBase.SaveDuty(Name, (Duty) action);
            else if(actionType == typeof(Transfer))
                _dataBase.SaveTransfer(Name, (Transfer) action);
        }
        // END ADMIN BANK FUNCTIONALITY
        
        // OPERATOR BANK FUNCTIONALITY
        public string GetStatistics()
        {
            string result = "";

            if (_actions.Count > 0)
            {
                result += "Actions: \n";
                foreach (var action in _actions)
                {
                    result += $"{action.Key}. {action.Value.ToString()}\n";
                }
            }

            if (_paymentProjects.Count > 0)
            {
                result += "Payment projects: \n";
                foreach (var project in _paymentProjects)
                {
                    result += $"{project.Key}. {project.Value.ToString()}\n";
                }
            }

            return result;
        }

        public void AcceptCompanyProject(uint companyProjectId)
        {
            if (!_paymentProjects.ContainsKey(companyProjectId))
                throw new NoSuchProjectException();
            
            PaymentProject project = _paymentProjects[companyProjectId];
            
            project.Accept();
            
            _dataBase.SaveProject(Name, project);
        }

        void IOperatorBank.RevertAction(uint actionId)
        {
            if (!IsActionExist(actionId))
                throw new NoSuchActionException();

            BankAction action = _actions[actionId];
            
            action.Revert();

            Type actionType = action.GetType();

            if(actionType == typeof(Credit))
                _dataBase.SaveCredit(Name, (Credit) action);
            else if(actionType == typeof(Duty))
                _dataBase.SaveDuty(Name, (Duty) action);
            else if(actionType == typeof(Transfer))
                _dataBase.SaveTransfer(Name, (Transfer) action);
        }
        // END OPERATOR BANK FUNCTIONALITY
        
        // MANAGER BANK FUNCTIONALITY
        public void AcceptDuty(uint dutyId)
        {
            if (!IsActionExist(dutyId))
                throw new NoSuchActionException();

            Duty duty;

            try
            {
                duty = (Duty) _actions[dutyId];
            }
            catch (InvalidCastException e)
            {
                throw new InvalidCastActionException();
            }
            
            duty.Accept();
            
            _dataBase.SaveDuty(Name, duty);
        }

        public void BlockAccount(uint accountId)
        {
            BankAccount account = TryGetBankAccount(accountId);
            account.IsBlocked = true;
            
            _dataBase.SaveBankAccount(Name, account);
        }
        
        public void UnblockAccount(uint accountId)
        {
            BankAccount account = TryGetBankAccount(accountId);
            account.IsBlocked = false;
            
            _dataBase.SaveBankAccount(Name, account);
        }
        // END MANAGER BANK FUNCTIONALITY
        
        private uint GenerateActionId()
        {
            if (_actions.Count == 0)
                return 1;
            return _actions.Keys.Max() + 1;
        }

        private bool IsActionExist(uint actionId) =>
            _actions.ContainsKey(actionId);

        private BankAccount TryGetBankAccount(uint id)
        {
            if (!_bankAccounts.ContainsKey(id))
                throw new NoSuchAccountException();

            return _bankAccounts[id];
        }

        private CompanyAccount TryGetCompanyAccount(Company company)
        {
            foreach (var account in _bankAccounts)
            {
                try
                {
                    CompanyAccount companyAccount = (CompanyAccount) account.Value;

                    if (companyAccount.CompanyName == company.Name)
                        return companyAccount;
                }
                catch (InvalidCastException)
                {
                }
            }

            throw new NoSuchAccountException();
        }
    }
}