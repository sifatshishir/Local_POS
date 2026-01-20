#include <gtest/gtest.h>
#include "FileLogger.h"
#include <fstream>
#include <string>
#include <filesystem>
#include <thread>

namespace fs = std::filesystem;
using namespace POS::Infrastructure;

class FileLoggerTests : public ::testing::Test {
protected:
    std::string testLogFile = "test_log.txt";

    void SetUp() override {
        // Clean up before test
        if (fs::exists(testLogFile)) {
            fs::remove(testLogFile);
        }
    }

    void TearDown() override {
        // Clean up after test
        if (fs::exists(testLogFile)) {
            fs::remove(testLogFile);
        }
    }

    bool FileContains(const std::string& path, const std::string& content) {
        std::ifstream file(path);
        if (!file.is_open()) return false;
        
        std::string line;
        while (std::getline(file, line)) {
            if (line.find(content) != std::string::npos) {
                return true;
            }
        }
        return false;
    }
};

TEST_F(FileLoggerTests, CreatesLogFileOnInitialization) {
    {
        FileLogger logger(testLogFile);
        logger.Info("Init Test");
    } // Logger destroyed here, file flushed/closed

    EXPECT_TRUE(fs::exists(testLogFile));
}

TEST_F(FileLoggerTests, WritesLogMessageWithCorrectLevel) {
    {
        FileLogger logger(testLogFile);
        logger.Info("Test Info Message");
        logger.Error("Test Error Message");
    }

    EXPECT_TRUE(FileContains(testLogFile, "[INFO ] Test Info Message"));
    EXPECT_TRUE(FileContains(testLogFile, "[ERROR] Test Error Message"));
}

TEST_F(FileLoggerTests, AppendsToExistingFile) {
    {
        FileLogger logger(testLogFile);
        logger.Info("First Message");
    }
    
    {
        FileLogger logger(testLogFile);
        logger.Info("Second Message");
    }

    EXPECT_TRUE(FileContains(testLogFile, "First Message"));
    EXPECT_TRUE(FileContains(testLogFile, "Second Message"));
}
