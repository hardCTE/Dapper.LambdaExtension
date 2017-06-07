﻿using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
 
using System.Text;
using Dapper.LambdaExtension.LambdaSqlBuilder;

 

namespace Dapper.LambdaExtension.Extentions
{
    public static class DapperLambdaExt
    {
        static DapperLambdaExt()
        {
            PreApplicationStart.RegisterTypeMaps();
        }

        public static string GetParameterString(IDictionary<string, object> dic)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in dic)
            {
                sb.AppendFormat("Key: {0}, Value: {1}", item.Key, item.Value);
                sb.AppendLine();
            }
            return sb.ToString();
        }
        public static IEnumerable<T> Query<T>(this IDbConnection db, Expression<Func<T, bool>> wherExpression=null, IDbTransaction trans = null, int? commandTimeout = null)
        {
            var sqllam = new SqlExp<T>(db.GetAdapter());

            if (wherExpression != null)
            {
                sqllam = sqllam.Where(wherExpression);
            }
             

            return db.Query<T>(sqllam.SqlString, sqllam.Parameters,trans,commandTimeout:commandTimeout);

        }

        public static T QueryFirstOrDefault<T>(this IDbConnection db, Expression<Func<T, bool>> wherExpression = null, IDbTransaction trans = null, int? commandTimeout = null)
        {

            var sqllam = new SqlExp<T>(db.GetAdapter());

            if (wherExpression != null)
            {
                sqllam = sqllam.Where(wherExpression);
            }
 
            return db.QueryFirstOrDefault<T>(sqllam.SqlString, sqllam.Parameters, trans, commandTimeout: commandTimeout);

        }

        public static int Insert<T>(this IDbConnection db, T entity,IDbTransaction trans=null,int? commandTimeout=null)
        {

            var sqllam = new SqlExp<T>(db.GetAdapter());

             
                sqllam = sqllam.Insert(entity);
 
            return db.Execute(sqllam.SqlString,entity,trans,commandTimeout,CommandType.Text);

        }

        public static int InsertList<T>(this IDbConnection db, IEnumerable<T> entitys, IDbTransaction trans = null, int? commandTimeout = null)
        {

            var sqllam = new SqlExp<T>(db.GetAdapter());


            sqllam = sqllam.Insert(entitys.FirstOrDefault());

 
            return db.Execute(sqllam.SqlString, entitys, trans, commandTimeout, CommandType.Text);

        }
        public static int Update<T>(this IDbConnection db, T entity, IDbTransaction trans = null, int? commandTimeout = null)
        {

            var sqllam = new SqlExp<T>(db.GetAdapter());


            sqllam = sqllam.Update(entity);
 
            return db.Execute(sqllam.SqlString, entity, trans, commandTimeout, CommandType.Text);

        }


      

        public static int UpdateList<T>(this IDbConnection db, IEnumerable<T> entitys, IDbTransaction trans = null, int? commandTimeout = null)
        {

            var sqllam = new SqlExp<T>(db.GetAdapter());


            sqllam = sqllam.Update(entitys.FirstOrDefault());
 

            return db.Execute(sqllam.SqlString, entitys, trans, commandTimeout, CommandType.Text);

        }



        public static IEnumerable<T> Query<T>(this IDbConnection db, Action<SqlExp<T>> action, IDbTransaction trans = null, int? commandTimeout = null)
        {

            var sqllam = new SqlExp<T>(db.GetAdapter());

            action?.Invoke(sqllam);
 

            return db.Query<T>(sqllam.SqlString, sqllam.Parameters,trans,commandTimeout:commandTimeout);

        }
        public static int Delete<T>(this IDbConnection db, T engity, IDbTransaction trans = null, int? commandTimeout = null)
        {

            var sqllam = new SqlExp<T>(db.GetAdapter());


            sqllam = sqllam.Delete();
 

            return db.Execute(sqllam.SqlString, engity, trans, commandTimeout, CommandType.Text);

        }

        public static int DeleteList<T>(this IDbConnection db, IEnumerable<T> engityList, IDbTransaction trans = null, int? commandTimeout = null)
        {

            var sqllam = new SqlExp<T>(db.GetAdapter());


            sqllam = sqllam.Delete();
 

            return db.Execute(sqllam.SqlString, engityList, trans, commandTimeout, CommandType.Text);

        }

        public static int Delete<T>(this IDbConnection db, Expression<Func<T,bool>> deleteExpression, IDbTransaction trans = null, int? commandTimeout = null)
        {
            if (deleteExpression == null)
            {
                throw new Exception("delete expression is null!");
            }

            var sqllam = new SqlExp<T>(db.GetAdapter());


            sqllam = sqllam.Delete(deleteExpression);
 
            return db.Execute(sqllam.SqlString, sqllam.Parameters, trans, commandTimeout, CommandType.Text);

        }


        public static PagedResult<T> PagedQuery<T>(this IDbConnection db,int pageSize,int pageNumber, Expression<Func<T, bool>> whereExpression = null, Expression<Func<T, object>> groupByexpression=null, IDbTransaction trans = null, int? commandTimeout = null,  Expression<Func<T, object>> orderbyExpression=null)
            where T:class
        {

            var sqllam = new SqlExp<T>(db.GetAdapter());
            var countSqlam = new SqlExp<T>(db.GetAdapter());
            if (whereExpression != null)
            {
                sqllam = sqllam.Where(whereExpression);
                countSqlam = countSqlam.Where(whereExpression);
            }

            if (orderbyExpression != null)
            {
                sqllam = sqllam.OrderBy(orderbyExpression);
            }

            if (groupByexpression != null)
            {
                sqllam = sqllam.GroupBy(groupByexpression);
            }

            countSqlam = countSqlam.Count();

            var countRet = db.Query<int>(countSqlam.SqlString, countSqlam.Parameters).FirstOrDefault();

            var sqlstring = sqllam.QueryPage(pageSize, pageNumber);

            var retlist = db.Query<T>(sqlstring, sqllam.Parameters, trans, commandTimeout: commandTimeout);

            return new PagedResult<T>(retlist, countRet, pageSize, pageNumber);

        }

        public static PagedResult<T> PagedQuery<T>(this IDbConnection db, int pageSize, int pageNumber, Action<SqlExp<T>> action=null, IDbTransaction trans = null, int? commandTimeout = null) where T :class
        {

            var sqllam = new SqlExp<T>(db.GetAdapter());

            var countSqlam = new SqlExp<T>(db.GetAdapter());

            if (action != null)
            {
                action?.Invoke(sqllam);

                action?.Invoke(countSqlam);
            }

            countSqlam = countSqlam.Count();

            var countRet =  db.Query<int>(countSqlam.SqlString, countSqlam.Parameters, trans, commandTimeout: commandTimeout).FirstOrDefault(); 
          
            var sqlstring = sqllam.QueryPage(pageSize, pageNumber);

            var retlist = db.Query<T>(sqlstring, sqllam.Parameters,trans,commandTimeout:commandTimeout);
 
            return new PagedResult<T>(retlist, countRet, pageSize, pageNumber);

        }

        public static IEnumerable<TResult> Query<TEntity, TResult>(this IDbConnection db, Action<SqlExp<TEntity>> action = null,
            IDbTransaction trans = null, int? commandTimeout = null) where TEntity : class
        {
            var sqllam = new SqlExp<TEntity>(db.GetAdapter());

            action?.Invoke(sqllam);

            return db.Query<TResult>(sqllam.SqlString, sqllam.Parameters, trans, commandTimeout: commandTimeout);

        }


        public static TResult ExecuteScalar<TEntity, TResult>(this IDbConnection db, Action<SqlExp<TEntity>> action = null,
            IDbTransaction trans = null, int? commandTimeout = null) where TEntity : class
        {
            var sqllam = new SqlExp<TEntity>(db.GetAdapter());

            action?.Invoke(sqllam);

            return db.ExecuteScalar<TResult>(sqllam.SqlString, sqllam.Parameters, trans, commandTimeout);

        }
    }
}
