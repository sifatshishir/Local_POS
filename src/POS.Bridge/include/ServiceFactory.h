#pragma once
#include <memory>
#include "pch.h"
#include "Services/MenuService.h"
#include "Services/OrderService.h"
#include "Services/PricingService.h"
#include "ConnectionManager.h"
#include "MenuRepository.h"
#include "OrderRepository.h"
#include "../../POS.Infrastructure/include/PrinterController.h"
#include "../../POS.Infrastructure/include/FileLogger.h"

// Forward declare native types to minimize includes in header if possible, 
// but for C++/CLI using native headers directly is often necessary.

namespace POS {
namespace Bridge {

    // Helper class to manage the lifetime of native services
    // Acts as the Composition Root for native graph
    public ref class ServiceFactory {
    public:
        static ServiceFactory^ Instance = gcnew ServiceFactory();

        ServiceFactory() {
            // 0. Infrastructure
            m_logger = std::make_shared<POS::Infrastructure::FileLogger>("pos_app.log");
            m_printer = std::make_shared<POS::Infrastructure::PrinterController>(m_logger);

            // 1. Connection Manager
            m_connManager = std::make_shared<POS::Data::ConnectionManager>();

            // 2. Repositories
            m_menuRepo = std::make_shared<POS::Data::MenuRepository>(m_connManager);
            m_orderRepo = std::make_shared<POS::Data::OrderRepository>(m_connManager);

            // 3. Domain Services
            m_pricingService = std::make_shared<POS::Core::Services::PricingService>();

            // 4. Application Services
            m_menuService = std::make_shared<POS::Core::Services::MenuService>(m_menuRepo);
            m_orderService = std::make_shared<POS::Core::Services::OrderService>(m_orderRepo, m_pricingService);
        }

        // Methods to retrieve native pointers (internal usage for Wrappers)
        std::shared_ptr<POS::Core::Services::MenuService> GetMenuService() { return m_menuService; }
        std::shared_ptr<POS::Core::Services::OrderService> GetOrderService() { return m_orderService; }
        std::shared_ptr<POS::Infrastructure::Interfaces::IPrinter> GetPrinter() { return m_printer; }

    private:
        std::shared_ptr<POS::Infrastructure::Interfaces::ILogger> m_logger;
        std::shared_ptr<POS::Infrastructure::Interfaces::IPrinter> m_printer;
        std::shared_ptr<POS::Data::ConnectionManager> m_connManager;
        std::shared_ptr<POS::Core::Interfaces::IMenuRepository> m_menuRepo;
        std::shared_ptr<POS::Core::Interfaces::IOrderRepository> m_orderRepo;
        std::shared_ptr<POS::Core::Services::PricingService> m_pricingService;
        std::shared_ptr<POS::Core::Services::MenuService> m_menuService;
        std::shared_ptr<POS::Core::Services::OrderService> m_orderService;
    };

} // namespace Bridge
} // namespace POS
