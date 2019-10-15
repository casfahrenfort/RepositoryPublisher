using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ThesisPrototype.Models.Figshare
{
    public class FigshareMetaData
    {
        public string title;
        public bool is_confidential;
        public string description;
        public int[] categories;
        public string defined_type;
        public string[] keywords;
        public FigshareAuthorName[] authors;
        public int license;
        //public Dictionary<string, string> custom_fields;
        public string[] references;
        public FigshareTimeline timeline;
    }

    public class FigshareAuthorName
    {
        public string name;
    }

    public class FigshareAuthorId
    {
        public int id;
    }

    public class FigshareTimeline
    {
        public string publisherPublication;
    }
}
