using Prism.Commands;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using SqlImport.Models;
using SqlImport.PopUpViewModels.Base;
using SqlImport.Utils;

namespace SqlImport.PopUpViewModels
{
    public class TaskViewModel : Controller, IDialogAware
    { /// <summary>
      /// 图片路径
      /// </summary>
        public string FilePath;

        private readonly IDialogService dialog;

        public TaskViewModel(IDialogService dialog)
        {
            this.dialog = dialog;
        }
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

        #region 主内容
        /// <summary>
        /// 紧急程度
        /// </summary>
        private string degreeEmergency;
        public string DegreeEmergency
        {
            get => degreeEmergency;
            set => _ = SetProperty(storage: ref degreeEmergency, value);
        }
        /// <summary>
        /// 任务号
        /// </summary>
        private string taskName;
        public string TaskName
        {
            get => taskName;
            set => _ = SetProperty(storage: ref taskName, value);
        }
        /// <summary>
        /// 图片数量
        /// </summary>
        private int imgCount;
        public int ImgCount
        {
            get => imgCount;
            set => _ = SetProperty(storage: ref imgCount, value);
        }
        /// <summary>
        /// Json数量
        /// </summary>
        private int jsonCount;
        public int JsonCount
        {
            get => jsonCount;
            set => _ = SetProperty(storage: ref jsonCount, value);
        }
        /// <summary>
        /// 其他文件数量
        /// </summary>
        private int elseCount;
        public int ElseCount
        {
            get => elseCount;
            set => _ = SetProperty(storage: ref elseCount, value);
        }
        /// <summary>
        /// 其他文件数量
        /// </summary>
        private int sqlCount;
        public int SqlCount
        {
            get => sqlCount;
            set => _ = SetProperty(storage: ref sqlCount, value);
        }
        /// <summary>
        /// 匹配不上图片的json数量
        /// </summary>
        private string elseJsonCount;
        public string ElseJsonCount
        {
            get => elseJsonCount;
            set => _ = SetProperty(storage: ref elseJsonCount, value);
        }
        /// <summary>
        /// 上传  Bool
        /// </summary>
        private bool confirmUploadBool;
        public bool ConfirmUploadBool
        {
            get => confirmUploadBool;
            set => _ = SetProperty(storage: ref confirmUploadBool, value);
        }
        /// <summary>
        /// 图片数据
        /// </summary>
        public ObservableCollection<FileModel> ImgFileModel { get; private set; } =
        new ObservableCollection<FileModel>();


        /// <summary>
        /// Json数据
        /// </summary>
        public ObservableCollection<FileModel> JsonFileModel { get; private set; } =
        new ObservableCollection<FileModel>();

        /// <summary>
        /// 多余的Json数据
        /// </summary>
        public ObservableCollection<FileModel> ElseJsonFileModel { get; private set; } =
        new ObservableCollection<FileModel>();
        /// <summary>
        /// sql数据
        /// </summary>
        public ObservableCollection<FileModel> SqlFileModel { get; private set; } =
        new ObservableCollection<FileModel>();
        /// <summary>
        /// 退出子窗体
        /// </summary>
        private DelegateCommand quit;
        public DelegateCommand Quit =>
        quit ??= new DelegateCommand(() => { RequestClose?.Invoke(new DialogResult(ButtonResult.No)); });

        /// <summary>
        /// 打开文件
        /// </summary>
        private DelegateCommand selectFile;
        public DelegateCommand SelectFile =>
        selectFile ??= new DelegateCommand(SelectFileMethod);

