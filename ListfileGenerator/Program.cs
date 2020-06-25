using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ListfileGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
                throw new Exception("You need to input directory");
            
            var directoryName = args[0];
            if (!Directory.Exists(directoryName))
                throw new Exception($"{directoryName} does not exist");

            Console.WriteLine("Preparing listfile...");
            Listfile.Prepare();

            foreach (var file in Directory.GetFiles(directoryName, "*.*", SearchOption.AllDirectories))
            {
                FileReader.Process(file);
            }

            Console.WriteLine("Writing all found listfile entries...");
            Listfile.FinishListfile();
        }
    }
}
