using Abp;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.EntityFrameworkCore;
using AutoMapper;
using Finance.Audit;
using Finance.Audit.Dto;
using Finance.Authorization.Users;
using Finance.EntityFrameworkCore;
using Finance.Job;
using Finance.MakeOffers;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Sundial;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Policy;
using System.Threading;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Finance.Job
{
    public class MailJob : IJob
    {
        private static readonly Lazy<MailAppService> lazy =new Lazy<MailAppService>(() => new MailAppService());
        public static MailAppService Instance { get { return lazy.Value; } }
        public async Task ExecuteAsync(JobExecutingContext context, CancellationToken stoppingToken)
        {
            Instance.GetIsMain();
        }
    }
}
