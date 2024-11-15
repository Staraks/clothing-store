<?php

// Класс для работы с таблицей Brand
class Brand {

    // Идентификатор бренда
    private $id;

    // Имя бренда
    private $name;

    // Опиание бренда
    private $description;

    // Конструктор. Инициализиурет все поля класса
    public function __construct(int $id, string $name, string $description) {
        $this->id = $id;
        $this->name = $name;
        $this->description = $description;
    }

    // Возвращет ID бренда
    public function getId(): string {
        return $this->id;
    }

    // Возвращет имя бренда
    public function getName(): string {
        return $this->name;
    }

    // Возвращет описание бренда
    public function getDescription(): string {
        return $this->description;
    }

    // Добавляет новый бренд в таблицу
    public static function add(mysqli $connection, Brand $brand) {
        $id = $brand->getId();
        $name = $brand->getName();
        $description = $brand->getDescription();
        $sql = "INSERT INTO brand VALUES ($id, '$name', '$description');";
        $connection->query($sql);
    }

    // Удаляет бренд из таблицы
    public static function delete(mysqli $connection, Brand $brand) {
        $id = $brand->getId();
        $sql = "DELETE FROM brand WHERE id = $id";
        $connection->query($sql);
    }

    // Обновляет запись
    public static function update(mysqli $connection, Brand $brand) {
        $id = $brand->getId();
        $name = $brand->getName();
        $description = $brand->getDescription();
        $sql = "UPDATE brand SET id = $id, name = '$name', description = '$description' WHERE id = $id";
        $connection->query($sql);
    }
}