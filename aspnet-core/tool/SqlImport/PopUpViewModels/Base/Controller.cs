using HandyControl.Controls;
using HandyControl.Data;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlImport.PopUpViewModels.Base
{
    public class Controller : BindableBase
    {
        /// <summary>
        /// 密码错误提示
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
