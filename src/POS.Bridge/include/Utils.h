#pragma once
#include <string>
#include <vector>
#include <msclr/marshal_cppstd.h>

using namespace System;
using namespace System::Collections::Generic;

namespace POS {
namespace Bridge {

    public ref class Utils {
    public:
        // Convert System::String^ to std::string
        static std::string ToNativeString(String^ str) {
            return msclr::interop::marshal_as<std::string>(str);
        }

        // Convert std::string to System::String^
        static String^ ToManagedString(const std::string& str) {
            return msclr::interop::marshal_as<String^>(str);
        }
    };

} // namespace Bridge
} // namespace POS
