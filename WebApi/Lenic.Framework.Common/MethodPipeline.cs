using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using Lenic.Framework.Common.Extensions;

namespace Lenic.Framework.Common
{
    /// <summary>
    /// * 方法包装管道
    /// </summary>
    /// <typeparam name="TResult">返回值类型</typeparam>
    public class MethodPipeline<TResult>
    {
        #region * 私有变量

        protected Expression<Func<TResult>> _func;
        protected MethodCallExpression _method;
        protected List<Action<object[], MethodPipelineResult<TResult>>> _errorHandles = new List<Action<object[], MethodPipelineResult<TResult>>>();
        protected List<Action<object[]>> _beforeHandles = new List<Action<object[]>>();
        protected List<Action<object[], MethodPipelineResult<TResult>>> _afterHandles = new List<Action<object[], MethodPipelineResult<TResult>>>();
        protected List<Action<object[], MethodPipelineResult<TResult>>> _successHandles = new List<Action<object[], MethodPipelineResult<TResult>>>();
        protected List<Action<object[], MethodPipelineResult<TResult>>> _finalHandles = new List<Action<object[], MethodPipelineResult<TResult>>>();

        #endregion

        #region * 静态函数

        /// <summary>
        /// * 包装方法
        /// </summary>
        /// <param name="func">方法委托</param>
        /// <returns></returns>
        public static MethodPipeline<TResult> Wrap(Expression<Func<TResult>> func)
        {
            return new MethodPipeline<TResult>(func);
        }

        #endregion

        #region * 构造函数

        protected MethodPipeline(Expression<Func<TResult>> func)
        {
            MethodCallExpression method = func.Body as MethodCallExpression;

            if (method == null)
            {
                throw new ArgumentException("只支持方法的包装");
            }

            this._method = method;
            this._func = func;
        }

        #endregion

        #region * 公有函数

        /// <summary>
        /// * 设置方法执行错误时要执行的委托
        /// </summary>
        /// <param name="func">委托</param>
        /// <returns></returns>
        public MethodPipeline<TResult> Error(Action<object[], MethodPipelineResult<TResult>> func)
        {
            this._errorHandles.Add(func);

            return this;
        }

        /// <summary>
        /// * 设置方法执行前要执行的委托
        /// </summary>
        /// <param name="func">委托</param>
        /// <returns></returns>
        public MethodPipeline<TResult> Before(Action<object[]> func)
        {
            this._beforeHandles.Add(func);

            return this;
        }

        /// <summary>
        /// * 设置方法执行后要执行的委托（After的执行在Error之前）
        /// </summary>
        /// <param name="func">委托</param>
        /// <returns></returns>
        public MethodPipeline<TResult> After(Action<object[], MethodPipelineResult<TResult>> func)
        {
            this._afterHandles.Add(func);

            return this;
        }

        /// <summary>
        /// * 设置方法执行成功时要执行的委托
        /// </summary>
        /// <param name="func">委托</param>
        /// <returns></returns>
        public MethodPipeline<TResult> Success(Action<object[], MethodPipelineResult<TResult>> func)
        {
            this._successHandles.Add(func);

            return this;
        }

        /// <summary>
        /// * 设置方法执行后最终时要执行的委托（Final的执行会在Error和Success之后，不论是否有错误）
        /// </summary>
        /// <param name="func">委托</param>
        /// <returns></returns>
        public MethodPipeline<TResult> Final(Action<object[], MethodPipelineResult<TResult>> func)
        {
            this._finalHandles.Add(func);

            return this;
        }

