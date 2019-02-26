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
        Graph<User> graph = new Graph<User>();

        public MainForm()
        {
            InitializeComponent();
        }

        public uint FillVertex(long id, int now, int depth)
        {
            uint errors = 0;
            User u = vk.Users.Get(new long[] { id }, ProfileFields.Photo200).FirstOrDefault();
            graph.AddVertex(u);
            try
            {
                var f = vk.Friends.Get(new VkNet.Model.RequestParams.FriendsGetParams
                {
                    UserId = id,
                    Count = 5,
                    Order = VkNet.Enums.SafetyEnums.FriendsOrder.Hints,
                });

                if (now < depth)
                {
                    foreach (var x in f)
                    {
                        graph.AddEdge(u, x);
                        FillVertex(x.Id, now + 1, depth);
                    }
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
            uint errors = 0;
            this.Hide();
            Progress p = new Progress();
            p.Show();
            Stopwatch w = new Stopwatch();
            w.Start();
            errors = FillVertex(id, 0, depth);
            w.Stop();
            foreach (var x in graph.vertexes())
                listBoxV.Items.Add(x.FirstName + " " + x.LastName);
            ErrorLabel.Text = "Errors: " + errors.ToString();
            TimeLabel.Text = "Time: " + w.Elapsed.Minutes + "m " + w.Elapsed.Seconds + "s ";
            VertexesLabel.Text = "Vertexes: " + graph.vertexes().Count;
            EdgesLabel.Text = "Edges: " + 0;
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
            pictureBox1.ImageLocation = vk.Users.Get(new long[] { vk.UserId.Value }, ProfileFields.Photo200).FirstOrDefault().Photo200.ToString();
            pictureBox1.Load();
        }

        private void listBoxV_SelectedIndexChanged(object sender, EventArgs e)
        {
            var x = graph.vertexes()[listBoxV.SelectedIndex].Photo200;
            if (x != null)
            pictureBox1.ImageLocation = x.ToString(); //немножко костыльно
            pictureBox1.Load();
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            vk.LogOut();
        }
    }
}
