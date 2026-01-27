using Microsoft.Extensions.Options;

namespace Messaging.Kafka.Configuration;

public sealed class KafkaConsumerOptionsValidator: IValidateOptions<KafkaConsumerOptions>
{
    public ValidateOptionsResult Validate(
        string? name,
        KafkaConsumerOptions options)
    {
        var failures = new List<string>();
        
        return failures.Count > 0
            ? ValidateOptionsResult.Fail(failures)
            : ValidateOptionsResult.Success;
    }
}
