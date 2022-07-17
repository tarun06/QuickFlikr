using QuickFlikr.Model;

namespace QuickFlikr.Service
{
    public interface IFlikrFeedService
    {
        Task<IEnumerable<FeedInfo>> GetFlikrFeedAsync(string searchInput, CancellationToken cancellationToken = default);
    }
}