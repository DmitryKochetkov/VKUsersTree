using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VkNet;

namespace UserEquality
{
    class Program
    {
        static void Main(string[] args)
        {
            VkApi vk = new VkApi();
            Console.WriteLine("Password for dimitri.kochetkov@gmail.com: ");
            string s = Console.ReadLine();
            try
            {
                vk.Authorize(new VkNet.Model.ApiAuthParams
                {
                    ApplicationId = 6852311,
                    Login = "dimitri.kochetkov@gmail.com",
                    Password = s,
                    Settings = VkNet.Enums.Filters.Settings.Offline
                });
            }
            finally
            {
                Console.WriteLine("Authorization error :(");
            }

            var Artem = vk.Users.Get(new string[] { "artembedrikov" });
            //var f = 

            Console.ReadKey();
        }
    }
}
