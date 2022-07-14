using QuickFlikr.Model;

namespace QuickFlikr.Service
{
    public interface IFlickrFeedService
    {
        Task<IEnumerable<FeedInfo>> GetFlickrFeedAsync(string searchInput, CancellationToken cancellationToken = default);
    }
}