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
        VKGraph graph = new VKGraph();

        public MainForm()
        {
            InitializeComponent();
        }

        public uint FillVertex(long id, int now, int depth)
        {
            //now++;
            uint errors = 0;
            User u = vk.Users.Get(new long[] { id }, ProfileFields.LastName | ProfileFields.FirstName | ProfileFields.ScreenName | ProfileFields.Photo200).FirstOrDefault(); //лишняя операция, для всех уровней кроме первого (добавление ниже)
            graph.AddVertex(u);
            VkNet.Utils.VkCollection<User> f;
            try
            {
                f = vk.Friends.Get(new VkNet.Model.RequestParams.FriendsGetParams
                {
                    UserId = id,
                    Count = 3,
                    Order = VkNet.Enums.SafetyEnums.FriendsOrder.Hints,
                    Fields = ProfileFields.FirstName | ProfileFields.LastName | ProfileFields.Photo200 | ProfileFields.ScreenName,
                });

                if (now < depth)
                {
                    foreach (var x in f)
                    {
                        graph.AddVertex(x);
                        graph.AddEdge(u, x);
                    }
                    foreach (var x in f)
                    {
                        if (now < depth)
                            FillVertex(x.Id, now + 1, depth);
                    }

                }
                if (now == depth)
                    foreach (var x in f)
                        foreach (var y in f)
                            //if (vk.Friends.AreFriends(new long[] { x.Id, y.Id }))
                                graph.AddEdge(x, y);
            }
            catch (VkNet.Exception.VkApiException)
            {
                errors += 1;
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
            foreach (var x in graph.Vertexes())
                listBoxV.Items.Add(x.FirstName + " " + x.LastName);
            ErrorLabel.Text = "Errors: " + errors.ToString();
            TimeLabel.Text = "Time: " + w.Elapsed.Minutes + "m " + w.Elapsed.Seconds + "s ";
            VertexesLabel.Text = "Vertexes: " + graph.Vertexes().Count;
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
            catch (System.AggregateException)
            {
                MessageBox.Show("No internet connection", "Error");
                return;
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
            var x = vk.Users.Get(new long[] { vk.UserId.Value }, ProfileFields.Photo200).FirstOrDefault().Photo200;
            if (x != null)
            pictureBox1.ImageLocation = x.ToString();
            pictureBox1.Load();
        }

        private void listBoxV_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBoxE.Items.Clear();

            var x = graph.Vertexes()[listBoxV.SelectedIndex].Photo200;
            if (x != null)
            pictureBox1.ImageLocation = x.ToString(); //немножко костыльно
            pictureBox1.Load();

            foreach (User u in graph.Edges(graph.Vertexes()[listBoxV.SelectedIndex]))
                listBoxE.Items.Add(u.FirstName + " " + u.LastName);
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            vk.LogOut();
        }
    }
}
