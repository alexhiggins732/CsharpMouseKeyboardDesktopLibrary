using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MacroStorageTests
{
    class Program
    {
        static void Main(string[] args)
        {

            var list = GetInts(1<<16);
            foreach (var i in list)
                Console.Write(i);
        }

        public static IEnumerable<int> GetInts(int count)
        {
            var range = Enumerable.Range(0, count);
            foreach (var i in range)
                yield return i;
        }



     
    }
}
