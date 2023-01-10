using Autofac;
using DocumentsStorage.Core.Interfaces;
using DocumentsStorage.Core.Services;

namespace DocumentsStorage.Core;

public class DefaultCoreModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<DocumentsService>()
            .As<IDocumentsService>().InstancePerLifetimeScope();
    }
}