# Hybrid POS System

A comprehensive Point of Sale system featuring a C++ WebSocket Server and C# WinForms Clients (Cashier UI & Kitchen Display).

## Architecture

*   **POS.Server** (C++): Central hub handling database operations and real-time WebSocket broadcasting.
*   **POS.UI** (C# WinForms): Cashier interface for creating orders and generating receipts.
*   **POS.KitchenDisplay** (C# WinForms): Kitchen interface for viewing and updating order status.
*   **POS.Client.Common** (C# Class Library): Shared logic for configuration and WebSocket communication.
*   **MySQL**: Relational database for persistent storage.

## Prerequisites

1.  **Visual Studio 2022** with:
    *   Desktop development with C++
    *   .NET Desktop Development
2.  **MySQL Server** (8.0+)
3.  **MySQL Connector/C++** (Ensure libraries are linked correctly).

## Setup Instructions

1.  **Database Setup**:
    *   Ensure your MySQL database `pos_db` exists.
    *   Import schema if needed (tables: `orders`, `order_items`).

2.  **Configuration (.env)**:
    *   The project uses a `.env` file in the solution root (`i:\Projects\VS\POS\.env`) for configuration.
    *   **Action**: Create or update `.env` in the root with your settings:

    ```ini
    # MySQL Database
    DB_HOST=127.0.0.1
    DB_PORT=3306
    DB_USER=root
    DB_PASS=password
    DB_NAME=pos_db

    # WebSocket Server
    WS_HOST=localhost
    WS_PORT=8080

    # Logging
    SERVER_LOG_PATH=server.log

    # Client
    RECEIPT_PATH=
    ```

3.  **Build**:
    *   Open `POS.sln` in Visual Studio.
    *   Build Solution (Ctrl+Shift+B).

## Running the Application

To run the full system efficiently:

1.  **Multiple Startup Projects**:
    *   Right-click Solution in Solution Explorer -> **Properties**.
    *   Go to **Common Properties** -> **Startup Project**.
    *   Select **Multiple startup projects**.
    *   Set **POS.Server**, **POS.KitchenDisplay**, and **POS.UI** to **Start**.
    *   Ensure `POS.Server` is listed first (top of the list) to start it slightly earlier.

2.  **Run**:
    *   Press **F5**.
    *   The Server console will open, followed by the two Client windows.
    *   Clients will automatically connect to `ws://localhost:8080`.

## Features
*   **Real-time Updates**: Orders created in the UI appear instantly in the Kitchen Display. Status changes in the Kitchen update the UI immediately.
*   **Dynamic Configuration**: All critical settings (DB, Ports, Paths) are configurable via `.env`.
*   **Resilience**: Clients auto-connect on startup.

## Troubleshooting

*   **Server Closes Immediately**: Check `server.log` for DB connection errors. Verify `.env` credentials.
*   **Clients Don't Connect**: Ensure Server is running on Port 8080. Check firewall.
*   **Build Errors**: Build `POS.Client.Common` explicitly if referenced projects report missing symbols.