        /// <summary>
        /// * 执行方法
        /// </summary>
        /// <param name="args">替换委托中的参数</param>
        /// <returns></returns>
        public MethodPipelineResult<TResult> Execute(params object[] args)
        {
            MethodPipelineResult<TResult> result = new MethodPipelineResult<TResult>();

            // 获取方法的原参数集合
            object[] inputArgs = this.GetParameters(this._method.Arguments.ToArray());

            Dictionary<int, int> originalArgs = new Dictionary<int, int>();

            // 记录原参数的索引和HashCode值
            for (int i = 0; i < inputArgs.Length; i++)
            {
                originalArgs.Add(i, inputArgs[i].GetHashCode());
            }

            if (args.Length != 0)
            {
                if (inputArgs.Length != args.Length)
                {
                    throw new ArgumentException("参数个数和方法签名不符");
                }

                inputArgs = args;
            }

            // 执行Before设置的委托
            if (this._beforeHandles.Count != 0)
            {
                this._beforeHandles.ForEach(v => v(inputArgs));
            }

            // 检查参数有没有变化，若有变化则重构方法委托
            this.CheckInputArgsOnBefore(inputArgs, originalArgs);

            try
            {
                result.Result = this._func.Compile().Invoke();
            }
            catch (Exception ce)
            {
                result.InnerException = ce;
            }

            // 执行After设置的委托
            this._afterHandles.ForEach(v => v(inputArgs, result));

            if (result.InnerException != null)
            {
                // 执行Error设置的委托
                this._errorHandles.ForEach(v => v(inputArgs, result));
            }
            else
            {
                // 执行Success设置的委托
                this._successHandles.ForEach(v => v(inputArgs, result));
            }

            // 执行Final设置的委托
            this._finalHandles.ForEach(v => v(inputArgs, result));

            return result;
        }

        #endregion

        #region * 私有函数

        /// <summary>
        /// * 获取委托的方法中的参数值
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        protected virtual object[] GetParameters(Expression[] args)
        {
            if (args.Length == 0) { return new object[0]; }

            object[] res = new object[args.Length];

            args.ForEach((exp, index) =>
            {

                res[index] = (Expression.Lambda(exp).Compile().DynamicInvoke());

            });

            return res;
        }

        /// <summary>
        /// * 检查输入参数在执行Before后有没有改变，有改变则重组委托
        /// </summary>
        /// <param name="args"></param>
        /// <param name="originalArgs"></param>
        protected virtual void CheckInputArgsOnBefore(object[] args, Dictionary<int, int> originalArgs)
        {
            var changeIndex = originalArgs.Where(v => !args.Select(arg => arg.GetHashCode()).Contains(v.Value)).ToArray();

            var change = changeIndex.Select(v => new Tuple<int, object>(v.Key, args[v.Key])).ToArray();

            if (change.Length != 0)
            {
                Expression[] newArgs = new Expression[this._method.Arguments.Count];

                for (int i = 0; i < newArgs.Length; i++)
                {
                    if (changeIndex.Where(v => v.Key == i).Count() != 0)
                    {
                        newArgs[i] = Expression.Constant(change.FirstOrDefault(v => v.Item1 == i).Item2);
                    }
                    else
                    {
                        newArgs[i] = this._method.Arguments[i];
                    }
                }

                MethodCallExpression newCall = Expression.Call(this._method.Object, this._method.Method, newArgs);

                this._method = newCall;
                this._func = Expression.Lambda(this._method) as Expression<Func<TResult>>;
            }
        }

        #endregion
    }

    /// <summary>
    /// * 方法包装管道执行结果
    /// </summary>
    /// <typeparam name="TResult">返回值类型</typeparam>
    public class MethodPipelineResult<TResult>
    {
        public TResult Result { get; set; }
        public Exception InnerException { get; set; }
    }

    /// <summary>
    /// * 方法包装管道
    /// </summary>
    public class MethodPipeline
    {
        #region * 私有变量

        protected Delegate _func;
        protected MethodCallExpression _method;
        protected List<Action<object[], MethodPipelineResult>> _errorHandles = new List<Action<object[], MethodPipelineResult>>();
        protected List<Action<object[]>> _beforeHandles = new List<Action<object[]>>();
        protected List<Action<object[], MethodPipelineResult>> _afterHandles = new List<Action<object[], MethodPipelineResult>>();
        protected List<Action<object[], MethodPipelineResult>> _successHandles = new List<Action<object[], MethodPipelineResult>>();
        protected List<Action<object[], MethodPipelineResult>> _finalHandles = new List<Action<object[], MethodPipelineResult>>();

