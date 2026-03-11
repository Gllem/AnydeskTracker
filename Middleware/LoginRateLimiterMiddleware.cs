// LoginRateLimiterMiddleware.cs

using Microsoft.Extensions.Caching.Memory;

public class LoginRateLimiterMiddleware(RequestDelegate next)
{
	private static readonly MemoryCache Cache = new(new MemoryCacheOptions());

	public async Task InvokeAsync(HttpContext context)
	{
		if (context.Request.Path.StartsWithSegments("/Identity/Account/Login")
		    && context.Request.Method == "POST")
		{
			var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
			var key = $"login_attempts_{ip}";

			var attempts = Cache.GetOrCreate(key, entry =>
			{
				entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1);
				return 0;
			});

			if (attempts >= 10)
			{
				context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
				await context.Response.WriteAsync("Too many attempts!");
				return;
			}

			Cache.Set(key, attempts + 1, TimeSpan.FromMinutes(1));
		}

		await next(context);
	}
}