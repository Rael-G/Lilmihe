CREATE VIEW products_categories AS
    SELECT p.id AS product_id, p.name AS product_name, p.price, c.id AS categorie_id, c.name AS categorie_name
        FROM products p
    JOIN categories c
        ON p.id = c.product_id;