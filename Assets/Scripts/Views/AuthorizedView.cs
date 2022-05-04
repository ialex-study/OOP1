using Models.CompanySystem;
using Models.Interfaces;
using UnityEngine;

namespace Views
{
    public abstract class AuthorizedView : MonoBehaviour
    {
        public abstract void Login(Company company);
        public abstract void Logout();
    }
}