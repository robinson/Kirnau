﻿namespace Kirnau.SimulatedIssuer
{
    using System.Collections.Generic;
    using Microsoft.Practices.Unity;
    using Kirnau.Survey.Web.Shared;
    using Kirnau.Survey.Web.Shared.Helpers;
    using Kirnau.Survey.Web.Shared.Models;
    using Kirnau.Survey.Web.Shared.Stores;
    using Kirnau.Survey.Web.Shared.Stores.Azure;
    using Kirnau.Survey.Web.Shared.Stores.AzureStorage;

    public static class ContainerBootstraper
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Microsoft.DisposeObjectsBeforeLosingScope", Justification = "This container is used in the controller factory and cannot be disposed.")]
        public static void RegisterTypes(IUnityContainer container)
        {
            container.RegisterInstance(CloudConfiguration.GetStorageAccount("DataConnectionString"));

            container.RegisterInstance<IRetryPolicyFactory>(new DefaultRetryPolicyFactory());
            
            var cloudStorageAccountType = typeof(Microsoft.WindowsAzure.Storage.CloudStorageAccount);
            var retryPolicyFactoryProperty = new InjectionProperty("RetryPolicyFactory", typeof(IRetryPolicyFactory));

            container
                .RegisterType<IAzureBlobContainer<Tenant>, EntitiesBlobContainer<Tenant>>(
                    new InjectionConstructor(cloudStorageAccountType, AzureConstants.BlobContainers.Tenants),
                    retryPolicyFactoryProperty)
                .RegisterType<IAzureBlobContainer<byte[]>, FilesBlobContainer>(
                    new InjectionConstructor(cloudStorageAccountType, AzureConstants.BlobContainers.Logos, "image/jpeg"),
                    retryPolicyFactoryProperty);

            container.RegisterType<ITenantStore, TenantStore>();
        }
    }
}
