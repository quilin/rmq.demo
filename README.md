# rmq.demo

### Для локального запуска:

1. В корне репозитория выполнить команду `docker compose up -d`, которая запустит ElasticSearch и RabbitMQ
2. Для сборки и запуска приложения нужно установить [dotnet 6.0](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) (sdk и runtime)
3. В корне репозитория выполнить команды `dotnet restore` `dotnet build` `dotnet run --project rmq.demo`

При успешном запуске приложение откроет браузер на странице своей документации.
