#pragma once

#include "../Domain/OrderItem.h"
#include <vector>

namespace POS {
namespace Core {
namespace Services {

    /// <summary>
    /// Service responsible for all pricing, tax, and total calculations.
    /// Pure logic service with no external dependencies.
    /// </summary>
    class PricingService {
    public:
        /// <summary>
        /// Calculates the sum of all item subtotals.
        /// </summary>
        double CalculateSubtotal(const std::vector<Domain::OrderItem>& items);

        /// <summary>
        /// Calculates tax amount based on subtotal.
        /// Default tax rate is 5% (0.05).
        /// </summary>
        double CalculateTax(double subtotal, double taxRate = 0.05);

        /// <summary>
        /// Calculates final total (Subtotal + Tax).
        /// </summary>
        double CalculateTotal(const std::vector<Domain::OrderItem>& items, double taxRate = 0.05);
    };

} // namespace Services
} // namespace Core
} // namespace POS
