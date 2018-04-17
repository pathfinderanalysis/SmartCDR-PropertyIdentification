using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyIdentication.DAL.Entities
{
    public class Property
    {
        public string LegalBlock { get; set; }
        public string LegalSection { get; set; }
        public string AccountNumber { get; set; }
        public string OCALUC { get; set; }
    }
}