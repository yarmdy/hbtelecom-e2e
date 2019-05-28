using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CTCCGoods.Controllers
{
    [Serializable]
    public class cuser
    {
        public int? id { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public int? utype { get; set; }
        public int? status { get; set; }
        public string pwd { get; set; }
        public int? wid{get;set;}
        public string wname { get; set; }
        public string tel { get; set; }
        public string contacts { get; set; }
        public List<int> wids { get; set; }
    }
}