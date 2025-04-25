namespace EcommerceAcyclicVisitor
{
    namespace Models
    {
        /// <summary>
        /// Представляет товарную позицию в заказе.
        /// </summary>
        public class OrderItem
        {
            public string ProductId { get; }
            public int Quantity { get; }
            public decimal PricePerUnit { get; }

            public OrderItem(string productId, int quantity, decimal pricePerUnit)
            {
                if (string.IsNullOrWhiteSpace(productId))
                    throw new ArgumentException("Product ID cannot be empty.", nameof(productId));
                if (quantity <= 0)
                    throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be positive.");
                if (pricePerUnit < 0)
                    throw new ArgumentOutOfRangeException(nameof(pricePerUnit), "Price cannot be negative.");

                ProductId = productId;
                Quantity = quantity;
                PricePerUnit = pricePerUnit;
            }

            public override string ToString()
            {
                return $"Product: {ProductId}, Qty: {Quantity}, Price: {PricePerUnit:C}";
            }
        }
    }
}