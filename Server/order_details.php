<?php

// Класс для работы с таблицей orders_item
class OrdersItem {

    // Идентификатор
    private $id;

    // Идентификатор заказа
    private $orderId;

    // Идентификатор товара
    private $productId;

    // Цена
    private $price;

    // Конструктор. Инициализирует поля класса
    public function __construct(int $id, int $orderId, int $productId, float $price) {
        $this->id = $id;
        $this->orderId = $orderId;
        $this->productId = $productId;
        $this->price = $price;
    }

    // Возвращает идентификатор
    public function getId(): int {
        return $this->id;
    }

    // Возвращает идентификатор заказа
    public function getOrderId(): int {
        return $this->orderId;
    }

    // Возвращает идентификатор товара
    public function getProductId(): int {
        return $this->productId;
    }

    // Возвращает цену
    public function getPrice(): float {
        return $this->price;
    }

    // Добавляет новые детали заказа в таблицу
    public static function add(mysqli $connection, OrdersItem $order) {
        $id = $order->getId();
        $orderId = $order->getOrderId();
        $productId = $order->getProductId();
        $price = $order->getPrice();
        $sql = "INSERT INTO orders_item VALUES ($id, $orderId, $productId, $price);";
        $connection->query($sql);
    }

    // Удаляет детали заказа из таблицы
    public static function delete(mysqli $connection, OrdersItem $order) {
        $id = $order->getId();
        $sql = "DELETE FROM orders_item WHERE id = $id";
        $connection->query($sql);
    }

    // Обновляет запись
    public static function update(mysqli $connection, OrdersItem $order) {
        $id = $order->getId();
        $orderId = $order->getOrderId();
        $productId = $order->getProductId();
        $price = $order->getPrice();
        $sql = "UPDATE orders_item SET id = $id, order_id = $orderId, product_id = $productId, price = $price;";
        $connection->query($sql);
    }
}