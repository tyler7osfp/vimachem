using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Infrastructure.Caching
{
    public static class CacheKeys
    {
        public static string AllBooks => "books:all";
        public static string Book(Guid id) => $"books:{id}";

        public static string AllCategories => "categories:all";
        public static string Category(Guid id) => $"categories:{id}";

        public static string AllParties => "parties:all";
        public static string Party(Guid id) => $"parties:{id}";
    }
}
