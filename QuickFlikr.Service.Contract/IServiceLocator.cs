namespace QuickFlikr.Service.Contract
{
    public interface IServiceLocator
    {
        public TService GetService<TService>();

        public void Register<TContract>();

        public void RegisterSingleton<TContract, TImplementation>() where TImplementation : TContract;

        public void RegisterSingleton<TContract>(Func<TContract> factory);
    }
}