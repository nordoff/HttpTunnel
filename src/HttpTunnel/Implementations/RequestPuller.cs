﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using HttpTunnel.Contracts;
using HttpTunnel.Models;

namespace HttpTunnel.Implementations
{
    public class RequestPuller : IRequestPuller
    {
        private readonly ITunnelClient tunnelClient;
        private readonly IBackwardSender backwardSender;

        public RequestPuller(ITunnelClient tunnelClient, IBackwardSender backwardSender)
        {
            this.tunnelClient = tunnelClient;
            this.backwardSender = backwardSender;
        }

        public async Task Start(CancellationToken cancellationToken)
        {
            while (true)
            {
                try
                {
                    var requestData = await this.tunnelClient.GetRequest();
                    if (requestData != null)
                    {
                        this.backwardSender.Send(requestData);
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.ToString());

                    // Wait a short period of time before retry.
                    await Task.Delay(TimeSpan.FromSeconds(15));
                }
            }
        }
    }
}