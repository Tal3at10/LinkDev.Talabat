using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkDev.Talabat.Core.Application.Common.Exeptions
{
    public class NotFoundExeption : ApplicationException
    {
        public NotFoundExeption(string name, object key) : base($"{name} with {key} is Not Found!")
        {
            
        }
    }
}
