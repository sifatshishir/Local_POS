#pragma once

#include <mysql_connection.h>
#include <cppconn/driver.h>
#include <cppconn/exception.h>
#include <cppconn/resultset.h>
#include <cppconn/statement.h>
#include <cppconn/prepared_statement.h>
#include <memory>
#include <string>

namespace POS {
namespace Data {

    /// <summary>
    /// Manages the MySQL database connection.
    /// Uses RAII to ensure connections are properly managed.
    /// </summary>
    class ConnectionManager {
    public:
        ConnectionManager();
        ~ConnectionManager();

        /// <summary>
        /// Establishes and returns a connection to the database.
        /// Throws sql::SQLException on failure.
        /// </summary>
        std::shared_ptr<sql::Connection> GetConnection();

    private:
        sql::Driver* m_driver;
        std::string m_host;
        std::string m_user;
        std::string m_pass;
        std::string m_schema;
        int m_port;

        void LoadConfig();
    };

} // namespace Data
} // namespace POS
