#pragma once

#include <string>
#include <map>
#include <optional>

namespace POS {
namespace Infrastructure {

    /// <summary>
    /// Loads configuration from .env files.
    /// Supports basic key=value parsing.
    /// </summary>
    class ConfigLoader {
    public:
        /// <summary>
        /// Load configuration from specified file path.
        /// </summary>
        /// <param name="filePath">Path to .env file.</param>
        /// <returns>True if loaded successfully, false otherwise.</returns>
        bool Load(const std::string& filePath);

        /// <summary>
        /// Get string value for key.
        /// </summary>
        /// <param name="key">Configuration key.</param>
        /// <param name="defaultValue">Value to return if key not found.</param>
        std::string GetString(const std::string& key, const std::string& defaultValue = "") const;

        /// <summary>
        /// Get integer value for key.
        /// </summary>
        /// <param name="key">Configuration key.</param>
        /// <param name="defaultValue">Value to return if key not found or invalid.</param>
        int GetInt(const std::string& key, int defaultValue = 0) const;

    private:
        std::map<std::string, std::string> m_config;

        /// <summary>
        /// Trim whitespace from string.
        /// </summary>
        std::string Trim(const std::string& str);
    };

} // namespace Infrastructure
} // namespace POS
