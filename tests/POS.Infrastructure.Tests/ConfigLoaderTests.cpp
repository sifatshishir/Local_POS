#include <gtest/gtest.h>
#include "ConfigLoader.h"
#include <fstream>
#include <filesystem>

using namespace POS::Infrastructure;
namespace fs = std::filesystem;

class ConfigLoaderTests : public ::testing::Test {
protected:
    std::string testEnvFile = "test.env";

    void SetUp() override {
        // Create a test .env file
        std::ofstream file(testEnvFile);
        file << "# This is a comment" << std::endl;
        file << "DB_HOST = localhost" << std::endl;
        file << "DB_PORT=3306" << std::endl;
        file << "  WHITESPACE_KEY  =   value with spaces   " << std::endl;
        file << "EMPTY_LINE=" << std::endl;
        file << std::endl;
        file << "VALID_KEY=valid_value" << std::endl;
        file.close();
    }

    void TearDown() override {
        if (fs::exists(testEnvFile)) {
            fs::remove(testEnvFile);
        }
    }
};

TEST_F(ConfigLoaderTests, Load_ReturnsTrue_ForExistingFile) {
    ConfigLoader config;
    EXPECT_TRUE(config.Load(testEnvFile));
}

TEST_F(ConfigLoaderTests, Load_ReturnsFalse_ForNonExistentFile) {
    ConfigLoader config;
    EXPECT_FALSE(config.Load("non_existent.env"));
}

TEST_F(ConfigLoaderTests, GetString_ReturnsCorrectValue) {
    ConfigLoader config;
    config.Load(testEnvFile);

    EXPECT_EQ(config.GetString("DB_HOST"), "localhost");
    EXPECT_EQ(config.GetString("VALID_KEY"), "valid_value");
}

TEST_F(ConfigLoaderTests, GetString_ReturnsDefault_WhenKeyMissing) {
    ConfigLoader config;
    config.Load(testEnvFile);

    EXPECT_EQ(config.GetString("MISSING_KEY", "default"), "default");
}

TEST_F(ConfigLoaderTests, GetInt_ReturnsCorrectValue) {
    ConfigLoader config;
    config.Load(testEnvFile);

    EXPECT_EQ(config.GetInt("DB_PORT"), 3306);
}

TEST_F(ConfigLoaderTests, GetInt_ReturnsDefault_WhenInvalid) {
    ConfigLoader config;
    config.Load(testEnvFile);

    EXPECT_EQ(config.GetInt("DB_HOST", 9999), 9999); // "localhost" is not an int
}

TEST_F(ConfigLoaderTests, HandlesWhitespaceCorrectly) {
    ConfigLoader config;
    config.Load(testEnvFile);

    EXPECT_EQ(config.GetString("WHITESPACE_KEY"), "value with spaces");
}
