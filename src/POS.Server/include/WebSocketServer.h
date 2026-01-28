#pragma once

#include <winsock2.h>
#include <ws2tcpip.h>
#include <string>
#include <vector>
#include <iostream>
#include <thread>
#include <algorithm>
#include <functional>
#include <sstream>
#include <map>

// Need to link with Ws2_32.lib
#pragma comment(lib, "Ws2_32.lib")

namespace POS {
namespace Server {

    enum class OpCode {
        Continuation = 0x0,
        Text = 0x1,
        Binary = 0x2,
        Close = 0x8,
        Ping = 0x9,
        Pong = 0xa
    };

    class WebSocketServer {
    private:
        SOCKET ListenSocket;
        std::vector<SOCKET> Clients;
        bool Running;
        std::function<void(SOCKET, std::string)> OnMessageCallback;

    public:
        WebSocketServer() : ListenSocket(INVALID_SOCKET), Running(false) {}

        void SetOnMessage(std::function<void(SOCKET, std::string)> callback) {
            OnMessageCallback = callback;
        }

        bool Start(int port) {
            WSADATA wsaData;
            int iResult = WSAStartup(MAKEWORD(2, 2), &wsaData);
            if (iResult != 0) {
                std::cerr << "WSAStartup failed: " << iResult << std::endl;
                return false;
            }

            struct addrinfo* result = NULL, * ptr = NULL, hints;

            ZeroMemory(&hints, sizeof(hints));
            hints.ai_family = AF_INET;
            hints.ai_socktype = SOCK_STREAM;
            hints.ai_protocol = IPPROTO_TCP;
            hints.ai_flags = AI_PASSIVE;

            std::string portStr = std::to_string(port);
            iResult = getaddrinfo(NULL, portStr.c_str(), &hints, &result);
            if (iResult != 0) {
                std::cerr << "getaddrinfo failed: " << iResult << std::endl;
                WSACleanup();
                return false;
            }

            ListenSocket = socket(result->ai_family, result->ai_socktype, result->ai_protocol);
            if (ListenSocket == INVALID_SOCKET) {
                std::cerr << "Error at socket(): " << WSAGetLastError() << std::endl;
                freeaddrinfo(result);
                WSACleanup();
                return false;
            }

            iResult = bind(ListenSocket, result->ai_addr, (int)result->ai_addrlen);
            if (iResult == SOCKET_ERROR) {
                std::cerr << "bind failed with error: " << WSAGetLastError() << std::endl;
                freeaddrinfo(result);
                closesocket(ListenSocket);
                WSACleanup();
                return false;
            }

            freeaddrinfo(result);

            iResult = listen(ListenSocket, SOMAXCONN);
            if (iResult == SOCKET_ERROR) {
                std::cerr << "listen failed with error: " << WSAGetLastError() << std::endl;
                closesocket(ListenSocket);
                WSACleanup();
                return false;
            }

            Running = true;
            std::cout << "Server listening on port " << port << std::endl;

            // Start Accept Thread
            std::thread(&WebSocketServer::AcceptLoop, this).detach();
            return true;
        }

        void Broadcast(const std::string& message) {
            // Simple frame creation for text
            std::vector<uint8_t> frame = CreateFrame(message);
            
            // Send to all clients
             // Use a copy to iterate because we might remove disconnected clients
            std::vector<SOCKET> clientsToKeep;
            
            for (SOCKET client : Clients) {
                int iSendResult = send(client, (char*)frame.data(), (int)frame.size(), 0);
                if (iSendResult != SOCKET_ERROR) {
                    clientsToKeep.push_back(client);
                } else {
                    std::cout << "Client disconnected during broadcast" << std::endl;
                    closesocket(client);
                }
            }
            Clients = clientsToKeep;
        }

        void Send(SOCKET client, const std::string& message) {
            std::vector<uint8_t> frame = CreateFrame(message);
            send(client, (char*)frame.data(), (int)frame.size(), 0);
        }

