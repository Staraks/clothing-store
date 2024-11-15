<?php

// Класс для работы с таблицей Order
class Order {

    // Идентификатор заказа
    private $id;

    // Идентификатор пользователя
    private $userId;

    // Дата заказа
    private $date;

    // Количество
    private $totalAmount;

    // Статус заказа
    private $status;

    // Адрес отправления
    private $shippingAddress;

    // Конструктор. Инициализиурет все поля класса
    public function __construct(int $id, int $userId, string $date, float $totalAmount, string $status, string $shippingAddress) {
        $this->id = $id;
        $this->userId = $userId;
        $this->date = $date;
        $this->totalAmount = $totalAmount;
        $this->status = $status;
        $this->shippingAddress = $shippingAddress;
    }

    // Возвращает ID заказа
    public function getId(): int {
        return $this->id;
    }

    // Возвращает ID пользователя
    public function getUserId(): int {
        return $this->userId;
    }

    // Возвращает дату заказа
    public function getDate(): string {
        return $this->date;
    }

    // Возвращает количество
    public function getTotalAmount(): float {
        return $this->totalAmount;
    }

    // Возвращает статус заказа
    public function getStatus(): string {
        return $this->status;
    }

    // Возвращает адрес отправки
    public function getShippingAddress(): string {
        return $this->shippingAddress;
    }

    // Добавляет новый заказ в таблицу
    public static function add(mysqli $connection, Order $order) {
        $id = $order->getId();
        $userId = $order->getUserId();
        $date = $order->getDate();
        $totalAmount = $order->getTotalAmount();
        $status = $order->getStatus();
        $shippingAddress = $order->getShippingAddress();
        $sql = "INSERT INTO orders VALUES ($id, $userId, '$date', $totalAmount, '$status', '$shippingAddress');";
        $connection->query($sql);
    }

    // Удаляет заказ из таблицы
    public static function delete(mysqli $connection, Order $order) {
        $id = $order->getId();
        $sql = "DELETE FROM order WHERE id = $id";
        $connection->query($sql);
    }

    // Обновляет запись
    public static function update(mysqli $connection, Order $order) {
        $id = $order->getId();
        $userId = $order->getUserId();
        $date = $order->getDate();
        $totalAmount = $order->getTotalAmount();
        $status = $order->getStatus();
        $shippingAddress = $order->getShippingAddress();
        $sql = "UPDATE orders SET id = $id, user_id = $userId, date = '$date', total_amount = $totalAmount, status = '$status', shipping_address = '$shippingAddress');";
        $connection->query($sql);
    }
}