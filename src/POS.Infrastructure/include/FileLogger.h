#pragma once

#include "ILogger.h"
#include <string>
#include <fstream>
#include <mutex>
#include <memory>

namespace POS {
namespace Infrastructure {

    /// <summary>
    /// Thread-safe logger implementation that writes to a file.
    /// Uses RAII for file handle management.
    /// </summary>
    class FileLogger : public ILogger {
    public:
        /// <summary>
        /// Constructor opens the log file in append mode.
        /// </summary>
        /// <param name="filePath">Path to the log file.</param>
        explicit FileLogger(const std::string& filePath);

        /// <summary>
        /// Destructor closes the file.
        /// </summary>
        ~FileLogger() override;

        // Prevent copying and assignment
        FileLogger(const FileLogger&) = delete;
        FileLogger& operator=(const FileLogger&) = delete;

        // ILogger Implementation
        void Log(LogLevel level, const std::string& message) override;
        void Debug(const std::string& message) override;
        void Info(const std::string& message) override;
        void Warn(const std::string& message) override;
        void Error(const std::string& message) override;

    private:
        std::ofstream m_logFile;
        std::mutex m_mutex;
        std::string m_filePath;

        /// <summary>
        /// Get current timestamp in "YYYY-MM-DD HH:MM:SS" format.
        /// </summary>
        std::string GetCurrentTimestamp();

        /// <summary>
        /// Convert LogLevel enum to string representation.
        /// </summary>
        std::string LevelToString(LogLevel level);
    };

} // namespace Infrastructure
} // namespace POS
