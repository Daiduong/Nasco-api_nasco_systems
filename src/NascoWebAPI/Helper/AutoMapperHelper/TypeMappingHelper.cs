using AutoMapper;
using System.Collections.Generic;


namespace NascoWebAPI.Helper.AutoMapperHelper
{
    public static class TypeMappingHelper
    {
        private static ISet<TDestination> ToISet<TSource, TDestination>(IEnumerable<TSource> source)
        {
            ISet<TDestination> set = null;
            if (source != null)
            {
                set = new HashSet<TDestination>();

                foreach (TSource item in source)
                {
                    set.Add(Mapper.Map<TSource, TDestination>(item));
                }
            }
            return set;
        }
    }
}