        #endregion

        #region * 静态函数

        /// <summary>
        /// * 包装方法
        /// </summary>
        /// <param name="func">方法委托</param>
        /// <returns></returns>
        public static MethodPipeline Wrap(Delegate func)
        {
            return new MethodPipeline(func);
        }

        #endregion

        #region * 构造函数

        protected MethodPipeline(Delegate func)
        {
            if (func == null)
            {
                throw new ArgumentNullException("func");
            }

            this._func = func;
        }

        #endregion

        #region * 公有函数

        /// <summary>
        /// * 设置方法执行错误时要执行的委托
        /// </summary>
        /// <param name="func">委托</param>
        /// <returns></returns>
        public MethodPipeline Error(Action<object[], MethodPipelineResult> func)
        {
            this._errorHandles.Add(func);

            return this;
        }

        /// <summary>
        /// * 设置方法执行前要执行的委托
        /// </summary>
        /// <param name="func">委托</param>
        /// <returns></returns>
        public MethodPipeline Before(Action<object[]> func)
        {
            this._beforeHandles.Add(func);

            return this;
        }

        /// <summary>
        /// * 设置方法执行后要执行的委托（After的执行在Error之前）
        /// </summary>
        /// <param name="func">委托</param>
        /// <returns></returns>
        public MethodPipeline After(Action<object[], MethodPipelineResult> func)
        {
            this._afterHandles.Add(func);

            return this;
        }

        /// <summary>
        /// * 设置方法执行成功时要执行的委托
        /// </summary>
        /// <param name="func">委托</param>
        /// <returns></returns>
        public MethodPipeline Success(Action<object[], MethodPipelineResult> func)
        {
            this._successHandles.Add(func);

            return this;
        }

        /// <summary>
        /// * 设置方法执行后最终时要执行的委托（Final的执行会在Error和Success之后，不论是否有错误）
        /// </summary>
        /// <param name="func">委托</param>
        /// <returns></returns>
        public MethodPipeline Final(Action<object[], MethodPipelineResult> func)
        {
            this._finalHandles.Add(func);

            return this;
        }

        /// <summary>
        /// * 执行方法
        /// </summary>
        /// <param name="args">替换委托中的参数</param>
        /// <returns></returns>
        public MethodPipelineResult Execute(params object[] args)
        {
            MethodPipelineResult result = new MethodPipelineResult();

            if (this._func.Method.GetParameters().Length != args.Length)
            {
                throw new ArgumentException("参数与方法签名不符");
            }

            // 执行Before设置的委托
            if (this._beforeHandles.Count != 0)
            {
                this._beforeHandles.ForEach(v => v(args));
            }

            // 检查参数有没有变化，若有变化则重构方法委托
            //this.CheckInputArgsOnBefore(inputArgs, originalArgs);

            try
            {
                result.Result = this._func.DynamicInvoke(args);
            }
            catch (Exception ce)
            {
                result.InnerException = ce;
            }

            // 执行After设置的委托
            this._afterHandles.ForEach(v => v(args, result));

            if (result.InnerException != null)
            {
                // 执行Error设置的委托
                this._errorHandles.ForEach(v => v(args, result));
            }
            else
            {
                // 执行Success设置的委托
                this._successHandles.ForEach(v => v(args, result));
            }

            // 执行Final设置的委托
            this._finalHandles.ForEach(v => v(args, result));

            return result;
        }

        #endregion
    }

    /// <summary>
    /// * 方法包装管道执行结果
    /// </summary>
    public class MethodPipelineResult
    {
        public object Result { get; set; }
        public Exception InnerException { get; set; }

        public T GetResult<T>() where T : class
        {
            return this.Result as T;
        }
    }
}
