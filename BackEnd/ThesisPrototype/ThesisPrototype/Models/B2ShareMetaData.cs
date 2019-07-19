namespace ThesisPrototype.Models
{
    public class B2ShareMetaData
    {
        public B2ShareTitle[] titles;
        public string community;
        public bool open_access;    
        public string contact_email;
        public B2ShareAlternateIdentifier[] alternate_identifiers;
        public B2ShareContributor[] contributors;
        public B2ShareCreator[] creators;
        public B2ShareDescription[] descriptions;
        public string[] disciplines;
        public string embargo_date;
        public string[] keywords;
        public B2ShareLicense license;
        public string publisher;
        public B2ShareResourceType[] resource_types;
        public string version;
        public string language;
    }

    public class B2ShareTitle
    {
        public string title;
    }

    public class B2ShareContributor
    {
        public string contributor_name;
        public string contributor_type;
    }

    public class B2ShareCreator
    {
        public string creator_name;
    }

    public class B2ShareDescription
    {
        public string description;
        public string description_type;
    }

    public class B2ShareLicense
    {
        public string license;
        public string license_uri;
    }

    public class B2ShareResourceType
    {
        public string resource_type;
        public string resource_type_general;
    }

    public class B2ShareAlternateIdentifier
    {
        public string alternate_identifier;
        public string alternate_identifier_type;
    }
}
