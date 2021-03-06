﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Net.Http;
using System.Threading;
using Microsoft.Azure.SignalR.Tests.Common;
using Xunit;

namespace Microsoft.Azure.SignalR.Common.Tests.RestClients
{
    public class RestClientBuilderFacts
    {
        private const string AccessKey = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private const string Endpoint = "http://endpoint/";
        private const string productInfo = "productInfo";
        private readonly string _connectionString = $"Endpoint={Endpoint};AccessKey={AccessKey};Version=1.0;";

        [Fact]
        public void RequestContainsAsrsUAFact()
        {
            void assertion(HttpRequestMessage request, CancellationToken t)
            {
                Assert.True(request.Headers.Contains(Constants.AsrsUserAgent));
                Assert.NotNull(request.Headers.GetValues(Constants.AsrsUserAgent));
            }

            TestRestClientBuilder(assertion);
        }

        [Fact]
        public void RequestContainsCredentials()
        {
            void assertion(HttpRequestMessage request, CancellationToken t)
            {
                var authHeader = request.Headers.Authorization;
                string scheme = authHeader.Scheme;
                string parameter = authHeader.Parameter;

                Assert.Equal("Bearer", scheme);
                Assert.NotNull(parameter);
            }
            TestRestClientBuilder(assertion);
        }

        [Fact]
        public void GetCustomiazeClient_BaseUriRightFact()
        {
            RestClientBuilder restClientBuilder = new RestClientBuilder(_connectionString, productInfo);
            using var restClient = restClientBuilder.Build();

            Assert.Equal(Endpoint, restClient.BaseUri.AbsoluteUri);
        }

        private async void TestRestClientBuilder(Action<HttpRequestMessage, CancellationToken> assertion)
        {
            var handler = new TestRootHandler(assertion);
            RestClientBuilder restClientBuilder = new RestClientBuilder(_connectionString, productInfo)
                .WithRootHandler(handler);

            using var restClient = restClientBuilder.Build();
            await restClient.HealthApi.GetHealthStatusWithHttpMessagesAsync();
        }
    }
}