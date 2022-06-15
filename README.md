# MiniTM

MiniTM(Mini Task Manager)是一个简单小型的任务管理器类库，适用于需要在后台执行任务工作项并随时查询进度或取消任务的场景。**该类库的设计理念是简单易用，若你的项目对可靠性有较高的要求，这里推荐你使用Hangfire**

MiniTM是基于 .Net Standard 2.1 的任务管理器类库，可以自由选择任务管理器的执行器(单线程，线程池)和数据存储。若需要支持分布式部署，配置使用Redis作为数据存储即可，详情请查看Demo代码。

## 使用说明

使用之前需要理解一些基础概念。

| 概念               | 接口                | 说明                                                         | 是否必须自行实现 |
| ------------------ | ------------------- | ------------------------------------------------------------ | ---------------- |
| 工作项逻辑         | IJobBo              | 表示具体的业务逻辑                                           | 是               |
| 不可重复工作项逻辑 | IDistinctJobBo      | 不允许重复的业务逻辑，待执行队列中不允许重复                 | 是               |
| 工作项逻辑工厂     | IJobBoFactory       | 用来获取工作项逻辑实例的工厂                                 | 是               |
| 数据存储           | IDataStorage        | 任务管理器中用于记录任务进度，工作项的存储                   | 否               |
| 任务执行器         | ITaskExecutor       | 任务管理器中用于执行工作项逻辑的对象                         | 否               |
| 任务取消通知器     | ITaskCancelNotifier | 任务管理器中用于通知任务取消的对象，分布式部署的情况下需要依靠它进行消息传递 | 否               |

### 1. 实现你的业务逻辑

将你的业务逻辑写成一个类并实现*IJobBo*接口，若你的业务逻辑不能在待执行列表中重复，则实现*IDistinctJobBo*接口。

向业务逻辑传递的参数都在*JobParams*中。

```C#
public class HelloJobBo : IJobBo
{
    public async Task<ExecResult> ExecuteAsync(JobParams param)
    {
        // 假装工作了2秒
        await Task.Delay(2000);
        ExecResult result = new ExecResult();
        string name = param.GetParam<string>("name");
        Console.WriteLine($"{name} say hello");
        result.Ok = true;
        result.Msg = "OK";
        return result;
    }
}
```

*IDistinctJobBo*接口比*IJobBo*接口多了一个获取唯一标识的方法。

```c#
/// <summary>
/// 不允许重复工作项示例，待执行队列中不允许重复
/// </summary>
/// <remark>参数将传用户名和身份证号，以身份证号作为唯一ID</remark>
public class AddUserBo : IDistinctJobBo
{
    public async Task<ExecResult> ExecuteAsync(JobParams param)
    {
        // 假装工作了2秒
        await Task.Delay(2000);
        ExecResult result = new ExecResult();
        string name = param.GetParam<string>("name");
        string idCard = param.GetParam<string>("idCard");
        Console.WriteLine($"Add user [{name}] idCard:{idCard}");
        result.Ok = true;
        result.Msg = "OK";
        return result;
    }

    public string GetDistinctString(JobParams param)
    {
        // 返回唯一标识
        string id = param.GetParam<string>("idCard");
        return id;
    }
}
```

### 2. 编写自己的工作项逻辑对象生成工厂

任务管理器并不知道你的工作项逻辑对象需要如何生成，故需要自行实现*IJobBoFactory*，推荐使用依赖注入的形式实现。

```c#
/// <summary>
/// 使用依赖注入的业务逻辑工厂
/// </summary>
public class DIJobFactory : IJobBoFactory
{
    private IServiceProvider m_Service;

    public DIJobFactory(IServiceProvider sp)
    {
        m_Service = sp;
    }

    public T GetProduct<T>() where T : IJobBo
    {
        var ret = m_Service.GetService<T>();
        return ret;
    }

    public IJobBo GetProduct(Type type)
    {
        var ret = m_Service.GetService(type);
        return (IJobBo)ret;
    }
}
```

### 3. 自由配置你的任务管理器

将上一步实现的工作项逻辑对象生成工厂配置到任务管理器里。

注意：此步不可忽略。

```c#
IJobBoFactory factory = new SimpleJobFactory();
TaskManager mng = TaskManagerFactory.CreateDefaultManager().UseJobBoFactory(factory);
```

*TaskManagerFactory.CreateDefaultManager()* 将默认使用普通执行器，本地内存作为数据存储。若你想使用其他配置可以继续UseXXX来自定义。

```c#
SimpleJobFactory factory = new SimpleJobFactory();

// 线程池执行器
var mng = TaskManagerFactory.CreateDefaultManager()
    .UseThreadPoolExecutor((cfg) =>
    {
     	cfg.MaxThreads = 3;         // 最大线程数
    	cfg.TaskQueueLength = 1024; // 队列长度
    })
    .UseJobBoFactory(factory);
```

### 4. 开始使用

*此处建议将任务管理器作为单例并使用依赖注入进行创建。*

```c#
ITaskManager mng = services.GetService<ITaskManager>();
string taskId = string.Empty;

JobParams p = new JobParams();
p.AddParam("name", "Mike");
// 添加任务(泛型)
taskId = await mng.AddTaskAsync<HelloJobBo>(p));
// 添加任务(指定类型)
taskId = await mng.AddTaskAsync(typeof(HelloJobBo),p));
// 查询进度
var progress = await mng.GetProgressAsync(taskId);
// 取消任务
await mng.TaskCancelAsync(taskId);
```

更多使用示例请查看**MiniTM.Demo**项目代码


## 组件扩展

你可以对任务管理器的各个组件(数据存储，执行器，任务取消通知器)进行魔改，使用时只需要将魔改后的组件调用TaskManager的UseDataStorage、UseExecutor、UseCancelNotifier方法将其配置进去即可。

