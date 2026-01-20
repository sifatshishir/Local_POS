#include "Services/OrderService.h"

namespace POS {
namespace Core {
namespace Services {

    OrderService::OrderService(std::shared_ptr<Interfaces::IOrderRepository> repository, 
                               std::shared_ptr<PricingService> pricingService)
        : m_repository(std::move(repository)), m_pricingService(std::move(pricingService)) {
    }

    Domain::Order OrderService::CreateDineInOrder(int tableNumber, const std::vector<Domain::OrderItem>& items) {
        Domain::Order order(0, tableNumber);
        order.Items = items;
        
        // Calculate Total
        if (m_pricingService) {
            order.TotalAmount = m_pricingService->CalculateTotal(order.Items);
        } else {
            order.CalculateTotal(); // Fallback to basic sum if no service? (Shouldn't happen)
        }

        // Save to Repo
        int newId = m_repository->Create(order);
        order.Id = newId;
        
        return order;
    }

    Domain::Order OrderService::CreateParcelOrder(Domain::ParcelProvider provider, const std::vector<Domain::OrderItem>& items) {
        Domain::Order order(0, provider);
        order.Items = items;

        if (m_pricingService) {
            order.TotalAmount = m_pricingService->CalculateTotal(order.Items);
        }

        int newId = m_repository->Create(order);
        order.Id = newId;

        return order;
    }

    void OrderService::UpdateOrderStatus(int orderId, Domain::OrderStatus newStatus) {
        m_repository->UpdateStatus(orderId, newStatus);
    }

    std::shared_ptr<Domain::Order> OrderService::GetOrderById(int id) {
        return m_repository->GetById(id);
    }

    std::vector<Domain::Order> OrderService::GetOrdersByStatus(Domain::OrderStatus status) {
        return m_repository->GetByStatus(status);
    }

} // namespace Services
} // namespace Core
} // namespace POS
