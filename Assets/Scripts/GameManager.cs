/*
using System.Collections.Generic;
using Models;
using Models.BankSystem;
using Models.CompanySystem;
using Models.Enums;
using Models.Interfaces;
using Models.UserSystem;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private void Awake()
    {
        Debug.Log("Hello World");
        IDataBase db = DataBase.GetInstance();

        Dictionary<uint, BankAccount> bankAccounts = new Dictionary<uint, BankAccount>();

        for (int i = 1; i <= 100; i++)
        {
            bankAccounts.Add((uint) i, new BankAccount((uint) i, "qwerty" + i));
        }
        Bank bank1 = new Bank("Беларус", "Somewhere", bankAccounts, null, null, null);
        Bank bank2 = new Bank("Русский", "Somewhere2", bankAccounts, null, null, null);
        Bank bank3 = new Bank("Пумпум", "Somewhere3", bankAccounts, null, null, null);

        Company company1 = new Company("OOO", "Name1", "somewhere1", bank1.Name);
        Company company2 = new Company("OAO", "Name2", "somewhere2", bank1.Name);
        Company company3 = new Company("OBO", "Name3", "somewhere3", bank1.Name);
        Company company4 = new Company("OCO", "Name4", "somewhere4", bank1.Name);
        Company company5 = new Company("ODO", "Name5", "somewhere1", bank1.Name);

        for (int i = 1; i <= 100; i++)
        {
            db.SaveClient(new Client("clientName" + i, "clientSurname" + i, "clientEmail" + i, "phone" + i,
                new Passport((uint) i, "MC", i.ToString() + i.ToString())));
        }

        for (uint i = 101; i <= 200; i++)
        {
            BankAccount account = new CompanyAccount(i, "pass1", company1.Name);
            Employee employee = new Employee("name", "surname", "emaso", "12345",
                new Passport(i, "MC", i.ToString() + i.ToString()), account);
            bank1.BankAccounts.Add(i, account);
            company1.HireEmployee(employee);
            
        }
        
        db.SaveBank(bank1);
        db.SaveBank(bank2);
        db.SaveBank(bank3);
        db.SaveCompany(company1);
        db.SaveCompany(company2);
        db.SaveCompany(company3);
        db.SaveCompany(company4);
        db.SaveCompany(company5);
        
        db.SaveUser("login1", "pass1", bank1, ClientType.Admin);
        db.SaveUser("login2", "pass1", bank1, ClientType.Operator);
        db.SaveUser("login3", "pass1", bank1, ClientType.Manager);
        db.SaveUser("login4", "pass1", company1, ClientType.Company);
    }
}
*/
