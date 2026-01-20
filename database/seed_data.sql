-- ============================================================================
-- Restaurant POS - Sample Menu Data
-- ============================================================================
-- Purpose: Seed database with realistic restaurant menu items for testing
-- Categories: Appetizers, Main Course, Beverages, Desserts
-- ============================================================================

-- Clear existing data (for clean reinstall)
DELETE FROM order_items;
DELETE FROM orders;
DELETE FROM menu_items;

-- Reset AUTO_INCREMENT
ALTER TABLE menu_items AUTO_INCREMENT = 1;
ALTER TABLE orders AUTO_INCREMENT = 1;
ALTER TABLE order_items AUTO_INCREMENT = 1;

-- ============================================================================
-- Category: Appetizers
-- ============================================================================
INSERT INTO menu_items (name, description, price, category, is_active) VALUES
('Spring Rolls', 'Crispy vegetable spring rolls with sweet chili sauce', 5.99, 'Appetizers', TRUE),
('Chicken Wings', 'Buffalo style chicken wings with ranch dip', 8.99, 'Appetizers', TRUE),
('Garlic Bread', 'Toasted bread with garlic butter and herbs', 4.50, 'Appetizers', TRUE),
('Mozzarella Sticks', 'Breaded mozzarella with marinara sauce', 6.99, 'Appetizers', TRUE),
('Nachos Supreme', 'Tortilla chips with cheese, jalapeños, and salsa', 9.99, 'Appetizers', TRUE);

-- ============================================================================
-- Category: Main Course
-- ============================================================================
INSERT INTO menu_items (name, description, price, category, is_active) VALUES
('Grilled Chicken Burger', 'Juicy grilled chicken with lettuce, tomato, and mayo', 12.99, 'Main Course', TRUE),
('Beef Steak', 'Premium ribeye steak with mashed potatoes and vegetables', 24.99, 'Main Course', TRUE),
('Margherita Pizza', 'Classic pizza with tomato sauce, mozzarella, and basil', 14.99, 'Main Course', TRUE),
('Pasta Carbonara', 'Creamy pasta with bacon, eggs, and parmesan', 13.99, 'Main Course', TRUE),
('Fish and Chips', 'Battered fish fillet with crispy fries and tartar sauce', 15.99, 'Main Course', TRUE),
('Vegetable Stir Fry', 'Mixed vegetables in Asian sauce with rice', 11.99, 'Main Course', TRUE),
('BBQ Ribs', 'Slow-cooked pork ribs with BBQ sauce and coleslaw', 18.99, 'Main Course', TRUE),
('Caesar Salad', 'Fresh romaine lettuce with Caesar dressing and croutons', 9.99, 'Main Course', TRUE),
('Chicken Tikka Masala', 'Spicy chicken curry with basmati rice and naan', 16.99, 'Main Course', TRUE),
('Lamb Chops', 'Grilled lamb chops with rosemary and roasted vegetables', 22.99, 'Main Course', TRUE);

-- ============================================================================
-- Category: Beverages
-- ============================================================================
INSERT INTO menu_items (name, description, price, category, is_active) VALUES
('Coca Cola', 'Classic Coca Cola (330ml)', 2.50, 'Beverages', TRUE),
('Sprite', 'Lemon-lime soda (330ml)', 2.50, 'Beverages', TRUE),
('Orange Juice', 'Freshly squeezed orange juice', 4.50, 'Beverages', TRUE),
('Iced Tea', 'Refreshing iced tea with lemon', 3.50, 'Beverages', TRUE),
('Coffee', 'Freshly brewed coffee', 3.00, 'Beverages', TRUE),
('Cappuccino', 'Espresso with steamed milk and foam', 4.50, 'Beverages', TRUE),
('Mineral Water', 'Still mineral water (500ml)', 2.00, 'Beverages', TRUE),
('Mango Smoothie', 'Fresh mango blended with yogurt', 5.50, 'Beverages', TRUE),
('Hot Chocolate', 'Rich hot chocolate with whipped cream', 4.00, 'Beverages', TRUE),
('Green Tea', 'Organic green tea', 3.00, 'Beverages', TRUE);

