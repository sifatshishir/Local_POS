#include <gtest/gtest.h>
#include "Services/OrderService.h"
#include "Interfaces/IOrderRepository.h"
#include "Domain/Order.h"

using namespace POS::Core::Services;
using namespace POS::Core::Interfaces;
using namespace POS::Core::Domain;

// Manual Mock for IOrderRepository
class MockOrderRepository : public IOrderRepository {
public:
    std::vector<Order> orders;
    int nextId = 1;

    int Create(const Order& order) override {
        Order newOrder = order;
        newOrder.Id = nextId++;
        orders.push_back(newOrder);
        return newOrder.Id;
    }

    void UpdateStatus(int id, OrderStatus status) override {
        for (auto& order : orders) {
            if (order.Id == id) {
                order.Status = status;
                break;
            }
        }
    }

    std::shared_ptr<Order> GetById(int id) override {
        for (const auto& order : orders) {
            if (order.Id == id) {
                return std::make_shared<Order>(order);
            }
        }
        return nullptr;
    }

    std::vector<Order> GetByStatus(OrderStatus status) override {
        std::vector<Order> result;
        for (const auto& order : orders) {
            if (order.Status == status) {
                result.push_back(order);
            }
        }
        return result;
    }
};

class OrderServiceTests : public ::testing::Test {
protected:
    std::shared_ptr<MockOrderRepository> mockRepo;
    std::shared_ptr<PricingService> pricingService;
    std::shared_ptr<OrderService> api;

    void SetUp() override {
        mockRepo = std::make_shared<MockOrderRepository>();
        pricingService = std::make_shared<PricingService>();
        api = std::make_shared<OrderService>(mockRepo, pricingService);
    }
};

TEST_F(OrderServiceTests, CreateDineInOrder_CreatesOrderWithCorrectType) {
    std::vector<OrderItem> items;
    items.emplace_back(1, "Burger", 10.0, 2);

    Order created = api->CreateDineInOrder(5, items);

    EXPECT_GT(created.Id, 0);
    EXPECT_EQ(created.Type, OrderType::DineIn);
    EXPECT_EQ(created.TableNumber, 5);
    EXPECT_EQ(created.Items.size(), 1);
    EXPECT_EQ(created.TotalAmount, 21.0); // (10 * 2) + 5% tax = 20 + 1
}

TEST_F(OrderServiceTests, CreateParcelOrder_CreatesOrderWithProvider) {
    std::vector<OrderItem> items;
    items.emplace_back(2, "Soda", 2.0, 5);

    Order created = api->CreateParcelOrder(ParcelProvider::FoodPanda, items);

    EXPECT_EQ(created.Type, OrderType::Parcel);
    EXPECT_EQ(created.Provider, ParcelProvider::FoodPanda);
    EXPECT_EQ(created.TableNumber, 0);
    EXPECT_EQ(created.TotalAmount, 10.5); // (2*5) + 5% = 10 + 0.5
}
