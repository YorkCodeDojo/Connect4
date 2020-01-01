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

        private void cmdOK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Please enter your name");
                txtName.Focus();
                return;
            }

            var playerID = API.RegisterTeam(txtName.Text, "23456236478236", "https://codedojoconnect4.azurewebsites.net/");
            var details = new APIDetails
            {
                PlayerID = playerID,
                Password = "23456236478236",
                URL = "https://codedojoconnect4.azurewebsites.net/",
            };

            var frm = new Form1(details);
            this.Hide();
            frm.Show();
        }
    }
}
