﻿using System.Linq.Expressions;

namespace Dapper.LambdaExtension.LambdaSqlBuilder.Resolver.ExpressionTree
{
    class OperationNode : Node
    {
        public ExpressionType Operator { get; set; }
        public Node Left { get; set; }
        public Node Right { get; set; }
    }
}
