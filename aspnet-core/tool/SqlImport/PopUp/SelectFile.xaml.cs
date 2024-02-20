using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SqlImport.PopUp
{
    /// <summary>
    /// SelectFile.xaml 的交互逻辑
    /// </summary>
    public partial class SelectFile : UserControl
    {
        public string FilePath { get; set; }
        public SelectFile()
        {
            InitializeComponent();
            this.Width = 350;
            this.Height = 350;
        }
        private void FileMethod(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.Description = "请选择Txt所在文件夹";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (!string.IsNullOrEmpty(dialog.SelectedPath))
                {
                    FilePath = dialog.SelectedPath;
                }
            }
        }
    }
}
