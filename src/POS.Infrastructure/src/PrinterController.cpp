#include "PrinterController.h"
#include <iostream>

namespace POS {
namespace Infrastructure {

    PrinterController::PrinterController(std::shared_ptr<ILogger> logger)
        : m_logger(std::move(logger)) {
    }

    void PrinterController::PrintReceipt(const std::string& content) {
        // In a real implementation, this would send commands to a POS printer
        // For now, we log it and print to console
        if (m_logger) {
            m_logger->Info("Printing Receipt:\n" + content);
        }

        std::cout << "===== RECEIPT START =====" << std::endl;
        std::cout << content << std::endl;
        std::cout << "===== RECEIPT END =====" << std::endl;
    }

    void PrinterController::PrintKitchenTicket(const std::string& content) {
        if (m_logger) {
            m_logger->Info("Printing Kitchen Ticket:\n" + content);
        }

        std::cout << "===== KITCHEN TICKET START =====" << std::endl;
        std::cout << content << std::endl;
        std::cout << "===== KITCHEN TICKET END =====" << std::endl;
    }

} // namespace Infrastructure
} // namespace POS
