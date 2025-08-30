using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkDev.Talabat.Core.Domain.Common
{
    public class BaseEntity <TKey>  where TKey : IEquatable<TKey>
    {
        public int Id { get; set; }
    }
}
