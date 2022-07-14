# 分库分表日志系统

分库分表的规则和思路
规则：一个月一个库，一天一张表（日志表）
➢ 实现思路：
（1）调用约束的DTO传递开始、结束时间
底层aop：
（2）AOP拦截到Controller，判断action方法是否需要动态注入连接
（3）根据开始、结束时间，把范围内的数据库都连接上
（4）从默认库中获取到项目表保存在内存中
（5）通过注入的那些连接来进行日志的查询（遍历dateList，打开每个dateDB连接，遍历date下的dayList，把每张day表下的日志数据都查出来，放到集合中。备注：其中查询语句加到ISugarQueryable的集合中，利用SugarClient.UnionAll方法获取数据）
（6）关联每个日志所属的项目，返回结果

技术栈：
● .Net Core 3.1 + 版本 webAPI
● Repository 仓储 + Service服务 模式（用来封装存储，操作数据库）
● Swagger 前后端文档说明，基于RESTful风格编写接口
● Autofac 轻量级IoC和DI依赖注入
● AutoMapper自动对象映射 （将数据模型转换为DTO试图模型）
● AOP基于切面编程技术
● 自定义特性及反射技术
● Validation 验证
● Redis 缓存
● Async和Await 异步编程
● Cors 简单的跨域解决方案
● WebSocket
● SqlSugar、FreeSQL 轻量级ORM框架，CodeFirst

1、 Repository 仓储 + Service服务 模式
项目结构：
1) Api 层、
2) 实体 Model 层、
3) 仓储模块（作为一个数据库管理员，直接操作数据库，实体模型）：BaseRepository（基类仓储） 继承实现了 接口IBaseRepository，这里放公共的方法
仓储官方的定义，他是一个管理者，去管理实体对象和ORM映射对象的一个集合，是ORM操作db，而Dal就是完完全全的操作db数据库了 

4）Service模块（处理业务逻辑，可以直接使用ViewModel视图模型）：BaseService 调用 BaseRepository，同时继承 IBaseService

2、 netcore三种注入方式
● AddSingleton：服务在第一次请求时被创建，后续延用 （某些公共类）
● AddTransient：每一次请求服务都创建一个新实例；它最好被用于轻量级无状态服务（如我们的Repository和ApplicationService服务）
● AddScoped：每一次请求获取的对象都是同一个；若该service在一个请求过程中多次被用到，并且可能共享其中的字段或者属性（例如：httpcontext）
如何获取已经注册的服务列表？ 
通过ServiceProvide.BuildServiceProvider.GetServices()<>;

3、 Autofac、IOC、DI 三者的关系？
关于Autofac的安装包有3个：
➢ Autofac 
主要操作对象：IContainer对象是Autofac的容器，由ContainerBuilder对象的Build()方法来创建；ContainerBuilder对象将依赖关系注入到IContainer容器之中，然后通过IContainer对象的Resolve<T>()方法来获取对象。
➢ Autofac.Extensions.DependencyInjection 
主要操作对象：IServiceCollection 、serviceProvider.GetRequiredService<>();
➢ Autofac.Extras.DynamicProxy --->>>命名空间 Castle.DynamicProxy 
主要操作对象：拦截器 IInterceptor、 IInvocation
IOC（控制反转）
控制反转的思想是：把创建对象的权利由调用者转变为提供者，它的实现方式有：依赖注入和依赖查找。
DI（依赖注入）
依赖注入意思就是：把对象之间的依赖关系注入到IOC容器中。这样做的目的就是为了解耦，以实现软件设计的高内聚，低耦合。
实现过程：
例如，通过services.AddSingleton<,>() 方法向依赖注入容器（IServiceCollection）中注册服务，然后在构造Controller的时候解析服务，根据其构造方法类型和入参，在容器中找到该对象并作为实参返回给构造方法。
注入方式有3种：构造方法注入、属性注入、接口注入

4、 AutoMapper自动对象映射 （将数据模型转换为DTO试图模型)
（1）使用NuGet引入automap框架
（2）创建“Profile”映射文件，并继承 Profile （映射类，所有继承Profile的类是一个映射集合）
（3）在Startup中注册服务 services.AddAutoMapper（这里可以是程序集，可以是映射类）; 如果是程序集，会自动扫描程序集中类型，把继承了 Profile 的类型提取出来，放到一个Imapper容器中
（4）在Controller的构造方法中注入IMapper，拿到Imapper的实例，通过_mapper<>()来映射

5、 AOP基于切面编程技术
6、 自定义特性反射技术
读取特性的相关信息，比如说：方法、属性* Type.GetMothods();  //获取此类的所有公开的方法 ```
* Type.GetMothods(string);  //获取此类的名为string的所有公开的方法
* 一堆is开头的bool属性
* ReturnType：获取这个方法的返回类型
* 利用invoke获取方法  ...GetMethod("").invoke();
vay typeInfo = typeof(普通类).GetTypeInfo();  //获取到这个类的信息
typeInfo.GetCustomAttribute<特性类>();  //返回特性的实例 

7、 Validation 验证
用来验证body数据的属性，比如说name没有填写, 或者name太长, 那么在执行action方法的时候就会报错, 这时候框架会自动抛出500异常, 表示是服务器的错误, 这是不对的. 这种错误是由客户端引起的, 所以需要返回400 Bad Request错误。
● System.ComponentModel.DataAnnotation命名空间下有一些内置的验证，比如：[Required]表示必填, [MinLength]表示最小长度, [StringLength]可以同时验证最小和最大长度, [Range]表示数值的范围等等很多
● 自定义的注解验证：利用 ModelState（本质是一个Dictionary）
每次请求进到Action的时候, 会检查Model上添加的注解的验证, 只要其中有一个验证没通过, 那么ModelState.IsValid属性就是False。如果有错误的话, 我们可以把ModelState当作 Bad Request的参数一起返回到前台。
8、 Redis 缓存
9、 Async和Await 异步编程
10、 Cors 简单的跨域解决方案
11、 WebSocket



