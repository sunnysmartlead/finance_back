using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Users.Dto
{
    /// <summary>
    /// 批量用户导入结果
    /// </summary>
    public class ExcelImportResult
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ExcelImportResult()
        {
            InsertTotal = 0;
            UpdateTotal = 0;
            NegativeTotal = 0;
        }
        /// <summary>
        /// Excel文件里的用户总数
        /// </summary>
        public virtual int Total { get; set; }

        /// <summary>
        /// 成功创建的数量
        /// </summary>
        public virtual int InsertTotal { get; set; }

        /// <summary>
        /// 成功更新和启用的数量
        /// </summary>
        public virtual int UpdateTotal { get; set; }

        /// <summary>
        /// 成功禁用的数量
        /// </summary>
        public virtual int NegativeTotal { get; set; }

        /// <summary>
        /// 是否成功
        /// </summary>
        public virtual bool IsSuccess { get; set; }

        /// <summary>
        /// 返回消息（成功则为“添加成功”，失败则为失败原因）
        /// </summary>
        public virtual string Message { get; set; }
    }

    public class ChangePasswordResult
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public virtual bool IsSuccess { get; set; }

        /// <summary>
        /// 返回消息（成功则为“修改成功”，失败则为失败原因）
        /// </summary>
        public virtual string Message { get; set; }
    }
}
