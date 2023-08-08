using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineCoursePlatform.Core.Convertors
{
    public class FixedText
    {
        public static string FixedEmail(string email) => email.Trim().ToLower();
    }
}
