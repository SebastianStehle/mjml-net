using Mjml.Net.Extensions;

namespace Mjml.Net;

/// <summary>
/// The default ID generator.
/// </summary>
public sealed class DefaultIDGenerator : IIdGenerator
{
    private int counter;

    /// <summary>
    /// The only instance of the <see cref="DefaultIDGenerator"/>.
    /// </summary>
    public static readonly IIdGenerator Instance = new DefaultIDGenerator();

    private DefaultIDGenerator()
    {
    }

    /// <inheritdoc />
    public string Next()
    {
        Interlocked.Increment(ref counter);

        return counter.ToInvariantString();
    }
}
