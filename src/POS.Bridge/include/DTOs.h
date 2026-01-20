#pragma once

using namespace System;
using namespace System::Collections::Generic;

namespace POS {
namespace Bridge {
namespace DataTransferObjects {

    public ref class MenuItemDTO {
    public:
        int Id;
        String^ Name;
        double Price;
        String^ Category;
        String^ Description;
        bool IsActive;
    };

    public ref class OrderItemDTO {
    public:
        int MenuItemId;
        String^ MenuName;
        double Price;
        int Quantity;
        
        property double Subtotal {
            double get() { return Price * Quantity; }
        }
    };

    public enum class OrderStatusDTO {
        Ordered,
        Processing,
        Done
    };

    public enum class OrderTypeDTO {
        DineIn,
        Parcel
    };

    public enum class ParcelProviderDTO {
        None,
        Self,
        FoodPanda
    };

    public ref class OrderDTO {
    public:
        int Id;
        OrderTypeDTO Type;
        ParcelProviderDTO Provider;
        int TableNumber;
        OrderStatusDTO Status;
        double TotalAmount;
        List<OrderItemDTO^>^ Items;
    
        OrderDTO() {
            Items = gcnew List<OrderItemDTO^>();
        }
    };

} // namespace DataTransferObjects
} // namespace Bridge
} // namespace POS
