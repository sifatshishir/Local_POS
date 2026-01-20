#pragma once
#include <memory>
#include "pch.h"
#include "Services/MenuService.h"
#include "Services/OrderService.h"
#include "Services/PricingService.h"
#include "ConnectionManager.h"
#include "MenuRepository.h"
#include "OrderRepository.h"
#include "PrinterController.h"
#include "FileLogger.h"

namespace POS {
namespace Bridge {

    // Helper class to manage the lifetime of native services
    // Acts as the Composition Root for native graph
    public ref class ServiceFactory {
    public:
        static ServiceFactory^ Instance = gcnew ServiceFactory();

        ServiceFactory() {
            // 0. Infrastructure
            m_logger = new std::shared_ptr<POS::Infrastructure::ILogger>(
                std::make_shared<POS::Infrastructure::FileLogger>("pos_app.log"));
            
            m_printer = new std::shared_ptr<POS::Infrastructure::IPrinter>(
                std::make_shared<POS::Infrastructure::PrinterController>(*m_logger));

            // 1. Connection Manager
            m_connManager = new std::shared_ptr<POS::Data::ConnectionManager>(
                std::make_shared<POS::Data::ConnectionManager>());

            // 2. Repositories
            m_menuRepo = new std::shared_ptr<POS::Core::Interfaces::IMenuRepository>(
                std::make_shared<POS::Data::MenuRepository>(*m_connManager));
            
            m_orderRepo = new std::shared_ptr<POS::Core::Interfaces::IOrderRepository>(
                std::make_shared<POS::Data::OrderRepository>(*m_connManager));

            // 3. Domain Services
            m_pricingService = new std::shared_ptr<POS::Core::Services::PricingService>(
                std::make_shared<POS::Core::Services::PricingService>());

            // 4. Application Services
            m_menuService = new std::shared_ptr<POS::Core::Services::MenuService>(
                std::make_shared<POS::Core::Services::MenuService>(*m_menuRepo));
            
            m_orderService = new std::shared_ptr<POS::Core::Services::OrderService>(
                std::make_shared<POS::Core::Services::OrderService>(*m_orderRepo, *m_pricingService));
        }

        ~ServiceFactory() {
            delete m_menuService;
            delete m_orderService;
            delete m_pricingService;
            delete m_orderRepo;
            delete m_menuRepo;
            delete m_connManager;
            delete m_printer;
            delete m_logger;
        }

        // Methods to retrieve native pointers types (return by value is fine for shared_ptr)
        std::shared_ptr<POS::Core::Services::MenuService> GetMenuService() { return *m_menuService; }
        std::shared_ptr<POS::Core::Services::OrderService> GetOrderService() { return *m_orderService; }
        std::shared_ptr<POS::Infrastructure::IPrinter> GetPrinter() { return *m_printer; }

    private:
        // Hold pointers to shared_ptrs to avoid "mixed type" error in managed class
        std::shared_ptr<POS::Infrastructure::ILogger>* m_logger;
        std::shared_ptr<POS::Infrastructure::IPrinter>* m_printer;
        std::shared_ptr<POS::Data::ConnectionManager>* m_connManager;
        std::shared_ptr<POS::Core::Interfaces::IMenuRepository>* m_menuRepo;
        std::shared_ptr<POS::Core::Interfaces::IOrderRepository>* m_orderRepo;
        std::shared_ptr<POS::Core::Services::PricingService>* m_pricingService;
        std::shared_ptr<POS::Core::Services::MenuService>* m_menuService;
        std::shared_ptr<POS::Core::Services::OrderService>* m_orderService;
    };

} // namespace Bridge
} // namespace POS
