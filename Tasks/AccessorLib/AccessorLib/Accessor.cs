using System.Linq.Expressions;
using System.Reflection;

namespace AccessorLib;

public class Accessor
{
    public static Func<T, ReturnT?> CreatePropertyAccessor<T, ReturnT>(string path)
    {
        string[] properties = path.Split('.');
        Type varType = typeof(T);
        var param = Expression.Parameter(varType);
        Expression member_expr = param;

        PropertyInfo? pInfo;
        foreach (var p in properties)
        {
            pInfo = varType.GetProperty(p, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
            if (pInfo != null)
            {
                member_expr = Expression.Property(null, pInfo);
            }
            else
            {
                pInfo = varType.GetProperty(p, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                if (pInfo == null)
                {
                    string errorMsg = $"{p} property doesn't exist!";
                    throw new ArgumentException(errorMsg);
                }
                member_expr = Expression.Property(member_expr, pInfo);
            }

            varType = pInfo.PropertyType;
        }
        var lExpr = Expression.Lambda<Func<T, ReturnT?>>(member_expr, param);
        return lExpr.Compile();
    }
}
