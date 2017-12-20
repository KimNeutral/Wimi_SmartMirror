using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLocation
{
    public class User
    {
        public int classNum { get; set; }
        public string name { get; set; }
        public string profilePic { get; set; }
        public Location location {get;set;}
    }
    public class Location
    {
        public int pId { get; set; }
        public int bId { get; set; }
        public int floor { get; set; }
        public string pname { get; set; }
        public string bname { get; set; }
        public int rssi { get; set; }
        public string time { get; set; }
    }
    public class RootObject
    {
        public List<User> users { get; set; }
    }
}
