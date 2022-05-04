using System.Collections.Generic;

namespace ObserverSystem
{
    public class Observable : IObserverable
    {
        private readonly List<IObserver> _observers = new List<IObserver>();

        public void Notify()
        {
            foreach (var observer in _observers)
                observer.Update();
        }

        public void Subscribe(IObserver observer)
        {
            if(!_observers.Contains(observer))
                _observers.Add(observer);
        }

        public void Unsubscribe(IObserver observer)
        {
            if (_observers.Contains(observer))
                _observers.Remove(observer);
        }
    }
}