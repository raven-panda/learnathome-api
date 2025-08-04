using Xunit.Abstractions;

namespace LearnAtHomeApi.FunctionalTests;

public class DisplayNameOrderer : ITestCollectionOrderer
{
    public IEnumerable<ITestCollection> OrderTestCollections(
        IEnumerable<ITestCollection> testCollections
    ) => testCollections.OrderBy(collection => collection.DisplayName);
}
