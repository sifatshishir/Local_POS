#include <gtest/gtest.h>
#include "OrderRepository.h"
#include "ConnectionManager.h"
#include "../../POS.Core/include/Domain/OrderItem.h"

using namespace POS::Data;
using namespace POS::Core::Domain;

class OrderRepositoryIntegrationTests : public ::testing::Test {
protected:
    std::shared_ptr<ConnectionManager> manager;
    std::shared_ptr<OrderRepository> repo;

    void SetUp() override {
        manager = std::make_shared<ConnectionManager>();
        repo = std::make_shared<OrderRepository>(manager);
    }
};

TEST_F(OrderRepositoryIntegrationTests, Create_InsertsOrderAndReturnsId) {
    try {
        Order order(0, OrderType::DineIn, 1);
        order.Status = OrderStatus::Ordered;
        order.TotalAmount = 50.0;
        
        // Add a dummy item (Requires MenuItem ID 1 to exist in DB seed)
        // If Seed data implies ID 1 exists, this works.
        // SAFEGUARD: We should ideally insert a menu item first or assume valid seed.
        // For now, assuming ID 1 is valid (e.g. Burger).
        order.Items.emplace_back(1, "TestItem", 50.0, 1);

        int newId = repo->Create(order);
        EXPECT_GT(newId, 0);

        auto fetched = repo->GetById(newId);
        ASSERT_NE(fetched, nullptr);
        EXPECT_EQ(fetched->TotalAmount, 50.0);
        EXPECT_EQ(fetched->Items.size(), 1);
    }
    catch (std::exception& e) {
        FAIL() << "Exception: " << e.what(); 
    }
}
