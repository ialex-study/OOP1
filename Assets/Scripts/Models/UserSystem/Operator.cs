using Models.Interfaces;

namespace Models.UserSystem
{
    public class Operator
    {
        private IOperatorBank _bank;

        public Operator(IOperatorBank bank) =>
            _bank = bank;

        public string GetStatistics()
        {
            return _bank.GetStatistics();
        }

        public void RevertAction(uint actionId)
        {
            _bank.RevertAction(actionId);
        }

        public void AcceptCompanyProject(uint companyProjectId)
        {
            _bank.AcceptCompanyProject(companyProjectId);
        }
    }
}