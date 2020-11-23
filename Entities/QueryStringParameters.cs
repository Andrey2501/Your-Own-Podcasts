using System;
using System.Collections.Generic;
using System.Text;

namespace Entities
{
    public class QueryStringParameters
    {
        const int maxPageSize = 50;
        private int _pageSize = 10;
        private int _pageNumber = 1;
        public int PageNumber
        {
            get
            {
                return _pageNumber;
            }
            set
            {
                _pageNumber = (value < 1) ? _pageNumber : value;
            }
        }
        public int PageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                _pageSize = (value > maxPageSize) ? maxPageSize : value;
            }
        }

        public string Name { get; set; }
        public string Genre { get; set; }
        public string AuthorName { get; set; }
        public string OrderBy { get; set; }
    }
}
