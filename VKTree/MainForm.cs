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

        public void Cycle(long[] arrid)
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
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            if (!Int64.TryParse(UserIDTextBox.Text, out long id))
            {
                MessageBox.Show("Cannot resolve field User ID!", "Error");
                return;
            }
            graph.AddVertex(id);

            var p = vk.Users.Get(new long[] { id }).FirstOrDefault();
            listBox1.Items.Add(p.FirstName + " " + p.LastName);
            var users = vk.Friends.Get(new VkNet.Model.RequestParams.FriendsGetParams
            {
                UserId = id,
                Fields = ProfileFields.FirstName,
            });
            foreach (User u in users)
            {
                listBox1.Items.Add(u.FirstName + " " + u.LastName);
            }
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
    }
}
