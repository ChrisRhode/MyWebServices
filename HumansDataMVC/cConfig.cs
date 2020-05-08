using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HumansDataMVC
{
    public class cConfig
    {
        public string GetConnectionString()
        {
            return "Data Source=xxx;Initial Catalog=yyy;Persist Security Info=True;User ID=zzz;Password=aaa";
        }

        public string GetMVCRoot()
        {
            return "/HumansDataMVC/humans";
            //return "/humans";
        }
    }

}