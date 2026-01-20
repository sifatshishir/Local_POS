-- ============================================================================
-- Restaurant POS Database Schema
-- ============================================================================
-- Design Principles:
-- 1. Third Normal Form (3NF) - No transitive dependencies
-- 2. Referential Integrity - Foreign keys with proper constraints
-- 3. Indexing Strategy - Optimized for read-heavy operations
-- 4. Audit Trail - created_at timestamps for all tables
-- 5. Soft Deletes - is_active flags instead of hard deletes
-- ============================================================================

-- Drop existing tables if they exist (for clean reinstall)
DROP TABLE IF EXISTS order_items;
DROP TABLE IF EXISTS orders;
DROP TABLE IF EXISTS menu_items;

-- ============================================================================
-- Table: menu_items
-- Purpose: Store restaurant menu items (products)
-- ============================================================================
CREATE TABLE menu_items (
    id INT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    description TEXT,
    price DECIMAL(10, 2) NOT NULL CHECK (price >= 0),
    category VARCHAR(50) NOT NULL,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    
    -- Indexes for performance
    INDEX idx_category (category),
    INDEX idx_active (is_active),
    INDEX idx_name (name)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================================
-- Table: orders
-- Purpose: Store customer orders with type and status
-- ============================================================================
CREATE TABLE orders (
    id INT AUTO_INCREMENT PRIMARY KEY,
    order_type ENUM('DineIn', 'Parcel') NOT NULL,
    parcel_provider ENUM('None', 'Self', 'FoodPanda') DEFAULT 'None',
    table_number VARCHAR(10) NULL,  -- NULL for Parcel, required for DineIn
    status ENUM('Ordered', 'Processing', 'Done') NOT NULL DEFAULT 'Ordered',
    total_amount DECIMAL(10, 2) NOT NULL CHECK (total_amount >= 0),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    
    -- Business Rules Constraints
    CONSTRAINT chk_table_number CHECK (
        (order_type = 'DineIn' AND table_number IS NOT NULL) OR
        (order_type = 'Parcel' AND table_number IS NULL)
    ),
    
    CONSTRAINT chk_parcel_provider CHECK (
        (order_type = 'DineIn' AND parcel_provider = 'None') OR
        (order_type = 'Parcel' AND parcel_provider IN ('Self', 'FoodPanda'))
    ),
    
    -- Indexes for performance
    INDEX idx_status (status),
    INDEX idx_order_type (order_type),
    INDEX idx_created_at (created_at),
    INDEX idx_status_created (status, created_at)  -- Composite index for kitchen display queries
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================================
-- Table: order_items
-- Purpose: Store line items for each order (many-to-many relationship)
-- ============================================================================
CREATE TABLE order_items (
    id INT AUTO_INCREMENT PRIMARY KEY,
    order_id INT NOT NULL,
    menu_item_id INT NOT NULL,
    quantity INT NOT NULL CHECK (quantity > 0),
    price DECIMAL(10, 2) NOT NULL CHECK (price >= 0),  -- Price at time of order (historical accuracy)
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    
    -- Foreign Keys with Referential Integrity
    CONSTRAINT fk_order_items_order
        FOREIGN KEY (order_id) 
        REFERENCES orders(id)
        ON DELETE CASCADE
        ON UPDATE CASCADE,
    
    CONSTRAINT fk_order_items_menu_item
        FOREIGN KEY (menu_item_id) 
        REFERENCES menu_items(id)
        ON DELETE RESTRICT  -- Prevent deletion of menu items that are in orders
        ON UPDATE CASCADE,
    
    -- Indexes for performance
    INDEX idx_order_id (order_id),
    INDEX idx_menu_item_id (menu_item_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================================
-- Views for Common Queries (Performance Optimization)
-- ============================================================================

-- View: Active Menu Items (frequently accessed)
CREATE OR REPLACE VIEW v_active_menu_items AS
SELECT 
    id,
    name,
    description,
    price,
    category
FROM menu_items
WHERE is_active = TRUE
ORDER BY category, name;

-- View: Active Orders with Details (for Kitchen Display)
CREATE OR REPLACE VIEW v_active_orders AS
SELECT 
    o.id,
    o.order_type,
    o.table_number,
    o.status,
    o.total_amount,
    o.created_at,
    COUNT(oi.id) as item_count
FROM orders o
LEFT JOIN order_items oi ON o.id = oi.order_id
WHERE o.status IN ('Ordered', 'Processing')
GROUP BY o.id, o.order_type, o.table_number, o.status, o.total_amount, o.created_at
ORDER BY o.created_at ASC;

-- ============================================================================
-- Stored Procedures (Business Logic Encapsulation)
-- ============================================================================

DELIMITER //

-- Procedure: Create Order with Items (Transaction Safety)
CREATE PROCEDURE sp_create_order(
    IN p_order_type VARCHAR(20),
    IN p_table_number VARCHAR(10),
    IN p_total_amount DECIMAL(10, 2),
    OUT p_order_id INT
)
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Error creating order';
    END;
    
    START TRANSACTION;
    
    INSERT INTO orders (order_type, table_number, status, total_amount)
    VALUES (p_order_type, p_table_number, 'Ordered', p_total_amount);
    
    SET p_order_id = LAST_INSERT_ID();
    
    COMMIT;
END //

-- Procedure: Update Order Status (with validation)
CREATE PROCEDURE sp_update_order_status(
    IN p_order_id INT,
    IN p_new_status VARCHAR(20)
)
BEGIN
    DECLARE current_status VARCHAR(20);
    
    -- Get current status
    SELECT status INTO current_status FROM orders WHERE id = p_order_id;
    
    -- Validate status transition
    IF current_status IS NULL THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Order not found';
    END IF;
    
    -- Update status
    UPDATE orders 
    SET status = p_new_status, updated_at = CURRENT_TIMESTAMP
    WHERE id = p_order_id;
END //

DELIMITER ;

-- ============================================================================
-- Triggers (Data Integrity and Audit)
-- ============================================================================

-- Trigger: Prevent modification of completed orders
DELIMITER //
CREATE TRIGGER trg_prevent_completed_order_modification
BEFORE UPDATE ON orders
FOR EACH ROW
BEGIN
    IF OLD.status = 'Done' AND NEW.status != 'Done' THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Cannot modify completed orders';
    END IF;
END //
DELIMITER ;

-- ============================================================================
-- Initial Data Validation
-- ============================================================================
-- Verify tables were created successfully
SELECT 
    TABLE_NAME, 
    ENGINE, 
    TABLE_ROWS, 
    CREATE_TIME
FROM information_schema.TABLES
WHERE TABLE_SCHEMA = DATABASE()
AND TABLE_NAME IN ('menu_items', 'orders', 'order_items')
ORDER BY TABLE_NAME;

-- ============================================================================
-- Performance Analysis Queries (for DBA reference)
-- ============================================================================
-- Check index usage:
-- SHOW INDEX FROM menu_items;
-- SHOW INDEX FROM orders;
-- SHOW INDEX FROM order_items;

-- Analyze query performance:
-- EXPLAIN SELECT * FROM v_active_orders;
-- ============================================================================
