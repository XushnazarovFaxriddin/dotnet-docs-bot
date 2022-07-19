using AngleSharp.Dom;
using DotNetUzBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetUzBot.Services
{
    public interface IDocsService
    {
        IEnumerable<Docs> GetBasicDocsList(IDocument document, string sourse);
    }
}
