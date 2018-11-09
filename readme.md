# DotnetSpider Lite
������ dotnet ���档������DonetSpider



# TODO
1. [ ] ����ע�� 
2. [ ] UI�������
3. [ ] ֧�ֲַ����� 
       

# Step
i. Install core package [DotnetSpiderLite.Core]()
~~~ c#
PM> install-package DotnetSpiderLite.Core
~~~

ii. Install ext package 
~~~ c#
PM> install-package DotnetSpiderLite.HtmlAgilityPack
~~~

# Useage
~~~ c#
Spider spider = Spider.Create("https://www.cnblogs.com/");
spider.AddPageProcessors(new CNBlogProcessor());
//spider.AddPipelines(new FilePipeline());
//spider.AddPipelines(new JsonFilePipeline()); 
//spider.ThreadNumber = 10;
spider.Run();
 
~~~

~~~ c# 
public class CNBlogProcessor : BasePageProcessor
{
    public override void HandlePage(Page page)
    {
        var listEle = page.Selector.SelectorAll(".post_item", HtmlSelectorPathType.Css);

        // �б�ҳ�� 
        if (listEle != null && listEle.Count > 0)
        {
            foreach (var item in listEle)
            {
                var title = item.Selector(".titlelnk", HtmlSelectorPathType.Css);
                var href = title.Attributes["href"];

                Console.WriteLine($"�б�:{title.InnerHtml} {href}");

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