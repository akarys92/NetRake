using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetRake;

namespace Driver
{
    class Program
    {
        static void Main(string[] args)
        {
            Rake r = new Rake();
            Dictionary<string, double> keywords = r.Run(@"Hi,\nI do not know how to use this script to generate manifest file.\nCould anyone help me to solve this issue?\nI think the code is different from the document described.\nPS 44 > GenerateNationalcloudmanifest D:\\WARM\\Drop blackforest\nCreating the RDPackage directory in build drop location D:\WARM\Drop      Directory: D:\");
            Console.Write(keywords);
        }
    }
}
