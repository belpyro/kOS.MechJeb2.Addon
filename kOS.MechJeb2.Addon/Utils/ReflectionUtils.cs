using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using kOS.Safe.Exceptions;

namespace kOS.MechJeb2.Addon.Utils
{
    public delegate object LateBoundMethod(object target, object[] args);

    public static class ReflectionUtils
    {
        public static Delegate BuildTypedGetter(this MemberInfo member)
        {
            // (object obj) => (resultType)((DeclaringType)obj).Field
            var objParam = Expression.Parameter(typeof(object), "obj");
            var castObj = Expression.Convert(objParam, member.DeclaringType);
            Expression valueExpr;
            Type memberType;
            if (member is PropertyInfo pi)
            {
                valueExpr = Expression.Property(castObj, pi);
                memberType = pi.PropertyType;
            }
            else if (member is FieldInfo fi)
            {
                valueExpr = Expression.Field(castObj, fi);
                memberType = fi.FieldType;
            }
            else
                throw new NotSupportedException("Only fields and properties are supported");

            var delegateType = typeof(Func<,>).MakeGenericType(typeof(object), memberType);

            return Expression.Lambda(delegateType, valueExpr, objParam).Compile();
        }

        private static readonly Dictionary<MethodInfo, LateBoundMethod> methodCache =
            new Dictionary<MethodInfo, LateBoundMethod>();

        private static readonly object methodCacheLock = new object();

        public static LateBoundMethod GetOrCreateMethodInvoker(this MethodInfo method)
        {
            if (method == null) throw new ArgumentNullException(nameof(method));

            lock (methodCacheLock)
            {
                LateBoundMethod invoker;
                if (methodCache.TryGetValue(method, out invoker))
                    return invoker;

                invoker = BuildMethodInvoker(method);
                methodCache[method] = invoker;
                return invoker;
            }
        }

        public static LateBoundMethod GetMethodInvoker(
            this Type type,
            string name,
            BindingFlags flags,
            params Type[] parameterTypes)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (name == null) throw new ArgumentNullException(nameof(name));

            var method = type.GetMethod(name, flags, null, parameterTypes ?? Type.EmptyTypes, null);
            if (method == null)
                throw new MissingMethodException(type.FullName, name);

            return GetOrCreateMethodInvoker(method);
        }

        private static LateBoundMethod BuildMethodInvoker(MethodInfo method)
        {
            var targetParam = Expression.Parameter(typeof(object), "target");
            var argsParam = Expression.Parameter(typeof(object[]), "args");

            // this / instance
            Expression instanceExpr = null;
            if (!method.IsStatic)
            {
                var declaringType = method.DeclaringType
                                    ?? throw new InvalidOperationException("Method has no declaring type");
                instanceExpr = Expression.Convert(targetParam, declaringType);
            }

            var methodParams = method.GetParameters();
            var argExprs = new Expression[methodParams.Length];

            for (int i = 0; i < methodParams.Length; i++)
            {
                var indexExpr = Expression.Constant(i);
                var arrayAccess = Expression.ArrayIndex(argsParam, indexExpr);
                argExprs[i] = Expression.Convert(arrayAccess, methodParams[i].ParameterType);
            }

            Expression callExpr = Expression.Call(instanceExpr, method, argExprs);

            Expression body;
            if (method.ReturnType == typeof(void))
            {
                var nullConst = Expression.Constant(null);
                body = Expression.Block(callExpr, nullConst);
            }
            else
            {
                body = Expression.Convert(callExpr, typeof(object));
            }

            var lambda = Expression.Lambda<LateBoundMethod>(body, targetParam, argsParam);
            return lambda.Compile();
        }

        public static bool HasAttributeNamed(this ICustomAttributeProvider provider, string attributeName) =>
            provider.GetCustomAttributes(inherit: false).Select(attr => attr.GetType())
                .Any(attr => attr.Name.Equals(attributeName, StringComparison.OrdinalIgnoreCase)
                             || attr.FullName.Equals(attributeName, StringComparison.OrdinalIgnoreCase));

        public static Func<object, double> BuildDoubleGetter(this MemberInfo member)
        {
            var objParam = Expression.Parameter(typeof(object), "obj");
            var declaringType = member.DeclaringType ?? throw new KOSException("Member has no declaring type");

            var castObj = Expression.Convert(objParam, declaringType);

            Expression valueExpr;
            Type valueType;

            switch (member)
            {
                case FieldInfo field:
                    valueExpr = Expression.Field(castObj, field);
                    valueType = field.FieldType;
                    break;
                case PropertyInfo prop:
                    valueExpr = Expression.Property(castObj, prop);
                    valueType = prop.PropertyType;
                    break;
                default:
                    throw new KOSException("Only fields and properties are supported");
            }

            if (valueType != typeof(double))
            {
                valueExpr = Expression.Convert(valueExpr, typeof(double));
            }

            var lambda = Expression.Lambda<Func<object, double>>(valueExpr, objParam);
            return lambda.Compile();
        }

