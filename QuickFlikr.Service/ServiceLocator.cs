using LightInject;
using QuickFlikr.Service.Contract;

namespace QuickFlikr.WinApp
{
    public sealed class ServiceLocator : IServiceLocator
    {
        #region Properties

        private readonly ServiceContainer _serviceContainer = new(new ContainerOptions
        {
            EnablePropertyInjection = false
        });

        #endregion Properties

        #region Methods

        public TService GetService<TService>()
        {
            return _serviceContainer.GetInstance<TService>();
        }

        public void Register<TContract>()
        {
            _serviceContainer.Register<TContract>();
        }

        public void RegisterSingleton<TContract, TImplementation>()
            where TImplementation : TContract
        {
            _serviceContainer.RegisterSingleton<TContract, TImplementation>();
        }

        public void RegisterSingleton<TContract>(Func<TContract> factory)
        {
            _serviceContainer.RegisterSingleton(_ => factory());
        }
        #endregion Methods
    }
}