using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using JetBrains.Annotations;
using Models.BankSystem;
using Models.CompanySystem;
using Models.Enums;
using Models.Interfaces;
using Models.UserSystem;
using Logger = Models.BankSystem.Logger;

namespace Models
{
    public class DataBase : IDataBase
    {
        private static DataBase _instance;
        private readonly XDocument _document;
        private readonly XElement _root;
        private readonly XElement _banks;
        private readonly XElement _clients;
        private readonly XElement _companies;
        private readonly string _filename = "database.xml";
        private readonly Queue<Action> _commandsPull = new Queue<Action>();
        private readonly Thread _executingThread;
        private object _queueLocker = new object();

        public static IDataBase GetInstance() =>
            _instance ??= new DataBase();

        public void SaveBank(Bank bank)
        {
            AddToPull(() => PullSaveBank(bank));
        }

        public void SaveBankAccount(string bankName, BankAccount bankAccount)
        {
            AddToPull(() => PullSaveBankAccount(bankName, bankAccount));
        }

        public void SaveDuty(string bankName, Duty duty)
        {
            AddToPull(() => PullSaveDuty(bankName, duty));
        }

        public void SaveCredit(string bankName, Credit credit)
        {
            AddToPull(() => PullSaveCredit(bankName, credit));
        }

        public void SaveTransfer(string bankName, Transfer transfer)
        {
            AddToPull(() => PullSaveTransfer(bankName, transfer));
        }

        public void SaveClient(Client client)
        {
            AddToPull(() => PullSaveClient(client));
        }

        public void SaveCompany(Company company)
        {
            AddToPull(() => PullSaveCompany(company));
        }

        public void SaveEmployee(string companyName, Employee employee)
        {
            AddToPull(() => PullSaveEmployee(companyName, employee));
        }

        public void SaveProject(string companyName, PaymentProject paymentProject)
        {
            AddToPull(() => PullSaveProject(companyName, paymentProject));
        }

        public void SaveUser(string login, string password, Company company, ClientType type)
        {
            AddToPull(() => PullSaveUser(login, password, company, type));
        }

        [CanBeNull]
        public Bank GetBank(string name)
        {
            XElement bankElement = GetElementByAttribute(_banks, "name", name);

            if (bankElement is null)
                return null;

            Dictionary<uint, BankAccount> accounts = new Dictionary<uint, BankAccount>();
            foreach (var account in bankElement.Elements("accounts").Elements())
            {
                uint id = uint.Parse(account.Attribute("id").Value);

                accounts.Add(id, GetBankAccount(name, id));
            }

            Dictionary<uint, PaymentProject> paymentProjects = new Dictionary<uint, PaymentProject>();
            foreach (var project in bankElement.Elements("projects").Elements())
            {
                uint id = uint.Parse(project.Attribute("id").Value);

                paymentProjects.Add(id, GetProject(name, id));
            }

            Dictionary<uint, BankAction> actions = new Dictionary<uint, BankAction>();
            foreach (var action in bankElement.Elements("actions").Elements())
            {
                uint id = uint.Parse(action.Attribute("id").Value);
                string type = action.Attribute("type").Value;

                if (type == nameof(Credit))
                {
                    actions.Add(id, GetCredit(name, id));
                }
                else if (type == nameof(Duty))
                {
                    actions.Add(id, GetDuty(name, id));
                }
                else if (type == nameof(Transfer))
                {
                    actions.Add(id, GetTransfer(name, id));
                }
            }

            string address = GetCompanyAddress(name);

            Bank bank = new Bank(name, address, accounts, paymentProjects, actions, new Logger());

            foreach (var key in bank.BankActions.Keys.ToList())
            {
                BankAction action = bank.BankActions[key];
                Type actionType = action.GetType();

                BankAction newAction = null;

                if (actionType == typeof(Credit))
                {
                    Credit credit = (Credit) action;
                    newAction = new Credit(key, bank.BankAccounts[credit.BankAccount.Id], credit.Cash, credit.DutyPeriod, credit.Percent,
                        credit.IsAccepted, credit.IsReverted);
                }
                else if (actionType == typeof(Duty))
                {
                    Duty duty = (Duty) action;
                    newAction = new Duty(key, bank.BankAccounts[duty.BankAccount.Id], duty.Cash, duty.DutyPeriod, duty.IsAccepted, duty.IsReverted);
                }
                else if (actionType == typeof(Transfer))
                {
                    Transfer transfer = (Transfer) action;
                    newAction = new Transfer(key, bank.BankAccounts[transfer.From.Id], transfer.To, transfer.Cash, transfer.IsAccepted,
                        transfer.IsReverted);
                }

                bank.BankActions[key] = newAction;
            }

            return bank;
        }

