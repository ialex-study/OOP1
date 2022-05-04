namespace Models.Interfaces
{
    public interface IAdminBank
    {
        public string GetLogs();
        public void RevertAction(uint actionId);
    }
}