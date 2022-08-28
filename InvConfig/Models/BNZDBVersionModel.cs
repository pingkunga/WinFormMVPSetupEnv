using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvConfig.Models
{
    public class BNZDBVersionModel
    {
        public string AppBase { get; set; }
        public string VersionId { get; set; }
        public DateTime CreateDate { get; set; }
        public string IsCompatible { get; set; }
    }
}