        [CanBeNull]
        public BankAccount GetBankAccount(string bankName, uint id)
        {
            XElement bankElement = GetElementByAttribute(_banks, "name", bankName);

            if (bankElement is null)
                throw new ArgumentNullException();
            
            XElement accounts = bankElement.Element("accounts");

            XElement accountElement = GetElementByAttribute(accounts, "id", id.ToString());

            if (accountElement is null)
                return null;

            string password = accountElement.Element("password").Value;
            float cash = float.Parse(accountElement.Element("cash").Value);
            string companyName = accountElement.Element("companyName")?.Value;
            
            if(companyName is null)
                return new BankAccount(id, password, cash);

            return new CompanyAccount(id, password, companyName);
        }

        [CanBeNull]
        public Duty GetDuty(string bankName, uint id)
        {
            XElement bankElement = GetElementByAttribute(_banks, "name", bankName);

            if (bankElement is null)
                throw new ArgumentNullException();
            
            XElement actions = bankElement.Element("actions");

            XElement dutyElement = GetElementByAttribute(actions, "id", id.ToString());

            if (dutyElement is null)
                return null;

            BankAccount bankAccount = GetBankAccount(bankName, uint.Parse(dutyElement.Element("bankAccount").Value));
            float cash = float.Parse(dutyElement.Element("cash").Value);

            DutyPeriod period;
            DutyPeriod.TryParse(dutyElement.Element("period").Value, out period);

            bool isAccepted = bool.Parse(dutyElement.Element("accepted").Value);
            bool isReverted = bool.Parse(dutyElement.Element("reverted").Value);

            return new Duty(id, bankAccount, cash, period, isAccepted, isReverted);
        }

        [CanBeNull]
        public Credit GetCredit(string bankName, uint id)
        {
            XElement bankElement = GetElementByAttribute(_banks, "name", bankName);

            if (bankElement is null)
                throw new ArgumentNullException();
            
            XElement actions = bankElement.Element("actions");
            
            XElement creditElement = GetElementByAttribute(actions, "id", id.ToString());

            if (creditElement is null)
                return null;

            BankAccount bankAccount = GetBankAccount(bankName, uint.Parse(creditElement.Element("bankAccount").Value));
            float cash = float.Parse(creditElement.Element("cash").Value);
            float percent = float.Parse(creditElement.Element("percent").Value);

            DutyPeriod period;
            DutyPeriod.TryParse(creditElement.Element("period").Value, out period);

            bool isAccepted = bool.Parse(creditElement.Element("accepted").Value);
            bool isReverted = bool.Parse(creditElement.Element("reverted").Value);

            return new Credit(id, bankAccount, cash, period, percent, isAccepted, isReverted);
        }

        [CanBeNull]
        public Transfer GetTransfer(string bankName, uint id)
        {
            XElement bankElement = GetElementByAttribute(_banks, "name", bankName);

            if (bankElement is null)
                throw new ArgumentNullException();

            XElement actions = bankElement.Element("actions");
            
            XElement transferElement = GetElementByAttribute(actions, "id", id.ToString());

            if (transferElement is null)
                return null;

            BankAccount from = GetBankAccount(bankName, uint.Parse(transferElement.Element("from").Value));
            BankAccount to = GetBankAccount(bankName, uint.Parse(transferElement.Element("to").Value));
            float cash = float.Parse(transferElement.Element("cash").Value);

            bool isAccepted = bool.Parse(transferElement.Element("accepted").Value);
            bool isReverted = bool.Parse(transferElement.Element("reverted").Value);

            return new Transfer(id, from, to, cash, isAccepted, isReverted);
        }

