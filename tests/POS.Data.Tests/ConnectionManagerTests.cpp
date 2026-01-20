#include <gtest/gtest.h>
#include "ConnectionManager.h"
#include <mysql_connection.h>

using namespace POS::Data;

TEST(ConnectionManagerIntegrationTests, GetConnection_ReturnsValidConnection) {
    try {
        ConnectionManager manager;
        auto conn = manager.GetConnection();
        ASSERT_NE(conn, nullptr);
        EXPECT_TRUE(conn->isValid());
        EXPECT_FALSE(conn->isClosed());
    }
    catch (std::exception& e) {
        FAIL() << "Exception thrown: " << e.what();
    }
}
