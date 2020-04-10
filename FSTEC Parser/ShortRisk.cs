using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PagedList;

namespace FSTEC_Parser
{
    public class ShortRisk : ICloneable
    {
        private string id;
        public string ID
        {
            get => "УБИ."+id;
            set => id = value;
        }
        public string Name { get; set; }


        public object Clone()
        {
            return new ShortRisk()
            {
                ID = ID.Split('.')[1],
                Name = Name
            };
        }
    }
}
