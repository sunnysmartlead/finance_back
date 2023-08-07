using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Finance.Ext
{
    public static class RightJoinExtensions
    {
        public static IQueryable<TResult> RightJoin<TLeft, TRight, TKey, TResult>(
            this IQueryable<TLeft> left,
            IQueryable<TRight> right,
            Expression<Func<TLeft, TKey>> leftKey,
            Expression<Func<TRight, TKey>> rightKey,
            Func<TLeft, TRight, TResult> resultFunc
        )
        {
            var query = right.LeftJoin(left, rightKey, leftKey, (i, o) => resultFunc(o, i));
            return query;
        }

        /// <summary>
        /// 右连接
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
        public static IEnumerable<TResult> RightJoin<TLeft, TRight, TKey, TResult>(
            this IEnumerable<TLeft> left,
            IEnumerable<TRight> right,
            Func<TLeft, TKey> leftKey,
            Func<TRight, TKey> rightKey,
            Func<TLeft, TRight, TResult> resultFunc
        )
        {
            var query = right.LeftJoin(left, rightKey, leftKey, (i, o) => resultFunc(o, i));
            return query;
        }

    }
}
