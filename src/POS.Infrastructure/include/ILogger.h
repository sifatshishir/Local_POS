#pragma once

#include <string>

namespace POS {
namespace Infrastructure {

/// <summary>
/// Log severity levels following standard logging conventions.
/// </summary>
enum class LogLevel {
    DEBUG = 0,  // Detailed information for debugging
    INFO = 1,   // General informational messages
    WARN = 2,   // Warning messages for potentially harmful situations
    ERROR = 3   // Error messages for serious problems
};

/// <summary>
/// Abstract logging interface following Dependency Inversion Principle.
/// Allows different logging implementations (File, Console, Database, Mock).
/// 
/// Design Pattern: Strategy Pattern
/// SOLID Principle: Dependency Inversion (depend on abstraction, not concretion)
/// 
/// Usage:
///   ILogger* logger = new FileLogger("app.log");
///   logger->Info("Application started");
/// </summary>
class ILogger {
public:
    virtual ~ILogger() = default;

    /// <summary>
    /// Log a message with specified severity level.
    /// </summary>
    /// <param name="level">Severity level of the log message</param>
    /// <param name="message">Log message content</param>
    virtual void Log(LogLevel level, const std::string& message) = 0;

    /// <summary>
    /// Log a debug message (detailed diagnostic information).
    /// </summary>
    virtual void Debug(const std::string& message) = 0;

    /// <summary>
    /// Log an informational message (general application flow).
    /// </summary>
    virtual void Info(const std::string& message) = 0;

    /// <summary>
    /// Log a warning message (potentially harmful situation).
    /// </summary>
    virtual void Warn(const std::string& message) = 0;

    /// <summary>
    /// Log an error message (serious problem).
    /// </summary>
    virtual void Error(const std::string& message) = 0;
};

} // namespace Infrastructure
} // namespace POS
