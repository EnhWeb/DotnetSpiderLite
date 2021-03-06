﻿using System;
using System.Collections.Generic;
using System.Net;
using DotnetSpiderLite;
using DotnetSpiderLite.Downloader;
using DotnetSpiderLite.Html;
using DotnetSpiderLite.PageProcessor;
using DotnetSpiderLite.Pipeline;
using DotnetSpiderLite.Pipeline.Database.Dapper;

namespace ConsoleApp
{
    class CnBlogTest
    {
        public void Run()
        {

            Spider spider = SpiderBuilder.CreateBuilder()
                .AddRequest("https://www.cnblogs.com/")
                .AddPageProcessor(new CNBlogProcessor())
                .Buid();

            spider.UseNLog();

            // spider.UseRedisScheduler("localhost");
            //spider.UseChromeWebDriverDownloader(@"C:\Users\admin\.nuget\packages\selenium.webdriver.chromedriver\2.44.0\driver\win32\");
            // spider.UseChromeWebDriverDownloader();
            //spider.AddDapperDataBasePipeline(new DapperDatabaseStore()
            //{
            //    OnSave = UseDapperStoreSave
            //});


            //spider.SetDownloaderProxy(new WebProxy("127.0.0.1", 1080)
            //{
            //    // Credentials = new NetworkCredential("[USERNAME]", "[PASSWORD]")
            //});

            //spider.SetDownloaderProxy(new DownloaderProxy(new WebProxy("127.0.0.1", 1080)));
            //spider.SetDownloaderProxy(new SimpleDownloaderProxyPools(
            //    new WebProxy("127.0.0.1", 1080),
            //    new WebProxy("192.168.1.1", 1080),
            //    new WebProxy("192.168.1.2", 1080)
            //  ));

            // proxy pools
            //spider.UseHttpProxyPools(100, 100, new WebProxy("127.0.0.1", 1080)
            //{
            //    Credentials = new NetworkCredential("[USERNAME]", "[PASSWORD]")
            //});

            Random random = new Random();


            spider.UseStaticSleepInterval = false;

            spider.NewRequestDynamicSleepInterval = () => random.Next(100, 1000);

            spider.OnNewRequesting += (_, interval) =>
            {
                Console.WriteLine("sleep:" + interval);
            };

            spider.Run();


        }

        private bool UseDapperStoreSave(IList<IDictionary<string, object>> data)
        {
            // TODO 

            return true;
        }

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

                        page.AddTargetRequest(href);
                    }
                }
                else
                {
                    // Console.WriteLine(page.Response.Request.Uri);

                    var title = page.Selector.Selector(".postTitle a", HtmlSelectorPathType.Css);
                    var body = page.Selector.Selector("#cnblogs_post_body", HtmlSelectorPathType.Css);

                    page.AddResultItem("标题", title?.InnerText?.Trim());
                    //page.AddResultItem("内容", body?.InnerHtml?.Trim());  
                    page.AddResultItem("链接", page.Response.ResponseUri);

                }
            }

        }
    }
}
