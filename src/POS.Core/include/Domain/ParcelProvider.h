#pragma once

namespace POS {
namespace Core {
namespace Domain {

    /// <summary>
    /// Specifies the service provider for Parcel orders.
    /// </summary>
    enum class ParcelProvider {
        None,       // Not applicable (e.g., for DineIn)
        Self,       // Customer picks up (Takeaway)
        FoodPanda   // Third-party delivery
    };

} // namespace Domain
} // namespace Core
} // namespace POS
