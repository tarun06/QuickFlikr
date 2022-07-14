using QuickFlikr.Model;
using System;
using System.Collections.Generic;

namespace QuickFlikr.Tests
{
    public class DummyFeedGenerator
    {
        public static IEnumerable<FeedInfo> GeneratorFeedInfo()
        {
            IList<FeedInfo> flikrFeeds = new List<FeedInfo>();
            flikrFeeds.Add(ConstructFeedInfo("Alexey", "cat", "new cat", "new_cat_path"));
            flikrFeeds.Add(ConstructFeedInfo("Maks Galaxy", "home", "home", "home_path"));
            flikrFeeds.Add(ConstructFeedInfo("NikSmith", "moon", "moon", "moon_path"));
            return flikrFeeds;
        }

        private static FeedInfo ConstructFeedInfo(string author, string title, string desription, string path)
        {
            return new FeedInfo()
            {
                Author = author,
                Description = desription,
                Link = author + desription,
                Media = new Media() { Path = path },
                Tags = "CatFamily",
                Published = DateTime.Now,
                Author_Id = author + new Random().Next(0, 100).ToString(),
                Title = title
            };
        }
    }
}