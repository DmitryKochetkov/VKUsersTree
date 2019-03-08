using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VkNet;
using VkNet.Enums.Filters;
using System.ComponentModel;

namespace UserEquality
{
    class Program
    {
        static void Main(string[] args)
        {
            VkApi vk = new VkApi();
            Console.WriteLine("Password for dimitri.kochetkov@gmail.com: ");
            string s = "";
            ConsoleKeyInfo key = Console.ReadKey(true);
            do
            {
                key = Console.ReadKey(true);
                if (key.Key != ConsoleKey.Enter && key.Key != ConsoleKey.Backspace)
                {
                    //if (key.Key == "")
                    s += key.KeyChar;
                    Console.Write("*");
                }
                if (key.Key == ConsoleKey.Backspace)
                    s.Remove(s.Length - 1);
            }
            while (key.Key != ConsoleKey.Enter);
            Console.WriteLine();

            try
            {
                vk.Authorize(new VkNet.Model.ApiAuthParams
                {
                    ApplicationId = 6852311,
                    Login = "dimitri.kochetkov@gmail.com",
                    Password = s,
                    Settings = Settings.Offline
                });
            }
            catch
            {
                Console.WriteLine("Authorization error :(");
                Console.ReadKey();
                return;
            }
            ProfileFields fields = ProfileFields.FirstName | ProfileFields.LastName | ProfileFields.Uid | ProfileFields.Photo200 | ProfileFields.ScreenName;

            var Artem = vk.Users.Get(new string[] { "artembedrikov" }, fields).FirstOrDefault();
            var f = vk.Friends.Get(new VkNet.Model.RequestParams.FriendsGetParams
            {
                UserId = vk.UserId,
                Count = 5,
                Order = VkNet.Enums.SafetyEnums.FriendsOrder.Hints,
                Fields = fields
            });
            
            int k = -1;
            for(int i = 0; i < f.Count; i++)
            {
                if (f[i].ScreenName == "artembedrikov")
                    k = i;
            }
            
            try
            {
                var u = f[k]; 
                if (u == Artem)
                    Console.WriteLine("Users are equal");
                else Console.WriteLine("Users are not equal");
            }
            catch (ArgumentOutOfRangeException)
            {
                Console.WriteLine("Argument Out Of Range Exception!");
            }   

            Console.ReadKey();
        }
    }
}
