#include "FileLogger.h"
#include <iostream>
#include <chrono>
#include <ctime>
#include <iomanip>
#include <sstream>

namespace POS {
namespace Infrastructure {

    FileLogger::FileLogger(const std::string& filePath) 
        : m_filePath(filePath) {
        // Open file in append mode
        m_logFile.open(filePath, std::ios::out | std::ios::app);
        
        if (!m_logFile.is_open()) {
            std::cerr << "Failed to open log file: " << filePath << std::endl;
        }
    }

    FileLogger::~FileLogger() {
        if (m_logFile.is_open()) {
            m_logFile.close();
        }
    }

    void FileLogger::Log(LogLevel level, const std::string& message) {
        std::lock_guard<std::mutex> lock(m_mutex);

        if (m_logFile.is_open()) {
            m_logFile << "[" << GetCurrentTimestamp() << "] "
                      << "[" << LevelToString(level) << "] "
                      << message << std::endl;
        }
    }

    void FileLogger::Debug(const std::string& message) {
        Log(LogLevel::DEBUG_LEVEL, message);
    }

    void FileLogger::Info(const std::string& message) {
        Log(LogLevel::INFO_LEVEL, message);
    }

    void FileLogger::Warn(const std::string& message) {
        Log(LogLevel::WARN_LEVEL, message);
    }

    void FileLogger::Error(const std::string& message) {
        Log(LogLevel::ERROR_LEVEL, message);
    }

    std::string FileLogger::GetCurrentTimestamp() {
        auto now = std::chrono::system_clock::now();
        auto in_time_t = std::chrono::system_clock::to_time_t(now);

        std::stringstream ss;
        
        // Use localtime_s for thread safety on Windows
        struct tm buf;
        localtime_s(&buf, &in_time_t);
        
        ss << std::put_time(&buf, "%Y-%m-%d %H:%M:%S");
        return ss.str();
    }

    std::string FileLogger::LevelToString(LogLevel level) {
        switch (level) {
            case LogLevel::DEBUG_LEVEL: return "DEBUG";
            case LogLevel::INFO_LEVEL:  return "INFO ";
            case LogLevel::WARN_LEVEL:  return "WARN ";
            case LogLevel::ERROR_LEVEL: return "ERROR";
            default:              return "UNKNOWN";
        }
    }

} // namespace Infrastructure
} // namespace POS
