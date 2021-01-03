# notebook
### API for notes management

Приложения предоставляет REST API для CRUD операций над двумя типами сущностей: Note и User.

Примеры запросов:

* GET /api/user/1 - возвращает данные юзера с ID 1
* POST /api/user/1/notes - создает новую заметку для юзера с ID 1
* GET /api/user/1/notes?page=2&size=10 - возвращает заметки пользователя, применяя пагинацию.
