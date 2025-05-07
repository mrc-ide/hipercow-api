// Copyright (c) Imperial College London. All rights reserved.

using Microsoft.Extensions.Caching.Memory;

/// <summary>
/// Memory for keeping the session information in ram on the server.
/// </summary>
public class UserSessionManager(IMemoryCache cache)
{
    private readonly IMemoryCache cache = cache;
    private readonly TimeSpan sessionLifetime = TimeSpan.FromDays(7);

    /// <summary>
    /// Store the session in the cache, with the session id as key.
    /// </summary>
    /// <param name="session">The session object to save.</param>
    /// <returns>The session id.</returns>
    public string StoreSession(UserSession session)
    {
        var sessionId = Guid.NewGuid().ToString();
        this.cache.Set(sessionId, session, this.sessionLifetime);
        return sessionId;
    }

    /// <summary>
    /// Retrieve a session from memory.
    /// </summary>
    /// <param name="sessionId">The session id to retrieve.</param>
    /// <returns>The session object.</returns>
    public UserSession? RetrieveSession(string sessionId)
    {
        return this.cache.TryGetValue(sessionId, out UserSession? session)
            ? session
            : null;
    }

    /// <summary>
    /// Remove a session from memory.
    /// </summary>
    /// <param name="sessionId">The session id to remove.</param>
    public void RemoveSession(string sessionId)
    {
        if (this.cache.TryGetValue(sessionId, out UserSession? session))
        {
            this.cache.Remove(sessionId);
        }
    }
}
