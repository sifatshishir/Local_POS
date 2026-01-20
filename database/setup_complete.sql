-- ============================================================================
-- POS Database - Complete Setup Script
-- This script will drop and recreate the entire database with schema and seed data
-- ============================================================================

-- Drop existing database if it exists
DROP DATABASE IF EXISTS pos_db;

-- Create fresh database
CREATE DATABASE pos_db;

-- Use the database
USE pos_db;

-- ============================================================================
-- SCHEMA: Tables
-- ============================================================================

-- Menu Items Table
CREATE TABLE menu_items (
    id INT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    description TEXT,
    price DECIMAL(10, 2) NOT NULL CHECK (price >= 0),
    category VARCHAR(50) NOT NULL,
    is_active BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    
    INDEX idx_category (category),
    INDEX idx_active (is_active),
    INDEX idx_category_active (category, is_active)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Orders Table
CREATE TABLE orders (
    id INT AUTO_INCREMENT PRIMARY KEY,
    order_type ENUM('DineIn', 'Parcel') NOT NULL,
    parcel_provider ENUM('None', 'Self', 'FoodPanda') DEFAULT 'None',
    table_number VARCHAR(10) NULL,
    status ENUM('Ordered', 'Processing', 'Done') NOT NULL DEFAULT 'Ordered',
    total_amount DECIMAL(10, 2) NOT NULL CHECK (total_amount >= 0),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    
    CONSTRAINT chk_table_number CHECK (
        (order_type = 'DineIn' AND table_number IS NOT NULL) OR
        (order_type = 'Parcel' AND table_number IS NULL)
    ),
    
    CONSTRAINT chk_parcel_provider CHECK (
        (order_type = 'DineIn' AND parcel_provider = 'None') OR
        (order_type = 'Parcel' AND parcel_provider IN ('Self', 'FoodPanda'))
    ),
    
    INDEX idx_status (status),
    INDEX idx_order_type (order_type),
    INDEX idx_created_at (created_at),
    INDEX idx_status_created (status, created_at)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Order Items Table
CREATE TABLE order_items (
    id INT AUTO_INCREMENT PRIMARY KEY,
    order_id INT NOT NULL,
    menu_item_id INT NOT NULL,
    quantity INT NOT NULL CHECK (quantity > 0),
    price DECIMAL(10, 2) NOT NULL CHECK (price >= 0),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    
    FOREIGN KEY (order_id) REFERENCES orders(id) ON DELETE CASCADE,
    FOREIGN KEY (menu_item_id) REFERENCES menu_items(id) ON DELETE RESTRICT,
    
    INDEX idx_order_id (order_id),
    INDEX idx_menu_item_id (menu_item_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================================
-- SEED DATA: Menu Items
-- ============================================================================

INSERT INTO menu_items (name, description, price, category, is_active) VALUES
-- Appetizers
('Spring Rolls', 'Crispy vegetable spring rolls', 5.99, 'Appetizers', TRUE),
('Chicken Wings', 'Spicy buffalo wings', 8.99, 'Appetizers', TRUE),
('Garlic Bread', 'Toasted bread with garlic butter', 4.50, 'Appetizers', TRUE),
('Mozzarella Sticks', 'Fried mozzarella with marinara', 6.99, 'Appetizers', TRUE),
('Nachos', 'Tortilla chips with cheese and salsa', 7.50, 'Appetizers', TRUE),

-- Main Course
('Grilled Chicken Burger', 'Juicy chicken with lettuce and tomato', 12.99, 'Main Course', TRUE),
('Beef Steak', 'Premium ribeye steak with sides', 24.99, 'Main Course', TRUE),
('Margherita Pizza', 'Classic tomato and mozzarella pizza', 14.99, 'Main Course', TRUE),
('Pasta Carbonara', 'Creamy pasta with bacon', 13.50, 'Main Course', TRUE),
('Fish and Chips', 'Battered fish with french fries', 15.99, 'Main Course', TRUE),

-- Beverages
('Coca Cola', 'Chilled soft drink', 2.50, 'Beverages', TRUE),
('Pepsi', 'Chilled soft drink', 2.50, 'Beverages', TRUE),
('Sprite', 'Lemon-lime soda', 2.50, 'Beverages', TRUE),
('Orange Juice', 'Fresh squeezed orange juice', 4.50, 'Beverages', TRUE),
('Iced Tea', 'Refreshing iced tea', 3.00, 'Beverages', TRUE),

-- Hot Drinks
('Espresso', 'Strong Italian coffee', 3.50, 'Hot Drinks', TRUE),
('Cappuccino', 'Espresso with steamed milk', 4.50, 'Hot Drinks', TRUE),
('Latte', 'Smooth coffee with milk', 4.50, 'Hot Drinks', TRUE),
('Hot Chocolate', 'Rich chocolate drink', 4.00, 'Hot Drinks', TRUE),
('Green Tea', 'Healthy green tea', 3.00, 'Hot Drinks', TRUE),

-- Desserts
('Chocolate Brownie', 'Warm brownie with ice cream', 6.99, 'Desserts', TRUE),
('Cheesecake', 'New York style cheesecake', 7.99, 'Desserts', TRUE),
('Ice Cream Sundae', 'Vanilla ice cream with toppings', 5.50, 'Desserts', TRUE),
('Apple Pie', 'Classic apple pie with cinnamon', 6.50, 'Desserts', TRUE),
('Tiramisu', 'Italian coffee-flavored dessert', 8.50, 'Desserts', TRUE);

-- ============================================================================
-- SEED DATA: Sample Test Orders
-- ============================================================================

-- Test Order 1: Dine-in
INSERT INTO orders (order_type, parcel_provider, table_number, status, total_amount) 
VALUES ('DineIn', 'None', 'T-05', 'Ordered', 45.97);
SET @order_id_1 = LAST_INSERT_ID();

INSERT INTO order_items (order_id, menu_item_id, quantity, price) VALUES
(@order_id_1, 1, 2, 5.99),   -- 2x Spring Rolls
(@order_id_1, 6, 1, 12.99),  -- 1x Grilled Chicken Burger
(@order_id_1, 11, 2, 2.50),  -- 2x Coca Cola
(@order_id_1, 21, 1, 6.99);  -- 1x Chocolate Brownie

-- Test Order 2: Parcel (Self Delivery)
INSERT INTO orders (order_type, parcel_provider, table_number, status, total_amount) 
VALUES ('Parcel', 'Self', NULL, 'Ordered', 29.98);
SET @order_id_2 = LAST_INSERT_ID();

INSERT INTO order_items (order_id, menu_item_id, quantity, price) VALUES
(@order_id_2, 8, 1, 14.99),  -- 1x Margherita Pizza
(@order_id_2, 14, 1, 4.50),  -- 1x Orange Juice
(@order_id_2, 10, 1, 15.99); -- 1x Fish and Chips

-- Test Order 3: Parcel (FoodPanda)
INSERT INTO orders (order_type, parcel_provider, table_number, status, total_amount) 
VALUES ('Parcel', 'FoodPanda', NULL, 'Ordered', 41.98);
SET @order_id_3 = LAST_INSERT_ID();

INSERT INTO order_items (order_id, menu_item_id, quantity, price) VALUES
(@order_id_3, 7, 1, 24.99),  -- 1x Beef Steak
(@order_id_3, 17, 1, 4.50),  -- 1x Cappuccino
(@order_id_3, 22, 1, 7.99);  -- 1x Cheesecake

-- ============================================================================
-- Verification
-- ============================================================================

-- Show summary
SELECT 'Database setup completed successfully!' as Status;
SELECT COUNT(*) as TotalMenuItems FROM menu_items WHERE is_active = TRUE;
SELECT COUNT(*) as TotalOrders FROM orders;
SELECT COUNT(*) as TotalOrderItems FROM order_items;

-- Show test orders
SELECT 
    o.id,
    o.order_type,
    o.parcel_provider,
    o.table_number,
    o.status,
    o.total_amount,
    o.created_at,
    COUNT(oi.id) as items
FROM orders o
LEFT JOIN order_items oi ON o.id = oi.order_id
GROUP BY o.id, o.order_type, o.parcel_provider, o.table_number, o.status, o.total_amount, o.created_at
ORDER BY o.created_at DESC;
