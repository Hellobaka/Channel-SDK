using Channel_SDK.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Channel_SDK
{
    public static class Helper
    {
        public static void OutError(string content)
        {
            Console.WriteLine($"[-][{DateTime.Now.ToLongTimeString()}] {content}");
        }
        public static void OutLog(string content)
        {
            Console.WriteLine($"[+][{DateTime.Now.ToLongTimeString()}] {content}");
        }
        public static long TimeStamp => (long)(DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds;
        public static string ToJson(this object json) => JsonConvert.SerializeObject(json, Formatting.None);
        public static string ATUser(this User user) => $"<@!{user.id}> ";
    }
}
