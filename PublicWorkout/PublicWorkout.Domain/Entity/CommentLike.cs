namespace PublicWorkout.Domain.Entity;

public class CommentLike
{
    public Guid CommentId { get; set; }
    public Guid UserId { get; set; }
    public DateTime CreatedAt { get; set; }
}
