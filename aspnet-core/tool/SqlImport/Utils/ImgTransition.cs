using HandyControl.Controls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace SqlImport.Utils
{
    public static class ImgTransition
    {
        #region//图片转换为二进制流
        public static byte[] PictureToBinaryStream(string fullPath)
        {
            try
            {
                if (!string.IsNullOrEmpty(fullPath))
                {

                    byte[] bytes = File.ReadAllBytes(fullPath);
                    //MemoryStream memoryStream = new MemoryStream(bytes);
                    //Image image = Image.FromStream(memoryStream);
                    //Bitmap bitMap = new Bitmap(image);

                    return bytes;
                }
            }
            catch (Exception)
            {
            }
            return null;
        }
        #endregion
        #region 读取json文件
        public static string Readjson(string fullPath)
        {
            try
            {
                if (!string.IsNullOrEmpty(fullPath))
                {
                    using (System.IO.StreamReader file = System.IO.File.OpenText(fullPath))
                    {
                        using (JsonTextReader reader = new JsonTextReader(file))
                        {
                            string value = JsonConvert.SerializeObject(JToken.ReadFrom(reader));
                            return value;
                        }
                    }
                }              
            }
            catch (Exception)
            {
            }
            return null;
        }
        #endregion

    }
}
