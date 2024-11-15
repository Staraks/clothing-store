<?php

// Класс для работы с таблицей users
class User {

    // Идентификатор пользователя
    public $id;

    // Имя пользователя
    public $name;

    // Фамилия пользователя
    public $surname;

    // Номер пользователя
    public $phoneNumber;

    // Пароль
    public $password;

    // Конструктор. Инициализирует поля класса
    public function __construct(int $id, string $name, string $surname, string $phoneNumber, string $password) {
        $this->id = $id;
        $this->name = $name;
        $this->surname = $surname;
        $this->phoneNumber = $phoneNumber;
        $this->password = $password;
    }

    // Возвращает идентификатор
    public function getId(): int {
        return $this->id;
    }

    // возвращает имя пользователя
    public function getName(): string {
        return $this->name;
    }

    // возвращает фамилию пользователя
    public function getSurname(): string {
        return $this->surname;
    }

    // возвращает номер пользователя
    public function getPhoneNumber(): string {
        return $this->phoneNumber;
    }

    // возвращает пароль пользователя
    public function getPassword(): string {
        return $this->password;
    }

    // Добавляет нового пользователя в таблицу
    public static function add(mysqli $connection, User $user) {
        $id = $user->getId();
        $name = $user->getName();
        $surname = $user->getSurname();
        $phone = $user->getPhoneNumber();
        $password = $user->getPassword();
        $sql = "INSERT INTO user VALUES ($id, '$name', '$surname', '$phone', '$password');";
        $connection->query($sql);
    }

    // Удаляет пользователя из таблицы
    public static function delete(mysqli $connection, User $user) {
        $id = $user->getId();
        $sql = "DELETE FROM user WHERE id = $id";
        $connection->query($sql);
    }

    // Обновляет запись
    public static function update(mysqli $connection, User $user) {
        $id = $user->getId();
        $name = $user->getName();
        $surname = $user->getSurname();
        $phone = $user->getPhoneNumber();
        $password = $user->getPassword();
        $sql = "UPDATE user SET id = $id, name = '$name', surname = '$surname', phone_number = '$phone', password = '$password');";
        $connection->query($sql);
    }
}