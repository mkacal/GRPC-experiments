using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeterServer
{
    public interface IBeforeVoidAspect : IAspect
    {
        void OnBefore();
    }
}
