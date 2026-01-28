using System.Collections.Concurrent;

public static class AgentPresence
{
	private static readonly ConcurrentDictionary<string, AgentState> agents = new();

	public static void SetOnline(string id)
	{
		agents[id] = new AgentState
		{
			Online = true,
			LastSeenUtc = DateTime.UtcNow
		};
	}

	public static void Touch(string id)
	{
		agents.AddOrUpdate(id,
			_ => new AgentState { Online = true, LastSeenUtc = DateTime.UtcNow },
			(_, s) =>
			{
				s.LastSeenUtc = DateTime.UtcNow;
				return s;
			});
	}

	public static void SetOffline(string id)
	{
		if (agents.TryGetValue(id, out var s))
		{
			s.Online = false;
			s.LastSeenUtc = DateTime.UtcNow;
		}
	}

	public static AgentState? Get(string id) =>
		agents.TryGetValue(id, out var s) ? s : null;
}