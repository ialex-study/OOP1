using Models.CompanySystem;
using ObserverSystem;
using TMPro;
using UnityEngine;
using ViewModels;

namespace Views
{
    public class AdminView : AuthorizedView, IObserver
    {
        [SerializeField] private TMP_Text logsText;
        [SerializeField] private TMP_InputField revertActionField;

        private AdminVM _adminVM;

        public void RevertAction()
        {
            uint actionId = uint.Parse(revertActionField.text);
            
            _adminVM.Bank.RevertAction(actionId);
        }

        public override void Login(Company company)
        {
            _adminVM.Login(company);
            gameObject.SetActive(true);
        }

        public override void Logout()
        {
            _adminVM.Logout();
            gameObject.SetActive(false);
        }

        void IObserver.Update()
        {
            logsText.text = _adminVM.Bank.GetLogs();
        }

        private void Start()
        {
            _adminVM = new AdminVM();
            _adminVM.Subscribe(this);
        }
    }
}