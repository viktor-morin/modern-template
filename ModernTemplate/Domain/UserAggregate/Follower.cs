namespace ModernTemplate.Domain.UserAggregate;

public record Follower
{
    private Follower()
    {
    }

    public static Follower Create()
    {
        return new Follower();
    }
}