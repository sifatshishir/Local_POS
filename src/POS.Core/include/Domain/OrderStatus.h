#pragma once

namespace POS {
namespace Core {
namespace Domain {

    /// <summary>
    /// Represents the current lifecycle state of an order.
    /// </summary>
    enum class OrderStatus {
        /// <summary>
        /// Order has been placed/created but not yet processed.
        /// </summary>
        Ordered,

        /// <summary>
        /// Kitchen has started preparing the order.
        /// </summary>
        Processing,

        /// <summary>
        /// Order is ready for serving/delivery.
        /// </summary>
        Done
    };

} // namespace Domain
} // namespace Core
} // namespace POS
