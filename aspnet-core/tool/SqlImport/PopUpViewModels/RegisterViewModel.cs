using Prism.Commands;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Data;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using SqlImport.Models;
using SqlImport.PopUpViewModels.Base;

namespace SqlImport.PopUpViewModels
{
    public class RegisterViewModel : Controller, IDialogAware
    {
        public RegisterViewModel()
        {

        }
        private readonly string _token;
        public RegisterViewModel(string token)
        {
            _token = token;
        }
        /// <summary>
        /// 用户名
        /// </summary>
        private string user;
        public string User
        {
            get => user;
            set => _ = SetProperty(storage: ref user, value);
        }
        /// <summary>
        /// 密码
        /// </summary>
        private string passwordName;
        public string PasswordName
        {
            get => passwordName;
            set => _ = SetProperty(storage: ref passwordName, value);
        }
        /// <summary>
        /// 保存事件
        /// </summary>
        private DelegateCommand saveCommand;
        public DelegateCommand SaveCommand => saveCommand ??= new DelegateCommand(AuthUser);
        /// <summary>
        /// 取消事件
        /// </summary>
        private DelegateCommand cancelCommand;
        public DelegateCommand CancelCommand => cancelCommand ??= new DelegateCommand(() =>
        {
            RequestClose?.Invoke(new DialogResult(ButtonResult.No));
        });

        public string Title { get; set; } = "登录";

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

        public async void AuthUser()
        {
            await Task.Run(() =>
            {
                string loginId = User;
                string password = PasswordName;
                User result = new User();
                try
                {
                    if (string.IsNullOrEmpty(loginId) || string.IsNullOrEmpty(password))
                    {
                        WarningGlobalCmd("账号密码不能为空").Invoke();
                        return;
                    }
                    //create a "principal context" - e.g.your domain(could be machine, too)
                    using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, "SUNNYOPTICAL"))
                    {
                        UserPrincipal user = UserPrincipal.FindByIdentity(pc, loginId);
                        // validate the credentials
                        bool isValid = pc.ValidateCredentials(loginId, password);
                        if (isValid)
                        {
                            InfoGlobalCmd("登录成功").Invoke();

                            // user.Description = 工号
                            result.Id = user.Description;
                            result.LoginId = loginId;
                            result.PassWord = password;
                            result.UserName = user.DisplayName;
                            DialogParameters param = new();
                            param.Add("Value", user.DisplayName);
                            Application.Current.Dispatcher.Invoke(delegate ()
                            {
                                RequestClose?.Invoke(new DialogResult(ButtonResult.OK, param));
                            });
                        }
                        else
                        {
                            WrongPassword("账号/密码错误").Invoke();
                        }
                    }

                    //DirectoryEntry entry = new DirectoryEntry("LDAP://10.1.21.11", @"SUNNYOPTICAL\"+loginId, password, AuthenticationTypes.None);
                    //if (entry.Properties != null && entry.Properties.Count > 0)
                    //{
                    //    InfoGlobalCmd("登录成功").Invoke();
                    //    DialogParameters param = new();
                    //    param.Add("Value", loginId);
                    //    Application.Current.Dispatcher.Invoke(delegate ()
                    //    {
                    //        RequestClose?.Invoke(new DialogResult(ButtonResult.OK, param));
                    //    });
                    //}
                    //else
                    //{
                    //    WrongPassword("账号/密码错误").Invoke();
                    //}

                }
                catch (Exception ex)
                {
                    WrongPassword(ex.Message).Invoke();
                }
            });
            // 返回JSON格式结果
        }
    }
}
