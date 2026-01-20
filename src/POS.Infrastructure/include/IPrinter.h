#pragma once

#include <string>

namespace POS {
namespace Infrastructure {

    /// <summary>
    /// Abstract interface for printing operations.
    /// Allows decoupling business logic from physical printing devices.
    /// </summary>
    class IPrinter {
    public:
        virtual ~IPrinter() = default;

        /// <summary>
        /// Print a receipt for the customer.
        /// </summary>
        /// <param name="content">Formatted receipt content.</param>
        virtual void PrintReceipt(const std::string& content) = 0;

        /// <summary>
        /// Print a ticket for the kitchen.
        /// </summary>
        /// <param name="content">Formatted kitchen ticket content.</param>
        virtual void PrintKitchenTicket(const std::string& content) = 0;
    };

} // namespace Infrastructure
} // namespace POS
