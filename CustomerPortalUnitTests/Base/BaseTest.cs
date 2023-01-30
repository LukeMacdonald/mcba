using Autofac;
using IContainer = System.ComponentModel.IContainer;

namespace CustomerPortalUnitTests.Base;

// THIS CODE CAME FROM LECTURES GIVEN
public abstract class BaseTest : IDisposable
{
    private static readonly object ContainerLock = new();

    private IContainer _container;
    protected ContainerBuilder Builder { get; }

    protected BaseTest()
    {
        Builder = new ContainerBuilder();
    }

    protected IContainer Container
    {
        get
        {
            // If the container is null it is built.
            if(_container == null)
                BuildContainer();

            return _container;
        }
    }

    private void BuildContainer()
    {
        lock(ContainerLock)
        {
            _container ??= Builder.Build();
        }
    }

    public virtual void Dispose() => Container.Dispose();
}