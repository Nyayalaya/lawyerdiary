using AspNetCoreHero.Results;
using AspNetCoreHero.ThrowR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace CourtApp.Application.Extensions
{
    public static class QueryableExtensions
    {
        public static async Task<PaginatedResult<T>> ToPaginatedListAsync<T>(this IQueryable<T> source, int pageNumber, int pageSize) where T : class
        {
            Throw.Exception.IfNull(source, nameof(source));
            pageNumber = pageNumber == 0 ? 1 : pageNumber;
            pageSize = pageSize == 0 ? 10 : pageSize;
            long count = await source.LongCountAsync();
            pageNumber = pageNumber <= 0 ? 1 : pageNumber;
            List<T> items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return PaginatedResult<T>.Success(items, count, pageNumber, pageSize);
        }

        public static IQueryable<T> OrderByDynamic<T>(this IQueryable<T> source, string propertyName)
        {
            return ApplyOrder<T>(source, propertyName, "OrderBy");
        }

        public static IQueryable<T> OrderByDescendingDynamic<T>(this IQueryable<T> source, string propertyName)
        {
            return ApplyOrder<T>(source, propertyName, "OrderByDescending");
        }

        private static IQueryable<T> ApplyOrder<T>(IQueryable<T> source, string propertyName, string methodName)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
                return source;

            Type entityType = typeof(T);
            PropertyInfo property = entityType.GetProperty(propertyName,
                BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            if (property == null)
                return source; // Fallback if property not found

            ParameterExpression parameter = Expression.Parameter(entityType, "x");
            MemberExpression propertyAccess = Expression.Property(parameter, property);
            LambdaExpression orderByExpression = Expression.Lambda(propertyAccess, parameter);

            MethodCallExpression resultExp = Expression.Call(
                typeof(Queryable),
                methodName,
                new Type[] { entityType, property.PropertyType },
                source.Expression,
                Expression.Quote(orderByExpression)
            );

            return source.Provider.CreateQuery<T>(resultExp);
        }
    }
}