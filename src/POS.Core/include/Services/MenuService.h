#pragma once

#include "../Interfaces/IMenuRepository.h"
#include <memory>
#include <vector>

namespace POS {
namespace Core {
namespace Services {

    /// <summary>
    /// Manages Menu Item logical operations.
    /// </summary>
    class MenuService {
    public:
        explicit MenuService(std::shared_ptr<Interfaces::IMenuRepository> repository);

        std::vector<Domain::MenuItem> GetAllMenuItems();
        std::shared_ptr<Domain::MenuItem> GetMenuItemById(int id);
        std::vector<Domain::MenuItem> GetMenuItemsByCategory(const std::string& category);

    private:
        std::shared_ptr<Interfaces::IMenuRepository> m_repository;
    };

} // namespace Services
} // namespace Core
} // namespace POS
