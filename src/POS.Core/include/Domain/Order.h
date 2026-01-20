#pragma once

#include "OrderType.h"
#include "OrderStatus.h"
#include "ParcelProvider.h"
#include "OrderItem.h"
#include <vector>
#include <ctime>
#include <algorithm>

namespace POS {
namespace Core {
namespace Domain {

    class Order {
    public:
        int Id;
        OrderType Type;
        ParcelProvider Provider; // Applicable if Type == Parcel
        int TableNumber;         // Applicable if Type == DineIn
        OrderStatus Status;
        std::vector<OrderItem> Items;
        double TotalAmount;
        std::time_t CreatedAt;

        // Constructor for DineIn
        Order(int id, int tableNumber)
            : Id(id), Type(OrderType::DineIn), Provider(ParcelProvider::None), TableNumber(tableNumber), 
              Status(OrderStatus::Ordered), TotalAmount(0.0) {
            CreatedAt = std::time(nullptr);
        }

        // Constructor for Parcel
        Order(int id, ParcelProvider provider)
            : Id(id), Type(OrderType::Parcel), Provider(provider), TableNumber(0), 
              Status(OrderStatus::Ordered), TotalAmount(0.0) {
            CreatedAt = std::time(nullptr);
        }

        void AddItem(const OrderItem& item) {
            Items.push_back(item);
            CalculateTotal();
        }

        void RemoveItem(int index) {
            if (index >= 0 && index < static_cast<int>(Items.size())) {
                Items.erase(Items.begin() + index);
                CalculateTotal();
            }
        }

        void CalculateTotal() {
            TotalAmount = 0.0;
            for (const auto& item : Items) {
                TotalAmount += item.GetSubtotal();
            }
        }
    };

} // namespace Domain
} // namespace Core
} // namespace POS
