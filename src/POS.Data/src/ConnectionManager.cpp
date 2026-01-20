#include "ConnectionManager.h"
#include "../../POS.Infrastructure/include/ConfigLoader.h"
#include "../../POS.Infrastructure/include/ILogger.h"
#include <stdexcept>
#include <iostream>
#include <vector>

namespace POS {
namespace Data {

    ConnectionManager::ConnectionManager() : m_driver(nullptr), m_port(3306) {
        try {
            m_driver = get_driver_instance();
        }
        catch (sql::SQLException& e) {
            std::cerr << "MySQL Driver Error: " << e.what() << std::endl;
            throw std::runtime_error("Failed to acquire MySQL driver instance.");
        }
        LoadConfig();
    }

    ConnectionManager::~ConnectionManager() {
        // Driver is managed by the connector library, we don't delete it.
    }

    void ConnectionManager::LoadConfig() {
        // Search for .env file starting from project root
        // Executable is in: i:\Projects\VS\POS\src\POS.UI\bin\Release\net8.0-windows\
        // Project root is: i:\Projects\VS\POS\
        // That's 5 levels up: ..\..\..\..\..\.env
        
        Infrastructure::ConfigLoader config;
        bool loaded = false;
        
        // Try multiple paths to find .env
        std::vector<std::string> paths = {
            ".env",                           // Current directory (for when copied to output)
            "../.env",                        // One level up
            "../../.env",                     // Two levels up
            "../../../.env",                  // Three levels up
            "../../../../.env",               // Four levels up
            "../../../../../.env"             // Five levels up (project root)
        };
        
        for (const auto& path : paths) {
            if (config.Load(path)) {
                loaded = true;
                std::cout << "Loaded .env from: " << path << std::endl;
                break;
            }
        }
        
        if (!loaded) {
            std::cerr << "WARNING: Could not load .env file from any location. Using defaults." << std::endl;
        }

        m_host = config.GetString("DB_HOST", "tcp://127.0.0.1");
        m_user = config.GetString("DB_USER", "root");
        m_pass = config.GetString("DB_PASS", "password"); // Default fallback
        m_schema = config.GetString("DB_NAME", "pos_db");
        m_port = config.GetInt("DB_PORT", 3306);

        // Adjust host string if it doesn't contain protocol
        if (m_host.find("tcp://") == std::string::npos) {
            m_host = "tcp://" + m_host;
        }
        
        // Append port to host if not present
        if (m_host.find(":") == std::string::npos || m_host.find(":", 6) == std::string::npos) {
             m_host += ":" + std::to_string(m_port);
        }
    }

    std::shared_ptr<sql::Connection> ConnectionManager::GetConnection() {
        if (!m_driver) {
            throw std::runtime_error("MySQL driver not initialized.");
        }

        try {
            std::shared_ptr<sql::Connection> conn(m_driver->connect(m_host, m_user, m_pass));
            conn->setSchema(m_schema);
            return conn;
        }
        catch (sql::SQLException& e) {
            std::cerr << "Connection Error: " << e.what() << " (Code: " << e.getErrorCode() << ")" << std::endl;
            throw;
        }
    }

} // namespace Data
} // namespace POS
