using WTNS.Net.Models;

namespace WTNS.Net;

public class Session
{
    public string Token { get; init; }
    public DateTime ExpiryTime { get; set; }
    public User User { get; init; }

    /// <summary>
    /// Session contructor
    /// </summary>
    /// <param name="timeoutMilliseconds">The amount of time in milliseconds to give the session before expiring.</param>
    public Session(int timeoutMilliseconds = 300000)
    {
        Token = Guid.NewGuid().ToString();
        ExpiryTime = DateTime.UtcNow.AddMinutes(timeoutMilliseconds);
    }

    /// <summary>
    /// Determines whether the Session is still active or has expired.
    /// </summary>
    /// <param name="session"></param>
    /// <returns></returns>
    public static bool IsActive(Session session)
    {
        return DateTime.UtcNow <= session.ExpiryTime;
    }

    /// <summary>
    /// Extends the time until session expiry.
    /// </summary>
    /// <param name="session">The session to extend.</param>
    /// <param name="miliseconds">The amount of time in milliseconds to extend it by.</param>
    public static void Extend(Session session, int miliseconds)
    {
        session.ExpiryTime = session.ExpiryTime.AddMilliseconds(miliseconds);
    }
}
