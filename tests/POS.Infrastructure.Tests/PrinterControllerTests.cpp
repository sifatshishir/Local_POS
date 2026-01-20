#include <gtest/gtest.h>
#include "PrinterController.h"
#include "ILogger.h"
#include <vector>
#include <string>

using namespace POS::Infrastructure;

class MockLogger : public ILogger {
public:
    std::vector<std::string> logs;

    void Log(LogLevel level, const std::string& message) override {
        logs.push_back(message);
    }
    void Debug(const std::string& message) override { Log(LogLevel::DEBUG, message); }
    void Info(const std::string& message) override { Log(LogLevel::INFO, message); }
    void Warn(const std::string& message) override { Log(LogLevel::WARN, message); }
    void Error(const std::string& message) override { Log(LogLevel::ERROR, message); }
};

TEST(PrinterControllerTests, PrintReceipt_LogsAction) {
    auto mockLogger = std::make_shared<MockLogger>();
    PrinterController printer(mockLogger);

    printer.PrintReceipt("Item 1 - $10.00");

    ASSERT_EQ(mockLogger->logs.size(), 1);
    EXPECT_NE(mockLogger->logs[0].find("Printing Receipt"), std::string::npos);
    EXPECT_NE(mockLogger->logs[0].find("Item 1 - $10.00"), std::string::npos);
}

TEST(PrinterControllerTests, PrintKitchenTicket_LogsAction) {
    auto mockLogger = std::make_shared<MockLogger>();
    PrinterController printer(mockLogger);

    printer.PrintKitchenTicket("Table 5 - Burger");

    ASSERT_EQ(mockLogger->logs.size(), 1);
    EXPECT_NE(mockLogger->logs[0].find("Printing Kitchen Ticket"), std::string::npos);
    EXPECT_NE(mockLogger->logs[0].find("Table 5 - Burger"), std::string::npos);
}

TEST(PrinterControllerTests, WorksWithoutLogger) {
    PrinterController printer(nullptr);
    // Should not crash
    printer.PrintReceipt("Test");
    printer.PrintKitchenTicket("Test");
}