-- ============================================================================
-- Category: Desserts
-- ============================================================================
INSERT INTO menu_items (name, description, price, category, is_active) VALUES
('Chocolate Brownie', 'Warm chocolate brownie with vanilla ice cream', 6.99, 'Desserts', TRUE),
('Cheesecake', 'New York style cheesecake with berry compote', 7.99, 'Desserts', TRUE),
('Tiramisu', 'Classic Italian tiramisu with coffee and mascarpone', 8.50, 'Desserts', TRUE),
('Ice Cream Sundae', 'Three scoops of ice cream with toppings', 5.99, 'Desserts', TRUE),
('Apple Pie', 'Homemade apple pie with cinnamon', 6.50, 'Desserts', TRUE),
('Crème Brûlée', 'French vanilla custard with caramelized sugar', 7.50, 'Desserts', TRUE);

-- ============================================================================
-- Inactive Items (for testing soft delete functionality)
-- ============================================================================
INSERT INTO menu_items (name, description, price, category, is_active) VALUES
('Discontinued Item', 'This item is no longer available', 10.00, 'Main Course', FALSE);

-- ============================================================================
-- Verification Query
-- ============================================================================
SELECT 
    category,
    COUNT(*) as item_count,
    MIN(price) as min_price,
    MAX(price) as max_price,
    AVG(price) as avg_price
FROM menu_items
WHERE is_active = TRUE
GROUP BY category
ORDER BY category;

-- Display all active items
SELECT 
    id,
    name,
    price,
    category
FROM menu_items
WHERE is_active = TRUE
ORDER BY category, name;

-- ============================================================================
-- Sample Test Orders (for development testing)
-- ============================================================================

-- Test Order 1: Dine-in
CALL sp_create_order('DineIn', 'T-05', 45.97, @order_id_1);
INSERT INTO order_items (order_id, menu_item_id, quantity, price) VALUES
(@order_id_1, 1, 2, 5.99),   -- 2x Spring Rolls
(@order_id_1, 6, 1, 12.99),  -- 1x Grilled Chicken Burger
(@order_id_1, 11, 2, 2.50),  -- 2x Coca Cola
(@order_id_1, 21, 1, 6.99);  -- 1x Chocolate Brownie

-- Test Order 2: Parcel
CALL sp_create_order('Parcel', NULL, 29.98, @order_id_2);
INSERT INTO order_items (order_id, menu_item_id, quantity, price) VALUES
(@order_id_2, 8, 1, 14.99),  -- 1x Margherita Pizza
(@order_id_2, 14, 1, 4.50),  -- 1x Orange Juice
(@order_id_2, 10, 1, 15.99); -- 1x Fish and Chips

-- Test Order 3: Foodpanda
CALL sp_create_order('Foodpanda', NULL, 41.98, @order_id_3);
INSERT INTO order_items (order_id, menu_item_id, quantity, price) VALUES
(@order_id_3, 7, 1, 24.99),  -- 1x Beef Steak
(@order_id_3, 17, 1, 4.50),  -- 1x Cappuccino
(@order_id_3, 22, 1, 7.99);  -- 1x Cheesecake

-- Verify test orders
SELECT 
    o.id,
    o.order_type,
    o.table_number,
    o.status,
    o.total_amount,
    o.created_at,
    COUNT(oi.id) as items
FROM orders o
LEFT JOIN order_items oi ON o.id = oi.order_id
GROUP BY o.id, o.order_type, o.table_number, o.status, o.total_amount, o.created_at
ORDER BY o.created_at DESC;

-- ============================================================================
-- Data Summary
-- ============================================================================
SELECT 'Menu Items' as Entity, COUNT(*) as Total FROM menu_items WHERE is_active = TRUE
UNION ALL
SELECT 'Orders', COUNT(*) FROM orders
UNION ALL
SELECT 'Order Items', COUNT(*) FROM order_items;

-- ============================================================================
