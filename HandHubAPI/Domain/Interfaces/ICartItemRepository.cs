namespace HandHubAPI.Domain.Repositories;

using HandHubAPI.Domain.Entities;
using HandHubAPI.Domain.Interfaces;

public interface ICartItemRepository : IBaseRepository<CartItemEntity>
{
    /// <summary>
    /// Gets the cart item by cart identifier and product identifier.
    /// </summary>
    /// <param name="cartId">The cart identifier.</param>
    /// <param name="productId">The product identifier.</param>
    /// <returns>The cart item entity if found, otherwise null.</returns>
    Task<CartItemEntity?> GetByCartAndProductIdAsync(int cartId, int productId);

    /// <summary>
    /// Checks if a cart item exists for the specified cart and product.
    /// </summary>
    /// <param name="cartId">The cart identifier.</param>
    /// <param name="productId">The product identifier.</param>
    /// <returns>True if the cart item exists, otherwise false.</returns>
    Task<bool> ExistsByCartAndProductIdAsync(int cartId, int productId);

    /// <summary>
    /// Gets all cart items by cart identifier.
    /// </summary>
    /// <param name="cartId">The cart identifier.</param>
    /// <returns>A list of cart item entities.</returns>
    Task<IEnumerable<CartItemEntity>> GetByCartIdAsync(int cartId, int userId = 0);
}