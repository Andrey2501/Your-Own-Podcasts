using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Entities
{
    public class PagedList<T> : List<T>
    {
        public MetaDataPages MetaData { get; set; }
        public PagedList(List<T> items, int count, int pageNumber, int pageSize)
        {
            int totalPages = (int)Math.Ceiling(count / (double)pageSize);
            MetaData = new MetaDataPages(pageNumber, totalPages, pageSize, count);
            AddRange(items);
        }
        public static PagedList<T> ToPagedList(IQueryable<T> source, int pageNumber, int pageSize)
        {
            var count = source.Count();
            var items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            return new PagedList<T>(items, count, pageNumber, pageSize);
        }
    }
}
