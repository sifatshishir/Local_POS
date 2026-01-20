#include "Services/MenuService.h"

namespace POS {
namespace Core {
namespace Services {

    MenuService::MenuService(std::shared_ptr<Interfaces::IMenuRepository> repository)
        : m_repository(std::move(repository)) {
    }

    std::vector<Domain::MenuItem> MenuService::GetAllMenuItems() {
        // Business logic could be here (e.g., filtering inactive items)
        // For now, pass through
        return m_repository->GetAll();
    }

    std::shared_ptr<Domain::MenuItem> MenuService::GetMenuItemById(int id) {
        return m_repository->GetById(id);
    }

    std::vector<Domain::MenuItem> MenuService::GetMenuItemsByCategory(const std::string& category) {
        return m_repository->GetByCategory(category);
    }

} // namespace Services
} // namespace Core
} // namespace POS
