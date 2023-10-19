using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Ext
{
    /// <summary>
    /// 深拷贝
    /// </summary>
    public static class ObjectExtensions
    {
        public static T DeepClone<T>(this T obj)
        {
            return  JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(obj));             
        }
    }


}
