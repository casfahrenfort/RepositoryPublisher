namespace ThesisPrototype.Models.Dataverse
{
    public class DataverseResponse : Response
    {
        public string status;
        public string message;
    }

    public class DataverseCreateResponse
    {
        public string status;
        public DataverseCreateResponseData data;
    }

    public class DataverseCreateResponseData
    {
        public int id;
        public string persistentId;
        public string persistentUrl;
    }
}
