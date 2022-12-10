using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanToPDF
{
    public class PhotoElement
    {
        private string path;
        private string name;

        public PhotoElement(string path, string name)
        {
            this.path = path;
            this.name = name;
        }

        public override string ToString()
        {
            return name;
        }

        public string getPath()
        {
            return path;
        }
    }
}
