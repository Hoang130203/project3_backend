using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildingBlocks.Functions
{
    public static class CompareGuid
    {
        public static bool Compare(Guid guid1, Guid guid2)
        {
            return guid1 == guid2;
        }
        public static bool Compare(string guid1, string guid2)
        {
            return Guid.Parse(guid1) == Guid.Parse(guid2);
        }

        public static bool Compare(Guid? guid1, string guid2)
        {
            var guid = Guid.Parse(guid2);

            return guid1 == guid;
        }

        public static bool Compare(string guid1, Guid? guid2)
        {
            var guid = Guid.Parse(guid1);

            return guid == guid2;
        }
    }
}
