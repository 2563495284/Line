using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using UnityEngine;


// 4. 装饰器类：包装原始IEnumerator并附加数据
public class TagedEnumerator : IEnumerator
{
    private readonly IEnumerator _originalEnumerator;
    public readonly string tag = "";
    public readonly string head = "";


    // 实现IEnumerator接口
    public object Current => _originalEnumerator.Current;
    public bool MoveNext() => _originalEnumerator.MoveNext();
    public void Reset() => _originalEnumerator.Reset();

    // 构造函数：包装原始迭代器并提供数据获取方法
    public TagedEnumerator(IEnumerator original, string tag, string head)
    {
        _originalEnumerator = original;
        this.tag = tag;
        this.head = head;
    }
}
public static class CoroutineTracker
{
    private static Action<string> startLogFunc = str => Debug.Log(str);
    private static Action<string> endLogFunc = str => Debug.Log(str);
    private static int CNT = 1;
    public static void BindLogFunc(Action<string> func, Action<string> endFunc)
    {
        startLogFunc = func;
        endLogFunc = endFunc;
    }

    // 5. 扩展方法：简化带数据的迭代器创建
    // 将普通迭代器包装为带数据的迭代器
    public static TagedEnumerator WithData(this IEnumerator enumerator, string tag, string head = "Cor")
    {
        return new TagedEnumerator(enumerator, tag, head);
    }
    // 启动带追踪功能的协程
    public static Coroutine StartTrackedCoroutine(this MonoBehaviour mono, IEnumerator enumerator, string name = null)
    {
        return StartTrackedCoroutine_Native(mono, enumerator, name, null, CNT++);
    }
    private static Coroutine StartTrackedCoroutine_Native(this MonoBehaviour mono, IEnumerator enumerator, string name, string rootLayer, int id)
    {
        string initialLayer = string.IsNullOrEmpty(rootLayer) ? name : rootLayer;
        string coroutineName = name;
        if (enumerator is TagedEnumerator e)
        {
            coroutineName = string.IsNullOrEmpty(name) ? e.tag : name;
            initialLayer = string.IsNullOrEmpty(rootLayer) ? e.tag : initialLayer;
            startLogFunc.Invoke($"【{id} {e.head} Start】{coroutineName}: {initialLayer}");
        }
        var wrappedEnumerator = WrapEnumerator(enumerator, initialLayer, coroutineName, id, mono);
        return mono.StartCoroutine(wrappedEnumerator);
    }

    // 包装迭代器，处理层级追踪
    private static IEnumerator WrapEnumerator(
        IEnumerator enumerator,
        string currentLayerPath,
        string name,
        int id,
        MonoBehaviour owner)
    {

        var depth = CoroutineContext.CurrentDepth;
        CoroutineContext.PushContext(currentLayerPath, name, depth + 1, id);
        bool moveNextResult = false;
        object currentValue = null;

        // 第一阶段：获取下一个值
        while (true)
        {
            moveNextResult = enumerator.MoveNext();
            if (moveNextResult)
                currentValue = enumerator.Current;
            else
                break;

            // 第二阶段：处理获取到的值
            if (moveNextResult)
            {
                if (currentValue == null)
                {
                    yield return null;
                    continue;
                }
                if (currentValue is not IEnumerator nestedEnumerator)
                {
                    yield return currentValue;
                    continue;
                }
                TagedEnumerator tagedEnumerator = nestedEnumerator as TagedEnumerator;
                if (tagedEnumerator == null)
                {
                    yield return owner.StartTrackedCoroutine_Native(
                    nestedEnumerator,
                    name,
                    currentLayerPath, // 传递新的层级路径作为根
                    id
                );
                    continue;
                }
                string layerName = tagedEnumerator.tag;
                // 构建新的层级路径（父层级 + 当前层名称）
                var newLayerPath = $"{currentLayerPath}_{layerName}";
                var nestedName = layerName;

                yield return owner.StartTrackedCoroutine_Native(
                    nestedEnumerator,
                    string.IsNullOrEmpty(nestedName) ? $"{name}的子协程" : nestedName,
                    newLayerPath, // 传递新的层级路径作为根
                    id
                );
            }
        }
        CoroutineContext.PopContext();

        if (enumerator is TagedEnumerator)
        {
            TagedEnumerator e = enumerator as TagedEnumerator;
            endLogFunc.Invoke($"【{id} {e.head} End】{name}: {currentLayerPath}");
        }
    }




