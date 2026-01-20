#pragma once

#include <string>

namespace POS {
namespace Core {
namespace Domain {

    /// <summary>
    /// Represents a single line item within an order.
    /// </summary>
    class OrderItem {
    public:
        int MenuItemId;
        std::string MenuItemName; // Cached name for display clarity
        double Price;             // Price per unit at the time of order
        int Quantity;

        OrderItem(int menuItemId, std::string menuItemName, double price, int quantity)
            : MenuItemId(menuItemId), MenuItemName(std::move(menuItemName)), Price(price), Quantity(quantity) {
        }

        /// <summary>
        /// Calculates the subtotal for this line item.
        /// </summary>
        double GetSubtotal() const {
            return Price * Quantity;
        }
    };

} // namespace Domain
} // namespace Core
} // namespace POS
