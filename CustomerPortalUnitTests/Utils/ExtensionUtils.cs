using Autofac;
using CustomerPortal.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CustomerPortalUnitTests.Utils;

// THIS CODE CAME FROM LECTURES GIVEN
public static class ExtensionUtils
{
    public static T ResolveController<T>(this IContainer container) where T :Controller
    {
        var controller = container.Resolve<T>();
        controller.ControllerContext.HttpContext = container.Resolve<HttpContext>();
        controller.ControllerContext.HttpContext.Session = container.Resolve<ISession>();

        return controller;

    }
    public static T ResolveControllerWithSeededData<T>(this IContainer container) where T :Controller
    {
        var controller = container.ResolveController<T>();
        var context = container.Resolve<MCBAContext>();
        SeedData.Initialize(context);
        return controller;
    }
    
}