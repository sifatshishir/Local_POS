#pragma once

#include "../Interfaces/IOrderRepository.h"
#include "PricingService.h"
#include <memory>
#include <vector>

namespace POS {
namespace Core {
    namespace Services {

        class OrderService {
        public:
            OrderService(std::shared_ptr<Interfaces::IOrderRepository> repository,
                std::shared_ptr<PricingService> pricingService);

            /// <summary>
            /// Creates a new Dine-in order.
            /// </summary>
            Domain::Order CreateDineInOrder(int tableNumber, const std::vector<Domain::OrderItem>& items);

            /// <summary>
            /// Creates a new Parcel order.
            /// </summary>
            Domain::Order CreateParcelOrder(Domain::ParcelProvider provider, const std::vector<Domain::OrderItem>& items);

            void UpdateOrderStatus(int orderId, Domain::OrderStatus newStatus);
            std::shared_ptr<Domain::Order> GetOrderById(int id);
            std::vector<Domain::Order> GetOrdersByStatus(Domain::OrderStatus status);

            // Pagination support
            std::vector<Domain::Order> GetOrdersByStatusPaginated(Domain::OrderStatus status, int pageNumber, int pageSize);
            int GetOrderCountByStatus(Domain::OrderStatus status);

        private:
            std::shared_ptr<Interfaces::IOrderRepository> m_repository;
            std::shared_ptr<PricingService> m_pricingService;
        };
    }
} // namespace Services
} // namespace Core
 // namespace POS
