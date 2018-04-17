using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyIdentication.DAL.Entities
{
    public class Document
    {
        public string DocumentID { get; set; }
        public int? CADId { get; set; }
        public string Lot { get; set; }
        public string Block { get; set; }
        public string Section { get; set; }
        public string LegalDescription { get; set; }
        public string FilmCode { get; set; }
        public int? Fips { get; set; }
        public string AccountNumber { get; set; }
        public int ProcessingState { get; set; }
        public string OCALUC { get; set; }
    }
}