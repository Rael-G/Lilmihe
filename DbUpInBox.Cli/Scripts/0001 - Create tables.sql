CREATE TABLE products
(
    id SERIAL PRIMARY KEY,
    name VARCHAR(255) NOT NULL UNIQUE,
    price DECIMAL(10,2) NOT NULL
);

CREATE TABLE categories
(
    id SERIAL PRIMARY KEY,
    name VARCHAR(255),
    product_id INT,

    CONSTRAINT fk_categories_products
        FOREIGN KEY (product_id)
        REFERENCES products(id)
);