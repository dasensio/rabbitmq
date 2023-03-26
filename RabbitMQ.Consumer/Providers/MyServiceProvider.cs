using Microsoft.Extensions.DependencyInjection;

public class MyServiceProvider : IServiceProvider
{
    private readonly IServiceCollection _services;

    public MyServiceProvider(IServiceCollection services)
    {
        _services = services;
    }

    public object? GetService(Type serviceType)
    {
        var descriptor = _services.FirstOrDefault(d => d.ServiceType == serviceType);

        if (descriptor == null)
        {
            return null;
        }

        if (descriptor.ImplementationInstance != null)
        {
            return descriptor.ImplementationInstance;
        }

        if (descriptor.ImplementationType != null)
        {
            return Activator.CreateInstance(descriptor.ImplementationType);
        }

        if (descriptor.ImplementationFactory != null)
        {
            return descriptor.ImplementationFactory(this);
        }

        throw new InvalidOperationException($"Cannot create service for type {serviceType}");
    }
}