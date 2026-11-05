using Hangfire.Common;
using Hangfire.Storage;
using Kahrad.Pricing.Sms.Application.Abstractions.Services;
using Xunit;
namespace Kahrad.Pricing.Hosts.IntegrationTests;
public sealed class WorkerSerializationTests
{
    [Fact]
    public void Sms_job_round_trips_using_the_worker_serializer()
    {
        var job = Job.FromExpression<ISmsOutboxProcessor>(processor => processor.ProcessPendingOutboxAsync(CancellationToken.None));
        var restored = InvocationData.SerializeJob(job).DeserializeJob();
        Assert.Equal(typeof(ISmsOutboxProcessor), restored.Type);
        Assert.Equal(nameof(ISmsOutboxProcessor.ProcessPendingOutboxAsync), restored.Method.Name);
        Assert.Single(restored.Args);
        Assert.Equal(CancellationToken.None, restored.Args[0]);
    }
}
