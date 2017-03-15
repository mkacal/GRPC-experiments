using Grpc.Core;
using Grpc.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeterClient
{
    public class MyInvoker : CallInvoker
    {
        private Channel[] _channels;
        private Channel channel;
        private int maxRetryCount = 3;
        /// <summary>
        /// Initializes a new instance of the <see cref="Grpc.Core.DefaultCallInvoker"/> class.
        /// </summary>
        /// <param name="channel">Channel to use.</param>
        public MyInvoker(Channel[] channels)
        {
            _channels = channels;
        }


        /// <summary>
        /// Invokes a simple remote call in a blocking fashion.
        /// </summary>
        public override TResponse BlockingUnaryCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options, TRequest request)
        {
            int retryCount = 0;

            while (true)
            {
                try
                {
                    var call = CreateCall(method, host, options);
                    return Calls.BlockingUnaryCall(call, request);
                }
                catch (RpcException ex)
                {
                    retryCount++;

                    if (channel.State != ChannelState.TransientFailure || retryCount > maxRetryCount)
                        throw;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }


        /// <summary>
        /// Invokes a simple remote call asynchronously.
        /// </summary>
        public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options, TRequest request)
        {
            var call = CreateCall(method, host, options);
            return Calls.AsyncUnaryCall(call, request);
        }

        /// <summary>
        /// Invokes a server streaming call asynchronously.
        /// In server streaming scenario, client sends on request and server responds with a stream of responses.
        /// </summary>
        public override AsyncServerStreamingCall<TResponse> AsyncServerStreamingCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options, TRequest request)
        {
            var call = CreateCall(method, host, options);
            return Calls.AsyncServerStreamingCall(call, request);
        }

        /// <summary>
        /// Invokes a client streaming call asynchronously.
        /// In client streaming scenario, client sends a stream of requests and server responds with a single response.
        /// </summary>
        public override AsyncClientStreamingCall<TRequest, TResponse> AsyncClientStreamingCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options)
        {
            var call = CreateCall(method, host, options);
            return Calls.AsyncClientStreamingCall(call);
        }

        /// <summary>
        /// Invokes a duplex streaming call asynchronously.
        /// In duplex streaming scenario, client sends a stream of requests and server responds with a stream of responses.
        /// The response stream is completely independent and both side can be sending messages at the same time.
        /// </summary>
        public override AsyncDuplexStreamingCall<TRequest, TResponse> AsyncDuplexStreamingCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options)
        {
            var call = CreateCall(method, host, options);
            return Calls.AsyncDuplexStreamingCall(call);
        }

        /// <summary>Creates call invocation details for given method.</summary>
        protected virtual CallInvocationDetails<TRequest, TResponse> CreateCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options)
                where TRequest : class
                where TResponse : class
        {
            channel = GrpcPreconditions.CheckNotNull(
                    _channels.Where(p => p.State != ChannelState.TransientFailure)
                    .FirstOrDefault()
                );

            return new CallInvocationDetails<TRequest, TResponse>(channel, method, host, options);
        }
    }
}
