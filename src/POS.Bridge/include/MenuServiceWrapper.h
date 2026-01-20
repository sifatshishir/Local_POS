#pragma once
#include "DTOs.h"
#include "ServiceFactory.h"
#include "Utils.h"

using namespace System;
using namespace System::Collections::Generic;

namespace POS {
namespace Bridge {

    public ref class MenuServiceWrapper {
    public:
        MenuServiceWrapper() {
            // Allocate native shared_ptr on heap to avoid mixed type error
            m_nativeService = new std::shared_ptr<POS::Core::Services::MenuService>(
                ServiceFactory::Instance->GetMenuService()
            );
        }

        ~MenuServiceWrapper() {
            delete m_nativeService;
        }

        !MenuServiceWrapper() {
            delete m_nativeService;
        }

        List<DataTransferObjects::MenuItemDTO^>^ GetAllMenuItems() {
            auto nativeItems = (*m_nativeService)->GetAllMenuItems();
            auto list = gcnew List<DataTransferObjects::MenuItemDTO^>();

            for (const auto& item : nativeItems) {
                auto dto = gcnew DataTransferObjects::MenuItemDTO();
                dto->Id = item.Id;
                dto->Name = Utils::ToManagedString(item.Name);
                dto->Price = item.Price;
                dto->Category = Utils::ToManagedString(item.Category);
                dto->Description = Utils::ToManagedString(item.Description);
                dto->IsActive = item.IsActive;
                list->Add(dto);
            }
            return list;
        }

        List<DataTransferObjects::MenuItemDTO^>^ GetMenuItemsByCategory(String^ category) {
            std::string nativeCategory = Utils::ToNativeString(category);
            auto nativeItems = (*m_nativeService)->GetMenuItemsByCategory(nativeCategory);
            auto list = gcnew List<DataTransferObjects::MenuItemDTO^>();

            for (const auto& item : nativeItems) {
                auto dto = gcnew DataTransferObjects::MenuItemDTO();
                dto->Id = item.Id;
                dto->Name = Utils::ToManagedString(item.Name);
                dto->Price = item.Price;
                dto->Category = Utils::ToManagedString(item.Category);
                dto->Description = Utils::ToManagedString(item.Description);
                dto->IsActive = item.IsActive;
                list->Add(dto);
            }
            return list;
        }

    private:
        std::shared_ptr<POS::Core::Services::MenuService>* m_nativeService;
    };

} // namespace Bridge
} // namespace POS
