#pragma once

#include "../../POS.Core/include/Interfaces/IMenuRepository.h"
#include "../../POS.Core/include/Domain/MenuItem.h"
#include "ConnectionManager.h"
#include <vector>
#include <memory>

namespace POS {
namespace Data {

    class MenuRepository : public Core::Interfaces::IMenuRepository {
    public:
        explicit MenuRepository(std::shared_ptr<ConnectionManager> connectionManager);

        std::vector<Core::Domain::MenuItem> GetAll() override;
        std::shared_ptr<Core::Domain::MenuItem> GetById(int id) override;
        std::vector<Core::Domain::MenuItem> GetByCategory(const std::string& category) override;

    private:
        std::shared_ptr<ConnectionManager> m_connectionManager;
        
        // Helper to map result set row to MenuItem
        Core::Domain::MenuItem MapRowToMenuItem(sql::ResultSet* res);
    };

} // namespace Data
} // namespace POS
