using PublicWorkout.Domain.Entity;
using PublicWorkout.Infrastructure;

namespace PublicWorkout.Domain.Aggregate;

public class Order : IAggregateRoot
{
    public Guid Id { get; }
    public DateTime CreationDate { get; }
    private List<Product> _items;
    private List<Tax> _taxes;

    public IReadOnlyCollection<Product> Items => this._items.AsReadOnly();
    public IReadOnlyCollection<Tax> Taxes => this._taxes.AsReadOnly();

    public Order(Guid id)
    {
        this.Id = id;
        this.CreationDate = DateTime.UtcNow;
        this._items = new List<Product>();
        this._taxes = new List<Tax>();
    }

    public void AddProduct(Product product)
    {
        // The Agregate can determine whether it is possible to add such a product
        if (!this.CanAddProduct(product))
        {
            return;
        }
        this._items.Add(product);
        // recalculating taxes and the total cost
        this.RecalculateTaxesAndTotalPrice();
    }

    public decimal GetTaxesAmount()
    {
        return this._taxes.Sum(x => x.Amount);
    }

    public decimal GetTotalPrice()
    {
        return this._items.Sum(item => item.Price * item.Quantity)
            + this._taxes.Sum(tax => tax.Amount);
    }

    private void RecalculateTaxesAndTotalPrice(int taxPercent = 0)
    {
        if (taxPercent != 0)
        {
            this._taxes.ForEach(tax => tax.DecreaseByPercentage(taxPercent));
        }
    }

    private bool CanAddProduct(Product product)
    {
        //some checks
        return true;
    }
}
