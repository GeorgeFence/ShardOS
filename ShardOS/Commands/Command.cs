using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShardOS.Commands
{
    public abstract class Command
    {
        // properties
        public string Name;
        public string Help;
        public string Usage;
        // execution
        public abstract void Execute(string line, string[] args);
    }
}
