using System.Text;
using Microsoft.Extensions.Options;

namespace Concepts.Crud.WebApi.Api;

public class ProblemHrefProvider(IOptions<ProblemHrefProvider.TOptions> options)
    : IProblemHrefProvider
{
    private readonly TOptions _options = options.Value;

    public string GetExecuteCommandProblemRef(string commandName, Guid commandId)
    {
        return AddQueryString(_options.ExecuteCommandProblemRef.BaseUrl, "command", commandName, "id", commandId.ToString("D"));
    }

    private static string AddQueryString(string baseUrl, params string[] kvp)
    {
        ArgumentNullException.ThrowIfNull(baseUrl);
        ArgumentNullException.ThrowIfNull(kvp);
        if (kvp.Length % 2 != 0 || kvp.Length == 0) throw new ArgumentException("length mismatch", nameof(kvp));

        var sb = new StringBuilder(baseUrl);
        if (!baseUrl.Contains("?"))
        {
            sb.Append('?');
        }

        for (var i = 0; i < kvp.Length; i += 2)
        {
            sb.Append(Uri.EscapeDataString(kvp[i]));
            sb.Append('=');
            sb.Append(Uri.EscapeDataString(kvp[i + 1]));
            sb.Append('&');
        }

        if (sb.Length == 0)
            return string.Empty;

        sb.Length -= 1;
        return sb.ToString();
    }

    public class TOptions
    {
        [Required]
        public required TExecuteCommandProblemRef ExecuteCommandProblemRef { get; init; }
        public class TExecuteCommandProblemRef
        {
            [Required] public required string BaseUrl { get; init; }
        }
    }
}