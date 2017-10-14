using System;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;

namespace Lenic.Framework.Common.Core
{
    /// <summary>
    /// 成员委托建造者
    /// </summary>
    [DebuggerStepThrough]
    public static class MemberDelegateBuilder
    {
        /// <summary>
        /// 创建一个构造函数委托，仅支持无参默认构造函数。
        /// </summary>
        /// <param name="constructor">默认构造器元数据。</param>
        /// <returns>一个构造函数委托。</returns>
        public static Func<object> NewConstructor(ConstructorInfo constructor)
        {
            if (constructor == null)
                throw new ArgumentNullException("constructor");
            if (constructor.GetParameters().Length > 0)
                throw new NotSupportedException("不支持有参数对象的创建");

            DynamicMethod dm = new DynamicMethod(
                "ctor",
                constructor.DeclaringType,
                Type.EmptyTypes,
                true);

            ILGenerator il = dm.GetILGenerator();
            il.Emit(OpCodes.Nop);
            il.Emit(OpCodes.Newobj, constructor);
            il.Emit(OpCodes.Ret);

            return (Func<object>)dm.CreateDelegate(typeof(Func<object>));
        }

        /// <summary>
        /// 创建一个方法执行委托。
        /// </summary>
        /// <param name="method">方法元数据。</param>
        /// <returns>一个方法执行委托。</returns>
        public static Func<object, object[], object> NewMethod(MethodInfo method)
        {
            ParameterInfo[] pi = method.GetParameters();

            DynamicMethod dm = new DynamicMethod("dm", typeof(object),
                new Type[] { typeof(object), typeof(object[]) },
                typeof(MemberDelegateBuilder), true);

            ILGenerator il = dm.GetILGenerator();

            if (!method.IsStatic)
                il.Emit(OpCodes.Ldarg_0);

            for (int index = 0; index < pi.Length; index++)
            {
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Ldc_I4, index);

                Type parameterType = pi[index].ParameterType;
                if (parameterType.IsByRef)
                {
                    parameterType = parameterType.GetElementType();
                    if (parameterType.IsValueType)
                    {
                        il.Emit(OpCodes.Ldelem_Ref);
                        il.Emit(OpCodes.Unbox, parameterType);
                    }
                    else
                    {
                        il.Emit(OpCodes.Ldelema, parameterType);
                    }
                }
                else
                {
                    il.Emit(OpCodes.Ldelem_Ref);

                    if (parameterType.IsValueType)
                    {
                        il.Emit(OpCodes.Unbox, parameterType);
                        il.Emit(OpCodes.Ldobj, parameterType);
                    }
                }
            }

            if ((method.IsAbstract || method.IsVirtual)
                && !method.IsFinal && !method.DeclaringType.IsSealed)
            {
                il.Emit(OpCodes.Callvirt, method);
            }
            else
            {
                il.Emit(OpCodes.Call, method);
            }

            if (method.ReturnType == typeof(void))
            {
                il.Emit(OpCodes.Ldnull);
            }
            else if (method.ReturnType.IsValueType)
            {
                il.Emit(OpCodes.Box, method.ReturnType);
            }
            il.Emit(OpCodes.Ret);

            return (Func<object, object[], object>)dm.CreateDelegate(typeof(Func<object, object[], object>));
        }

        /// <summary>
        /// 创建一个属性获得（get）委托。
        /// </summary>
        /// <param name="property">属性成员元数据。</param>
        /// <returns>一个属性获得委托。</returns>
        public static Func<object, object> NewPropertyGetter(PropertyInfo property)
        {
            if (property == null)
                throw new ArgumentNullException("property");

            if (!property.CanRead) return null;

            MethodInfo getMethod = property.GetGetMethod();
            if (getMethod == null)   //maybe is private
                getMethod = property.GetGetMethod(true);

            DynamicMethod dm = new DynamicMethod("propg", typeof(object),
                new Type[] { typeof(object) },
                property.DeclaringType, true);

            ILGenerator il = dm.GetILGenerator();

            if (!getMethod.IsStatic)
            {
                il.Emit(OpCodes.Ldarg_0);
                il.EmitCall(OpCodes.Callvirt, getMethod, null);
            }
            else
                il.EmitCall(OpCodes.Call, getMethod, null);

            if (property.PropertyType.IsValueType)
                il.Emit(OpCodes.Box, property.PropertyType);

            il.Emit(OpCodes.Ret);

            return (Func<object, object>)dm.CreateDelegate(typeof(Func<object, object>));
        }

