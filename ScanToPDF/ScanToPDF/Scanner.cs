using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanToPDF
{
    class Scanner
    {
        private string name;
        private string description;
        private string port;

        public Scanner(string name, string description, string port)
        {
            this.name = name;
            this.description = description;
            this.port = port;
        }

        public String getName()
        {
            return name;
        }

        public String getDescription()
        {
            return description;
        }

        public string getPort()
        {
            return port;
        }

        public override string ToString()
        {
            return getName();
        }
    }
}
