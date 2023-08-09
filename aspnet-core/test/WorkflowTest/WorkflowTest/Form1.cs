using Flurl.Http;

namespace WorkflowTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            try
            {
                var userName = textBox1.Text;
                var password = textBox2.Text;
                var login = await $"{UserInfo.Url}/api/AccountManagement/Login".PostJsonAsync(new
                {
                    userNameOrEmailAddress = userName,
                    password = password,
                    rememberClient = true
                }).ReceiveJson();
                UserInfo.UserName = userName;
                UserInfo.AccessToken = login.result.accessToken;
                this.Hide();
                new Main().Show();
            }
            catch (Exception)
            {
                MessageBox.Show("用户名或密码错误！", "提示");
            }
        }
    }
}