        [CanBeNull]
        public Client GetClient(uint id)
        {
            XElement clientElement = GetElementByAttribute(_clients, "id", id.ToString());

            if (clientElement is null)
                return null;

            string name = clientElement.Element("name").Value;
            string surname = clientElement.Element("surname").Value;
            string email = clientElement.Element("email").Value;
            string phone = clientElement.Element("phone").Value;

            uint passportId = uint.Parse(clientElement.Element("passportId").Value);
            string seria = clientElement.Element("seria").Value;
            string number = clientElement.Element("number").Value;

            Passport passport = new Passport(passportId, seria, number);
            
            return new Client(name, surname, email, phone, passport);
        }

        [CanBeNull]
        public Company GetCompany(string name)
        {
            XElement companyElement = GetElementByAttribute(_companies, "name", name);

            if (companyElement is null)
                return null;

            string type = companyElement.Element("type").Value;
            string address = companyElement.Element("address").Value;
            string bankName = companyElement.Element("bankName")?.Value;

            List<Employee> employees = new List<Employee>();

            foreach (var element in companyElement.Element("employees").Elements())
            {
                employees.Add(GetEmployee(name, uint.Parse(element.Value)));
            }

            return new Company(type, name, address, bankName, employees);
        }

        [CanBeNull]
        public Employee GetEmployee(string companyName, uint id)
        {
            Client employeeData = GetClient(id);

            BankAccount account = GetBankAccount(GetElementByAttribute(_companies, "name", companyName).Element("bankName").Value, id);

            return new Employee(employeeData.Name, employeeData.Surname, employeeData.Email, employeeData.Phone,
                employeeData.Passport, account);
        }

        public List<Bank> GetBanks()
        {
            List<Bank> result = new List<Bank>();

            foreach (var element in _banks.Elements())
            {
                result.Add(GetBank(element.Attribute("name").Value));
            }

            return result;
        }
        
        public List<Company> GetCompanies()
        {
            List<Company> result = new List<Company>();

            foreach (var element in _companies.Elements())
            {
                result.Add(GetCompany(element.Attribute("name").Value));
            }

            return result;
        }

        [CanBeNull]
        public PaymentProject GetProject(string bankName, uint id)
        {
            XElement bankElement = GetElementByAttribute(_banks, "name", bankName);

            if (bankElement is null)
                throw new ArgumentNullException();

            XElement projects = bankElement.Element("projects");
            
            XElement projectElement = GetElementByAttribute(projects, "id", id.ToString());

            if (projectElement is null)
                return null;

            Company company = GetCompany(projectElement.Element("company").Value);
            bool accepted = bool.Parse(projectElement.Element("accepted").Value);

            return new PaymentProject(company, accepted);
        }

        public (string, ClientType) GetUser(string login, Company company)
        {
            XElement companyElement = GetElementByAttribute(_companies, "name", company.Name);

            if (companyElement is null)
                throw new ArgumentNullException();
            
            XElement userElement = GetElementByAttribute(companyElement, "login", login);

            if (userElement is null)
                return (null, ClientType.None);

            string password = userElement.Element("password").Value;
            ClientType type;
            ClientType.TryParse(userElement.Element("type").Value, out type);

            return (password, type);
        }

        private DataBase()
        {
            try
            {
                _document = XDocument.Load(_filename);
                _root = _document.Element("root");
            }
            catch (XmlException)
            {
                _document = new XDocument();
                _root = new XElement("root");
                _document.Add(_root);
                
                _root.Add(new XElement("banks"));
                _root.Add(new XElement("clients"));
                _root.Add(new XElement("companies"));
            }

            _banks = _root.Element("banks");
            _clients = _root.Element("clients");
            _companies = _root.Element("companies");

            _executingThread = new Thread(ExecuteCommandPull);
            _executingThread.Start();
        }

        ~DataBase()
        {
            lock (_queueLocker)
            {
                _executingThread.Abort();
                
                while(_commandsPull.Count > 0)
                    _commandsPull.Dequeue()();
                
                SaveData();
            }
        }

        private void SaveData()
        {
            _document.Save(_filename);
        }

        private void AddToPull(Action command)
        {
            lock (_queueLocker)
            {
                _commandsPull.Enqueue(command);
            }
        }

