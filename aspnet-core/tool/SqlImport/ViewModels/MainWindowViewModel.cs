using HandyControl.Controls;
using Prism.Commands;
using Prism.Services.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using SqlImport.Models;
using SqlImport.Utils;
using SqlImport.ViewModels.Base;

namespace SqlImport.ViewModels
{
    public class MainWindowViewModel : Controller
    {
        #region 主页面       
        public async void AllStartMethod()
        {
            await Task.Run(() =>
            {
                foreach (var item in UploadlistMethod)
                {
                    if (item.State != Uploading.上传完成.ToString())
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            item.manualResetEvent.Set();
                            item.State = Uploading.上传中.ToString();
                        });
                    }
                }
            });
        }
        public async void AllStopMethod()
        {
            await Task.Run(() =>
            {
                foreach (var item in UploadlistMethod)
                {
                    if (item.State != Uploading.上传完成.ToString())
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            item.manualResetEvent.Reset();
                            item.State = Uploading.暂停.ToString();
                        });
                    }
                }
            });
        }
        public async void AllOverMethod()
        {
            if (UploadlistMethod.Count() >= 1)
            {
                await Task.Run(() => Growl.Ask("是否全部取消", isConfirmed =>
                {
                    if (isConfirmed)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            UploadlistMethod.Clear();
                        });
                        Growl.Info("取消成功");
                    }
                    return true;
                }));
            }
            else
            {
                InfoGlobalCmd("现在没有可以取消的任务了").Invoke();
            }

        }

        public async void StopMethod(string para)
        {
            await Task.Run(() =>
            {
                foreach (var item in UploadlistMethod)
                {
                    if (item.ID == para)
                    {
                        if (item.State != Uploading.上传完成.ToString())
                        {
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                item.manualResetEvent.Reset();
                                item.State = Uploading.暂停.ToString();
                            });
                        }
                    }
                }
            });
        }

        public async void StartMethod(string para)
        {
            await Task.Run(() =>
            {
                foreach (var item in UploadlistMethod)
                {
                    if (item.ID == para)
                    {
                        if (item.State != Uploading.上传完成.ToString())
                        {
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                item.manualResetEvent.Set();
                                item.State = Uploading.上传中.ToString();
                            });
                        }
                    }
                }
            });
        }
        public async void OverMethod(string para)
        {
            await Task.Run(() => Growl.Ask("是否取消", isConfirmed =>
            {
                if (isConfirmed)
                {
                    foreach (var item in UploadlistMethod)
                    {
                        if (item.ID == para)
                        {
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                UploadlistMethod.Remove(item);

                            });
                            break;
                        }

                    }
                }
                return true;
            }));
        }
        /// <summary>
        /// 全部开启
        /// </summary>
        private DelegateCommand allStart;
        public DelegateCommand AllStart =>
        allStart ??= new DelegateCommand(AllStartMethod);
        /// <summary>
        /// 全部结束
        /// </summary>
        private DelegateCommand allStop;
        public DelegateCommand AllStop =>
        allStop ??= new DelegateCommand(AllStopMethod);
        /// <summary>
        /// 全部结束
        /// </summary>
        private DelegateCommand allOver;
        public DelegateCommand AllOver =>
        allOver ??= new DelegateCommand(AllOverMethod);
        /// <summary>
        /// 单个暂停
        /// </summary>
        private DelegateCommand<string> stop;
        public DelegateCommand<string> Stop =>
        stop ??= new DelegateCommand<string>(StopMethod);

        /// <summary>
        /// 单个开始
        /// </summary>
        private DelegateCommand<string> start;
        public DelegateCommand<string> Start =>
        start ??= new DelegateCommand<string>(StartMethod);

        /// <summary>
        /// 单个取消
        /// </summary>
        private DelegateCommand<string> over;
        public DelegateCommand<string> Over =>
        over ??= new DelegateCommand<string>(OverMethod);
        /// <summary>
        /// 图片数据
        /// </summary>
        public ObservableCollection<Uploadlist> UploadlistMethod { get; private set; } =
        new ObservableCollection<Uploadlist>();
        /// <summary>
        /// 负责人
        /// </summary>
        private string principal;
        public string Principal
        {
            get => principal;
            set => _ = SetProperty(storage: ref principal, value);
        }

        private readonly IDialogService dialog;

        public MainWindowViewModel(IDialogService dialog)
        {

            this.dialog = dialog;
        }
        /// <summary>
        /// 登录
        /// </summary>
        private DelegateCommand register;
        public DelegateCommand Register =>
        register ??= new DelegateCommand(RegisterMethod);
        /// <summary>
        /// 退出登录
        /// </summary>
        private DelegateCommand logout;
        public DelegateCommand LogOut =>
        logout ??= new DelegateCommand(LogOutMethod);
        /// <summary>
        /// 添加任务
        /// </summary>
        private DelegateCommand addTask;

        public DelegateCommand AddTask => addTask ??= new DelegateCommand(AddTaskMethod);
        /// <summary>
        /// 添加任务的方法
        /// </summary>
        public void AddTaskMethod()
        {
            if (!string.IsNullOrEmpty(Principal))
            {

                dialog.ShowDialog("Task", null, arg =>
                {
                    try
                    {
                        if (arg.Result == ButtonResult.OK)
                        {
                            TaskList taskList = arg.Parameters.GetValue<TaskList>("Value");
                            string guid = Guid.NewGuid().ToString()+"-"+DateTime.Today.ToString("yyyyMMdd");
                            ManualResetEvent _resetEvent = new ManualResetEvent(true);
                            CancellationTokenSource tokenSource = new CancellationTokenSource();

                            Task task = Task.Run(() =>
                            {
                                int count = 1;                               
                                foreach (FileModel item in taskList.FileModel)
                                {
                                    string only = Guid.NewGuid().ToString() + "-" + DateTime.Today.ToString("yyyyMMdd");
                                    Application.Current.Dispatcher.Invoke(() =>
                                    {
                                        Uploadlist price = UploadlistMethod.Where(p => p.ID.Equals(guid)).FirstOrDefault();
                                        price.ProgressBar = (int)(count * 1.0 / taskList.FileModel.Count * 100);
                                    });
                                    count++;
                                  
                                    _resetEvent.WaitOne();
                                }
                               
                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    Uploadlist price = UploadlistMethod.Where(p => p.ID.Equals(guid)).FirstOrDefault();
                                    price.State = Uploading.整合中.ToString();
                                });
                                try
                                {                                     
                                    Application.Current.Dispatcher.Invoke(() =>
                                    {
                                        Uploadlist price = UploadlistMethod.Where(p => p.ID.Equals(guid)).FirstOrDefault();
                                        price.State = Uploading.上传完成.ToString();
                                        price.tokenSource.Cancel();
                                    });
                                }
                                catch (Exception ex)
                                {
                                    Uploadlist price = UploadlistMethod.Where(p => p.ID.Equals(guid)).FirstOrDefault();
                                    price.State = ex.Message.ToString();
                                }
                            });
                            Uploadlist uploadlist = new()
                            { ID = guid, TaskName = taskList.TaskName.ToString(), ProgressBar = 0, Task = task, manualResetEvent = _resetEvent, TaskList = taskList, State = Uploading.上传中.ToString(), tokenSource = tokenSource };
                            UploadlistMethod.Add(uploadlist);
                        }
                    }
                    catch (AggregateException ex)
                    {
                        WrongPassword(ex.Message).Invoke();
                    }
                    catch (Exception ex)
                    {
                        WrongPassword(ex.Message).Invoke();
                    }

                });
            }
            else
            {
                InfoGlobalCmd("请先登录").Invoke();
            }
        }
        public void LogOutMethod()
        {
            if (string.IsNullOrEmpty(Principal))
            {
                InfoGlobalCmd("请先登录").Invoke();
            }
            else
            {
                Principal = string.Empty;
                InfoGlobalCmd("已退出登录").Invoke();
            }
        }

        public void RegisterMethod()
        {
            dialog.ShowDialog("Register", null, arg =>
            {
                if (arg.Result == ButtonResult.OK)
                {
                    Principal = arg.Parameters.GetValue<string>("Value");
                }
            });
        }

        #endregion

    }
    public enum Uploading
    {
        上传中,
        暂停,
        上传完成,
        整合中
    }
}
