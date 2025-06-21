namespace HandHubAPI.Domain.Repositories;

using HandHubAPI.Domain.Entities;
using HandHubAPI.Domain.Interfaces;

public interface ICartRepository : IBaseRepository<CartEntity>
{
    /// <summary>
    /// Gets the cart by user identifier.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <returns>The cart entity associated with the user.</returns>
    Task<CartEntity?> GetByUserIdAsync(int userId);

    /// <summary>
    /// Checks if a cart exists for the specified user.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <returns>True if a cart exists, otherwise false.</returns>
    Task<bool> ExistsByUserIdAsync(int userId);
}