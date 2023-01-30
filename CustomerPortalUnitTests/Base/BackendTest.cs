using Autofac;
using CustomerPortalUnitTests.Modules;

namespace CustomerPortalUnitTests.Base;

// THIS CODE CAME FROM LECTURES GIVEN
public abstract class BackendTest : BaseTest
{
    protected BackendTest() => Builder.RegisterModule<BackendModule>();
}