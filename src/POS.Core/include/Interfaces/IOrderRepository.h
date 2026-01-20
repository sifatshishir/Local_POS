#pragma once

#include "../Domain/Order.h"
#include <vector>
#include <memory>

namespace POS {
namespace Core {
namespace Interfaces {

    class IOrderRepository {
    public:
        virtual ~IOrderRepository() = default;

        /// <summary>
        /// Creates a new order.
        /// </summary>
        /// <returns>The ID of the newly created order.</returns>
        virtual int Create(const Domain::Order& order) = 0;

        virtual void UpdateStatus(int id, Domain::OrderStatus status) = 0;
        virtual std::shared_ptr<Domain::Order> GetById(int id) = 0;
        virtual std::vector<Domain::Order> GetByStatus(Domain::OrderStatus status) = 0;
    };

} // namespace Interfaces
} // namespace Core
} // namespace POS