    // #region New

    // // 2. 增强的实例解析逻辑（核心修改）
    // private static object ResolveTargetInstance(IEnumerator enumerator)
    // {
    //     object instance = null;

    //     // 步骤1: 尝试常规__this字段（基础场景）
    //     instance = TryGetInstanceFromThisFields(enumerator);
    //     if (instance != null)
    //         return instance;

    //     // 步骤2: 处理委托包装场景（新增核心逻辑）
    //     instance = TryGetInstanceFromDelegate(enumerator);
    //     if (instance != null)
    //         return instance;

    //     // 步骤3: 尝试从委托方法信息中获取
    //     instance = TryGetInstanceFromDelegateMethod(enumerator);
    //     return instance;
    // }

    // // 专门处理委托的实例解析
    // private static object TryGetInstanceFromDelegate(IEnumerator enumerator)
    // {
    //     // 查找所有委托字段
    //     var delegateFields = enumerator.GetType()

    //         .GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
    //     var methodInfo = delegateFields.FirstOrDefault(f => f.FieldType == typeof(MethodInfo));
    //     if (methodInfo != null)
    //     {
    //         var method = methodInfo.GetValue(enumerator) as MethodInfo;
    //         if (method != null)
    //         {
    //             // 获取方法声明类型的实例（静态方法返回类型本身）
    //             return method.IsStatic ? method.DeclaringType : Activator.CreateInstance(method.DeclaringType);
    //         }
    //     }
    //     // 查找所有委托方法
    //     var fieldInfo = delegateFields.FirstOrDefault(f => typeof(Delegate).IsAssignableFrom(f.FieldType) && (f.GetValue(enumerator) as Delegate) != null);
    //     if (fieldInfo != null)
    //     {
    //         var instance = fieldInfo.GetValue(enumerator) as Delegate;
    //         return instance.Target != null ? instance.Target : instance.Method.DeclaringType;
    //     }
    //     return null;
    // }

    // // 从委托方法信息中提取实例
    // private static object TryGetInstanceFromDelegateMethod(IEnumerator enumerator)
    // {
    //     try
    //     {
    //         // 查找存储方法信息的字段（委托包装类通常会有）
    //         var methodField = enumerator.GetType()

    //             .GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
    //             .FirstOrDefault(f => f.FieldType == typeof(MethodInfo));

    //         if (methodField != null)
    //         {
    //             var method = methodField.GetValue(enumerator) as MethodInfo;
    //             if (method != null)
    //             {
    //                 // 获取方法声明类型的实例（静态方法返回类型本身）
    //                 return method.IsStatic ? method.DeclaringType : Activator.CreateInstance(method.DeclaringType);
    //             }
    //         }
    //     }
    //     catch
    //     {
    //         // 忽略静态类等无法实例化的情况
    //     }
    //     return null;
    // }

    // // // 3. 增强的方法匹配逻辑（支持委托方法）
    // // private static bool IsTraceableCoroutine(IEnumerator enumerator, out string layerName)
    // // {
    // //     layerName = null;
    // //     if (enumerator == null) return false;

    // //     var enumeratorType = enumerator.GetType();

    // //     var targetInstance = ResolveTargetInstance(enumerator); // 使用新的解析逻辑
    // //     if (targetInstance == null) return false;

    // //     // 获取所有候选方法（包括委托指向的方法）
    // //     var candidateMethods = GetAllCandidateMethods(targetInstance, enumerator);

    // //     foreach (var method in candidateMethods)
    // //     {

    // //         var attribute = method.GetCustomAttribute<TraceableCoroutineAttribute>();
    // //         if (attribute == null)
    // //             continue;

    // //         // 验证方法是否生成当前迭代器
    // //         if (IsMatchingEnumeratorMethod(method, targetInstance, enumeratorType))
    // //         {
    // //             layerName = attribute.LayerName;
    // //             return true;
    // //         }
    // //     }

    // //     return false;
    // // }

