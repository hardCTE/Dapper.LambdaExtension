﻿using System.Linq.Expressions;

namespace Dapper.LambdaExtension.LambdaSqlBuilder.Resolver.ExpressionTree
{
    class SingleOperationNode : Node
    {
        public ExpressionType Operator { get; set; }
        public Node Child { get; set; }
    }
}
