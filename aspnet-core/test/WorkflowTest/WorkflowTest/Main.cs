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
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace WorkflowTest
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private async void Main_Load(object sender, EventArgs e)
        {
            await LoadTask();
            label1.Text = UserInfo.UserName;
        }

        private async Task LoadTask()
        {
            var data = await $"{UserInfo.Url}/api/services/app/WorkflowInstance/GetTaskByUserId?userId=0"
                .WithHeader("Authorization", $"Bearer {UserInfo.AccessToken}")
                .GetJsonAsync<ReulstData<UserTask>>();
            var list = data.Result.Items.ToList();
            dataGridView1.DataSource = list;
        }

        private async void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            var id = dataGridView1.Rows[e.RowIndex].Cells["Id"].FormattedValue.ToString();
            var url = $"{UserInfo.Url}/api/services/app/WorkflowInstance/GetEvalDataByNodeInstanceId?nodeInstanceId={id}";
            var data = await url
                .WithHeader("Authorization", $"Bearer {UserInfo.AccessToken}")
                .GetJsonAsync<ReulstData<FinanceDictionaryListDto>>();
            dataGridView2.DataSource = data.Result.Items.ToList();

        }

        private async void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0 || dataGridView2.SelectedRows.Count == 0)
            {
                MessageBox.Show("请点击选择处理的节点和审批意见！", "提示");
                return;
            }
            var nodeInstanceId = Convert.ToInt64(dataGridView1.SelectedRows[0].Cells["Id"].FormattedValue);
            var financeDictionaryDetailId = dataGridView2.SelectedRows[0].Cells["Id"].FormattedValue.ToString();
            var result = await $"{UserInfo.Url}/api/services/app/WorkflowInstance/SubmitNode"
                .WithHeader("Authorization", $"Bearer {UserInfo.AccessToken}")
                .PostJsonAsync(new
                {
                    NodeInstanceId = nodeInstanceId,
                    FinanceDictionaryDetailId = financeDictionaryDetailId,
                    Comment = "测试程序提交"
                }).ReceiveJson<ReulstData<FinanceDictionaryListDto>>();
            if (result != null)
            {
                MessageBox.Show("提交成功！", "提示");
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            await LoadTask();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            new NewProcess().Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            new TaskCompleted().Show();
        }
    }
}
