using ThesisPrototype.Models;
using ThesisPrototype.Models.Dataverse;

namespace ThesisPrototype.Converters
{
    public static class DataverseMetaDataConverter
    {
        public static DataverseMetaData ToDataverseMetaData(this MetaData metaData)
        {
            DataverseMetaData result = new DataverseMetaData();

            result.AddField(new DataverseField()
            {
                value = metaData.title,
                typeClass = "primitive",
                multiple = false,
                typeName = "title"
            });

            result.AddField(new DataverseMultipleField()
            {
                value = new object[]
                {
                    new
                    {
                        authorName = new DataverseField()
                        {
                            value = metaData.author,
                            multiple = false,
                            typeClass = "primitive",
                            typeName="authorName"
                        }
                    }
                },
                typeClass = "compound",
                multiple = true,
                typeName = "author"
            });

            result.AddField(new DataverseMultipleField()
            {
                value = new object[]
                {
                    new
                    {
                        dsDescriptionValue = new DataverseField()
                        {
                            value = metaData.description,
                            multiple = false,
                            typeClass = "primitive",
                            typeName="dsDescriptionValue"
                        }
                    }
                },
                typeClass = "compound",
                multiple = true,
                typeName = "dsDescription"
            });

            return result;
        }
    }
}
