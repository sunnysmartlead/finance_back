﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using LinqKit.Core;

namespace Finance.Ext
{
    public static class LeftJoinExtensions
    {
        public static IQueryable<TResult> LeftJoin<TLeft, TRight, TKey, TResult>(
            this IQueryable<TLeft> left, 
            IQueryable<TRight> right,
            Expression<Func<TLeft, TKey>> leftKey,
            Expression<Func<TRight, TKey>> rightKey,
            Func<TLeft, TRight, TResult> resultFunc
            )
        {
            /*
            var query = (
                    from l in left
                    join r in right on leftKey(l) equals rightKey(r)
                    into j1
                    from r1 in j1.DefaultIfEmpty()
                    select resultFunc(l, r1)
                    );
             */
//            var query = left
//                .GroupJoin(right, leftKey, rightKey, (l, j1) => new {l, j1})
//                .SelectMany(t => t.j1.DefaultIfEmpty(), (t, r1) => resultFunc(t.l, r1));
            var result = left
                .AsExpandable()// Tell LinqKit to convert everything into an expression tree.
                .GroupJoin(
                    right,
                    leftKey,
                    rightKey,
                    (outerItem, innerItems) => new { outerItem, innerItems })
                .SelectMany(
                    joinResult => joinResult.innerItems.DefaultIfEmpty(),
                    (joinResult, innerItem) => 
                        resultFunc(joinResult.outerItem, innerItem));

            return result;
        }     
        
        /// <summary>
        /// 左连接
        /// </summary>
        /// <typeparam name="TLeft"></typeparam>
        /// <typeparam name="TRight"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="leftKey"></param>
        /// <param name="rightKey"></param>
        /// <param name="resultFunc"></param>
        /// <returns></returns>
        public static IEnumerable<TResult> LeftJoin<TLeft, TRight, TKey, TResult>(
            this IEnumerable<TLeft> left, 
            IEnumerable<TRight> right,
            Func<TLeft, TKey> leftKey,
            Func<TRight, TKey> rightKey,
            Func<TLeft, TRight, TResult> resultFunc
            )
        {
            /*
            var query = (
                    from l in left
                    join r in right on leftKey(l) equals rightKey(r)
                    into j1
                    from r1 in j1.DefaultIfEmpty()
                    select resultFunc(l, r1)
                    );
             */
            var query = left
                .GroupJoin(right, leftKey, rightKey, (l, j1) => new {l, j1})
                .SelectMany(t => t.j1.DefaultIfEmpty(), (t, r1) => resultFunc(t.l, r1));
            return query;
        }
    }
}
