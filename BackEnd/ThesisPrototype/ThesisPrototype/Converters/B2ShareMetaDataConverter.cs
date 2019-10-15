using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThesisPrototype.Models;

namespace ThesisPrototype.Converters
{
    public static class B2ShareMetaDataConverter
    {
        public static B2ShareMetaData ToB2ShareMetaData(this MetaData metaData)
        {
            return new B2ShareMetaData()
            {
                titles = new B2ShareTitle[] { new B2ShareTitle() { title = metaData.title } },
                open_access = metaData.open_access,
                contributors = metaData.contributors.Split(',').Select(x => 
                    new B2ShareContributor() { contributor_name = x, contributor_type = "Producer" })
                    .ToArray(),
                creators = new B2ShareCreator[] { new B2ShareCreator() { creator_name = metaData.author } },
                descriptions = new B2ShareDescription[] { new B2ShareDescription() { description = metaData.description, description_type = "Abstract" }  },
                resource_types = new B2ShareResourceType[] { new B2ShareResourceType() { resource_type = metaData.type, resource_type_general = "Software" } }
            };
        }
    }
}
