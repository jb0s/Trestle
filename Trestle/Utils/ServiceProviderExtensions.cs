using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Trestle.Utils
{
    /// <summary>
    /// https://stackoverflow.com/questions/33943876/how-do-i-see-all-services-that-a-net-iserviceprovider-can-provide
    /// </summary>
    public static class ServiceProviderExtensions
    {
        /// <summary>
        /// Get all registered <see cref="ServiceDescriptor"/>
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static Dictionary<Type, ServiceDescriptor> GetAllServiceDescriptors(this IServiceProvider provider)
        {
            if (provider is ServiceProvider serviceProvider)
            {
                var result = new Dictionary<Type, ServiceDescriptor>();

                var engine = serviceProvider.GetFieldValue("_engine");
                var callSiteFactory = engine.GetPropertyValue("CallSiteFactory");
                var descriptorLookup = callSiteFactory.GetFieldValue("_descriptorLookup");
                if (descriptorLookup is IDictionary dictionary)
                {
                    foreach (DictionaryEntry entry in dictionary)
                    {
                        result.Add((Type)entry.Key, (ServiceDescriptor)entry.Value.GetPropertyValue("Last"));
                    }
                }

                return result;
            }

            throw new NotSupportedException($"Type '{provider.GetType()}' is not supported!");
        }
    }
}