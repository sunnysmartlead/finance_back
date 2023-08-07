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
    public partial class NewProcess : Form
    {
        public NewProcess()
        {
            InitializeComponent();
        }
        private async Task Create(string title, string financeDictionaryDetailId)
        {
            var result = await $"{UserInfo.Url}/api/services/app/WorkflowInstance/StartWorkflowInstance"
                .WithHeader("Authorization", $"Bearer {UserInfo.AccessToken}")
                .PostJsonAsync(new
                {
                    WorkflowId = "主流程",
                    Title = title,
                    FinanceDictionaryDetailId = financeDictionaryDetailId,
                }).ReceiveJson<ReulstData2<long>>();
            MessageBox.Show(result.Result == 0 ? "创建失败！" : "创建成功！", "提示");
            this.Close();
        }
        private async void button1_Click(object sender, EventArgs e)
        {
            await Create(button1.Text, "EvalReason_Yp");
        }



        private async void button2_Click(object sender, EventArgs e)
        {
            await Create(button2.Text, "EvalReason_Ffabg");

        }

        private async void button3_Click(object sender, EventArgs e)
        {
            await Create(button3.Text, "EvalReason_Nj");
        }

        private async void button4_Click(object sender, EventArgs e)
        {
            await Create(button4.Text, "EvalReason_Shj");

        }

        private async void button5_Click(object sender, EventArgs e)
        {
            await Create(button5.Text, "EvalReason_Bb1");

        }

        private async void button6_Click(object sender, EventArgs e)
        {
            await Create(button6.Text, "EvalReason_Fabg");

        }

        private async void button7_Click(object sender, EventArgs e)
        {
            await Create(button7.Text, "EvalReason_Qt");

        }

        private async void button8_Click(object sender, EventArgs e)
        {
            await Create(button8.Text, "EvalReason_Jdcbpg");

        }

        private async void button9_Click(object sender, EventArgs e)
        {
            await Create(button9.Text, "EvalReason_Xmbg");

        }
    }
}
