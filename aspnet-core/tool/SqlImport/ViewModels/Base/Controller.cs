using HandyControl.Controls;
using HandyControl.Data;
using Prism.Mvvm;
using System;

namespace SqlImport.ViewModels.Base
{
    public class Controller : BindableBase
    { 
        /// <summary>
        /// 错误提示
        /// </summary>
        public Action WrongPassword(string para) => new(() => Growl.ErrorGlobal(para));
        /// <summary>
        /// 非空提示
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public Action WarningGlobalCmd(string para) => new(() => Growl.WarningGlobal(new GrowlInfo { Message = para }));
        /// <summary>
        /// 消息提示
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public Action InfoGlobalCmd(string para) => new(() => Growl.InfoGlobal(para));
    }
}
