using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetUzBot.Models
{
    public class Docs
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }

        public override string ToString()
        {
            return $@"Id: {Id},
Name: {Name},
Url: {Url}
";
        }
    }
}