        /// <summary>
        /// 创建一个属性设置（set）委托。
        /// </summary>
        /// <param name="property">属性成员元数据。</param>
        /// <returns>一个属性设置委托。</returns>
        public static Action<object, object> NewPropertySetter(PropertyInfo property)
        {
            if (property == null)
                throw new ArgumentNullException("property");

            if (!property.CanWrite) return null;

            MethodInfo setMethod = property.GetSetMethod();
            if (setMethod == null)   //maybe is private
                setMethod = property.GetSetMethod(true);

            DynamicMethod dm = new DynamicMethod("props", null,
                new Type[] { typeof(object), typeof(object) },
                property.DeclaringType, true);

            ILGenerator il = dm.GetILGenerator();

            if (!setMethod.IsStatic)
            {
                il.Emit(OpCodes.Ldarg_0);
            }
            il.Emit(OpCodes.Ldarg_1);

            EmitCastToReference(il, property.PropertyType);
            if (!setMethod.IsStatic && !property.DeclaringType.IsValueType)
            {
                il.EmitCall(OpCodes.Callvirt, setMethod, null);
            }
            else
                il.EmitCall(OpCodes.Call, setMethod, null);

            il.Emit(OpCodes.Ret);

            return (Action<object, object>)dm.CreateDelegate(typeof(Action<object, object>));
        }

        /// <summary>
        /// 创建一个字段获得（get）委托。
        /// </summary>
        /// <param name="field">字段成员元数据。</param>
        /// <returns>一个字段获得委托。</returns>
        public static Func<object, object> NewFieldGetter(FieldInfo field)
        {
            if (field == null)
                throw new ArgumentNullException("field");

            DynamicMethod dm = new DynamicMethod("fldg", typeof(object),
                new Type[] { typeof(object) },
                field.DeclaringType, true);

            ILGenerator il = dm.GetILGenerator();

            if (!field.IsStatic)
            {
                il.Emit(OpCodes.Ldarg_0);

                EmitCastToReference(il, field.DeclaringType);  //to handle struct object

                il.Emit(OpCodes.Ldfld, field);
            }
            else
                il.Emit(OpCodes.Ldsfld, field);

            if (field.FieldType.IsValueType)
                il.Emit(OpCodes.Box, field.FieldType);

            il.Emit(OpCodes.Ret);

            return (Func<object, object>)dm.CreateDelegate(typeof(Func<object, object>));
        }

        /// <summary>
        /// 创建一个字段设置（set）委托。
        /// </summary>
        /// <param name="field">字段成员元数据。</param>
        /// <returns>一个字段设置委托。</returns>
        public static Action<object, object> NewFieldSetter(FieldInfo field)
        {
            if (field == null)
                throw new ArgumentNullException("field");

            DynamicMethod dm = new DynamicMethod("flds", null,
                new Type[] { typeof(object), typeof(object) },
                field.DeclaringType, true);

            ILGenerator il = dm.GetILGenerator();

            if (!field.IsStatic)
            {
                il.Emit(OpCodes.Ldarg_0);
            }
            il.Emit(OpCodes.Ldarg_1);

            EmitCastToReference(il, field.FieldType);

            if (!field.IsStatic)
                il.Emit(OpCodes.Stfld, field);
            else
                il.Emit(OpCodes.Stsfld, field);
            il.Emit(OpCodes.Ret);

            return (Action<object, object>)dm.CreateDelegate(typeof(Action<object, object>));
        }

        #region Private Methods

        private static void EmitCastToReference(ILGenerator il, Type type)
        {
            if (type.IsValueType)
                il.Emit(OpCodes.Unbox_Any, type);
            else
                il.Emit(OpCodes.Castclass, type);
        }

        #endregion Private Methods
    }
}