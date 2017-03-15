using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using System.Text;
using System.Threading.Tasks;

namespace GreeterServer
{
    public class TransparentProxy<T, TI> : RealProxy where T : TI, new() 
    {
        private TransparentProxy()
            : base(typeof(TI))
        {

        }

        public static TI GenerateProxy()
        {
            var instance = new TransparentProxy<T, TI>();
            return (TI)instance.GetTransparentProxy();
        }

        public override IMessage Invoke(IMessage msg)
        {
            var methodCallMessage = msg as IMethodCallMessage;
            ReturnMessage returnMessage = null;

            try
            {
                var realType = typeof(T);
                var mInfo = realType.GetMethod(methodCallMessage.MethodName);
                var aspects = (AspectBase[])mInfo.GetCustomAttributes(typeof(AspectBase), true);

                FillAspectContext(methodCallMessage);

                var aspectsOrdered = aspects.OrderBy(p => p.Order);
                foreach (var aspect in aspectsOrdered)
                {
                    CheckBeforeAspect(aspect);
                }

                object result = null;

                result = methodCallMessage.MethodBase.Invoke(new T(), methodCallMessage.InArgs);
                returnMessage = new ReturnMessage(result, null, 0, methodCallMessage.LogicalCallContext, methodCallMessage);

                CheckAfterAspect(result, aspects);

                return returnMessage;
            }
            catch (Exception ex)
            {
                return new ReturnMessage(ex, methodCallMessage);
            }
        }

        private void FillAspectContext(IMethodCallMessage methodCallMessage)
        {
            AspectContext.Instance.MethodName = methodCallMessage.MethodName;
            AspectContext.Instance.Arguments = methodCallMessage.InArgs;
        }

        private object CheckBeforeAspect(object aspect)
        {
            if (aspect is IBeforeVoidAspect)
            {
                ((IBeforeVoidAspect)aspect).OnBefore();
                return null;
            }
            else if (aspect is IBeforeAspect)
            {
                return ((IBeforeAspect)aspect).OnBefore();
            }
            else if (aspect is IBeforeControlAspect)
            {
                var asp = (IBeforeControlAspect)aspect;
                var result = asp.OnBefore();
                if (!result)
                    asp.ThrowControlException();
                return result;
            }
            else
                throw new Exception("Unknown aspect!");

        }




        private void CheckAfterAspect(object result, object[] aspects)
        {
            foreach (IAspect loopAttribute in aspects)
            {
                if (loopAttribute is IAfterVoidAspect)
                {
                    ((IAfterVoidAspect)loopAttribute).OnAfter(result);
                }
            }
        }
    }
}
