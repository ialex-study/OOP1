using Models.CompanySystem;

namespace Models.BankSystem
{
    public class PaymentProject
    {
        private uint _id;
        private readonly Company _company;
        private bool _accepted = false;

        public uint Id => _id;
        public Company Company => _company;
        public bool IsAccepted => _accepted;

        public PaymentProject(Company company, bool isAccepted = false) =>
            (_company, _accepted) = (company, isAccepted);

        public void Accept()
        {
            _accepted = true;
        }

        public override string ToString()
        {
            return $"Company name: {Company.Name}";
        }
    }
}