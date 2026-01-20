#include "MenuRepository.h"
#include <iostream>

namespace POS {
namespace Data {

    MenuRepository::MenuRepository(std::shared_ptr<ConnectionManager> connectionManager)
        : m_connectionManager(std::move(connectionManager)) {
    }

    Core::Domain::MenuItem MenuRepository::MapRowToMenuItem(sql::ResultSet* res) {
        return Core::Domain::MenuItem(
            res->getInt("id"),
            res->getString("name"),
            res->getDouble("price"),
            res->getString("category"),
            res->getString("description"),
            res->getBoolean("is_active")
        );
    }

    std::vector<Core::Domain::MenuItem> MenuRepository::GetAll() {
        std::vector<Core::Domain::MenuItem> items;
        try {
            auto conn = m_connectionManager->GetConnection();
            std::unique_ptr<sql::PreparedStatement> stmt(conn->prepareStatement("SELECT * FROM menu_items WHERE is_active = 1"));
            std::unique_ptr<sql::ResultSet> res(stmt->executeQuery());

            while (res->next()) {
                items.push_back(MapRowToMenuItem(res.get()));
            }
        }
        catch (sql::SQLException& e) {
            std::cerr << "SQL Error in GetAll: " << e.what() << std::endl;
            // Depending on reliability requirements, we might throw or return partial
            throw; 
        }
        return items;
    }

    std::shared_ptr<Core::Domain::MenuItem> MenuRepository::GetById(int id) {
        try {
            auto conn = m_connectionManager->GetConnection();
            std::unique_ptr<sql::PreparedStatement> stmt(conn->prepareStatement("SELECT * FROM menu_items WHERE id = ?"));
            stmt->setInt(1, id);
            std::unique_ptr<sql::ResultSet> res(stmt->executeQuery());

            if (res->next()) {
                return std::make_shared<Core::Domain::MenuItem>(MapRowToMenuItem(res.get()));
            }
        }
        catch (sql::SQLException& e) {
            std::cerr << "SQL Error in GetById: " << e.what() << std::endl;
            throw;
        }
        return nullptr;
    }

    std::vector<Core::Domain::MenuItem> MenuRepository::GetByCategory(const std::string& category) {
        std::vector<Core::Domain::MenuItem> items;
        try {
            auto conn = m_connectionManager->GetConnection();
            std::unique_ptr<sql::PreparedStatement> stmt(conn->prepareStatement("SELECT * FROM menu_items WHERE category = ? AND is_active = 1"));
            stmt->setString(1, category);
            std::unique_ptr<sql::ResultSet> res(stmt->executeQuery());

            while (res->next()) {
                items.push_back(MapRowToMenuItem(res.get()));
            }
        }
        catch (sql::SQLException& e) {
            std::cerr << "SQL Error in GetByCategory: " << e.what() << std::endl;
            throw;
        }
        return items;
    }

} // namespace Data
} // namespace POS
