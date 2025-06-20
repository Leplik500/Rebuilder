namespace PublicWorkout.Domain.Entity;

public class Tax
{
    public Guid Id { get; set; }
    public decimal Amount { get; private set; }

    public void DecreaseByPercentage(int percent)
    {
        var dec = this.Amount / 100 * percent;
        this.Amount = this.Amount - dec;
    }
}
