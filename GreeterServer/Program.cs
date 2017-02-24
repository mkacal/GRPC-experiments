// Copyright 2015, Google Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are
// met:
//
//     * Redistributions of source code must retain the above copyright
// notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above
// copyright notice, this list of conditions and the following disclaimer
// in the documentation and/or other materials provided with the
// distribution.
//     * Neither the name of Google Inc. nor the names of its
// contributors may be used to endorse or promote products derived from
// this software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
// "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
// A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
// OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
// SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
// LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
// THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using System;
using System.Threading.Tasks;
using Grpc.Core;
using Helloworld;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace GreeterServer
{
    class GreeterImpl : Greeter.GreeterBase
    {
        // Server side handler of the SayHello RPC
        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            var token = context.RequestHeaders.Where(p => p.Key == "token"); //context.RequestHeaders.ToList();


            return Task.FromResult(new HelloReply { Message = "Hello " + request.Name });
        }
    }

    class Program
    {
        const int Port = 50051;

        public static void Main(string[] args)
        {
            var callCre = CallCredentials.FromInterceptor(MyAsyncAuthInterceptor);

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



        public static Task MyAsyncAuthInterceptor(AuthInterceptorContext context, Metadata metadata)
        {

            return Task.FromResult(0);
        }



    }
}
