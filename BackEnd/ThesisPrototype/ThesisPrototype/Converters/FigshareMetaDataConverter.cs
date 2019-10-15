using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThesisPrototype.Models;
using ThesisPrototype.Models.Figshare;

namespace ThesisPrototype.Converters
{
    public static class FigshareMetaDataConverter
    {
        public static FigshareMetaData ToFigshareMetaData(this MetaData metaData)
        {
            return new FigshareMetaData()
            {
                title = metaData.title,
                is_confidential = !metaData.open_access,
                description = metaData.description,
                keywords = new string[] { "hello", "you" },
                authors = new FigshareAuthorName[] { new FigshareAuthorName() { name = metaData.author } },
                categories = new int[] { 1, 2 },
                defined_type = metaData.type,
                license = 1
            };
        }
    }
}
