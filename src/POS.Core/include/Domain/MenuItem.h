#pragma once

#include <string>

namespace POS {
namespace Core {
namespace Domain {

    /// <summary>
    /// Represents an item on the restaurant menu.
    /// </summary>
    class MenuItem {
    public:
        int Id;
        std::string Name;
        double Price;
        std::string Category;
        std::string Description;
        bool IsActive;

        MenuItem(int id, std::string name, double price, std::string category, std::string description = "", bool isActive = true)
            : Id(id), Name(std::move(name)), Price(price), Category(std::move(category)), Description(std::move(description)), IsActive(isActive) {
        }
    };

} // namespace Domain
} // namespace Core
} // namespace POS
