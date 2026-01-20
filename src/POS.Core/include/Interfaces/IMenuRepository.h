#pragma once

#include "../Domain/MenuItem.h"
#include <vector>
#include <memory>
#include <string>
#include <optional>

namespace POS {
namespace Core {
namespace Interfaces {

    /// <summary>
    /// Interface for accessing Menu data.
    /// Implemented by Infrastructure layer.
    /// </summary>
    class IMenuRepository {
    public:
        virtual ~IMenuRepository() = default;

        virtual std::vector<Domain::MenuItem> GetAll() = 0;
        virtual std::shared_ptr<Domain::MenuItem> GetById(int id) = 0;
        virtual std::vector<Domain::MenuItem> GetByCategory(const std::string& category) = 0;
    };

} // namespace Interfaces
} // namespace Core
} // namespace POS
