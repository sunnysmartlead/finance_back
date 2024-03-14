using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Ext
{
    public static class Ip
    {
        /// <summary>
        /// 获取登录服务器的IP
        /// </summary>
        /// <returns></returns>
        public static string GetLoginAddr()
        {
            string loginIP = null;
            var ipList = new List<IPAddress>();
            var addressList = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
            foreach (var add in addressList)
            {
                try
                {
                    long a = add.ScopeId;
                }
                catch
                {
                    ipList.Add(add);
                }
            }
            if (ipList.Count > 0)
            {
                loginIP = ipList[0].ToString();
            }
            return loginIP;
        }
    }
}