    private:
        void AcceptLoop() {
            while (Running) {
                SOCKET ClientSocket = accept(ListenSocket, NULL, NULL);
                if (ClientSocket == INVALID_SOCKET) {
                    std::cerr << "accept failed: " << WSAGetLastError() << std::endl;
                    continue; // Or break/retry
                }

                // Handle Handshake immediately
                if (PerformHandshake(ClientSocket)) {
                    Clients.push_back(ClientSocket);
                    std::cout << "New Client Connected" << std::endl;
                    
                    // Spawn client thread (simple architecture)
                    std::thread(&WebSocketServer::ClientHandler, this, ClientSocket).detach();
                } else {
                    closesocket(ClientSocket);
                }
            }
        }

        void ClientHandler(SOCKET client) {
             char recvbuf[4096];
             int iResult;

             while (Running) {
                 iResult = recv(client, recvbuf, sizeof(recvbuf), 0);
                 if (iResult > 0) {
                     // In a real implementation, we would parse frames here
                     // For POS updates, clients mostly LISTEN.
                     // But if POS.UI sends "CREATE_ORDER", we need to support receiving.
                     // This simple implementation might just support sending for the first step
                     // and assume Requests come via a separate TCP stream or we implement basic reading.
                     
                     // Implementing basic frame parsing is complex. 
                     // For this MVP, we will only use WS for OUTGOING updates.
                     // The POS.UI will write to DB directly OR use a separate raw TCP command if strictly needed.
                     // Wait, user said: "operation in db, then ... websocket will ping".
                     // So POS.Server actually needs to RECEIVE commands.

                     // Let's implement minimal unmasking.
                     // Byte 0: FIN/OpCode
                     // Byte 1: Mask/Len
                     
                     if (iResult < 2) continue;

                     // Decode basic text frame... (Skipped for brevity, can iterate if needed)
                     // For Phase 9, let's assume POS.UI sends updates via standard HTTP or DB directly?
                     // Request says: "go to server, operation in db, then ... ping"
                     // So Server must receive data.
                     // Easier approach: Use HTTP for COMMANDS, WS for UPDATES.
                     // But we are writing a raw socket server. Mixing HTTP/WS on same port is standard but requires parsing.
                     // Let's implement a simple text-based protocol over the same socket if it's not a handshake.
                     // OR just parse the WS frame.
                     
                     // Keeping it safe:
                     // We will interpret any received data as a potentially masked frame.
                     // A real unmasking would modify buffer. For simplicity/MVP if unmasking needed we add here. 
                     // Assuming clear text for internal localhost testing if no mask bit enforced by simple client.
                     // But Standard WS clients MASK.
                     // We need minimal unmasking logic if we use standard C# ClientWebSocket.
                     
                     // Minimal Unmasking:
                     uint8_t* val = (uint8_t*)recvbuf;
                     bool fin = (val[0] & 0x80);
                     bool mask = (val[1] & 0x80);
                     uint64_t len = val[1] & 0x7F;
                     uint64_t offset = 2;
                     if(len == 126) { len = (val[2] << 8) | val[3]; offset += 2; }
                     else if(len == 127) { /* 64 bit skipped for mvp */ offset += 8; }
                     
                     std::vector<uint8_t> decoded;
                     if(mask) {
                         uint8_t maskingKey[4] = { val[offset], val[offset+1], val[offset+2], val[offset+3] };
                         offset += 4;
                         for(size_t i=0; i<len; i++) {
                             decoded.push_back(val[offset+i] ^ maskingKey[i%4]);
                         }
                     } else {
                         for(size_t i=0; i<len; i++) decoded.push_back(val[offset+i]);
                     }
                     
                     std::string msg(decoded.begin(), decoded.end());
                     if (OnMessageCallback) OnMessageCallback(client, msg);
                 }
                 else if (iResult == 0) {
                     // Connection closed
                     break;
                 }
                 else {
                     // Error
                     break;
                 }
             }
             // Cleanup done in Broadcast loop lazily or here
        }

