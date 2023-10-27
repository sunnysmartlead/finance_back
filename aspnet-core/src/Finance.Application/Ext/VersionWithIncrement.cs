using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.BaseLibrary
{
    public class VersionWithIncrement
    {
        private int major = 0;
        private int minor = 0;
        private int build = 0;
        private int revision = 0;

        /// <summary>
        /// 版本初始化
        /// </summary>
        /// <param name="Version">格式：x.x.x.x;默认：1.0.0.0</param>
        /// <exception cref="Exception"></exception>
        public VersionWithIncrement(string Version = "1.0.0.0")
        {
            try
            {
                string[] vs = Version.Split('.');
                int major = 0;
                int.TryParse(vs[0], out major);
                int minor = 0;
                int.TryParse(vs[1], out minor);
                int build = 0;
                int.TryParse(vs[2], out build);
                int revision = 0;
                int.TryParse(vs[3], out revision);

                this.major = major;
                this.minor = minor;
                this.build = build;
                this.revision = revision;
            }
            catch (Exception ex)
            {

                throw new Exception("版本格式不正确，请使用正确的版本个数。");
            }
        }

        public string IncrementRevision()
        {
            if (revision >= 10)
            {
                revision = 0;
                if (build >= 10)
                {
                    build = 0;
                    if (minor >= 10)
                    {
                        minor = 0;
                        major++;
                    }
                    else
                    {
                        minor++;
                    }
                }
                else
                {
                    build++;
                }
            }
            else
            {
                revision++;
            }
            return $"{major}.{minor}.{build}.{revision}";
        }
    }
}
