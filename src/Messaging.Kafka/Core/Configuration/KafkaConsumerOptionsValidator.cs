using Microsoft.Extensions.Options;

namespace Messaging.Kafka.Core.Configuration;

public sealed class KafkaConsumerOptionsValidator: IValidateOptions<KafkaConsumerOptions>
{
    public ValidateOptionsResult Validate(
        string? name,
        KafkaConsumerOptions options)
    {
        var failures = new List<string>();
        
        if (string.IsNullOrWhiteSpace(options.GroupId))
        {
            failures.Add("KafkaConsumerOptions: GroupId is required and cannot be empty.");
        }
        
        return failures.Count > 0
            ? ValidateOptionsResult.Fail(failures)
            : ValidateOptionsResult.Success;
    }
}
