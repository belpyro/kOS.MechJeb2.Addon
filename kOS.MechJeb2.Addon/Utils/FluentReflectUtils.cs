using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace kOS.MechJeb2.Addon.Utils
{
    public static class Reflect
    {
        public static ReflectContext On(object instance) =>
            new ReflectContext(instance);

        public static ReflectTypeContext OnType(Type type) =>
            new ReflectTypeContext(type);
    }

    #region Cache

    internal static class ReflectionCache
    {
        private static readonly ConcurrentDictionary<string, MemberInfo> _memberCache =
            new ConcurrentDictionary<string, MemberInfo>();
        
        private static readonly ConcurrentDictionary<string, Delegate> _delegateCache =
            new ConcurrentDictionary<string, Delegate>();

        public static MemberInfo GetOrAddMember(
            Type type,
            string name,
            MemberKinds kind,
            Type[] paramTypes,
            Func<MemberInfo> factory)
        {
            var signatureKey = paramTypes != null && paramTypes.Length > 0
                ? string.Join(",", paramTypes.Select(t => t.FullName))
                : string.Empty;

            var key = $"{type.AssemblyQualifiedName}|{kind}|{name}|{signatureKey}";

            return _memberCache.GetOrAdd(key, _ =>
            {
                var member = factory();
                if (member == null)
                    throw new MissingMemberException(type.FullName, name);
                return member;
            });
        }

        public static TDelegate GetOrAddDelegate<TDelegate>(
            MemberInfo member,
            string delegateKind,
            Type resultOrValueType,
            Func<TDelegate> factory)
            where TDelegate : Delegate
        {
            var key = $"{member.DeclaringType.AssemblyQualifiedName}|{member.MetadataToken}|{delegateKind}|{resultOrValueType.AssemblyQualifiedName}";

            var del = _delegateCache.GetOrAdd(key, _ => factory());
            return (TDelegate)del;
        }

        internal enum MemberKinds
        {
            Property,
            Field,
            Method
        }
    }

    #endregion

    #region Contexts

    public class ReflectContext
    {
        public object Instance { get; }
        public Type Type => Instance.GetType();

        public ReflectContext(object instance)
        {
            Instance = instance ?? throw new ArgumentNullException(nameof(instance));
        }

        public PropertyContext Property(string name) =>
            Reflect.OnType(Type).Property(name);

        public FieldContext Field(string name) =>
            Reflect.OnType(Type).Field(name);

        public MethodContext Method(string name) =>
            Reflect.OnType(Type).Method(name).BindToInstance(Instance);
    }

    public class ReflectTypeContext
    {
        public Type Type { get; }

        private static readonly BindingFlags Flags =
            BindingFlags.Instance | BindingFlags.Static |
            BindingFlags.Public | BindingFlags.NonPublic |
            BindingFlags.IgnoreCase;

        public ReflectTypeContext(Type type)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
        }

        public PropertyContext Property(string name)
        {
            var member = (PropertyInfo)ReflectionCache.GetOrAddMember(
                Type,
                name,
                ReflectionCache.MemberKinds.Property,
                null,
                () => Type.GetProperty(name, Flags));

            return new PropertyContext(this, member);
        }

        public FieldContext Field(string name)
        {
            var member = (FieldInfo)ReflectionCache.GetOrAddMember(
                Type,
                name,
                ReflectionCache.MemberKinds.Field,
                null,
                () => Type.GetField(name, Flags));

            return new FieldContext(this, member);
        }

        public MethodContext Method(string name)
        {
            // overloads
            var methods = Type
                .GetMethods(Flags)
                .Where(m => m.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                .ToArray();

            if (methods.Length == 0)
                throw new MissingMethodException(Type.FullName, name);

            return new MethodContext(this, methods);
        }
    }

    #endregion

    #region Property

    public class PropertyContext
    {
        private readonly ReflectTypeContext _ctx;
        private readonly PropertyInfo _prop;

        public PropertyContext(ReflectTypeContext ctx, PropertyInfo prop)
        {
            _ctx = ctx;
            _prop = prop ?? throw new MissingMemberException("Property not found.");
        }

        public Func<object, T> AsGetter<T>()
        {
            return ReflectionCache.GetOrAddDelegate<Func<object, T>>(
                _prop,
                "getter",
                typeof(T),
                BuildGetter<T>);
        }

        public Action<object, T> AsSetter<T>()
        {
            if (!_prop.CanWrite)
                throw new InvalidOperationException($"Property {_prop.Name} has no setter.");

            return ReflectionCache.GetOrAddDelegate<Action<object, T>>(
                _prop,
                "setter",
                typeof(T),
                BuildSetter<T>);
        }

        private Func<object, T> BuildGetter<T>()
        {
            var objParam = Expression.Parameter(typeof(object), "obj");
            var castObj = Expression.Convert(objParam, _prop.DeclaringType!);
            var access = Expression.Property(castObj, _prop);
            var convert = Expression.Convert(access, typeof(T));
            return Expression.Lambda<Func<object, T>>(convert, objParam).Compile();
        }

        private Action<object, T> BuildSetter<T>()
        {
            var objParam = Expression.Parameter(typeof(object), "obj");
            var valParam = Expression.Parameter(typeof(T), "value");

            var castObj = Expression.Convert(objParam, _prop.DeclaringType!);
            var castVal = Expression.Convert(valParam, _prop.PropertyType);
            var access = Expression.Property(castObj, _prop);
            var assign = Expression.Assign(access, castVal);

            return Expression.Lambda<Action<object, T>>(assign, objParam, valParam).Compile();
        }
    }

    #endregion

    #region Field

    public class FieldContext
    {
        private readonly ReflectTypeContext _ctx;
        private readonly FieldInfo _field;

        public FieldContext(ReflectTypeContext ctx, FieldInfo field)
        {
            _ctx = ctx;
            _field = field ?? throw new MissingMemberException("Field not found.");
        }

        public Func<object, T> AsGetter<T>()
        {
            return ReflectionCache.GetOrAddDelegate<Func<object, T>>(
                _field,
                "getter",
                typeof(T),
                BuildGetter<T>);
        }

        public Action<object, T> AsSetter<T>()
        {
            return ReflectionCache.GetOrAddDelegate<Action<object, T>>(
                _field,
                "setter",
                typeof(T),
                BuildSetter<T>);
        }

        private Func<object, T> BuildGetter<T>()
        {
            var objParam = Expression.Parameter(typeof(object), "obj");
            var castObj = Expression.Convert(objParam, _field.DeclaringType!);
            var access = Expression.Field(castObj, _field);
            var convert = Expression.Convert(access, typeof(T));
            return Expression.Lambda<Func<object, T>>(convert, objParam).Compile();
        }

        private Action<object, T> BuildSetter<T>()
        {
            var objParam = Expression.Parameter(typeof(object), "obj");
            var valParam = Expression.Parameter(typeof(T), "value");

            var castObj = Expression.Convert(objParam, _field.DeclaringType!);
            var castVal = Expression.Convert(valParam, _field.FieldType);
            var access = Expression.Field(castObj, _field);
            var assign = Expression.Assign(access, castVal);

            return Expression.Lambda<Action<object, T>>(assign, objParam, valParam).Compile();
        }
    }

    #endregion

    #region Method

    public class MethodContext
    {
        private readonly ReflectTypeContext _ctx;
        private readonly MethodInfo[] _methods;
        private MethodInfo _selected;
        private object _boundInstance;

        public MethodContext(ReflectTypeContext ctx, MethodInfo[] methods)
        {
            _ctx = ctx;
            _methods = methods ?? throw new ArgumentNullException(nameof(methods));
        }

        public MethodContext BindToInstance(object instance)
        {
            _boundInstance = instance;
            return this;
        }

        public MethodContext WithArgs(params Type[] argTypes)
        {
            _selected = (MethodInfo)ReflectionCache.GetOrAddMember(
                _ctx.Type,
                _methods[0].Name,
                ReflectionCache.MemberKinds.Method,
                argTypes,
                () =>
                {
                    return _methods.FirstOrDefault(m =>
                    {
                        var ps = m.GetParameters();
                        if (ps.Length != argTypes.Length) return false;
                        for (int i = 0; i < ps.Length; i++)
                        {
                            if (ps[i].ParameterType != argTypes[i])
                                return false;
                        }
                        return true;
                    });
                });

            return this;
        }

        public Func<object[], object> AsInvoker()
        {
            if (_selected == null)
            {
                if (_methods.Length == 1)
                    _selected = _methods[0];
                else
                    throw new InvalidOperationException("Multiple overloads. Call WithArgs(...) first.");
            }

            return ReflectionCache.GetOrAddDelegate<Func<object[], object>>(
                _selected,
                _boundInstance == null ? "staticInvoker" : "instanceInvoker",
                typeof(object),
                BuildInvoker);
        }

        /// <summary>
        /// Returns an Action that calls this method on an instance with a single argument.
        /// Used for calling instance methods like Users.Add(object) where the instance varies.
        /// </summary>
        public Action<object, object> AsAction()
        {
            if (_selected == null)
            {
                if (_methods.Length == 1)
                    _selected = _methods[0];
                else
                    throw new InvalidOperationException("Multiple overloads. Call WithArgs(...) first.");
            }

            return ReflectionCache.GetOrAddDelegate<Action<object, object>>(
                _selected,
                "action",
                typeof(object),
                BuildAction);
        }

        private Action<object, object> BuildAction()
        {
            var method = _selected!;
            var instanceParam = Expression.Parameter(typeof(object), "instance");
            var argParam = Expression.Parameter(typeof(object), "arg");

            var castInstance = Expression.Convert(instanceParam, method.DeclaringType!);
            var parameters = method.GetParameters();

            Expression[] callArgs;
            if (parameters.Length == 1)
            {
                callArgs = new[] { Expression.Convert(argParam, parameters[0].ParameterType) };
            }
            else
            {
                callArgs = new Expression[0];
            }

            var call = Expression.Call(castInstance, method, callArgs);
            return Expression.Lambda<Action<object, object>>(call, instanceParam, argParam).Compile();
        }

        private Func<object[], object> BuildInvoker()
        {
            var method = _selected!;
            var argsParam = Expression.Parameter(typeof(object[]), "args");

            var parameters = method.GetParameters();
            var callArgs = new Expression[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                var index = Expression.Constant(i);
                var argObj = Expression.ArrayIndex(argsParam, index);
                var cast = Expression.Convert(argObj, parameters[i].ParameterType);
                callArgs[i] = cast;
            }

            Expression instanceExpr = null;
            if (!method.IsStatic)
            {
                if (_boundInstance == null)
                    throw new InvalidOperationException("Instance method requires bound instance.");

                instanceExpr = Expression.Constant(_boundInstance);
                instanceExpr = Expression.Convert(instanceExpr, method.DeclaringType!);
            }

            var call = instanceExpr != null
                ? Expression.Call(instanceExpr, method, callArgs)
                : Expression.Call(method, callArgs);

            Expression body;
            if (method.ReturnType == typeof(void))
            {
                body = Expression.Block(call, Expression.Constant(null, typeof(object)));
            }
            else
            {
                body = Expression.Convert(call, typeof(object));
            }

            return Expression.Lambda<Func<object[], object>>(body, argsParam).Compile();
        }
    }

    #endregion
}