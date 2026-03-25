namespace TwoDrive.IntegrationTest.TestInfrastructure;

[CollectionDefinition(Name)]
public sealed class TestContainersCollection : ICollectionFixture<TestContainersFixture>
{
    public const string Name = "test-containers";
}
