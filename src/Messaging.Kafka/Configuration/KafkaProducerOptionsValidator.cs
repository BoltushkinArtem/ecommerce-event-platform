using Microsoft.Extensions.Options;

namespace Messaging.Kafka.Configuration;

public sealed class KafkaProducerOptionsValidator: IValidateOptions<KafkaProducerOptions>
{
    public ValidateOptionsResult Validate(
        string? name,
        KafkaProducerOptions options)
    {
        var failures = new List<string>();
        
        return failures.Count > 0
            ? ValidateOptionsResult.Fail(failures)
            : ValidateOptionsResult.Success;
    }
}