        private void ExecuteCommandPull()
        {
            while (true)
            {
                if (_commandsPull.Count == 0)
                {
                    Thread.Sleep(5000);
                }
                else
                {
                    lock (_queueLocker)
                        while(_commandsPull.Count > 0)
                            _commandsPull.Dequeue()();
                    
                    SaveData();
                }
            }
        }

        [CanBeNull]
        private XElement GetElementByAttribute(XElement root, string attributeName, string attributeValue)
        {
            foreach (var element in root.Elements())
            {
                if (element.Attribute(attributeName)?.Value == attributeValue)
                    return element;
            }

            return null;
        }

        [CanBeNull]
        private XElement GetElementByValue(XElement root, string value)
        {
            foreach (var element in root.Elements())
            {
                if (element.Value == value)
                    return element;
            }

            return null;
        }

        private XElement SaveAction(string bankName, BankAction action)
        {
            XElement bankElement = GetElementByAttribute(_banks, "name", bankName);

            if (bankElement is null)
                throw new ArgumentNullException();

            XElement actions = bankElement.Element("actions");
            
            XElement actionElement = GetElementByAttribute(actions, "id", action.Id.ToString());

            if (actionElement is null)
            {
                actionElement = new XElement("action");
                actions.Add(actionElement);

                actionElement.Add(new XAttribute("id", action.Id));
            }

            actionElement.SetElementValue("accepted", action.IsAccepted);
            actionElement.SetElementValue("reverted", action.IsReverted);

            return actionElement;
        }

        private string GetCompanyAddress(string companyName)
        {
            XElement companyElement = GetElementByAttribute(_companies, "name", companyName);

            return companyElement?.Element("address").Value;
        }

        private void PullSaveBank(Bank bank)
        {
            PullSaveCompany(bank);
            string bankName = bank.Name;
            
            XElement bankElement = GetElementByAttribute(_banks, "name", bank.Name);

            if (bankElement is null)
            {
                bankElement = new XElement("bank");
                _banks.Add(bankElement);

                bankElement.Add(new XAttribute("name", bank.Name));
                
                bankElement.Add(new XElement("accounts"));
                bankElement.Add(new XElement("projects"));
                bankElement.Add(new XElement("actions"));
            }

            foreach (var pair in bank.BankAccounts)
            {
                PullSaveBankAccount(bankName, pair.Value);
            }

            foreach (var pair in bank.PaymentProjects)
            {
                PullSaveProject(bankName, pair.Value);
            }

            foreach (var pair in bank.BankActions)
            {
                Type actionType = pair.Value.GetType();

                if (actionType == typeof(Credit))
                {
                    PullSaveCredit(bankName, (Credit) pair.Value);
                }
                else if (actionType == typeof(Duty))
                {
                    PullSaveDuty(bankName, (Duty) pair.Value);
                }
                else if (actionType == typeof(Transfer))
                {
                    PullSaveTransfer(bankName, (Transfer) pair.Value);
                }
            }
        }

        private void PullSaveBankAccount(string bankName, BankAccount bankAccount)
        {
            XElement bankElement = GetElementByAttribute(_banks, "name", bankName);

            if (bankElement is null)
                throw new ArgumentNullException();

            XElement accounts = bankElement.Element("accounts");
            
            XElement accountElement = GetElementByAttribute(accounts, "id", bankAccount.Id.ToString());

            if (accountElement is null)
            {
                accountElement = new XElement("account");
                accounts.Add(accountElement);

                accountElement.Add(new XAttribute("id", bankAccount.Id));
            }

            accountElement.SetElementValue("password", bankAccount.Password);
            accountElement.SetElementValue("cash", bankAccount.Cash);
            
            if(bankAccount.GetType() == typeof(CompanyAccount))
                accountElement.SetElementValue("companyName", ((CompanyAccount) bankAccount).CompanyName);
        }

        private void PullSaveDuty(string bankName, Duty duty)
        {
            XElement dutyElement = SaveAction(bankName, duty);

            dutyElement.SetAttributeValue("type", nameof(Duty));
            dutyElement.SetElementValue("bankAccount", duty.BankAccount.Id);
            dutyElement.SetElementValue("cash", duty.Cash);
            dutyElement.SetElementValue("period", duty.DutyPeriod);
            
            PullSaveBankAccount(bankName, duty.BankAccount);
        }

