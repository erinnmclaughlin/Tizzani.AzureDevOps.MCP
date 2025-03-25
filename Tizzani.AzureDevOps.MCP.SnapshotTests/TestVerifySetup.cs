namespace Tizzani.AzureDevOps.MCP.SnapshotTests;

public class TestVerifySetup
{
    [Fact]
    public Task CheckVerifySetup() => VerifyChecks.Run();
}