using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeterServer
{
    public abstract class AspectBase : Attribute, IAspect
    {
        public abstract int Order { get; }
    }
}
