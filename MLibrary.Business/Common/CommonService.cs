using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MLibrary.Business.Common
{
    class CommonService
    {
        public static string GenerateRandomString(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789qwertyuıopasdfghjklzxcvbnm.,_";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
