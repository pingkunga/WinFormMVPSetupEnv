using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InvConfig.Models
{
    public class UserInvestModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public Boolean IsOldEncrypt { get; set; }
        public string DecryptPassword { get; set; }
        public Boolean IsSupend { get; set; }
        public DateTime LastLogin { get; set; }
    }
}
