using Models.CompanySystem;
using ObserverSystem;
using TMPro;
using UnityEngine;
using ViewModels;

namespace Views
{
    public class ManagerView : AuthorizedView, IObserver
    {
        [SerializeField] private TMP_Text logsText;
        [SerializeField] private TMP_InputField revertActionField;
        [SerializeField] private TMP_InputField paymentProjectField;
        [SerializeField] private TMP_InputField dutyIdField;

        private ManagerVM _managerVM;

        public void RevertAction()
        {
            uint actionId = uint.Parse(revertActionField.text);
            
            _managerVM.Bank.RevertAction(actionId);
        }

        public void AcceptPaymentProject()
        {
            uint projectId = uint.Parse(paymentProjectField.text);
            
            _managerVM.Bank.AcceptCompanyProject(projectId);
        }

        public void AcceptDuty()
        {
            uint dutyId = uint.Parse(dutyIdField.text);

            _managerVM.Bank.AcceptDuty(dutyId);
        }

        public override void Login(Company company)
        {
            _managerVM.Login(company);
            gameObject.SetActive(true);
        }

        public override void Logout()
        {
            _managerVM.Logout();
            gameObject.SetActive(false);
        }

        void IObserver.Update()
        {
            logsText.text = _managerVM.Bank.GetStatistics();
        }

        private void Start()
        {
            _managerVM = new ManagerVM();
            _managerVM.Subscribe(this);
        }
    }
}