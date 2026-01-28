#include <iostream>
#include <string>
#include <vector>
#include <sstream>
#include <memory>
#include <regex>

#include "WebSocketServer.h"

// Core Includes
#include "Domain/Order.h"
#include "Domain/OrderType.h"
#include "Domain/ParcelProvider.h"
#include "Interfaces/IOrderRepository.h"
#include "Services/OrderService.h"
#include "Services/PricingService.h"

// Data Includes
#include "OrderRepository.h"
#include "ConnectionManager.h"

// Infrastructure Includes
#include "FileLogger.h" // ConsoleLogger not found, using FileLogger or ILogger
// If ConsoleLogger is desired but missing, we might use ILogger or just std::cout for now
// But 'ConsoleLogger.h' was in the error. I'll check src to see if it was just not in include.

using namespace POS::Server;
using namespace POS::Core::Domain;
using namespace POS::Core::Services;
using namespace POS::Data;
using namespace POS::Infrastructure;

// Global Services
std::shared_ptr<OrderService> g_orderService;
std::shared_ptr<WebSocketServer> g_server;

// Helper to parse line: "KEY:VALUE"
std::pair<std::string, std::string> ParseLine(const std::string& line) {
    size_t colonPos = line.find(':');
    if (colonPos == std::string::npos) return { "", "" };
    return { line.substr(0, colonPos), line.substr(colonPos + 1) };
}

// Helper to parse item: "ID,NAME,PRICE,QTY"
OrderItem ParseItem(const std::string& value) {
    std::stringstream ss(value);
    std::string segment;
    std::vector<std::string> parts;
    
    // Split by comma
    while(std::getline(ss, segment, ',')) {
        parts.push_back(segment);
    }
    
    if (parts.size() < 4) return OrderItem(0, "Unknown", 0, 0);
    
    int id = std::stoi(parts[0]);
    std::string name = parts[1];
    double price = std::stod(parts[2]);
    int qty = std::stoi(parts[3]);
    
    return OrderItem(id, name, price, qty);
}

void HandleMessage(SOCKET client, std::string message) {
    // Protocol:
    // CMD:CREATE
    // TYPE:DineIn (or Parcel)
    // ...
    // END
    
    std::cout << "Received Message: " << message << std::endl;

    if (message.find("CMD:CREATE") != std::string::npos) {
        std::cout << "Processing Create Order..." << std::endl;
        
        std::stringstream ss(message);
        std::string line;
        
        OrderType type = OrderType::DineIn;
        int tableNumber = 0;
        ParcelProvider provider = ParcelProvider::None;
        std::vector<OrderItem> items;
        
        while (std::getline(ss, line)) {
            // Remove \r if present
            if (!line.empty() && line.back() == '\r') line.pop_back();
            if (line == "END") break;
            
            auto kv = ParseLine(line);
            if (kv.first == "TYPE") {
                if (kv.second == "Parcel") type = OrderType::Parcel;
            }
            else if (kv.first == "TABLE") {
                if (!kv.second.empty()) tableNumber = std::stoi(kv.second);
            }
            else if (kv.first == "PROVIDER") {
                if (kv.second == "FoodPanda") provider = ParcelProvider::FoodPanda;
                else if (kv.second == "Self") provider = ParcelProvider::Self;
            }
            else if (kv.first == "ITEM") {
                items.push_back(ParseItem(kv.second));
            }
        }
        
        try {
            Order newOrder(0, 0); // Placeholder
            if (type == OrderType::DineIn) {
                 newOrder = g_orderService->CreateDineInOrder(tableNumber, items);
            } else {
                 newOrder = g_orderService->CreateParcelOrder(provider, items);
            }
            
            std::cout << "Order Created: #" << newOrder.Id << std::endl;
            
            // Send Response Private to Client
            std::string response = "ORDER_CREATED:" + std::to_string(newOrder.Id);
            g_server->Send(client, response);

            // Broadcast Refresh
            g_server->Broadcast("EVENT:REFRESH_QUEUE");
            
        } catch (const std::exception& e) {
            std::cerr << "Error creating order: " << e.what() << std::endl;
            g_server->Send(client, "ERROR:" + std::string(e.what()));
        }
    }
    else if (message.find("CMD:UPDATE_STATUS") != std::string::npos) {
        std::cout << "Processing Update Status..." << std::endl;
         std::stringstream ss(message);
        std::string line;
        int orderId = 0;
        OrderStatus status = OrderStatus::Ordered;
        
        while (std::getline(ss, line)) {
            if (!line.empty() && line.back() == '\r') line.pop_back();
            if (line == "END") break;
            
             auto kv = ParseLine(line);
            if (kv.first == "ID") {
                 orderId = std::stoi(kv.second);
            }
            else if (kv.first == "STATUS") {
                if (kv.second == "Processing") status = OrderStatus::Processing;
                else if (kv.second == "Done") status = OrderStatus::Done;
                else if (kv.second == "Ordered") status = OrderStatus::Ordered;
            }
        }
        
        try {
            g_orderService->UpdateOrderStatus(orderId, status);
            std::cout << "Order #" << orderId << " Updated to " << (int)status << std::endl;
             g_server->Broadcast("EVENT:REFRESH_QUEUE");
        } catch (const std::exception& e) {
             std::cerr << "Error updating order: " << e.what() << std::endl;
        }
    }
}

int main() {
    std::cout << "Starting POS.Server..." << std::endl;

    // 1. Initialize DB Connection
    // ConnectionManager loads config internally (e.g. from .env or hardcoded)
    auto connectionManager = std::make_shared<ConnectionManager>();
    
    // 2. Initialize Repositories & Services
    auto logger = std::make_shared<FileLogger>("server.log");
    auto orderRepo = std::make_shared<OrderRepository>(connectionManager);
    auto pricingService = std::make_shared<PricingService>();
    g_orderService = std::make_shared<OrderService>(orderRepo, pricingService);

    // 3. Start WebSocket Server
    g_server = std::make_shared<WebSocketServer>();
    
    g_server->SetOnMessage(HandleMessage);
    
    if (g_server->Start(8080)) {
        std::cout << "WebSocket Server running on port 8080" << std::endl;
        
        // Keep main thread alive
        std::string input;
        while (std::getline(std::cin, input)) {
            if (input == "quit" || input == "exit") break;
            if (input == "refresh") g_server->Broadcast("EVENT:REFRESH_QUEUE");
        }
    } else {
        std::cerr << "Failed to start server" << std::endl;
        return 1;
    }

    return 0;
}
