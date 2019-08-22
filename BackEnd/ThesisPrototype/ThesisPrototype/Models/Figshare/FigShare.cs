using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ThesisPrototype.Models.Figshare
{
    public class FigshareCreateFile
    {
        public string name;
        public int size;
    }

    public class FigshareFile
    {
        public string status;
        public bool is_link_only;
        public string name;
        public string viewer_type;
        public string preview_state;
        public string download_url;
        public string supplied_md5;
        public string computed_md5;
        public string upload_token;
        public string upload_url;
        public int id;
        public int size;
    }
}
