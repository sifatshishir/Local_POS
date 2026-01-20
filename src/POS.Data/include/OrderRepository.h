#pragma once

#include "../../POS.Core/include/Interfaces/IOrderRepository.h"
#include "../../POS.Core/include/Domain/Order.h"
#include "ConnectionManager.h"
#include <vector>
#include <memory>

namespace POS {
namespace Data {

    class OrderRepository : public Core::Interfaces::IOrderRepository {
    public:
        explicit OrderRepository(std::shared_ptr<ConnectionManager> connectionManager);

        int Create(const Core::Domain::Order& order) override;
        void UpdateStatus(int id, Core::Domain::OrderStatus status) override;
        std::shared_ptr<Core::Domain::Order> GetById(int id) override;
        std::vector<Core::Domain::Order> GetByStatus(Core::Domain::OrderStatus status) override;
        
        // Pagination support
        std::vector<Core::Domain::Order> GetByStatusPaginated(Core::Domain::OrderStatus status, int pageNumber, int pageSize);
        int GetCountByStatus(Core::Domain::OrderStatus status);

    private:
        std::shared_ptr<ConnectionManager> m_connectionManager;
        
        // Helper to fetch items for an order
        std::vector<Core::Domain::OrderItem> GetItemsForOrder(int orderId, sql::Connection* conn);
        
        // Helper to parse OrderType/Provider
        Core::Domain::OrderType StringToOrderType(const std::string& typeStr);
        Core::Domain::ParcelProvider StringToProvider(const std::string& provStr);
        Core::Domain::OrderStatus StringToStatus(const std::string& statusStr);

        std::string OrderTypeToString(Core::Domain::OrderType type);
        std::string ProviderToString(Core::Domain::ParcelProvider provider);
        std::string StatusToString(Core::Domain::OrderStatus status);
    };

} // namespace Data
} // namespace POS
