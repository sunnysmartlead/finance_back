using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SqlImport.Models
{
    public class Uploadlist : INotifyPropertyChanged
    {
        /// <summary>
        /// id
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 任务名
        /// </summary>
        public string TaskName { get; set; }
        /// <summary>
        /// 变化后出发的事件
        /// </summary>
        int progressBar;
        /// <summary>
        /// 变化后出发的事件
        /// </summary>
        public int ProgressBar { get { return progressBar; } set { progressBar = value; OnPropertyChanged("ProgressBar"); } }
        /// <summary>
        /// 变化后出发的事件
        /// </summary>
        protected internal virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// 线程
        /// </summary>
        public Task Task { get; set; }
        /// <summary>
        /// 控制线程停止开启
        /// </summary>
        public ManualResetEvent manualResetEvent;
        /// <summary>
        /// 控制线程取消
        /// </summary>
        public CancellationTokenSource tokenSource;
        /// <summary>
        /// 上传的数据
        /// </summary>
        public TaskList TaskList { get; set; }
        string state;
        /// <summary>
        /// 状态
        /// </summary>
        public string State { get { return state; } set { state = value; OnPropertyChanged("State"); } }
    }
}
