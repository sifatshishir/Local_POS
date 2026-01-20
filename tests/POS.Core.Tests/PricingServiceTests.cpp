#include <gtest/gtest.h>
#include "Services/PricingService.h"
#include "Domain/OrderItem.h"

using namespace POS::Core::Services;
using namespace POS::Core::Domain;

TEST(PricingServiceTests, CalculateSubtotal_SumOfItems) {
    PricingService service;
    std::vector<OrderItem> items;
    items.emplace_back(1, "Burger", 10.0, 2); // 20.0
    items.emplace_back(2, "Fries", 5.0, 1);   // 5.0

    double subtotal = service.CalculateSubtotal(items);
    EXPECT_DOUBLE_EQ(subtotal, 25.0);
}

TEST(PricingServiceTests, CalculateSubtotal_EmptyReturnsZero) {
    PricingService service;
    std::vector<OrderItem> items;
    
    EXPECT_DOUBLE_EQ(service.CalculateSubtotal(items), 0.0);
}

TEST(PricingServiceTests, CalculateTax_StandardRate) {
    PricingService service;
    double tax = service.CalculateTax(100.0, 0.05); // 5%
    EXPECT_DOUBLE_EQ(tax, 5.0);
}

TEST(PricingServiceTests, CalculateTotal_AddsTax) {
    PricingService service;
    std::vector<OrderItem> items;
    items.emplace_back(1, "Burger", 100.0, 1);

    double total = service.CalculateTotal(items, 0.05);
    EXPECT_DOUBLE_EQ(total, 105.0); // 100 + 5
}
