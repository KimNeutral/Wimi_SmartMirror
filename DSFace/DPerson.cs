using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DSFace
{
    [DataContract()]
    public class DPerson
    {
        [DataMember()]
        public Guid Id { get; set; }
        [DataMember()]
        public string Name { get; set; }

        public DPerson() { }

        public DPerson(string name)
        {
            this.Name = name;
        }

        public DPerson(string name, Guid id)
        {
            this.Name = name;
            this.Id = id;
        }

        public static DPerson GetDPersonByGuidFromList(List<DPerson> person, Guid guid)
        {
            DPerson finded = person.Find(
                        delegate (DPerson per)
                        {
                            if (per.Id == guid)
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
            );
            return finded;
        }
    }
}
