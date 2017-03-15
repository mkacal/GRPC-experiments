using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeterServer
{
    public interface IBeforeControlAspect : IAspect
    {
        bool OnBefore();
        Exception ThrowControlException();
    }
}
