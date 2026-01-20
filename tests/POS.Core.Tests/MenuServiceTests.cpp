#include <gtest/gtest.h>
#include "Services/MenuService.h"
#include "Interfaces/IMenuRepository.h"
#include "Domain/MenuItem.h"

using namespace POS::Core::Services;
using namespace POS::Core::Interfaces;
using namespace POS::Core::Domain;

// Manual Mock for IMenuRepository
class MockMenuRepository : public IMenuRepository {
public:
    std::vector<MenuItem> items;

    MockMenuRepository() {
        items.emplace_back(1, "Burger", 10.0, "Fast Food", "Tasty Burger", true);
        items.emplace_back(2, "Soda", 2.0, "Drinks", "Cold Soda", true);
        items.emplace_back(3, "InactiveItem", 5.0, "Unknown", "Old item", false);
    }

    std::vector<MenuItem> GetAll() override {
        // Return only active items to simulate proper behavior? 
        // Or Repository returns all, Service filters? 
        // Let's assume Repository returns specific set.
        return items;
    }

    std::shared_ptr<MenuItem> GetById(int id) override {
        for (const auto& item : items) {
            if (item.Id == id) {
                return std::make_shared<MenuItem>(item);
            }
        }
        return nullptr;
    }

    std::vector<MenuItem> GetByCategory(const std::string& category) override {
        std::vector<MenuItem> result;
        for (const auto& item : items) {
            if (item.Category == category) {
                result.push_back(item);
            }
        }
        return result;
    }
};

TEST(MenuServiceTests, GetAllMenuItems_ReturnsAllItems) {
    auto mockRepo = std::make_shared<MockMenuRepository>();
    MenuService service(mockRepo);

    auto result = service.GetAllMenuItems();
    EXPECT_EQ(result.size(), 3);
    EXPECT_EQ(result[0].Name, "Burger");
}

TEST(MenuServiceTests, GetMenuItemById_ReturnsCorrectItem) {
    auto mockRepo = std::make_shared<MockMenuRepository>();
    MenuService service(mockRepo);

    auto item = service.GetMenuItemById(1);
    ASSERT_NE(item, nullptr);
    EXPECT_EQ(item->Name, "Burger");
}

TEST(MenuServiceTests, GetMenuItemById_ReturnsNullForInvalidId) {
    auto mockRepo = std::make_shared<MockMenuRepository>();
    MenuService service(mockRepo);

    auto item = service.GetMenuItemById(999);
    EXPECT_EQ(item, nullptr);
}

TEST(MenuServiceTests, GetMenuItemsByCategory_ReturnsFilteredList) {
    auto mockRepo = std::make_shared<MockMenuRepository>();
    MenuService service(mockRepo);

    auto result = service.GetMenuItemsByCategory("Drinks");
    EXPECT_EQ(result.size(), 1);
    EXPECT_EQ(result[0].Name, "Soda");
}
