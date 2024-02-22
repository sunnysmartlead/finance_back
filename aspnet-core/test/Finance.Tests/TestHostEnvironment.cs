using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Tests
{
    public class TestHostEnvironment : IWebHostEnvironment
    {
        public string EnvironmentName { get { return Environments.Development; } set { EnvironmentName = value; } }
        public string ContentRootPath { get { return Directory.GetCurrentDirectory(); } set { ContentRootPath = value; } }
        
        public string WebRootPath { get { return Path.Combine(ContentRootPath, "wwwroot"); } set { WebRootPath = value; } }
        
        public IFileProvider WebRootFileProvider { get; set; } // 或者你可以提供一个实际的文件提供者  

        public string ApplicationName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IFileProvider ContentRootFileProvider { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
