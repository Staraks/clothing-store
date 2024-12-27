using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clothing_Store
{
    public static class SessionManager
    {
        public static int CurrentUserId { get; set; }
        public static string Role { get; set; }
    }
}
