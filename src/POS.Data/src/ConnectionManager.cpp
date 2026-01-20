#include "ConnectionManager.h"
#include "../../POS.Infrastructure/include/ConfigLoader.h"
#include "../../POS.Infrastructure/include/ILogger.h"
#include <stdexcept>
#include <iostream>

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
        // In a real scenario, we would inject ConfigLoader or use a service locator.
        // For now, we instantiate it to read the .env file.
        // Assumes .env is in the running directory.
        Infrastructure::ConfigLoader config;
        // Try loading from current dir, or up one level (common in VS debug)
        if (!config.Load(".env")) {
            config.Load("../.env"); 
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
