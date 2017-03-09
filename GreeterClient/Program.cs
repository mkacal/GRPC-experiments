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
using Grpc.Core;
using Helloworld;
using System.Threading.Tasks;
using System.IO;
using Google.Protobuf;

namespace GreeterClient
{
    class Program
    {
        public static Channel channel;

        public static void Main(string[] args)
        {
            var callCre = CallCredentials.FromInterceptor(MyAsyncAuthInterceptor);

            var cacert = File.ReadAllText(@"C:\Sertifika\ca.crt");
            var clientcert = File.ReadAllText(@"C:\Sertifika\client.crt");
            var clientkey = File.ReadAllText(@"C:\Sertifika\client.key");
            var sslCrd = new SslCredentials(cacert, new KeyCertificatePair(clientcert, clientkey));

            var crd = ChannelCredentials.Create(sslCrd, callCre);

            channel = new Channel("localhost:50051", crd);
            var channel2 = new Channel("localhost:50052", crd);

            var inv = new MyInvoker(new Channel[] { channel, channel2 });

            var client = new Greeter.GreeterClient(inv);

            while (true)
            {
                var user = Console.ReadLine();

                if (user == "q")
                    break;
                var reply = client.SayHello(new HelloRequest { Name = user });
                Console.WriteLine("Greeting: " + reply.Message);
            }

            channel.ShutdownAsync().Wait();
            channel2.ShutdownAsync().Wait();
        }



        public static Task MyAsyncAuthInterceptor(AuthInterceptorContext context, Metadata metadata)
        {
            metadata.Add("token", "abc");
            var st = channel.State;
            return Task.FromResult(0);
        }

    }
}
