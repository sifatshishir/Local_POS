#pragma once
#include "DTOs.h"
#include "ServiceFactory.h"
#include "Utils.h"
#include "Domain/OrderStatus.h"

using namespace System;
using namespace System::Collections::Generic;
using namespace POS::Bridge::DataTransferObjects;

namespace POS {
namespace Bridge {

    public ref class OrderServiceWrapper {
    public:
        OrderServiceWrapper() {
            m_nativeService = ServiceFactory::Instance->GetOrderService();
        }

        int CreateOrder(OrderDTO^ orderDto) {
            // Map DTO to Native Enums
            POS::Core::Domain::OrderType nativeType = 
                (orderDto->Type == OrderTypeDTO::Parcel) ? POS::Core::Domain::OrderType::Parcel : POS::Core::Domain::OrderType::DineIn;
            
            POS::Core::Domain::ParcelProvider nativeProvider = POS::Core::Domain::ParcelProvider::None;
            if (orderDto->Provider == ParcelProviderDTO::Self) nativeProvider = POS::Core::Domain::ParcelProvider::Self;
            if (orderDto->Provider == ParcelProviderDTO::FoodPanda) nativeProvider = POS::Core::Domain::ParcelProvider::FoodPanda;

            // Map Items
            std::vector<POS::Core::Domain::OrderItem> nativeItems;
            for each (OrderItemDTO^ item in orderDto->Items) {
                nativeItems.emplace_back(
                    item->MenuItemId,
                    Utils::ToNativeString(item->MenuName),
                    item->Price,
                    item->Quantity
                );
            }

            if (nativeType == POS::Core::Domain::OrderType::DineIn) {
                 return m_nativeService->CreateDineInOrder(orderDto->TableNumber, nativeItems);
            } else {
                 return m_nativeService->CreateParcelOrder(nativeProvider, nativeItems);
            }
        }

        void UpdateOrderStatus(int orderId, OrderStatusDTO status) {
            POS::Core::Domain::OrderStatus nativeEnum = POS::Core::Domain::OrderStatus::Ordered;
            if (status == OrderStatusDTO::Processing) nativeEnum = POS::Core::Domain::OrderStatus::Processing;
            if (status == OrderStatusDTO::Done) nativeEnum = POS::Core::Domain::OrderStatus::Done;
            
            m_nativeService->UpdateOrderStatus(orderId, nativeEnum);
        }

        List<OrderDTO^>^ GetOrdersByStatus(OrderStatusDTO status) {
            POS::Core::Domain::OrderStatus nativeEnum = POS::Core::Domain::OrderStatus::Ordered;
            if (status == OrderStatusDTO::Processing) nativeEnum = POS::Core::Domain::OrderStatus::Processing;
            if (status == OrderStatusDTO::Done) nativeEnum = POS::Core::Domain::OrderStatus::Done;

            auto nativeOrders = m_nativeService->GetOrdersByStatus(nativeEnum);
            auto list = gcnew List<OrderDTO^>();

            for (const auto& order : nativeOrders) {
                OrderDTO^ dto = gcnew OrderDTO();
                dto->Id = order.Id;
                dto->TableNumber = order.TableNumber;
                dto->Status = status; // We requested this status
                dto->TotalAmount = order.TotalAmount;
                
                dto->Type = (order.Type == POS::Core::Domain::OrderType::Parcel) ? OrderTypeDTO::Parcel : OrderTypeDTO::DineIn;
                
                if (order.Provider == POS::Core::Domain::ParcelProvider::Self) dto->Provider = ParcelProviderDTO::Self;
                else if (order.Provider == POS::Core::Domain::ParcelProvider::FoodPanda) dto->Provider = ParcelProviderDTO::FoodPanda;
                else dto->Provider = ParcelProviderDTO::None;

                for (const auto& item : order.Items) {
                    OrderItemDTO^ itemDto = gcnew OrderItemDTO();
                    itemDto->MenuItemId = item.MenuItemId;
                    itemDto->MenuName = Utils::ToManagedString(item.MenuItemName);
                    itemDto->Quantity = item.Quantity;
                    itemDto->Price = item.Price;
                    dto->Items->Add(itemDto);
                }
                list->Add(dto);
            }
            return list;
        }

    private:
        std::shared_ptr<POS::Core::Services::OrderService> m_nativeService;
    };

} // namespace Bridge
} // namespace POS
