#include "OrderRepository.h"
#include <iostream>

namespace POS {
namespace Data {

    OrderRepository::OrderRepository(std::shared_ptr<ConnectionManager> connectionManager)
        : m_connectionManager(std::move(connectionManager)) {
    }

    int OrderRepository::Create(const Core::Domain::Order& order) {
        try {
            auto conn = m_connectionManager->GetConnection();
            conn->setAutoCommit(false); // Start Transaction

            // 1. Insert Order
            std::unique_ptr<sql::PreparedStatement> stmt(conn->prepareStatement(
                "INSERT INTO orders (order_type, table_number, status, total_amount, parcel_provider, created_at) VALUES (?, ?, ?, ?, ?, NOW())"
            ));

            stmt->setString(1, OrderTypeToString(order.Type));
            
            // Handle table_number - set NULL for Parcel orders
            if (order.Type == Core::Domain::OrderType::DineIn) {
                stmt->setInt(2, order.TableNumber);
            } else {
                stmt->setNull(2, sql::DataType::INTEGER);
            }
            
            stmt->setString(3, StatusToString(order.Status));
            stmt->setDouble(4, order.TotalAmount);
            stmt->setString(5, ProviderToString(order.Provider));
            
            stmt->executeUpdate();

            // Get Generated ID
            std::unique_ptr<sql::Statement> idStmt(conn->createStatement());
            std::unique_ptr<sql::ResultSet> res(idStmt->executeQuery("SELECT LAST_INSERT_ID()"));
            int newOrderId = 0;
            if (res->next()) {
                newOrderId = res->getInt(1);
            }

            // 2. Insert Order Items (Batching would be better, but loop is simple for now)
            std::unique_ptr<sql::PreparedStatement> itemStmt(conn->prepareStatement(
                "INSERT INTO order_items (order_id, menu_item_id, quantity, price) VALUES (?, ?, ?, ?)"
            ));

            for (const auto& item : order.Items) {
                itemStmt->setInt(1, newOrderId);
                itemStmt->setInt(2, item.MenuItemId);
                itemStmt->setInt(3, item.Quantity);
                itemStmt->setDouble(4, item.Price);
                itemStmt->executeUpdate();
            }

            conn->commit(); // Commit Transaction
            conn->setAutoCommit(true);
            return newOrderId;
        }
        catch (sql::SQLException& e) {
            std::cerr << "SQL Error in Create Order: " << e.what() << std::endl;
            // Rollback is implicit if connection closes, but good to be explicit if shared
            // However, rolling back a shared connection might affect others if not careful.
            // With AutoCommit=false, it should just rollback on destruction or explicitly.
            auto conn = m_connectionManager->GetConnection();
            conn->rollback();
            conn->setAutoCommit(true);
            throw;
        }
    }

    void OrderRepository::UpdateStatus(int id, Core::Domain::OrderStatus status) {
        try {
            auto conn = m_connectionManager->GetConnection();
            std::unique_ptr<sql::PreparedStatement> stmt(conn->prepareStatement(
                "UPDATE orders SET status = ? WHERE id = ?"
            ));
            stmt->setString(1, StatusToString(status));
            stmt->setInt(2, id);
            stmt->executeUpdate();
        }
        catch (sql::SQLException& e) {
            std::cerr << "SQL Error in UpdateStatus: " << e.what() << std::endl;
            throw;
        }
    }

    std::shared_ptr<Core::Domain::Order> OrderRepository::GetById(int id) {
        try {
            auto conn = m_connectionManager->GetConnection();
            std::unique_ptr<sql::PreparedStatement> stmt(conn->prepareStatement("SELECT * FROM orders WHERE id = ?"));
            stmt->setInt(1, id);
            std::unique_ptr<sql::ResultSet> res(stmt->executeQuery());

            if (res->next()) {
                // Determine Type/Provider first to call correct constructor
                std::string typeStr = res->getString("order_type");
                Core::Domain::OrderType type = StringToOrderType(typeStr);
                int tableNum = res->getInt("table_number");

                // Note: This logic assumes if type is Parcel, we use the provider constructor, 
                // but Order class is currently simpler. We can just set properties.
                
                auto order = std::make_shared<Core::Domain::Order>(res->getInt("id"), tableNum);
                order->Type = type;
                order->Status = StringToStatus(res->getString("status"));
                order->TotalAmount = res->getDouble("total_amount");
                order->Provider = StringToProvider(res->getString("parcel_provider"));
                
                // Fetch Items
                order->Items = GetItemsForOrder(id, conn.get());
                
                return order;
            }
        }
        catch (sql::SQLException& e) {
            std::cerr << "SQL Error in GetById: " << e.what() << std::endl;
            throw;
        }
        return nullptr;
    }

    std::vector<Core::Domain::Order> OrderRepository::GetByStatus(Core::Domain::OrderStatus status) {
        std::vector<Core::Domain::Order> orders;
        try {
            auto conn = m_connectionManager->GetConnection();
            std::unique_ptr<sql::PreparedStatement> stmt(conn->prepareStatement("SELECT * FROM orders WHERE status = ?"));
            stmt->setString(1, StatusToString(status));
            std::unique_ptr<sql::ResultSet> res(stmt->executeQuery());

            while (res->next()) {
                 std::string typeStr = res->getString("order_type");
                 Core::Domain::OrderType type = StringToOrderType(typeStr);
                 int id = res->getInt("id");

                 Core::Domain::Order order(id, res->getInt("table_number"));
                 order.Type = type;
                 order.Status = status; // Known
                 order.TotalAmount = res->getDouble("total_amount");
                 order.Provider = StringToProvider(res->getString("parcel_provider"));
                 
                 // Ideally we lazily load items or fetch in batch, but for now:
                 order.Items = GetItemsForOrder(id, conn.get());
                 
                 orders.push_back(order);
            }
        }
        catch (sql::SQLException& e) {
            std::cerr << "SQL Error in GetByStatus: " << e.what() << std::endl;
            throw;
        }
        return orders;
    }

