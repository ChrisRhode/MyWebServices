using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UPCSvcs_SendBackJSON
{
    public class SendBackJSON
    {
    }

    public class aDescr
    {
        public string description_value { get; set; }
       
    }

    public class sendbackRoot
    {
        public string response_type { get; set; }
        public string known_description { get; set; }
        public IList<aDescr> possible_descriptions { get; set; }
    }
}