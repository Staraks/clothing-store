<?php

// Файля для запуска сервера

include 'connection.php';
include 'brand.php';

// Создание объекта базы данных
$connection = new DBConnection('localhost', 'root', '', 'thendont');
$connection->connect();

$brand = new Brand(1, 'samsung', 'samsung desc');
Brand::delete($connection->getConnection(), $brand);