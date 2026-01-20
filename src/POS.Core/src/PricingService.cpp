#include "Services/PricingService.h"

namespace POS {
namespace Core {
namespace Services {

    double PricingService::CalculateSubtotal(const std::vector<Domain::OrderItem>& items) {
        double subtotal = 0.0;
        for (const auto& item : items) {
            subtotal += item.GetSubtotal();
        }
        return subtotal;
    }

    double PricingService::CalculateTax(double subtotal, double taxRate) {
        return subtotal * taxRate;
    }

    double PricingService::CalculateTotal(const std::vector<Domain::OrderItem>& items, double taxRate) {
        double subtotal = CalculateSubtotal(items);
        double tax = CalculateTax(subtotal, taxRate);
        return subtotal + tax;
    }

} // namespace Services
} // namespace Core
} // namespace POS
