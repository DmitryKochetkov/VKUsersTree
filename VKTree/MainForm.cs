using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Diagnostics;
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
        //Graph<User> graph = new Graph<User>();
        Graph<long> graph = new Graph<long>();

        public MainForm()
        {
            InitializeComponent();
        }

        public uint FillVertex(long id, int now, int depth)
        {
            uint errors = 0;
            User u = vk.Users.Get(new long[] { id }).FirstOrDefault();
            graph.AddVertex(u.Id);
            try
            {
                var f = vk.Friends.Get(new VkNet.Model.RequestParams.FriendsGetParams
                {
                    UserId = id,
                    Count = 5,
                });

                if (now < depth)
                {
                    foreach (var x in f)
                    {
                        FillVertex(x.Id, now + 1, depth);
                    }
                }
                if (now == depth)
                {

                }
            }
            catch
            {
                errors++;
            }
            return errors;
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            listBoxV.Items.Clear();
            listBoxE.Items.Clear();
            graph.Clear();
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
            Stopwatch w = new Stopwatch();
            w.Start();
            errors = FillVertex(id, 0, depth);

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
                catch
                {
                    errors++;
                }
            }

            var vertex = vk.Users.Get(graph.edges.Keys.ToArray<long>());
            foreach (var x in vertex)
            {
                listBoxV.Items.Add(x.Id + " " + x.FirstName + " " + x.LastName);
            }
            w.Stop();
            ErrorLabel.Text = "Errors: " + errors.ToString();
            TimeLabel.Text = "Time: " + w.Elapsed.Minutes + "m " + w.Elapsed.Seconds + "s ";
            p.Close();
            this.Show();
        }

        private void GetTokenButton_Click(object sender, EventArgs e)
        {
            try
            {
                vk.Authorize(new ApiAuthParams
                {
                    ApplicationId = 6852311,
                    Login = LoginTextBox.Text,
                    Password = PassTextBox.Text,
                    Settings = Settings.Offline
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
            //listBoxE.Items.Clear();
            //String s = listBoxV.SelectedItem.ToString();
            //var f = vk.Friends.Get(new long[] { Int64.Parse(s.Substring(0, s.IndexOf(" "))) });
            //foreach (User u in f)
            //{
            //    listBoxE.Items.Add(u.Id + " " + u.FirstName + " " + u.LastName);
            //}
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            vk.LogOut();
        }
    }
}
