using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swapi1
{
    public static class Extensions
    {
        public static void Write(this IEnumerable<object> target, char separator = ',')
        {
            /*if (target == null) {
                Console.WriteLine("La collection est nulle.");
                return;
            }*/

            Console.Write(String.Join(separator, target));
        }

    }
}
