#pragma once
#include "ServiceFactory.h"
#include "Utils.h"

using namespace System;

namespace POS {
namespace Bridge {

    public ref class PrintServiceWrapper {
    public:
        PrintServiceWrapper() {
            m_nativePrinter = ServiceFactory::Instance->GetPrinter();
        }

        void PrintReceipt(String^ content) {
            std::string nativeContent = Utils::ToNativeString(content);
            m_nativePrinter->PrintReceipt(nativeContent);
        }

        void PrintKitchenTicket(String^ content) {
            std::string nativeContent = Utils::ToNativeString(content);
            m_nativePrinter->PrintKitchenTicket(nativeContent);
        }

    private:
        std::shared_ptr<POS::Infrastructure::Interfaces::IPrinter> m_nativePrinter;
    };

} // namespace Bridge
} // namespace POS
