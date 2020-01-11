using System;
using System.Windows.Forms;

namespace EsthersConnectFour
{
    public partial class Welcome : Form
    {
        public Welcome()
        {
            InitializeComponent();
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private async void cmdOK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Please enter your name");
                txtName.Focus();
                return;
            }

            // var url = "https://codedojoconnect4.azurewebsites.net/";
            var url = "https://localhost:5001/";
            var api = new API(new Uri(url));
            var playerID = await api.RegisterTeam(txtName.Text, "23456236478236");
            var details = new APIDetails
            {
                PlayerID = playerID,
                Password = "23456236478236",
            };

            await api.NewGame(playerID);

            var frm = new Form1(details, api);
            this.Hide();
            frm.Show();
        }
    }
}
