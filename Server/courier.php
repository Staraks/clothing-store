<?php

// Класс для работы с таблицей payments
class Payment {

    // Идентификатор платежа
    private $id;

    // Идентификатор заказа
    private $orderId;

    // Дата платежа
    private $date;

    // Количество
    private $amount;

    // Статус заказа
    private $status;

    // Конструктор. Инициализирует поля класса
    public function __construct(int $id, int $orderId, string $date, float $amount, string $status) {
        $this->id = $id;
        $this->orderId = $orderId;
        $this->date = $date;
        $this->amount = $amount;
        $this->status = $status;
    }

    // Возвращает идентификатор
    public function getId(): int {
        return $this->id;
    }

    // Возвращает идентификатор заказа
    public function getOrderId(): int {
        return $this->orderId;
    }

    // Возвращает дату заказа
    public function getDate(): string {
        return $this->date;
    }

    // Возвращает количество
    public function getAmount(): float {
        return $this->amount;
    }

    // Возвращает статус заказа
    public function getStatus(): string {
        return $this->status;
    }

    // Добавляет новый платеж в таблицу
    public static function add(mysqli $connection, Payment $payment) {
        $id = $payment->getId();
        $orderId = $payment->getOrderId();
        $date = $payment->getDate();
        $amount = $payment->getAmount();
        $status = $payment->getStatus();
        $sql = "INSERT INTO payments VALUES ($id, $orderId, '$date', $amount, '$status');";
        $connection->query($sql);
    }

    // Удаляет плватеж из таблицы
    public static function delete(mysqli $connection, Payment $payment) {
        $id = $payment->getId();
        $sql = "DELETE FROM payments WHERE id = $id";
        $connection->query($sql);
    }

    // Обновляет запись
    public static function update(mysqli $connection, Payment $payment) {
        $id = $payment->getId();
        $orderId = $payment->getOrderId();
        $date = $payment->getDate();
        $amount = $payment->getAmount();
        $status = $payment->getStatus();
        $sql = "UPDATE payments SET id = $id, order_id = $orderId, date = '$date', amount = $amount, status = $status;";
        $connection->query($sql);
    }
}