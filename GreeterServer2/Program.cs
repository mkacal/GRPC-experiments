using Grpc.Core;
using Helloworld;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeterServer2
{
    class GreeterImpl : Greeter.GreeterBase
    {
        // Server side handler of the SayHello RPC
        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            var token = context.RequestHeaders.Where(p => p.Key == "token");


            return Task.FromResult(new HelloReply { Message = "Server2 Hello " + request.Name });
        }
    }

    class Program
    {
        const int Port = 50052;

        public static void Main(string[] args)
        {
            var cacert = File.ReadAllText(@"C:\Sertifika\ca.crt");
            var servercert = File.ReadAllText(@"C:\Sertifika\server.crt");
            var serverkey = File.ReadAllText(@"C:\Sertifika\server.key");
            var keypair = new KeyCertificatePair(servercert, serverkey);
            var sslCredentials = new SslServerCredentials(new List<KeyCertificatePair>() { keypair }, cacert, false);


            Server server = new Server
            {
                Services = { Greeter.BindService(new GreeterImpl()) },
                Ports = { new ServerPort("localhost", Port, sslCredentials) }
            };

            server.Start();

            Console.WriteLine("Greeter server listening on port " + Port);
            Console.WriteLine("Press any key to stop the server...");
            Console.ReadKey();

            server.ShutdownAsync().Wait();
        }

    }
}