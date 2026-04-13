-- Default categories for a furniture store
INSERT OR IGNORE INTO categories (id, name, description, parent_id, sort_order) VALUES
    (1,  'Sofas & Sectionals',    'All sofa types',              NULL, 1),
    (2,  'Chairs',                'Accent, dining, office',      NULL, 2),
    (3,  'Tables',                'Coffee, dining, side tables', NULL, 3),
    (4,  'Beds & Bedroom',        'Bed frames, headboards',      NULL, 4),
    (5,  'Storage',               'Wardrobes, cabinets, shelves',NULL, 5),
    (6,  'Outdoor',               'Garden & patio furniture',    NULL, 6),
    (7,  'Sectional Sofas',       'L-shaped & modular',         1,    1),
    (8,  'Loveseat',              'Two-seat sofas',              1,    2),
    (9,  'Accent Chairs',         'Decorative single chairs',   2,    1),
    (10, 'Office Chairs',         'Ergonomic & desk chairs',    2,    2),
    (11, 'Coffee Tables',         'Low living room tables',     3,    1),
    (12, 'Dining Tables',         'Kitchen & dining sets',      3,    2);
