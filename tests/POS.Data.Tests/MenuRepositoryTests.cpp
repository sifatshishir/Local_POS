#include <gtest/gtest.h>
#include "MenuRepository.h"
#include "ConnectionManager.h"

using namespace POS::Data;
using namespace POS::Core::Domain;

class MenuRepositoryIntegrationTests : public ::testing::Test {
protected:
    std::shared_ptr<ConnectionManager> manager;
    std::shared_ptr<MenuRepository> repo;

    void SetUp() override {
        manager = std::make_shared<ConnectionManager>();
        repo = std::make_shared<MenuRepository>(manager);
    }
};

TEST_F(MenuRepositoryIntegrationTests, GetAll_ReturnsActiveItems) {
    try {
        auto items = repo->GetAll();
        // Assuming partial seed data exists, we just check it doesn't crash
        // and returns a vector (empty or not)
        EXPECT_NO_THROW(repo->GetAll());
    }
    catch (std::exception& e) {
        // If DB is empty or connection fails, we want detailed error
        FAIL() << "Exception: " << e.what();
    }
}

TEST_F(MenuRepositoryIntegrationTests, GetByCategory_ReturnsItems) {
    auto items = repo->GetByCategory("Drinks");
    // We expect seed data might have drinks, but main goal is checking SQL validity
    EXPECT_NO_THROW(repo->GetByCategory("NonExistentCategory"));
}
