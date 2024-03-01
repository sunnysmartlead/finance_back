//using Finance.EntityFrameworkCore;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Finance.Tests.Ext
//{
//    internal static class DbUpdate
//    {
//        /// <summary>
//        /// 将列表的内容更新到数据库。列表有数据库无，插入。
//        /// </summary>
//        internal async static void ToDb<T>(this FinanceDbContext _context, List<T> rolesList) 
//        {
//            var roles = _context.Roles.Select(p => p.Name).ToList();
//            var noDb = rolesList.Select(p => p.Name).Where(p => !roles.Contains(p));
//            if (noDb.Any())
//            {
//                _context.AddRange(rolesList.Where(p => noDb.Contains(p.Name)));
//                _context.SaveChanges();
//            }
//        }
//    }
//}
