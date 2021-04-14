using DWorldProject.Data.Entities;
using Nest;

namespace DWorldProject.Utils
{
    public static class ElasticSearchMapping
    {
        public static CreateIndexDescriptor LogMapping(this CreateIndexDescriptor descriptor)
        {
            return descriptor.Map<Log>(m => 
                m.Properties(p =>
                    p.Keyword(k =>
                        k.Name(n => n.Id))
                        .Text(t => t.Name(n => n.Name))));
        }
    }
}
