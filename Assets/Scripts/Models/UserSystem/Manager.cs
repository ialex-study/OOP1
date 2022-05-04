using Models.Interfaces;

namespace Models.UserSystem
{
    public class Manager
    {
        private IManagerBank _bank;
        
        public Manager(IManagerBank bank) =>
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

        public void AcceptDuty(uint dutyId)
        {
            _bank.AcceptDuty(dutyId);
        }
    }
}