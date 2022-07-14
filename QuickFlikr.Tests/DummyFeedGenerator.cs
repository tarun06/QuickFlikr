using QuickFlikr.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QuickFlikr.Tests
{
    public class DummyFeedGenerator
    {
        public static IEnumerable<FeedInfo> GeneratorFeedInfo()
        {
            IList<FeedInfo> flikrFeeds = new List<FeedInfo>();
            flikrFeeds.Add(ConstructFeedInfo("Alexey", "cat", "cat", "new cat", "new_cat_path"));
            flikrFeeds.Add(ConstructFeedInfo("Alexey Roger", "ketty", "cat", "new ketty", "new_ketty_path"));
            flikrFeeds.Add(ConstructFeedInfo("Maks Bupa", "doll", "doll", "new doll", "new_doll_path"));
            flikrFeeds.Add(ConstructFeedInfo("Maks Galaxy", "home", "home", "home", "home_path"));
            flikrFeeds.Add(ConstructFeedInfo("NikSmith", "moon", "moon", "moon", "moon_path"));
            return flikrFeeds;
        }

        private static FeedInfo ConstructFeedInfo(string author, string title, string tags, string desription, string path)
        {
            return new FeedInfo()
            {
                Author = author,
                Description = desription,
                Link = author + desription,
                Media = new Media() { Path = path },
                Tags = tags,
                Published = DateTime.Now,
                Author_Id = author + new Random().Next(0, 100).ToString(),
                Title = title
            };
        }

        internal static IEnumerable<FeedInfo> GeneratorFeedInfo(string searchText)
        {
            return GeneratorFeedInfo().ToList().Where(x => x.Tags == searchText);
        }
    }
}