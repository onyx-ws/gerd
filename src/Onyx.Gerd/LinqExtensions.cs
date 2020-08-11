using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onyx.Gerd
{
    public static class LinqExtensions
    {
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
        {
            Random random = new Random();
            for (int i = 0; i < random.Next(3, 10); i++)
            {
                source = source.OrderBy(a => random.Next());
            }
            return source;
        }
    }
}
