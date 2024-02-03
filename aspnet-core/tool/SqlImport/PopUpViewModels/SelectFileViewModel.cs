using Prism.Commands;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using SqlImport.PopUpViewModels.Base;

namespace SqlImport.PopUpViewModels
{
    public class SelectFileViewModel : Controller, IDialogAware
    {
        public string Title { get; set; }

        public event Action<IDialogResult> RequestClose;

        public bool CanCloseDialog()
        {
            return true;
        }
        public void OnDialogClosed()
        {

        }
        public void OnDialogOpened(IDialogParameters parameters)
        {

        }
        /// <summary>
        /// 退出子窗体
        /// </summary>
        private DelegateCommand quit;
        public DelegateCommand Quit =>
        quit ??= new DelegateCommand(() => { RequestClose?.Invoke(new DialogResult(ButtonResult.No)); });
        /// <summary>
        ///文件选择 点击事件
        /// </summary>
        private DelegateCommand file;
        public DelegateCommand File =>
        file ??= new DelegateCommand(FileMethod);
        public void FileMethod()
        {
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.Description = "请选择Txt所在文件夹";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (!string.IsNullOrEmpty(dialog.SelectedPath))
                {
                    Application.Current.Dispatcher.Invoke(delegate ()
                    {
                        DialogParameters param = new();
                        param.Add("Value", dialog.SelectedPath);
                        Application.Current.Dispatcher.Invoke(delegate ()
                        {
                            RequestClose?.Invoke(new DialogResult(ButtonResult.OK, param));
                        });
                    });
                }

            }
        }
        /// <summary>
        ///文件选择 鼠标拖进选匡区
        /// </summary>
        private DelegateCommand fileDragEnter;
        public DelegateCommand FileDragEnter =>
        fileDragEnter ??= new DelegateCommand(FileMethodDragEnter);
        public void FileMethodDragEnter()
        {
            InfoGlobalCmd("检测到了文件").Invoke();
        }
        /// <summary>
        ///文件选择 鼠标松开选匡区
        /// </summary>
        private DelegateCommand<DragEventArgs> fileDragDrop;
        public DelegateCommand<DragEventArgs> FileDragDrop =>
        fileDragDrop ??= new DelegateCommand<DragEventArgs>(FileMethodDrop);
        public void FileMethodDrop(DragEventArgs e)
        {
            string fileName = ((Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
            InfoGlobalCmd(fileName).Invoke();
            DialogParameters param = new();
            param.Add("Value", fileName);
            Application.Current.Dispatcher.Invoke(delegate ()
            {
                RequestClose?.Invoke(new DialogResult(ButtonResult.OK, param));
            });

        }
    }
}