        private void PullSaveCredit(string bankName, Credit credit)
        {
            PullSaveDuty(bankName, credit);
            
            XElement actions = GetElementByAttribute(_banks, "name", bankName).Element("actions");

            XElement creditElement = GetElementByAttribute(actions, "id", credit.Id.ToString());

            creditElement.SetAttributeValue("type", nameof(Credit));
            creditElement.SetElementValue("percent", credit.Percent);
            
            PullSaveBankAccount(bankName, credit.BankAccount);
        }

        private void PullSaveTransfer(string bankName, Transfer transfer)
        {
            XElement transferElement = SaveAction(bankName, transfer);

            transferElement.SetAttributeValue("type", nameof(Transfer));
            transferElement.SetElementValue("from", transfer.From.Id);
            transferElement.SetElementValue("to", transfer.To.Id);
            transferElement.SetElementValue("cash", transfer.Cash);
            
            PullSaveBankAccount(bankName, transfer.From);
            PullSaveBankAccount(bankName, transfer.To);
        }

        private XElement PullSaveClient(Client client)
        {
            XElement clientElement = GetElementByAttribute(_clients, "id", client.Passport.Id.ToString());

            if (clientElement is null)
            {
                clientElement = new XElement("client");
                _clients.Add(clientElement);

                clientElement.Add(new XAttribute("id", client.Passport.Id));
            }

            clientElement.SetElementValue("name", client.Name);
            clientElement.SetElementValue("surname", client.Surname);
            clientElement.SetElementValue("email", client.Email);
            clientElement.SetElementValue("phone", client.Phone);
            clientElement.SetElementValue("passportId", client.Passport.Id);
            clientElement.SetElementValue("seria", client.Passport.Seria);
            clientElement.SetElementValue("number", client.Passport.Number);

            return clientElement;
        }

        private void PullSaveCompany(Company company)
        {
            XElement companyElement = GetElementByAttribute(_companies, "name", company.Name);

            if (companyElement is null)
            {
                companyElement = new XElement("company");
                _companies.Add(companyElement);

                companyElement.Add(new XAttribute("name", company.Name));
                companyElement.Add(new XElement("employees"));
            }

            companyElement.SetElementValue("type", company.Type);
            companyElement.SetElementValue("address", company.Address);
            companyElement.SetElementValue("bankName", company.BankName);

            XElement employees = companyElement.Element("employees");
            foreach (var employee in company.Employees)
            {
                if (GetElementByValue(employees, employee.Passport.Id.ToString()) is null)
                    employees.Add(new XElement("employee", employee.Passport.Id));

                PullSaveEmployee(company.Name, employee);
            }
        }

        private void PullSaveEmployee(string companyName, Employee employee)
        {
            XElement employeeElement = PullSaveClient(employee);

            employeeElement.SetElementValue("bankAccount", employee.BankAccount.Id);
        }

        private void PullSaveProject(string bankName, PaymentProject paymentProject)
        {
            XElement bankElement = GetElementByAttribute(_banks, "name", bankName);

            if (bankElement is null)
                throw new ArgumentNullException();

            XElement projects = bankElement.Element("projects");
            
            XElement projectElement = GetElementByAttribute(projects, "id", paymentProject.Id.ToString());

            if (projectElement is null)
            {
                projectElement = new XElement("project");
                projects.Add(projectElement);

                projectElement.Add(new XAttribute("id", paymentProject.Id));
            }

            projectElement.SetElementValue("accepted", paymentProject.IsAccepted);
            projectElement.SetElementValue("company", paymentProject.Company.Name);
        }

        private void PullSaveUser(string login, string password, Company company, ClientType type)
        {
            XElement companyElement = GetElementByAttribute(_companies, "name", company.Name);

            if (companyElement is null)
                throw new ArgumentNullException();
            
            XElement userElement = GetElementByAttribute(companyElement, "login", login);

            if (userElement is null)
            {
                userElement = new XElement("user");
                companyElement.Add(userElement);
                
                userElement.Add(new XAttribute("login", login));
            }
            
            userElement.SetElementValue("password", password);
            userElement.SetElementValue("type", type);
        }
    }
}