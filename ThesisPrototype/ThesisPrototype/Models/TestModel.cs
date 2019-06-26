using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ThesisPrototype.Models
{
    public class TestModel
    {
        public TitleModel[] titles;
        public string community;
        public bool open_access;
    }

    public class TitleModel
    {
        public string title;
    }
}
