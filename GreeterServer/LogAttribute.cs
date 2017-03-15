using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeterServer
{
    public class LogAttribute : AspectBase, IBeforeVoidAspect, IAfterVoidAspect
    {
        public override int Order { get => 20; }

        public void OnBefore()
        {
            StringBuilder sbLogMessage = new StringBuilder();

            sbLogMessage.AppendLine(string.Format("Method Name: {0}", AspectContext.Instance.MethodName));
            sbLogMessage.AppendLine(string.Format("Arguments: {0}", string.Join(",", AspectContext.Instance.Arguments)));

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Logging: {0}", sbLogMessage.ToString());
            Console.ResetColor();
        }

        public void OnAfter(object value)
        {
            StringBuilder sbLogMessage = new StringBuilder();

            sbLogMessage.AppendLine(string.Format("Method Name: {0}", AspectContext.Instance.MethodName));
            sbLogMessage.AppendLine(string.Format("Arguments: {0}", string.Join(",", AspectContext.Instance.Arguments)));

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Logged: {0}", sbLogMessage.ToString());
            Console.ResetColor();
        }
    }
}
