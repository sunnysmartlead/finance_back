using Flurl.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WorkflowTest
{
    public partial class TaskCompleted : Form
    {
        public TaskCompleted()
        {
            InitializeComponent();
        }

        private void TaskCompleted_Load(object sender, EventArgs e)
        {

        }

        private async void button1_Click(object sender, EventArgs e)
        {
            var data = await $"{UserInfo.Url}/api/services/app/WorkflowInstance/GetTaskCompletedByUserId?userId=0&workflowState=0"
                .WithHeader("Authorization", $"Bearer {UserInfo.AccessToken}")
                .GetJsonAsync<ReulstData<UserTask>>();
            var list = data.Result.Items.ToList();
            dataGridView1.DataSource = list;
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            var data = await $"{UserInfo.Url}/api/services/app/WorkflowInstance/GetTaskCompletedByUserId?userId=0&workflowState=1"
                .WithHeader("Authorization", $"Bearer {UserInfo.AccessToken}")
                .GetJsonAsync<ReulstData<UserTask>>();
            var list = data.Result.Items.ToList();
            dataGridView1.DataSource = list;
        }
    }
}