        /// <summary>
        /// 上传事件
        /// </summary>
        private DelegateCommand confirmUpload;
        public DelegateCommand ConfirmUpload =>
        confirmUpload ??= new DelegateCommand(ConfirmUploadMethod);
        /// <summary>
        /// 上传方法
        /// </summary>
        public async void ConfirmUploadMethod()
        {
            if (string.IsNullOrEmpty(TaskName))
            {
                ConfirmUploadBool = false;
                WarningGlobalCmd("请输入任务名").Invoke();
            }
            else
            {
                ConfirmUploadBool = true;
                await Task.Run(() => { ConfirmUploadSon(); });
            }
        }
        public void ConfirmUploadSon()
        {
            if (ImgCount != 0)
            {
                List<FileModel> fileModels = new();
                TaskList taskList = new();
                taskList.DegreeEmergency = DegreeEmergency;
                taskList.TaskName = TaskName;
                foreach (var item in ImgFileModel)
                {
                    FileModel JsonName = JsonFileModel.Where(p => p.Name.Equals(item.Name)).FirstOrDefault();
                    FileModel fileModel = new();
                    fileModel.ImgAllName = item.ImgAllName;
                    fileModel.ImgName = item.ImgName;
                    if (JsonName is not null)
                    {
                        fileModel.JsonAllName = JsonName.JsonAllName;
                        fileModel.JsonName = JsonName.JsonName;
                    }
                    fileModels.Add(fileModel);
                    taskList.FileModel = fileModels;
                }
                DialogParameters param = new();
                param.Add("Value", taskList);
                Application.Current.Dispatcher.Invoke(delegate ()
                {
                    RequestClose?.Invoke(new DialogResult(ButtonResult.OK, param));
                });
                ConfirmUploadBool = false;

            }
            else
            {
                ConfirmUploadBool = false;
                InfoGlobalCmd("加载中..../没有扫描到图片").Invoke();
            }
        }
        /// <summary>
        /// 打开文件方法
        /// </summary>
        public async void SelectFileMethod()
        {
            dialog.ShowDialog("SelectFile", null, arg =>
            {
                if (arg.Result == ButtonResult.OK)
                {
                    FilePath = arg.Parameters.GetValue<string>("Value");
                }
            });
            await Task.Run(() => { SearchFile(); });
        }
        public void SearchFile()
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    ImgFileModel.Clear();
                    JsonFileModel.Clear();
                    ElseJsonFileModel.Clear();
                    SqlFileModel.Clear();
                });
                if (FileHelp.IsDir(FilePath))
                {
                    List<FileModel> imglist = new();  //图片
                    List<FileModel> jsonlist = new();//josn
                    List<string> elselist = new();//其他
                    List<FileModel> jsonsurpluslist = new();//多余点json
                    List<FileModel> Sqllist = new();//多余点json
                    // 绑定到指定的文件夹目录
                    DirectoryInfo Dir = new DirectoryInfo(FilePath);
                    // 检索表示当前目录的文件和子目录
                    FileSystemInfo[] Fsinfos = Dir.GetFileSystemInfos();

                    foreach (var Path in Fsinfos)
                    {
                        //if (Path.Extension is ".jpg" or ".bmp" or ".png") //筛选图片格式
                        //{
                        //    Application.Current.Dispatcher.Invoke(() =>
                        //    {
                        //        //存图片不带后缀的名称
                        //        imglist.Add(new FileModel { Name = Path.Name.Split(".")[0].ToString() });
                        //        ImgFileModel.Add(new FileModel { ImgName = Path.Name, ImgAllName = Path.FullName, Name = Path.Name.Split(".")[0].ToString() });
                        //    });
                        //}
                        //else if (Path.Extension is ".json") //筛选json
                        //{
                        //    Application.Current.Dispatcher.Invoke(() =>
                        //    {
                        //        //json不带后缀的名称
                        //        jsonlist.Add(new FileModel { Name = Path.Name.Split(".")[0].ToString() });
                        //        JsonFileModel.Add(new FileModel { JsonName = Path.Name, JsonAllName = Path.FullName, Name = Path.Name.Split(".")[0].ToString() });
                        //    });
                        //}
                        //else 
                        if (Path.Extension is ".sql") //筛选sql格式
                        {
                            Sqllist.Add(new FileModel { Name = Path.Name.Split(".")[0].ToString() });
                            SqlFileModel.Add(new FileModel { JsonName = Path.Name, JsonAllName = Path.FullName, Name = Path.Name.Split(".")[0].ToString() });
                        }
                        else
                        {
                            elselist.Add(Path.Name);
                        }
                    }
                    //过滤出多余的json
                    List<FileModel> exp2 = jsonlist.Where(a => !imglist.Exists(t => a.Name.Equals(t.Name))).ToList();
                    foreach (var item in exp2.Take(200))
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            ElseJsonFileModel.Add(item);
                        });
                    }
                    //ElseJsonCount = exp2.Count().ToString();
                    //显示数量
                    //ImgCount = ImgFileModel.Count();
                    //JsonCount = JsonFileModel.Count();
                    ElseCount = elselist.Count();
                    SqlCount = SqlFileModel.Count();
                }
                else
                {
                    List<FileModel> imglist = new();  //图片
                    List<FileModel> jsonlist = new();//josn
                    List<string> elselist = new();//其他
                    List<FileModel> Sqllist = new();//多余点json


                    // 绑定到指定的文件夹目录
                    DirectoryInfo Dir = new DirectoryInfo(FilePath);
                    // 检索表示当前目录的文件和子目录
                    FileSystemInfo[] Fsinfos = Dir.GetFileSystemInfos();

                    string PathExt = FilePath.Substring(FilePath.Length - 4, 4);
                    if (PathExt != ".sql")
                    {
                        InfoGlobalCmd("此文件并不是sql文件").Invoke();
                        elselist.Add(FilePath);
                    }
                    else
                    {
                        
                        SqlCount = 1;
                    }                               
                }
                InfoGlobalCmd("加载结束").Invoke();
            }
            catch (Exception ex)
            {
                InfoGlobalCmd(ex.Message.ToString()).Invoke();
            }
        }
        #endregion
    }
}
