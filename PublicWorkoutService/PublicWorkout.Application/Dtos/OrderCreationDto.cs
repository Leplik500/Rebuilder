namespace PublicWorkout.Application.Dtos;

public class OrderCreationDto
{
    public OrderCreationDto(Guid id, List<ProductDto> products)
    {
        this.Id = id;
        this.Products = products;
    }

    public Guid Id { get; }
    public List<ProductDto> Products { get; set; }
}
