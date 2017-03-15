using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeterServer
{
    public class AspectContext
    {
        private readonly static Lazy<AspectContext> _Instance = new Lazy<AspectContext>(() => new AspectContext());

        private AspectContext()
        {
        }

        public static AspectContext Instance
        {
            get
            {
                return _Instance.Value;
            }
        }

        public string MethodName { get; set; }
        public object[] Arguments { get; set; }
    }
}
