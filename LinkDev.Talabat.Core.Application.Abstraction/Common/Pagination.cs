using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinkDev.Talabat.Core.Application.Abstraction.Models.Products;

namespace LinkDev.Talabat.Core.Application.Abstraction.Common
{
    public class Pagination<T>
    {
       

        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int Count { get; set; }
        public IEnumerable<T> Data { get; set; }

        public Pagination(int pageIndex, int pageSize, IEnumerable<T> data)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            Data = data;

        }
    }
}
