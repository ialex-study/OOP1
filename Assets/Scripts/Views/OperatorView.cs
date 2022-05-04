using Models.CompanySystem;
using ObserverSystem;
using TMPro;
using UnityEngine;
using ViewModels;

namespace Views
{
    public class OperatorView : AuthorizedView, IObserver
    {
        [SerializeField] private TMP_Text logsText;
        [SerializeField] private TMP_InputField revertActionField;
        [SerializeField] private TMP_InputField paymentProjectField;

        private OperatorVM _operatorVM;

        public void RevertAction()
        {
            uint actionId = uint.Parse(revertActionField.text);
            
            _operatorVM.Bank.RevertAction(actionId);
        }

        public void AcceptPaymentProject()
        {
            uint projectId = uint.Parse(paymentProjectField.text);
            
            _operatorVM.Bank.AcceptCompanyProject(projectId);
        }

        public override void Login(Company company)
        {
            _operatorVM.Login(company);
            gameObject.SetActive(true);
        }

        public override void Logout()
        {
            _operatorVM.Logout();
            gameObject.SetActive(false);
        }

        void IObserver.Update()
        {
            logsText.text = _operatorVM.Bank.GetStatistics();
        }

        private void Start()
        {
            _operatorVM = new OperatorVM();
            _operatorVM.Subscribe(this);
        }
    }
}