using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdaptiveCardRazorSample
{
    public class Person
    {
        public string Name { get; set; }
        public string First { get; set; }
        public string Last { get; set; }
        public DateTime Birthday { get; set; }

        public List<KeyValuePair<string, string>> Factoids { get; set; } = new List<KeyValuePair<string, string>>();
    }
}
