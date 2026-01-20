#pragma once

#include "IPrinter.h"
#include "ILogger.h"
#include <memory>

namespace POS {
namespace Infrastructure {

    /// <summary>
    /// Controls printing operations.
    /// Currently implements a Mock/Console printer for development.
    /// </summary>
    class PrinterController : public IPrinter {
    public:
        /// <summary>
        /// Constructor with logger dependency.
        /// </summary>
        /// <param name="logger">Logger for recording print jobs.</param>
        explicit PrinterController(std::shared_ptr<ILogger> logger);

        void PrintReceipt(const std::string& content) override;
        void PrintKitchenTicket(const std::string& content) override;

    private:
        std::shared_ptr<ILogger> m_logger;
    };

} // namespace Infrastructure
} // namespace POS
