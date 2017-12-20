using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLocation
{

    public class Location
    {
        public string bname { get; set; }
        public int bId { get; set; }
        public string pname { get; set; }
        public int floor { get; set; }
        public string pId { get; set; }
        public string cur { get; set; }
        public string rssi { get; set; }
    }

    public class User
    {
        public string _id { get; set; }
        public string proFilePic { get; set; }
        public string name { get; set; }
        public int classNum { get; set; }
        public int __v { get; set; }
        public Location location { get; set; }
    }

    public class RootObject
    {
        public List<User> users { get; set; }
    }
}
