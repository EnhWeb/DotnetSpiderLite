﻿# DotnetSpiderLite

轻量级 dotnet 爬虫。复刻自 DotnetSpider

> 造这个轮子的原因是， DotnetSpider各个之间依赖太强了，本来想简单的采集一些东西，安装后却来了一个全家桶... 我的期望是各个组件之间尽可能的解耦，但需要的时候就安装这个组件。

[![Build Status](https://dev.azure.com/jxnkwlp/DotnetSpiderLite/_apis/build/status/GitHub-CI)](https://dev.azure.com/jxnkwlp/DotnetSpiderLite/_build/latest?definitionId=1)

## 名词

 1. Downloader : 下载器
 2. PageProcessor : 页面下载器
 3. Pipeline ： 数据管道
 4. Scheduler ： 队列

## TODO

 1. [ ] 完善注释
 2. [ ] UI管理界面
 3. [x] [使用 Redis 作为队列](#使用redis作为队列程序)
 4. [ ] 数据库 支持
 5. [x] [支持使用代理](#使用代理)
 6. [ ] 注解模式

## 安装

i. 安装包 [DotnetSpiderLite.Core](https://www.nuget.org/packages/DotnetSpiderLite.Core/)

~~~ c#
PM> install-package DotnetSpiderLite.Core
~~~

ii. 安装html解析扩展包（可选，如果需求解析html，则需要安装） 目前实现了 [HtmlAgilityPack](https://www.nuget.org/packages/DotnetSpiderLite.HtmlAgilityPack/) 和 [AngleSharp](https://www.nuget.org/packages/DotnetSpiderLite.AngleSharp/)

~~~ c#
PM> install-package DotnetSpiderLite.HtmlAgilityPack
~~~

## 使用

~~~ C# 
Spider spider = SpiderBuilder.CreateBuilder()
    .AddRequest("https://www.cnblogs.com/")
    .AddPageProcessor(new CNBlogProcessor())
    .Buid();

spider.Run();
~~~

~~~ c#
public class CNBlogProcessor : BasePageProcessor
{
    public override void HandlePage(Page page)
    {
        var listEle = page.Selector.SelectorAll(".post_item", HtmlSelectorPathType.Css);

        // 列表页面 
        if (listEle != null && listEle.Count > 0)
        {
            foreach (var item in listEle)
            {
                var title = item.Selector(".titlelnk", HtmlSelectorPathType.Css);
                var href = title.Attributes["href"];

                Console.WriteLine($"列表:{title.InnerHtml} {href}");

                page.AddTargetRequest(href);
            }
        }
        else
        {
            Console.WriteLine(page.Response.Request.Uri);

            var title = page.Selector.Selector(".postTitle a", HtmlSelectorPathType.Css);
            var body = page.Selector.Selector("#cnblogs_post_body", HtmlSelectorPathType.Css);

            page.AddResultItem("title", title?.InnerText?.Trim());
            //page.AddResultItem("content", body?.InnerHtml?.Trim());
        }
    }

}
~~~

## 使用redis作为队列程序

安装包 [DotnetSpiderLite.Scheduler.StackExchange.Redis](https://www.nuget.org/packages/DotnetSpiderLite.Scheduler.StackExchange.Redis/)， 这个默认为使用 [StackExchange.Redis](https://www.nuget.org/packages/StackExchange.Redis/) 组件。

~~~ csharp
Spider spider = SpiderBuilder.CreateBuilder()
    .AddRequest("https://www.cnblogs.com/")
    .AddPageProcessor(new CNBlogProcessor())
    .Buid();
// ...
spider.UseRedisScheduler("localhost");
// ...
spider.Run();
~~~

如果需要使用其他 redis 组件，可安装包 [DotnetSpiderLite.Scheduler.Redis](https://www.nuget.org/packages/DotnetSpiderLite.Scheduler.Redis/) , 然后实现 IRedisStore 接口。

~~~ csharp
Spider spider = SpiderBuilder.CreateBuilder()
    .AddRequest("https://www.cnblogs.com/")
    .AddPageProcessor(new CNBlogProcessor())
    .Buid();
// ...

IRedisStore myRedisStore = [YOU REDISSTORE];
spider.UseRedisScheduler(myRedisStore);

// ...
spider.Run();
~~~

## 使用代理

在一些场景下，可能需要使用代理，有3种形式可以配置。

i. 单个代理。 只指定一个代理，适合用在稳定的代理场景

~~~ cs
spider.SetDownloaderProxy(new WebProxy("127.0.0.1", 1080)
{
    // Credentials = new NetworkCredential("[USERNAME]", "[PASSWORD]")
});
~~~

ii. 多个代理。指定多个代理，每次请求前挑选一个使用。同样，适合在有稳定的代理场景。因为不会有失败检测。

~~~ cs
spider.SetDownloaderProxy(new SimpleDownloaderProxyPools(
    new WebProxy("127.0.0.1", 1080),
    new WebProxy("192.168.1.1", 1080),
    new WebProxy("192.168.1.2", 1080)
    ));
~~~

iii. 使用默认实现的代理池。有失败检测，适合有很多个代理的场景下

安装包 [DotnetSpiderLite.ProxyPools](https://www.nuget.org/packages/DotnetSpiderLite.ProxyPools/) , 实现 IHttpProxyFinder 接口。

~~~ cs
var finder = [HttpProxyFinder];
spider.UseHttpProxyPools(finder);
~~~
