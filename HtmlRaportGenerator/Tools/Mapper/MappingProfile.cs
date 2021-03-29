using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace HtmlRaportGenerator.Tools.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
            => ApplyMappingsFromAssembly(Assembly.GetExecutingAssembly());

        private void ApplyMappingsFromAssembly(Assembly assembly)
        {
            List<Type> types = assembly.GetExportedTypes()
            .Where(t => t.GetInterfaces()
                                            .Any(i =>
                                                        i.IsGenericType &&
                                                        i.GetGenericTypeDefinition() == typeof(IMapFrom<>)
                                                )
                    )
            .ToList();

            foreach (Type type in types)
            {
                object instance = Activator.CreateInstance(type)!;

                instance.CheckNotNull(nameof(instance));

                string mappingMethodName = nameof(IMapFrom<object>.Mapping);

                MethodInfo? methodInfo = type.GetMethod(mappingMethodName)
                    ?? type.GetInterface(typeof(IMapFrom<>).Name)!.GetMethod(mappingMethodName);

                methodInfo?.Invoke(instance, new object[] { this });

            }
        }
    }
}
