<?php

// Класс для подключения к базе данных
class DBConnection {
    
    // Имя хоста
    private $host;

    // Имя пользователя
    private $username;

    // Пароль
    private $password;

    // Имя базы данных
    private $database;

    // Глобальная переменная для подключения к базе данных. Используется для запросов к базе
    private $connection;

    // Конструктор. Инициализиурет все поля класса
    public function __construct(string $host, string $username, string $password, string $database) {
        $this->host = $host;
        $this->username = $username;
        $this->password = $password;
        $this->database = $database;
        $this->connection = null;
    }

    // Подключается к базе данных
    public function connect() {
        try {
            $this->connection = new mysqli($this->host, $this->username, $this->password, $this->database);
            echo "successful!";
        }
        catch (Exception $e) {
            echo $e->getMessage();
        }
    }

    // Возвращает объект mysqli
    public function getConnection(): mysqli {
        return $this->connection;
    }
    
    // Закрывает подключение
    public function disconnect() {
        try {
            $this->connection->close();
        }
        catch (Exception $e) {
            echo $e->getMessage();
        }
    }
}