using Models.Interfaces;

namespace Models.UserSystem
{
    public class Admin
    {
        protected IAdminBank _bank;

        public Admin(IAdminBank bank) =>
            _bank = bank;

        public string GetLogs()
        {
            return _bank.GetLogs();
        }

        public void RevertAction(uint actionId)
        {
            _bank.RevertAction(actionId);
        }
    }
}