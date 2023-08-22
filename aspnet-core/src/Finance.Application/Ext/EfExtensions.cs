using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Ext
{
    /// <summary>
    /// EF 扩展方法
    /// </summary>
    public static class EfExtensions
    {
        /// <summary>
        /// 多条数据插入扩展方法
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TPrimaryKey"></typeparam>
        /// <param name="repository"></param>
        /// <param name="entities"></param>
        /// <returns></returns>
        public static async Task<List<TEntity>> BulkInsertAsync<TEntity, TPrimaryKey>(this IRepository<TEntity, TPrimaryKey> repository, List<TEntity> entities) where TEntity : class, IEntity<TPrimaryKey>
        {
            try
            {
                if (entities == null || entities.Count == 0)
                {
                    return null;
                }
                var dbContext = repository.GetDbContext();
                var entitySet = dbContext.Set<TEntity>();
                await entitySet.AddRangeAsync(entities);
                await dbContext.SaveChangesAsync();
                return entities;
            }
            catch (Exception ex)
            {
                throw new FriendlyException(ex.Message);
            }

        }
        /// <summary>
        /// 插入多条数据的时候如果 数据id在数据库存在则修改不存在则添加
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TPrimaryKey"></typeparam>
        /// <param name="repository"></param>
        /// <param name="entities"></param>
        /// <returns></returns>
        [Obsolete("已报废,不推荐使用")]
        public static async Task<List<TEntity>> BulkInsertOrUpdateAsyncpObsolete<TEntity, TPrimaryKey>(this IRepository<TEntity, TPrimaryKey> repository, List<TEntity> entities) where TEntity : class, IEntity<TPrimaryKey> where TPrimaryKey : struct, IEquatable<TPrimaryKey>
        {
            try
            {
                if (entities == null || entities.Count == 0)
                {
                    return null;
                }
                var dbContext = repository.GetDbContext();
                var entitySet = dbContext.Set<TEntity>();
                var existingEntities = await entitySet.ToListAsync();
                foreach (var entity in entities)
                {
                    var existingEntity = existingEntities.FirstOrDefault(e => e.Id.Equals(entity.Id));
                    if (existingEntity != null)
                    {
                        dbContext.Entry(existingEntity).CurrentValues.SetValues(entity);
                    }
                    else
                    {
                        await entitySet.AddAsync(entity);
                    }
                }
                await dbContext.SaveChangesAsync();
                return entities;
            }
            catch (Exception ex)
            {
                throw new FriendlyException(ex.Message);
            }
        }
        /// <summary>
        /// 插入多条数据的时候如果 数据id在数据库存在则修改不存在则添加
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TPrimaryKey"></typeparam>
        /// <param name="repository"></param>
        /// <param name="entities"></param>
        /// <returns></returns>
        public static async Task<List<TEntity>> BulkInsertOrUpdateAsync<TEntity, TPrimaryKey>(this IRepository<TEntity, TPrimaryKey> repository, List<TEntity> entities) where TEntity : class, IEntity<TPrimaryKey> where TPrimaryKey : struct, IEquatable<TPrimaryKey>
        {
            try
            {
                if (entities == null || entities.Count == 0)
                {
                    return null;
                }
                var dbContext = repository.GetDbContext();
                var entitySet = dbContext.Set<TEntity>();
                var existingIds = await entitySet.Select(e => e.Id).ToListAsync();
                var newEntities = new List<TEntity>();
                foreach (var entity in entities)
                {
                    if (existingIds.Contains(entity.Id))
                    {
                        dbContext.Attach(entity);
                        dbContext.Entry(entity).State = EntityState.Modified;
                    }
                    else
                    {
                        newEntities.Add(entity);
                    }
                }
                if (newEntities.Count > 0)
                {
                    await entitySet.AddRangeAsync(newEntities);
                }
                await dbContext.SaveChangesAsync();
                return entities;
            }
            catch (Exception ex)
            {
                throw new FriendlyException(ex.Message);
            }
        }
        /// <summary>
        /// 插入一条数据的时候如果 数据id在数据库存在则修改不存在则添加
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TPrimaryKey"></typeparam>
        /// <param name="repository"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static async Task<TEntity> BulkInsertOrUpdateAsync<TEntity, TPrimaryKey>(this IRepository<TEntity, TPrimaryKey> repository, TEntity entity) where TEntity : class, IEntity<TPrimaryKey> where TPrimaryKey : struct, IEquatable<TPrimaryKey>
        {
            try
            {
                var dbContext = repository.GetDbContext();
                var entitySet = dbContext.Set<TEntity>();
                var existingEntity = await entitySet.FindAsync(entity.Id);
                if (existingEntity != null)
                {
                    dbContext.Entry(existingEntity).CurrentValues.SetValues(entity);
                }
                else
                {
                    await entitySet.AddAsync(entity);
                }
                await dbContext.SaveChangesAsync();
                return entity;
            }
            catch (Exception ex)
            {
                throw new FriendlyException(ex.Message);
            }
        }
    }


}

