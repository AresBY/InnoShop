namespace InnoShop.Products.Application.Dtos
{
    public record ProductDto
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = default!;
        public string Description { get; init; } = default!;
        public decimal Price { get; init; }
        public bool IsAvailable { get; init; }
        public Guid CreatedByUserId { get; init; }
        public DateTime CreatedAt { get; init; }

        public bool IsVisible { get; set; }
    }
}
