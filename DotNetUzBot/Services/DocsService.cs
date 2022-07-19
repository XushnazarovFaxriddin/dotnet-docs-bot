using AngleSharp.Dom;
using DotNetUzBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;

namespace DotNetUzBot.Services
{
    public class DocsService : IDocsService
    {
        public IEnumerable<Docs> GetBasicDocsList(IDocument document, string sourse)
        {
            int i = -1;
            foreach (var a in document.GetElementsByClassName("css-1dbjc4n r-1w6e6rj r-1ygmrgt")[0].GetElementsByTagName("a"))
            {
                i++;
                yield return new Docs
                {
                    Id = i,
                    Name = a.GetElementsByTagName("div")[0].InnerHtml,
                    Url = a.GetAttribute("href")!
                };
            }
        }
    }
}
