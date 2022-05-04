using System.Data;
using Models.CompanySystem;
using Models.Enums;
using Models.Interfaces;

namespace Models.UserSystem
{
    public static class Authorizer
    {
        
        public static (Company, ClientType) AuthorizeByCredentials(string login, string inputPassword, Company company)
        {
            IDataBase dataBase = DataBase.GetInstance();

            (string password, ClientType type) = dataBase.GetUser(login, company);

            if (password != inputPassword)
                return (null, ClientType.None);

            return (company, type);
        }

        public static Client AuthorizeByPassport(Passport passport)
        {
            Client client = DataBase.GetInstance().GetClient(passport.Id);
            return client;
        }
    }
}