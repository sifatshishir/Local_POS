#include "ConfigLoader.h"
#include <fstream>
#include <sstream>
#include <iostream>
#include <algorithm>

namespace POS {
namespace Infrastructure {

    bool ConfigLoader::Load(const std::string& filePath) {
        std::ifstream file(filePath);
        if (!file.is_open()) {
            return false;
        }

        std::string line;
        while (std::getline(file, line)) {
            // Remove comments
            size_t commentPos = line.find('#');
            if (commentPos != std::string::npos) {
                line = line.substr(0, commentPos);
            }

            line = Trim(line);
            if (line.empty()) continue;

            // Parse key=value
            size_t equalPos = line.find('=');
            if (equalPos != std::string::npos) {
                std::string key = Trim(line.substr(0, equalPos));
                std::string value = Trim(line.substr(equalPos + 1));
                
                if (!key.empty()) {
                    m_config[key] = value;
                }
            }
        }

        return true;
    }

    std::string ConfigLoader::GetString(const std::string& key, const std::string& defaultValue) const {
        auto it = m_config.find(key);
        if (it != m_config.end()) {
            return it->second;
        }
        return defaultValue;
    }

    int ConfigLoader::GetInt(const std::string& key, int defaultValue) const {
        auto it = m_config.find(key);
        if (it != m_config.end()) {
            try {
                return std::stoi(it->second);
            } catch (...) {
                return defaultValue;
            }
        }
        return defaultValue;
    }

    std::string ConfigLoader::Trim(const std::string& str) {
        if (str.empty()) return str;

        size_t first = str.find_first_not_of(" \t\r\n");
        if (std::string::npos == first) return str;

        size_t last = str.find_last_not_of(" \t\r\n");
        return str.substr(first, (last - first + 1));
    }

} // namespace Infrastructure
} // namespace POS
