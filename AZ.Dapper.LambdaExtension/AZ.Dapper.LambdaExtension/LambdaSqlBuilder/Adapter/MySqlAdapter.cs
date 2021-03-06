﻿using System;
using Dapper.LambdaExtension.LambdaSqlBuilder.Attributes;
using Dapper.LambdaExtension.LambdaSqlBuilder.Entity;

namespace Dapper.LambdaExtension.LambdaSqlBuilder.Adapter
{
    
    class MySqlAdapter : AdapterBase
    {
        public override string AutoIncrementDefinition { get; } = "AUTO_INCREMENT";
 

        public override string ParamStringPrefix { get; } = "@";

        public override string PrimaryKeyDefinition { get; } = " Primary Key";
        public override string SelectIdentitySql { get; set; } = "SELECT LAST_INSERT_ID()";

        public MySqlAdapter()
            : base(SqlConst.LeftTokens[1], SqlConst.RightTokens[1], SqlConst.ParamPrefixs[0])
        {

        }
        
        public override string QueryPage(SqlEntity entity)
        {
            int pageSize = entity.PageSize;
            int limit = pageSize * (entity.PageNumber - 1);

            return string.Format("SELECT {0} FROM {1} {2} {3} LIMIT {4},{5}", entity.Selection, entity.TableName, entity.Conditions, entity.OrderBy, limit, pageSize);
        }

        public override string Field(string filedName)
        {
            return string.Format("{0}{1}{2}", _leftToken, filedName, _rightToken);
        }

        public override string Field(string tableName, string fieldName)
        {
            return fieldName;//string.Format("{1}", this.Table(tableName), this.Field(fieldName));
        }

        public override string CreateTablePrefix
        {
            get { return "CREATE TABLE if not EXISTS "; }
        }

        protected override string DbTypeBoolean(string fieldLength)
        {
            return "TINYINT(1)";
        }

        public override string DropTableSql(string tableName, string tableSchema)
        {
            var tablename = tableName;
            if (!string.IsNullOrEmpty(tableSchema))
            {
                tablename = $"{tableSchema}.{tablename}";
            }

            return $" DROP TABLE IF EXISTS {tablename}";
        }
    }
}