    std::vector<Core::Domain::Order> OrderRepository::GetByStatusPaginated(Core::Domain::OrderStatus status, int pageNumber, int pageSize) {
        std::vector<Core::Domain::Order> orders;
        try {
            auto conn = m_connectionManager->GetConnection();
            
            // Calculate offset (pageNumber is 1-indexed)
            int offset = (pageNumber - 1) * pageSize;
            
            std::unique_ptr<sql::PreparedStatement> stmt(conn->prepareStatement(
                "SELECT * FROM orders WHERE status = ? ORDER BY id DESC LIMIT ? OFFSET ?"
            ));
            stmt->setString(1, StatusToString(status));
            stmt->setInt(2, pageSize);
            stmt->setInt(3, offset);
            std::unique_ptr<sql::ResultSet> res(stmt->executeQuery());

            while (res->next()) {
                 std::string typeStr = res->getString("order_type");
                 Core::Domain::OrderType type = StringToOrderType(typeStr);
                 int id = res->getInt("id");

                 Core::Domain::Order order(id, res->getInt("table_number"));
                 order.Type = type;
                 order.Status = status;
                 order.TotalAmount = res->getDouble("total_amount");
                 order.Provider = StringToProvider(res->getString("parcel_provider"));
                 
                 order.Items = GetItemsForOrder(id, conn.get());
                 
                 orders.push_back(order);
            }
        }
        catch (sql::SQLException& e) {
            std::cerr << "SQL Error in GetByStatusPaginated: " << e.what() << std::endl;
            throw;
        }
        return orders;
    }

    int OrderRepository::GetCountByStatus(Core::Domain::OrderStatus status) {
        try {
            auto conn = m_connectionManager->GetConnection();
            std::unique_ptr<sql::PreparedStatement> stmt(conn->prepareStatement(
                "SELECT COUNT(*) as total FROM orders WHERE status = ?"
            ));
            stmt->setString(1, StatusToString(status));
            std::unique_ptr<sql::ResultSet> res(stmt->executeQuery());

            if (res->next()) {
                return res->getInt("total");
            }
        }
        catch (sql::SQLException& e) {
            std::cerr << "SQL Error in GetCountByStatus: " << e.what() << std::endl;
            throw;
        }
        return 0;
    }

    std::vector<Core::Domain::OrderItem> OrderRepository::GetItemsForOrder(int orderId, sql::Connection* conn) {
        std::vector<Core::Domain::OrderItem> items;
        std::unique_ptr<sql::PreparedStatement> stmt(conn->prepareStatement(
            "SELECT oi.*, m.name as menu_name FROM order_items oi JOIN menu_items m ON oi.menu_item_id = m.id WHERE order_id = ?"
        ));
        stmt->setInt(1, orderId);
        std::unique_ptr<sql::ResultSet> res(stmt->executeQuery());

        while (res->next()) {
            items.emplace_back(
                res->getInt("menu_item_id"),
                res->getString("menu_name"),
                res->getDouble("price"),
                res->getInt("quantity")
            );
        }
        return items;
    }

    // --- Helpers ---
    std::string OrderRepository::OrderTypeToString(Core::Domain::OrderType type) {
        return (type == Core::Domain::OrderType::DineIn) ? "DineIn" : "Parcel";
    }

    Core::Domain::OrderType OrderRepository::StringToOrderType(const std::string& typeStr) {
        return (typeStr == "DineIn") ? Core::Domain::OrderType::DineIn : Core::Domain::OrderType::Parcel;
    }

    std::string OrderRepository::ProviderToString(Core::Domain::ParcelProvider provider) {
        if (provider == Core::Domain::ParcelProvider::Self) return "Self";
        if (provider == Core::Domain::ParcelProvider::FoodPanda) return "FoodPanda";
        return "None";
    }

    Core::Domain::ParcelProvider OrderRepository::StringToProvider(const std::string& provStr) {
        if (provStr == "Self") return Core::Domain::ParcelProvider::Self;
        if (provStr == "FoodPanda") return Core::Domain::ParcelProvider::FoodPanda;
        return Core::Domain::ParcelProvider::None;
    }

    std::string OrderRepository::StatusToString(Core::Domain::OrderStatus status) {
        if (status == Core::Domain::OrderStatus::Processing) return "Processing";
        if (status == Core::Domain::OrderStatus::Done) return "Done";
        return "Ordered";
    }

    Core::Domain::OrderStatus OrderRepository::StringToStatus(const std::string& statusStr) {
        if (statusStr == "Processing") return Core::Domain::OrderStatus::Processing;
        if (statusStr == "Done") return Core::Domain::OrderStatus::Done;
        return Core::Domain::OrderStatus::Ordered;
    }

} // namespace Data
} // namespace POS
