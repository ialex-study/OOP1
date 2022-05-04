namespace ObserverSystem
{
    public interface IObserverable
    {
        public void Notify();
        public void Subscribe(IObserver subject);
        public void Unsubscribe(IObserver subject);
    }
}