    // // 获取包括委托方法在内的所有候选方法
    // private static IEnumerable<MethodInfo> GetAllCandidateMethods(object instance, IEnumerator enumerator)
    // {
    //     var methods = new List<MethodInfo>();

    //     //         if (method.ReturnType != typeof(IEnumerator))
    //     //             continue;
    //     // 添加实例本身的方法
    //     if (instance is Type type)
    //         methods.AddRange(type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).Where(m => m.ReturnType == typeof(IEnumerator)));
    //     else if (instance != null)
    //         methods.AddRange(instance.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));

    //     // 添加委托指向的方法（如果是委托包装）
    //     if (enumerator is Delegate delegateWrapper)
    //         methods.Add(delegateWrapper.Method);



    //     //method.GetCustomAttribute<TraceableCoroutineAttribute>();
    //     return methods.Distinct();
    // }
    // #endregion


    private static MethodInfo GetIEFuncInfo(IEnumerator enumerator)
    {
        Type type = enumerator.GetType();

        BindingFlags flag = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;//不知道static方法能不能检测，先屏蔽了
        FieldInfo objField = type.GetFields(flag).FirstOrDefault(f => f.Name.EndsWith("__this", StringComparison.Ordinal));
        if (objField == null)
        {
            //非显式枚举器，不可追踪
            return null;
        }
        bool Validate(MethodInfo m)
        {
            var paramseters = m.GetParameters();
            var args = paramseters.Length > 0 ? new object[paramseters.Length] : null;
            return m.Invoke(objField.GetValue(enumerator), args).GetType() == type;
        }
        //查找由显式定义直接生成的迭代器
        MethodInfo GetInfoByDefine()
        {
            MethodInfo methodInfo = objField.FieldType
           .GetMethods(flag | BindingFlags.Static)
           .Where(m => m.ReturnType == typeof(IEnumerator))
           .FirstOrDefault(m => Validate(m));
            return methodInfo;
        }

        // MethodInfo GetInfoByDelegate()
        // {
        //     // 查找所有委托方法
        //     var del = type.GetFields(flag)
        //     .Where(f => typeof(Delegate).IsAssignableFrom(f.FieldType))
        //     .Select(f => f.GetValue(enumerator) as Delegate)
        //     .FirstOrDefault(d => Validate(d.Method));
        //     return del != null ? del.Method : null;
        // }
        // MethodInfo GetInfoByDelegateInfo()
        // {
        //     var delegateFields = type.GetFields(flag);
        //     var methodInfo = delegateFields.Where(f => f.FieldType == typeof(MethodInfo));
        //     if (methodInfo != null)
        //     {
        //         var method = methodInfo.GetValue(enumerator) as MethodInfo;
        //         if (method != null)
        //         {
        //             // 获取方法声明类型的实例（静态方法返回类型本身）
        //             return method.IsStatic ? method.DeclaringType : Activator.CreateInstance(method.DeclaringType);
        //         }
        //     }
        // }
        // MethodInfo target = null;
        // if ((target = GetInfoByDefine()) != null)
        //     return target;
        // if ((target = GetInfoByDelegate()) != null)
        //     return target;
        return GetInfoByDefine();
    }


}

// 协程上下文管理
public static class CoroutineContext
{
    private static readonly ThreadLocal<Stack<CoroutineInfo>> _contextStack =
        new ThreadLocal<Stack<CoroutineInfo>>(() => new Stack<CoroutineInfo>());

    public static CoroutineInfo Current => _contextStack.Value.Count > 0 ? _contextStack.Value.Peek() : null;
    public static int CurrentDepth => _contextStack.Value.Count;

    public static void PushContext(string layerPath, string name, int depth, int id)
    {
        _contextStack.Value.Push(new CoroutineInfo(layerPath, name, depth, id));
    }

    public static void PopContext()
    {
        if (_contextStack.Value.Count > 0)
            _contextStack.Value.Pop();
    }
}

// 协程信息结构体（使用字符串层级路径）
public class CoroutineInfo
{
    public string LayerPath { get; }
    public string Name { get; }
    public int Depth { get; }
    public int Id { get; }

    public CoroutineInfo(string layerPath, string name, int depth, int id)
    {
        LayerPath = layerPath;
        Name = name;
        Depth = depth;
        Id = id;
    }
}
