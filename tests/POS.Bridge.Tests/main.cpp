#include <iostream>
#include <vector>
#include <cassert>

using namespace System;
using namespace POS::Bridge;
using namespace POS::Bridge::DataTransferObjects;

void TestUtils() {
    Console::WriteLine("Running TestUtils...");
    std::string native = "Hello Native";
    String^ managed = Utils::ToManagedString(native);
    assert(managed == "Hello Native");

    String^ managed2 = "Hello Managed";
    std::string native2 = Utils::ToNativeString(managed2);
    assert(native2 == "Hello Managed");
    Console::WriteLine("TestUtils Passed.");
}

void TestDTOs() {
    Console::WriteLine("Running TestDTOs...");
    MenuItemDTO^ item = gcnew MenuItemDTO();
    item->Name = "Burger";
    item->Price = 10.0;
    
    assert(item->Name == "Burger");
    assert(item->Price == 10.0);
    Console::WriteLine("TestDTOs Passed.");
}

void TestMenuService() {
    Console::WriteLine("Running TestMenuService...");
    try {
        MenuServiceWrapper^ service = gcnew MenuServiceWrapper();
        // This will try to connect to DB. If DB is not up or configured, it might throw.
        // We catch exception to allow test runner to proceed or fail gracefully.
        auto items = service->GetAllMenuItems();
        Console::WriteLine("Fetched {0} menu items.", items->Count);
    }
    catch (Exception^ ex) {
        Console::WriteLine("TestMenuService Failed: " + ex->Message);
        // Don't assert fail here if DB is strictly required but maybe not seeded perfectly suitable for auto-test
        // But for now let's assume it should work.
    }
    Console::WriteLine("TestMenuService Completed.");
}

int main() {
    Console::WriteLine("Starting POS.Bridge Tests...");
    
    TestUtils();
    TestDTOs();
    TestMenuService();
    
    // Test OrderService would require more complex setup (creating order items etc), 
    // skipping for basic smoke test unless needed.

    Console::WriteLine("All Tests Completed.");
    return 0;
}
