namespace Models.Interfaces
{
    public interface IOperatorBank
    {
        public string GetStatistics();
        public void RevertAction(uint actionId);
        public void AcceptCompanyProject(uint companyProjectId);
    }
}