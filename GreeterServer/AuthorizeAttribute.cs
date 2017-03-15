using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeterServer
{
    public class AuthorizeAttribute : AspectBase, IBeforeControlAspect
    {
        public override int Order { get => 10; }

        public bool OnBefore()
        {
            return true;
        }

        public Exception ThrowControlException()
        {
            throw new Grpc.Core.RpcException(new Grpc.Core.Status(Grpc.Core.StatusCode.PermissionDenied, "Not authorized!"));
        }
    }
}
