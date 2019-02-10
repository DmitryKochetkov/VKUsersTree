using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Model;
using Newtonsoft.Json;
using System.Net;

namespace VKTree
{
    public partial class MainForm : Form
    {
        VkApi vk = new VkApi();
        Graph<long> graph = new Graph<long>();

        public MainForm()
        {
            InitializeComponent();
        }

        public int Cycle(long[] arrid)
        {
            var p = vk.Users.Get(arrid);
            foreach (User u in p)
            {
                graph.AddVertex(u.Id);
                var f = vk.Friends.Get(new VkNet.Model.RequestParams.FriendsGetParams
                {
                    UserId = u.Id,
                    Fields = ProfileFields.FirstName,
                });
            }
            return 1;
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            uint errors = 0;
            if (!Int64.TryParse(UserIDTextBox.Text, out long id))
            {
                MessageBox.Show("Cannot resolve field User ID!", "Error");
                return;
            }
            if (!Int32.TryParse(DepthTextBox.Text, out int depth))
            {
                MessageBox.Show("Cannot resolve field Depth! (Natural number required)", "Error");
                return;
            }
            this.Hide();
            Progress p = new Progress();
            p.Show();
            graph.AddVertex(id);
            var f = vk.Friends.Get(new VkNet.Model.RequestParams.FriendsGetParams
            {
                UserId = id,
            });
            foreach (User u in f)
            {
                graph.AddVertex(u.Id);
                graph.AddEdge(id, u.Id);
            }
            foreach (User u in f)
            {
                try
                {
                    var ids = vk.Friends.GetMutual(new VkNet.Model.RequestParams.FriendsGetMutualParams
                    {
                        TargetUid = u.Id,
                        SourceUid = id
                    });
                    foreach (long n in ids)
                    {
                        graph.AddEdge(u.Id, n);
                    }
                }
                catch (VkNet.Exception.CannotBlacklistYourselfException)
                {
                    //MessageBox.Show("Ошибка");
                    errors++;
                }
            }
            
            var pizda = vk.Users.Get(graph.edges.Keys.ToArray<long>());
            foreach (var x in pizda)
            {
                listBoxV.Items.Add(x.Id + " " + x.FirstName + " " + x.LastName);
            }
            ErrorLabel.Text = "Errors: " + errors.ToString();
            p.Close();
            this.Show();
            
            //var p = vk.Users.Get(new long[] { id }).FirstOrDefault();
            ////listBox1.Items.Add(p.FirstName + " " + p.LastName);
            //var users = vk.Friends.Get(new VkNet.Model.RequestParams.FriendsGetParams
            //{
            //    UserId = id,
            //    Fields = ProfileFields.FirstName,
            //});
            //foreach (User u in users)
            //{
            //    //listBox1.Items.Add(u.FirstName + " " + u.LastName);
            //    graph.AddVertex(u.Id);
            //    graph.AddEdge(id, u.Id);
            //    var ufriends = vk.Friends.Get(new VkNet.Model.RequestParams.FriendsGetParams
            //    {
            //        UserId = u.Id,
            //        Fields = ProfileFields.FirstName,
            //    });

            //}
        }

        private void GetTokenButton_Click(object sender, EventArgs e)
        {
            if (!UInt64.TryParse(AppIDTextBox.Text, out ulong aid))
            {
                MessageBox.Show("Cannot resolve field Application ID!", "Error");
                return;
            }
            try
            {
                vk.Authorize(new ApiAuthParams
                {
                    ApplicationId = aid,
                    Login = LoginTextBox.Text,
                    Password = PassTextBox.Text,
                    Settings = Settings.All
                });
            }
            catch (VkNet.Exception.VkApiException)
            {
                TokenReceived.Text = "Have no access token!";
                TokenReceived.ForeColor = Color.Firebrick;
                StartButton.Enabled = false;
                return;
            }
            finally
            {
                TokenReceived.Text = "Have no access token!";
                TokenReceived.ForeColor = Color.Firebrick;
                StartButton.Enabled = false;
            }
            TokenReceived.Text = "Access token received!";
            TokenReceived.ForeColor = Color.Green;
            StartButton.Enabled = true;
        }

        private void listBoxV_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBoxE.Items.Clear();
            int pos = listBoxV.SelectedItem.ToString().IndexOf(' ');
            long id = Int64.Parse((listBoxV.SelectedItem.ToString().Substring(0, pos)));
            long[] ids = graph.edges[id].ToArray<long>();
            var x = vk.Users.Get(ids);
            foreach (User u in x)
            {
                listBoxE.Items.Add(u.Id + " " + u.FirstName +  " " + u.LastName);
            }
        }
    }
}
