#  Diplom WPF Client

Клиентское приложение на WPF. Предоставляет интерфейс для авторизации и регистрации пользователей через Django API.

## Краткое описание

Приложение на C# и WPF, которое выполняет:
-  Графический интерфейс для входа в систему
-  Графический интерфейс для регистрации
-  Взаимодействие с Django API через HTTP-запросы
-  Управление сессией пользователя (Singleton)
-  Валидацию данных на клиенте


## Системные требования

| Компонент | Требование |
|-----------|------------|
| ОС | Windows 10/11 |
| .NET | .NET 6.0 или выше |
| IDE | Visual Studio 2019/2022 (рекомендуется) |
| Память | Минимум 4 GB RAM |
| Место на диске | Минимум 1 GB |


### Шаг 1

git clone https://github.com/superdr0chun/diplom-wpf-client.git

cd diplom-wpf-client

### Шаг 2

Запустите Visual Studio 2019/2022

Выберите File → Open → Project/Solution

Найдите файл DjangoWPFClient.sln и откройте его

### Шаг 3

При первом открытии Visual Studio автоматически восстановит NuGet-пакеты.

Или вручную через Package Manager Console:

- Update-Package


# Структура

<img width="293" height="456" alt="Снимок экрана 2026-04-02 141307" src="https://github.com/user-attachments/assets/e2b244de-ffd4-4fad-a7bb-35748be63eb9" />

