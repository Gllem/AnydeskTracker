namespace AnydeskTracker.Extensions;

public static class StringExtensions
{
	public static string NormalizeUrl(this string url)
	{
		url = url.Trim();
		if (url.EndsWith("/")) url = url[..^1];
		return url.ToLowerInvariant();
	}
}