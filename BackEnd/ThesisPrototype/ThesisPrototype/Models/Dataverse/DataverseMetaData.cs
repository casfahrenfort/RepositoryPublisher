using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ThesisPrototype.Models.Dataverse
{
    public class DataverseMetaData
    {
        public DataverseDatasetVersion datasetVersion;

        public DataverseMetaData()
        {
            datasetVersion = new DataverseDatasetVersion()
            {
                metadataBlocks = new DataverseMetaDataBlocks()
                {
                    citation = new DataverseCitation()
                    {
                        fields = new DataverseField[0]
                    }
                }
            };
        }

        public void AddField(DataverseField field)
        {
            datasetVersion.metadataBlocks.citation.fields =
                datasetVersion.metadataBlocks.citation.fields.Append(field).ToArray();
        }
    }

    public class DataverseDatasetVersion
    {
        public DataverseMetaDataBlocks metadataBlocks;
    }

    public class DataverseMetaDataBlocks
    {
        public DataverseCitation citation;
    }

    public class DataverseCitation
    {
        public DataverseField[] fields;
        public string displayName;
    }

    public class DataverseField
    {
        public string value;
        public string typeClass;
        public bool multiple = false;
        public string typeName;
    }

    public class DataverseMultipleField : DataverseField
    {
        public object[] value;
        public string typeClass = "compound";
        public bool multiple = true;
        public string typeName;
    }


}
