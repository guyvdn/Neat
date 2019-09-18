using System;
using System.Linq.Expressions;
using System.Reflection;

namespace GuyVdN.Neat
{
    public static class Guard
    {
        public static void GreaterThanZero(Expression<Func<int>> expression)
        {
            var param = (MemberExpression)expression.Body;
            var fieldInfo = (FieldInfo)param.Member;
            var paramValue = (int)fieldInfo.GetValue(((ConstantExpression)param.Expression).Value);

            if (paramValue <= 0)
            {
                throw new ArgumentOutOfRangeException(fieldInfo.Name, "Value should be greater than zero.");
            }
        }

        public static void GreaterThanZero(Expression<Func<double>> expression)
        {
            var param = (MemberExpression)expression.Body;
            var fieldInfo = (FieldInfo)param.Member;
            var paramValue = (double)fieldInfo.GetValue(((ConstantExpression)param.Expression).Value);

            if (paramValue <= 0d)
            {
                throw new ArgumentOutOfRangeException(fieldInfo.Name, "Value should be greater than zero.");
            }
        }

        public static void GreaterThanOrEqualToZero(Expression<Func<double>> expression)
        {
            var param = (MemberExpression)expression.Body;
            var fieldInfo = (FieldInfo)param.Member;
            var paramValue = (double)fieldInfo.GetValue(((ConstantExpression)param.Expression).Value);

            if (paramValue < 0d)
            {
                throw new ArgumentOutOfRangeException(fieldInfo.Name, "Value should be greater than or equal to zero.");
            }
        }
    }
}