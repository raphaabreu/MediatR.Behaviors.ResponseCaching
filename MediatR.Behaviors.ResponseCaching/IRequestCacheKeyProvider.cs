namespace MediatR.Behaviors.ResponseCaching
{
    public interface IRequestCacheKeyProvider<in TRequest>
    {
        string GetCacheKey(TRequest request);
    }
}