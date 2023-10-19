using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MichaelKappel.Repositories.EntityFrameworkRepositoryBase
{
    public static class PredicateBuilder
    {
        public static Expression<Func<T, bool>> New<T>(Expression<Func<T, bool>> expr) => expr;

        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expr1,
            Expression<Func<T, bool>> expr2)
        {
            return CombineExpressions(Expression.OrElse, expr1, expr2);
        }

        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expr1,
            Expression<Func<T, bool>> expr2)
        {
            return CombineExpressions(Expression.AndAlso, expr1, expr2);
        }

        private static Expression<Func<T, bool>> CombineExpressions<T>(
            Func<Expression, Expression, BinaryExpression> combineFunction,
            Expression<Func<T, bool>> expr1,
            Expression<Func<T, bool>> expr2)
        {
            var parameter = Expression.Parameter(typeof(T));

            var leftVisitor = new ReplaceExpressionVisitor(expr1.Parameters[0], parameter);
            var left = leftVisitor.Visit(expr1.Body);

            var rightVisitor = new ReplaceExpressionVisitor(expr2.Parameters[0], parameter);
            var right = rightVisitor.Visit(expr2.Body);

            return Expression.Lambda<Func<T, bool>>(combineFunction(left, right), parameter);
        }
    }

    internal class ReplaceExpressionVisitor : ExpressionVisitor
    {
        private readonly Expression _oldValue;
        private readonly Expression _newValue;

        public ReplaceExpressionVisitor(Expression oldValue, Expression newValue)
        {
            _oldValue = oldValue;
            _newValue = newValue;
        }

        public override Expression Visit(Expression node) => node == _oldValue ? _newValue : base.Visit(node);
    }

}
