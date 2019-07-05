namespace ThesisPrototype.Models
{
    public class MetaData
    {
        public Title[] titles;
        public string community;
        public bool open_access;
        public string contact_email;
        public AlternateIdentifier[] alternate_identifiers;
        public Contributor[] contributors;
        public Creator[] creators;
        public Description[] descriptions;
        public string[] disciplines;
        public string embargo_date;
        public string[] keywords;
        public License license;
        public string publisher;
        public ResourceType[] resource_types;
        public string version;
        public string language;
    }

    public class Title
    {
        public string title;
    }

    public class Contributor
    {
        public string contributor_name;
        public string contributor_type;
    }

    public class Creator
    {
        public string creator_name;
    }

    public class Description
    {
        public string description;
        public string description_type;
    }

    public class License
    {
        public string license;
        public string license_uri;
    }

    public class ResourceType
    {
        public string resource_type;
        public string resource_type_general;
    }

    public class AlternateIdentifier
    {
        public string alternate_identifier;
        public string alternate_identifier_type;
    }
}