        public static Func<object, string> BuildStringMethod(this MethodInfo method)
        {
            var objParam = Expression.Parameter(typeof(object), "obj");
            var declaringType = method.DeclaringType ??
                                throw new InvalidOperationException("Method has no declaring type");
            var castObj = Expression.Convert(objParam, declaringType);

            var call = Expression.Call(castObj, method);
            var body = Expression.Convert(call, typeof(string));

            var lambda = Expression.Lambda<Func<object, string>>(body, objParam);
            return lambda.Compile();
        }

        public static Func<object, double> BuildDoubleMethod(this MethodInfo method)
        {
            var objParam = Expression.Parameter(typeof(object), "obj");
            var declaringType = method.DeclaringType ??
                                throw new InvalidOperationException("Method has no declaring type");
            var castObj = Expression.Convert(objParam, declaringType);

            var call = Expression.Call(castObj, method);
            var body = Expression.Convert(call, typeof(double));

            var lambda = Expression.Lambda<Func<object, double>>(body, objParam);
            return lambda.Compile();
        }

        public static Func<object, int> BuildIntMethod(this MethodInfo method)
        {
            var objParam = Expression.Parameter(typeof(object), "obj");
            var declaringType = method.DeclaringType ??
                                throw new InvalidOperationException("Method has no declaring type");
            var castObj = Expression.Convert(objParam, declaringType);

            var call = Expression.Call(castObj, method);
            var body = Expression.Convert(call, typeof(int));

            var lambda = Expression.Lambda<Func<object, int>>(body, objParam);
            return lambda.Compile();
        }

        public static Func<object, float> BuildFloatMethod(this MethodInfo method)
        {
            var objParam = Expression.Parameter(typeof(object), "obj");
            var declaringType = method.DeclaringType ??
                                throw new InvalidOperationException("Method has no declaring type");
            var castObj = Expression.Convert(objParam, declaringType);

            var call = Expression.Call(castObj, method);
            var body = Expression.Convert(call, typeof(float));

            var lambda = Expression.Lambda<Func<object, float>>(body, objParam);
            return lambda.Compile();
        }

        public static Func<object[], object> MakeStaticMethodWithArgs(this MethodInfo method)
        {
            if (method == null) throw new ArgumentNullException(nameof(method));
            if (!method.IsStatic)
                throw new NotSupportedException("MakeStaticMethodWithArgs supports only static methods.");

            var argsInfo = method.GetParameters();
            var arrParam = Expression.Parameter(typeof(object[]), "args");

            var locals = new List<ParameterExpression>();
            var callArgs = new List<Expression>();
            var outVars = new List<ParameterExpression>();

            // Разбираем параметры метода
            for (int i = 0; i < argsInfo.Length; i++)
            {
                var p = argsInfo[i];

                if (p.IsOut)
                {
                    var elementType = p.ParameterType.GetElementType()
                                      ?? throw new InvalidOperationException("Out parameter has no element type.");
                    var varExpr = Expression.Variable(elementType, p.Name ?? ("out" + i));
                    locals.Add(varExpr);
                    outVars.Add(varExpr);
                    callArgs.Add(varExpr); // ref/out передаём саму переменную
                }
                else
                {
                    var index = Expression.Constant(i);
                    var access = Expression.ArrayIndex(arrParam, index);
                    var cast = Expression.Convert(access, p.ParameterType);
                    callArgs.Add(cast);
                }
            }

            if (outVars.Count > 1)
                throw new NotSupportedException("Only methods with 0 or 1 out parameter are supported.");

            var callExpr = Expression.Call(method, callArgs);

            Expression body;

            // Случай 0: нет out-параметров
            if (outVars.Count == 0)
            {
                if (method.ReturnType == typeof(void))
                {
                    // { call; return null; }
                    body = Expression.Block(
                        callExpr,
                        Expression.Constant(null, typeof(object))
                    );
                }
                else
                {
                    // { return (object) call; }
                    body = Expression.Convert(callExpr, typeof(object));
                }

                return Expression.Lambda<Func<object[], object>>(body, arrParam).Compile();
            }

            // Тут outVars.Count == 1
            var outVar = outVars[0];

            // Случай 1: есть out, но метод void → как раньше: возвращаем только out
            if (method.ReturnType == typeof(void))
            {
                // { TOut outVar; call; return (object)outVar; }
                body = Expression.Block(
                    locals,
                    callExpr,
                    Expression.Convert(outVar, typeof(object))
                );

                return Expression.Lambda<Func<object[], object>>(body, arrParam).Compile();
            }

            // Случай 2: есть out и есть результат → возвращаем (result, outVar) как ValueTuple
            var resultVar = Expression.Variable(method.ReturnType, "result");
            locals.Insert(0, resultVar); // чтобы result тоже был локалом

            var assignResult = Expression.Assign(resultVar, callExpr);

            var tupleType = typeof(ValueTuple<,>).MakeGenericType(method.ReturnType, outVar.Type);
            var ctor = tupleType.GetConstructor(new[] { method.ReturnType, outVar.Type })
                       ?? throw new InvalidOperationException("Tuple constructor not found.");

            var newTuple = Expression.New(ctor, resultVar, outVar);
            var boxedTuple = Expression.Convert(newTuple, typeof(object));

            body = Expression.Block(
                locals,
                assignResult,
                boxedTuple
            );

            return Expression.Lambda<Func<object[], object>>(body, arrParam).Compile();
        }
    }
}