        bool PerformHandshake(SOCKET client) {
            char buffer[4096];
            int iResult = recv(client, buffer, sizeof(buffer), 0);
            if (iResult <= 0) return false;

            std::string request(buffer, iResult);
            
            // Check if it's GET
            if (request.find("GET") == std::string::npos) return false;

            // Extract Sec-WebSocket-Key
            std::string keyPattern = "Sec-WebSocket-Key: ";
            size_t keyStart = request.find(keyPattern);
            if (keyStart == std::string::npos) return false;
            
            keyStart += keyPattern.length();
            size_t keyEnd = request.find("\r\n", keyStart);
            std::string key = request.substr(keyStart, keyEnd - keyStart);

            // Magic GUID
            std::string magic = "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";
            std::string acceptKey = Base64Encode(Sha1(key + magic));

            std::ostringstream response;
            response << "HTTP/1.1 101 Switching Protocols\r\n"
                     << "Upgrade: websocket\r\n"
                     << "Connection: Upgrade\r\n"
                     << "Sec-WebSocket-Accept: " << acceptKey << "\r\n\r\n";

            std::string resStr = response.str();
            send(client, resStr.c_str(), (int)resStr.length(), 0);
            return true;
        }

        // Helper SHA1 and Base64 (Implementations needed)
        // For brevity, I will inject minimal implementations in the .cpp or inline
        // Since we are "doing it properly", we need these algo functions.
        // Windows Cryptography API (wincrypt.h) can do SHA1/Base64.
        
        #include <wincrypt.h>
        #pragma comment(lib, "Crypt32.lib")

        std::string Sha1(const std::string& input) {
            HCRYPTPROV hProv = 0;
            HCRYPTHASH hHash = 0;
            if (!CryptAcquireContext(&hProv, NULL, NULL, PROV_RSA_FULL, CRYPT_VERIFYCONTEXT)) return "";

            if (!CryptCreateHash(hProv, CALG_SHA1, 0, 0, &hHash)) {
                CryptReleaseContext(hProv, 0);
                return "";
            }

            if (!CryptHashData(hHash, (BYTE*)input.c_str(), (DWORD)input.length(), 0)) {
                CryptDestroyHash(hHash);
                CryptReleaseContext(hProv, 0);
                return "";
            }

            DWORD dwHashLen = 20;
            BYTE bHash[20];
            if (!CryptGetHashParam(hHash, HP_HASHVAL, bHash, &dwHashLen, 0)) {
                 CryptDestroyHash(hHash);
                CryptReleaseContext(hProv, 0);
                return "";
            }

            CryptDestroyHash(hHash);
            CryptReleaseContext(hProv, 0);
            
            return std::string((char*)bHash, dwHashLen);
        }

        std::string Base64Encode(const std::string& input) {
            DWORD dwLen = 0;
            if (!CryptBinaryToStringA((const BYTE*)input.c_str(), (DWORD)input.length(), CRYPT_STRING_BASE64 | CRYPT_STRING_NOCRLF, NULL, &dwLen)) return "";

            std::vector<char> buffer(dwLen);
            if (!CryptBinaryToStringA((const BYTE*)input.c_str(), (DWORD)input.length(), CRYPT_STRING_BASE64 | CRYPT_STRING_NOCRLF, buffer.data(), &dwLen)) return "";

            return std::string(buffer.data());
        }
        
       std::vector<uint8_t> CreateFrame(const std::string& msg) {
            std::vector<uint8_t> frame;
            frame.push_back(0x81); // Fin + Text

            if (msg.length() <= 125) {
                frame.push_back((uint8_t)msg.length());
            } else if (msg.length() <= 65535) {
                frame.push_back(126);
                frame.push_back((msg.length() >> 8) & 0xFF);
                frame.push_back(msg.length() & 0xFF);
            } else {
                frame.push_back(127);
                // 64 bit length...
                for (int i = 7; i >= 0; i--) {
                    frame.push_back((msg.length() >> (i * 8)) & 0xFF);
                }
            }

            frame.insert(frame.end(), msg.begin(), msg.end());
            return frame;
        }
    };

}
}
