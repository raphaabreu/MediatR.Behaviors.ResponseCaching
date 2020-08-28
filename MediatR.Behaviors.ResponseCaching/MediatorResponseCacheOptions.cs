using System;
using Microsoft.Extensions.Caching.Distributed;

namespace MediatR.Behaviors.ResponseCaching
{
    public class MediatorResponseCacheOptions<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        public DistributedCacheEntryOptions EntryOptions { get; set; } = new DistributedCacheEntryOptions();
        public string KeyPrefix { get; set; }
        public Func<TRequest, string> RequestHashDelegate { get; set; } = DefaultRequestHasher.JsonMd5;
    }
}
