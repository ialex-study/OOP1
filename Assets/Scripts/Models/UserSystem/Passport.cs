namespace Models.UserSystem
{
    public class Passport
    {
        private readonly uint _id;
        private readonly string _seria;
        private readonly string _number;

        public uint Id => _id;
        public string Seria => _seria;
        public string Number => _number;

        public Passport(uint id, string seria, string number) =>
            (_id, _seria, _number) = (id, seria, number);
    }
}