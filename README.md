# Monopoly-Stock
## Техническое задание
***
*Разработать консольное .NET приложение для склада, удовлетворяющее следующим требованиям:*

- Построить иерархию классов, описывающих объекты на складе - паллеты и коробки:

    - Помимо общего набора стандартных свойств (ID, ширина, высота, глубина, вес), паллета может содержать в себе коробки.

    - У коробки должен быть указан срок годности или дата производства. Если указана дата производства, то срок годности вычисляется из даты производства плюс 100 дней.

        - Срок годности и дата производства — это конкретная дата без времени (например, 01.01.2023).

    - Срок годности паллеты вычисляется из наименьшего срока годности коробки, вложенной в паллету. Вес паллеты вычисляется из суммы веса вложенных коробок + 30кг.

    - Объем коробки вычисляется как произведение ширины, высоты и глубины.

    - Объем паллеты вычисляется как сумма объема всех находящихся на ней коробок и произведения ширины, высоты и глубины паллеты.

    - Каждая коробка не должна превышать по размерам паллету (по ширине и глубине).

- Консольное приложение:

    - Получение данных для приложения можно организовать одним из способов:

        - Генерация прямо в приложении

        - Чтение из файла или БД

        - Пользовательский ввод

    - Вывести на экран:

        - Сгруппировать все паллеты по сроку годности, отсортировать по возрастанию срока годности, в каждой группе отсортировать паллеты по весу.

        - 3 паллеты, которые содержат коробки с наибольшим сроком годности, отсортированные по возрастанию объема.

- (Опционально) Покрыть функционал unit-тестами.

- (Очень желательно) Код должен быть написан в соответствии с https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions

- (Совершенно не обязательно) Вместо консольного приложения сделать полноценный пользовательский интерфейс. На оценку решения никак не влияет.
***

**Стек технологий:**
+ .NET 8
+ Entity Framework
+ PostgreSQL 15
+ Spectre.Console
***
Для написания данного консольного приложения было принято решение использовать слоистую архитектуру, так как это позволяет проекту быть масштабируемым, облегчает читаемость кода, структурирует его и делает взаимодействие с базой данных удобным.
Первым делом создаём _2 сущности_ Коробку и Паллет:

![image](https://github.com/user-attachments/assets/5c706488-b3c5-4692-85ed-8ca3877099e5)

![image](https://github.com/user-attachments/assets/21c7b401-505b-4acc-b8b0-e96678f265dc)

и настраиваем в констексте данных связь 1 к многим (В паллете может содержаться много коробок). Настраиваем контекст данных, создаём интерфейсы к сервисам, репозиториям, реализуем их самих (_можно посмотреть в PR_), создаём миграции. Можно сказать, что наша CRUD-база готова(не все CRUD-методы используются в проекте, но т.к. архитектура подразумевает масштабируемость, не вижу ничего плохого в наличии данного функционала).

![image](https://github.com/user-attachments/assets/039dba1e-d4e7-47b3-8c4b-91e01a0d0932)
Убедимся, что в БД у нас всё отобразилось:

![image](https://github.com/user-attachments/assets/e1c1f3fb-17e8-487e-b1b3-39f1c7d41c6b)

Также добавим проект с утилитами для реализации основных задач ТЗ.
***
Рассмотрим саму реализацию консольного приложения:

![image](https://github.com/user-attachments/assets/ebcb4395-1c53-4f94-b86b-06dc41752126)
Нас, встречает незамысловатое меню. Начнём по порядку:

![image](https://github.com/user-attachments/assets/9a6bb9d4-bfd7-42ca-9b93-36e457448a67)
В техническом задании значился пункт, в котором можно было выбрать источник данных, я решил читать данные из файла и заносить их в БД. Нужно указать путь к файлу с коробками, заодно проверим, как поведёт себя программа если подать на вход "абы-какую" строку:

![image](https://github.com/user-attachments/assets/b1f1e5e6-1d9c-4ca0-bda6-fdbc0305334d)
С валидацией всё в порядке, так что дубль 2:

![image](https://github.com/user-attachments/assets/288ee3d0-8e89-4f4f-8c7f-e79ec1b62327)
Проверяем наличие данных в БД:
![image](https://github.com/user-attachments/assets/a45ab913-130c-4487-99a4-1e020cd82a9c)
![image](https://github.com/user-attachments/assets/c1bfb382-77b5-4532-89b1-46321ca5fe33)
Данные совпадают, можно ехать дальше.

_Можно было бы продемонстрировать работу базовых CRUD-операций, но это не самое главное, так что не хочется заострять на этом внимание..._
***
Переходим в пункт "Операции с данными о паллетах":
![image](https://github.com/user-attachments/assets/46b81bcf-c03b-483d-96a8-8a9aaee17681)

Сразу можем заметить пункты, которые значатся в ТЗ:
![image](https://github.com/user-attachments/assets/af5ba6a2-f24d-4b14-ac00-efac9250f3c3)

Осуществим загрузку файла:
![image](https://github.com/user-attachments/assets/568491c8-ef49-4544-9596-492ebbca4c61)
***
Распределим все наши коробки по паллетам:

```
Box 0a8951d6-53f2-46b7-bb41-5feeaa32fed1 added to Pallet 03c5406b-7505-498a-897d-ac0e84ddb3c8
Box 0ac26cac-e0af-451e-9f6e-7406deb16e02 added to Pallet 03c5406b-7505-498a-897d-ac0e84ddb3c8
Box 0b556a24-8fe4-405d-b602-755a07f45931 added to Pallet 03c5406b-7505-498a-897d-ac0e84ddb3c8
Pallet ID: 03c5406b-7505-498a-897d-ac0e84ddb3c8
Dimensions: 20,979998 x 14,259993 x 41,439995
Volume: 27246,15
Weight: 28,41
Expiration Date:
Number of Boxes: 3
Box ID: 0a8951d6-53f2-46b7-bb41-5feeaa32fed1, Width: 17,73, Height: 24,84, Depth: 21,19, Weight: 9,89 Volume: 9286,89, Expiration Date: 13.04.2023
Box ID: 0ac26cac-e0af-451e-9f6e-7406deb16e02, Width: 16,14, Height: 24,95, Depth: 19,28, Weight: 7,87 Volume: 7795,69, Expiration Date: 20.04.2023
Box ID: 0b556a24-8fe4-405d-b602-755a07f45931, Width: 20,03, Height: 26,58, Depth: 19,12, Weight: 10,65 Volume: 10163,57, Expiration Date: 18.04.2023

Box 0b627484-bdad-4d35-9809-fd1db6cdc4df added to Pallet 05f7e0e8-fcca-4ace-87e0-dac32793bb44
Box 0f1a8b3c-da1f-4e4e-9bce-e7af37958cf1 added to Pallet 05f7e0e8-fcca-4ace-87e0-dac32793bb44
Box 123f14b6-c1b5-47ee-9df9-6d5a49f157ca added to Pallet 05f7e0e8-fcca-4ace-87e0-dac32793bb44
Pallet ID: 05f7e0e8-fcca-4ace-87e0-dac32793bb44
Dimensions: 15,340004 x 42,260002 x 30,760004
Volume: 25340,08
Weight: 27,27
Expiration Date:
Number of Boxes: 3
Box ID: 0b627484-bdad-4d35-9809-fd1db6cdc4df, Width: 17,89, Height: 22,76, Depth: 20,34, Weight: 9,03 Volume: 8261,29, Expiration Date: 16.04.2023
Box ID: 0f1a8b3c-da1f-4e4e-9bce-e7af37958cf1, Width: 17,98, Height: 22,67, Depth: 25,45, Weight: 9,79 Volume: 10383,51, Expiration Date: 14.04.2023
Box ID: 123f14b6-c1b5-47ee-9df9-6d5a49f157ca, Width: 18,23, Height: 21,89, Depth: 16,78, Weight: 8,45 Volume: 6695,28, Expiration Date: 11.04.2023

Box 1d443e37-ff1a-4a85-8e9b-8f067780e057 added to Pallet 1ad67e31-ebbe-4e6b-b080-d62ceb0d2cdf
Box 1d69d3c1-0f41-4323-bb77-a5e582b4fe56 added to Pallet 1ad67e31-ebbe-4e6b-b080-d62ceb0d2cdf
Box 257af4dc-d4a7-43be-810a-a3030dab24e4 added to Pallet 1ad67e31-ebbe-4e6b-b080-d62ceb0d2cdf
Box 4229d286-c19f-46f9-b38b-036ff09528c7 added to Pallet 1ad67e31-ebbe-4e6b-b080-d62ceb0d2cdf
Pallet ID: 1ad67e31-ebbe-4e6b-b080-d62ceb0d2cdf
Dimensions: 21,550003 x 10,320007 x 0,6399994
Volume: 39438,2
Weight: 40,870003
Expiration Date:
Number of Boxes: 4
Box ID: 1d443e37-ff1a-4a85-8e9b-8f067780e057, Width: 16,67, Height: 24,53, Depth: 23,12, Weight: 8,72 Volume: 9444,96, Expiration Date: 21.04.2023
Box ID: 1d69d3c1-0f41-4323-bb77-a5e582b4fe56, Width: 20,34, Height: 26,78, Depth: 19,74, Weight: 11,45 Volume: 10690,85, Expiration Date: 16.04.2023
Box ID: 257af4dc-d4a7-43be-810a-a3030dab24e4, Width: 18,06, Height: 25,64, Depth: 22,48, Weight: 9,94 Volume: 10393,46, Expiration Date: 18.04.2023
Box ID: 4229d286-c19f-46f9-b38b-036ff09528c7, Width: 19,64, Height: 20,14, Depth: 22,45, Weight: 10,76 Volume: 8908,93, Expiration Date: 14.04.2023

Box 562f9acd-0158-49d2-909d-7ea33fb839f9 added to Pallet 1b558712-fcdc-4856-91a7-8774d7a41eb1
Box 656bbf22-ecc9-4009-973e-e5506a6cb693 added to Pallet 1b558712-fcdc-4856-91a7-8774d7a41eb1
Box 6583311e-ad85-4f69-97d5-0efdb0d5d13f added to Pallet 1b558712-fcdc-4856-91a7-8774d7a41eb1
Pallet ID: 1b558712-fcdc-4856-91a7-8774d7a41eb1
Dimensions: 27,209993 x 30,659996 x 7,6299963
Volume: 30322,691
Weight: 31,8
Expiration Date:
Number of Boxes: 3
Box ID: 562f9acd-0158-49d2-909d-7ea33fb839f9, Width: 19,34, Height: 21,62, Depth: 24,76, Weight: 11,12 Volume: 10377,94, Expiration Date: 17.04.2023
Box ID: 656bbf22-ecc9-4009-973e-e5506a6cb693, Width: 22,54, Height: 24,42, Depth: 20,84, Weight: 12,14 Volume: 11447,57, Expiration Date: 20.04.2023
Box ID: 6583311e-ad85-4f69-97d5-0efdb0d5d13f, Width: 20,87, Height: 25,54, Depth: 15,94, Weight: 8,54 Volume: 8497,18, Expiration Date: 15.04.2023

Box 6a7a2ae1-f846-46f7-87e2-19073ddca4bd added to Pallet 2d497685-2ab4-4633-8825-f85f23f600bc
Box 6aed251c-ed8c-4532-bf3e-d172f55d04bb added to Pallet 2d497685-2ab4-4633-8825-f85f23f600bc
Box 6ff29e8b-5db0-41be-ac2f-3829483be6a9 added to Pallet 2d497685-2ab4-4633-8825-f85f23f600bc
Box 70251422-53ba-40ed-97ed-5f1436381ff0 added to Pallet 2d497685-2ab4-4633-8825-f85f23f600bc
Pallet ID: 2d497685-2ab4-4633-8825-f85f23f600bc
Dimensions: 29,660004 x 2,0600033 x 28,080002
Volume: 30921,64
Weight: 37,11
Expiration Date:
Number of Boxes: 4
Box ID: 6a7a2ae1-f846-46f7-87e2-19073ddca4bd, Width: 12,34, Height: 25,67, Depth: 14,56, Weight: 5,23 Volume: 4595,57, Expiration Date: 11.04.2023
Box ID: 6aed251c-ed8c-4532-bf3e-d172f55d04bb, Width: 22,69, Height: 21,54, Depth: 19,42, Weight: 11,78 Volume: 9462,57, Expiration Date: 12.04.2023
Box ID: 6ff29e8b-5db0-41be-ac2f-3829483be6a9, Width: 16,49, Height: 24,19, Depth: 18,94, Weight: 8,65 Volume: 7528,94, Expiration Date: 17.04.2023
Box ID: 70251422-53ba-40ed-97ed-5f1436381ff0, Width: 23,18, Height: 21,98, Depth: 18,32, Weight: 11,45 Volume: 9334,56, Expiration Date: 14.04.2023

Box 79523117-4378-4e23-8499-4bf47ee8206a added to Pallet 3a3cbae2-1ecd-4fb9-b769-abc86e1ae467
Box 7da0fa7b-3489-4c50-8151-3a2f783cacf8 added to Pallet 3a3cbae2-1ecd-4fb9-b769-abc86e1ae467
Box 82b6f527-c297-43a2-99d2-370df57538fd added to Pallet 3a3cbae2-1ecd-4fb9-b769-abc86e1ae467
Pallet ID: 3a3cbae2-1ecd-4fb9-b769-abc86e1ae467
Dimensions: 20,899998 x 18,06 x 35,799995
Volume: 29465,771
Weight: 29,89
Expiration Date:
Number of Boxes: 3
Box ID: 79523117-4378-4e23-8499-4bf47ee8206a, Width: 23,14, Height: 25,23, Depth: 21,91, Weight: 11,34 Volume: 12811,58, Expiration Date: 19.04.2023
Box ID: 7da0fa7b-3489-4c50-8151-3a2f783cacf8, Width: 18,54, Height: 23,95, Depth: 24,07, Weight: 10,43 Volume: 10690,95, Expiration Date: 15.04.2023
Box ID: 82b6f527-c297-43a2-99d2-370df57538fd, Width: 15,23, Height: 20,87, Depth: 18,75, Weight: 8,12 Volume: 5963,24, Expiration Date: 18.04.2023

Box 86bb6d9c-36f0-450c-be14-fdfee21355ed added to Pallet 4cbc9105-e5d8-4ef1-a13e-4b4bf447708d
Box 8707ae67-5c15-4bde-9795-6fb0b129053e added to Pallet 4cbc9105-e5d8-4ef1-a13e-4b4bf447708d
Box 97131f3b-f52a-434c-b572-ad8614e3e484 added to Pallet 4cbc9105-e5d8-4ef1-a13e-4b4bf447708d
Pallet ID: 4cbc9105-e5d8-4ef1-a13e-4b4bf447708d
Dimensions: 51,339996 x 19,550001 x 5,289997
Volume: 31793,7
Weight: 29,32
Expiration Date:
Number of Boxes: 3
Box ID: 86bb6d9c-36f0-450c-be14-fdfee21355ed, Width: 14,52, Height: 27,48, Depth: 23,76, Weight: 9,67 Volume: 9485,57, Expiration Date: 12.04.2023
Box ID: 8707ae67-5c15-4bde-9795-6fb0b129053e, Width: 17,34, Height: 22,12, Depth: 24,83, Weight: 9,42 Volume: 9534,22, Expiration Date: 20.04.2023
Box ID: 97131f3b-f52a-434c-b572-ad8614e3e484, Width: 19,72, Height: 27,38, Depth: 23,65, Weight: 10,23 Volume: 12773,91, Expiration Date: 20.04.2023

Box 9e259d5e-819d-4a86-8d15-cd39ffcb24e0 added to Pallet 7193c2ce-322d-47bd-aa9e-ff1170bf086a
Box a1850c0f-c738-455c-880e-61cf2c8c9993 added to Pallet 7193c2ce-322d-47bd-aa9e-ff1170bf086a
Box a539662b-15f5-4ba6-baf7-a7424a2c7bf9 added to Pallet 7193c2ce-322d-47bd-aa9e-ff1170bf086a
Pallet ID: 7193c2ce-322d-47bd-aa9e-ff1170bf086a
Dimensions: 45,310005 x 13,199999 x 22,44
Volume: 24855,47
Weight: 27,86
Expiration Date:
Number of Boxes: 3
Box ID: 9e259d5e-819d-4a86-8d15-cd39ffcb24e0, Width: 18,95, Height: 23,45, Depth: 20,64, Weight: 9,54 Volume: 9137,53, Expiration Date: 19.04.2023
Box ID: a1850c0f-c738-455c-880e-61cf2c8c9993, Width: 21,49, Height: 20,98, Depth: 18,82, Weight: 10,87 Volume: 8496,13, Expiration Date: 13.04.2023
Box ID: a539662b-15f5-4ba6-baf7-a7424a2c7bf9, Width: 14,83, Height: 23,51, Depth: 20,63, Weight: 7,45 Volume: 7221,81, Expiration Date: 16.04.2023

Box a7bcc950-73ba-41f1-abb8-4397318f475f added to Pallet 945b6210-7e95-461e-bd23-c43472377197
Box ade9cebe-5936-4903-a8d8-365fae4a831b added to Pallet 945b6210-7e95-461e-bd23-c43472377197
Box b3f8955c-0bd5-49e1-ae94-ee5f39f6578b added to Pallet 945b6210-7e95-461e-bd23-c43472377197
Pallet ID: 945b6210-7e95-461e-bd23-c43472377197
Dimensions: 1,5699978 x 44,299995 x 53,68
Volume: 27729,55
Weight: 29,91
Expiration Date:
Number of Boxes: 3
Box ID: a7bcc950-73ba-41f1-abb8-4397318f475f, Width: 20,45, Height: 28,34, Depth: 13,79, Weight: 7,67 Volume: 7996,72, Expiration Date: 12.04.2023
Box ID: ade9cebe-5936-4903-a8d8-365fae4a831b, Width: 21,39, Height: 23,87, Depth: 19,65, Weight: 11,26 Volume: 10053,82, Expiration Date: 14.04.2023
Box ID: b3f8955c-0bd5-49e1-ae94-ee5f39f6578b, Width: 21,12, Height: 26,03, Depth: 17,64, Weight: 10,98 Volume: 9679,01, Expiration Date: 19.04.2023

Box bbd74dfe-9425-4bc6-970f-fbb6bfebe94a added to Pallet b0d514a6-b5c4-4c47-a147-20f74e4de17d
Box bbf44c3e-0ee2-4837-b903-3ec03d9daac8 added to Pallet b0d514a6-b5c4-4c47-a147-20f74e4de17d
Box be55f33d-9a37-4c5d-bc8c-9fc05ede9e78 added to Pallet b0d514a6-b5c4-4c47-a147-20f74e4de17d
Pallet ID: b0d514a6-b5c4-4c47-a147-20f74e4de17d
Dimensions: 19,060001 x 16,300003 x 18,230001
Volume: 32974,15
Weight: 30,690002
Expiration Date:
Number of Boxes: 3
Box ID: bbd74dfe-9425-4bc6-970f-fbb6bfebe94a, Width: 19,89, Height: 27,81, Depth: 19,03, Weight: 10,02 Volume: 10510,98, Expiration Date: 21.04.2023
Box ID: bbf44c3e-0ee2-4837-b903-3ec03d9daac8, Width: 22,14, Height: 26,29, Depth: 16,72, Weight: 10,56 Volume: 9735,59, Expiration Date: 15.04.2023
Box ID: be55f33d-9a37-4c5d-bc8c-9fc05ede9e78, Width: 20,95, Height: 28,11, Depth: 21,53, Weight: 10,11 Volume: 12727,58, Expiration Date: 20.04.2023

Box c3698b90-60a5-479e-991a-4d25ca057b4f added to Pallet b376912b-06aa-4b19-98c0-ce7ab7d9975a
Box ccc2552f-9afd-459e-8e34-dc3856bfb6d8 added to Pallet b376912b-06aa-4b19-98c0-ce7ab7d9975a
Box cee2ffcd-d4b1-4fcf-9c11-0636bdd8daf6 added to Pallet b376912b-06aa-4b19-98c0-ce7ab7d9975a
Pallet ID: b376912b-06aa-4b19-98c0-ce7ab7d9975a
Dimensions: 1,289999 x 45,239994 x 8,149999
Volume: 23168,01
Weight: 26,49
Expiration Date:
Number of Boxes: 3
Box ID: c3698b90-60a5-479e-991a-4d25ca057b4f, Width: 18,68, Height: 22,36, Depth: 19,56, Weight: 9,23 Volume: 8218,51, Expiration Date: 16.04.2023
Box ID: ccc2552f-9afd-459e-8e34-dc3856bfb6d8, Width: 17,29, Height: 19,58, Depth: 22,34, Weight: 9,58 Volume: 7598,14, Expiration Date: 13.04.2023
Box ID: cee2ffcd-d4b1-4fcf-9c11-0636bdd8daf6, Width: 16,74, Height: 28,34, Depth: 15,47, Weight: 7,68 Volume: 7351,36, Expiration Date: 15.04.2023

Box d9a37c2a-a23e-4e8d-bbae-7590a4ca3856 added to Pallet b8b1bbc4-00ff-4650-b3a2-9630e49f9927
Box ddef61db-0365-41b4-8183-8bfa368dd1f0 added to Pallet b8b1bbc4-00ff-4650-b3a2-9630e49f9927
Box e30f1f94-64f9-4475-aaf6-4fed914fae8e added to Pallet b8b1bbc4-00ff-4650-b3a2-9630e49f9927
Pallet ID: b8b1bbc4-00ff-4650-b3a2-9630e49f9927
Dimensions: 50,6 x 7,83 x 28,130003
Volume: 28579,379
Weight: 29,439999
Expiration Date:
Number of Boxes: 3
Box ID: d9a37c2a-a23e-4e8d-bbae-7590a4ca3856, Width: 15,35, Height: 23,34, Depth: 20,98, Weight: 8,43 Volume: 7507,89, Expiration Date: 16.04.2023
Box ID: ddef61db-0365-41b4-8183-8bfa368dd1f0, Width: 21,25, Height: 26,67, Depth: 22,21, Weight: 10,78 Volume: 12584,93, Expiration Date: 17.04.2023
Box ID: e30f1f94-64f9-4475-aaf6-4fed914fae8e, Width: 22,87, Height: 21,45, Depth: 17,24, Weight: 10,23 Volume: 8486,56, Expiration Date: 11.04.2023

Box e323cb9d-3c16-43d9-b0c3-659ce624a054 added to Pallet c00d7242-cd50-4573-a0a0-a305e7670e51
Box e613b9c2-36e1-46d7-a5c9-820b4bdd9b37 added to Pallet c00d7242-cd50-4573-a0a0-a305e7670e51
Box e90d3eb1-353c-4db4-9b07-8134c3e0a00e added to Pallet c00d7242-cd50-4573-a0a0-a305e7670e51
Box f2ad035b-41cd-4e02-8307-c983ace93715 added to Pallet c00d7242-cd50-4573-a0a0-a305e7670e51
Pallet ID: c00d7242-cd50-4573-a0a0-a305e7670e51
Dimensions: 14,309998 x 12,189991 x 3,4200058
Volume: 39476,38
Weight: 39,17
Expiration Date:
Number of Boxes: 4
Box ID: e323cb9d-3c16-43d9-b0c3-659ce624a054, Width: 15,43, Height: 24,35, Depth: 20,74, Weight: 6,89 Volume: 7800,26, Expiration Date: 12.04.2023
Box ID: e613b9c2-36e1-46d7-a5c9-820b4bdd9b37, Width: 23,47, Height: 23,09, Depth: 21,32, Weight: 12,34 Volume: 11522,97, Expiration Date: 18.04.2023
Box ID: e90d3eb1-353c-4db4-9b07-8134c3e0a00e, Width: 14,79, Height: 28,01, Depth: 21,43, Weight: 8,02 Volume: 8892,47, Expiration Date: 19.04.2023
Box ID: f2ad035b-41cd-4e02-8307-c983ace93715, Width: 23,71, Height: 25,45, Depth: 18,64, Weight: 11,92 Volume: 11260,68, Expiration Date: 19.04.2023

Box f9c8b640-2339-4fd0-a79d-a7aee5f0f58c added to Pallet d579645c-1ef5-4ccf-be11-b198ea763922
Box ff4dea59-ab3a-4049-91f4-eac3d95ab55d added to Pallet d579645c-1ef5-4ccf-be11-b198ea763922
Box ffff2dc4-216e-4488-b50b-6a3730c383d6 added to Pallet d579645c-1ef5-4ccf-be11-b198ea763922
Pallet ID: 03c5406b-7505-498a-897d-ac0e84ddb3c8
Dimensions: 20,979998 x 14,259993 x 41,439995
Volume: 39643,945
Weight: 58,41
Expiration Date: 13.04.2023
Number of Boxes: 3
Box ID: 0a8951d6-53f2-46b7-bb41-5feeaa32fed1, Width: 17,73, Height: 24,84, Depth: 21,19, Weight: 9,89 Volume: 9286,89, Expiration Date: 13.04.2023
Box ID: 0ac26cac-e0af-451e-9f6e-7406deb16e02, Width: 16,14, Height: 24,95, Depth: 19,28, Weight: 7,87 Volume: 7795,69, Expiration Date: 20.04.2023
Box ID: 0b556a24-8fe4-405d-b602-755a07f45931, Width: 20,03, Height: 26,58, Depth: 19,12, Weight: 10,65 Volume: 10163,57, Expiration Date: 18.04.2023

Pallet ID: 05f7e0e8-fcca-4ace-87e0-dac32793bb44
Dimensions: 15,340004 x 42,260002 x 30,760004
Volume: 45280,83
Weight: 57,27
Expiration Date: 11.04.2023
Number of Boxes: 3
Box ID: 0b627484-bdad-4d35-9809-fd1db6cdc4df, Width: 17,89, Height: 22,76, Depth: 20,34, Weight: 9,03 Volume: 8261,29, Expiration Date: 16.04.2023
Box ID: 0f1a8b3c-da1f-4e4e-9bce-e7af37958cf1, Width: 17,98, Height: 22,67, Depth: 25,45, Weight: 9,79 Volume: 10383,51, Expiration Date: 14.04.2023
Box ID: 123f14b6-c1b5-47ee-9df9-6d5a49f157ca, Width: 18,23, Height: 21,89, Depth: 16,78, Weight: 8,45 Volume: 6695,28, Expiration Date: 11.04.2023

Pallet ID: 1ad67e31-ebbe-4e6b-b080-d62ceb0d2cdf
Dimensions: 21,550003 x 10,320007 x 0,6399994
Volume: 39580,53
Weight: 70,87
Expiration Date: 14.04.2023
Number of Boxes: 4
Box ID: 1d443e37-ff1a-4a85-8e9b-8f067780e057, Width: 16,67, Height: 24,53, Depth: 23,12, Weight: 8,72 Volume: 9444,96, Expiration Date: 21.04.2023
Box ID: 1d69d3c1-0f41-4323-bb77-a5e582b4fe56, Width: 20,34, Height: 26,78, Depth: 19,74, Weight: 11,45 Volume: 10690,85, Expiration Date: 16.04.2023
Box ID: 257af4dc-d4a7-43be-810a-a3030dab24e4, Width: 18,06, Height: 25,64, Depth: 22,48, Weight: 9,94 Volume: 10393,46, Expiration Date: 18.04.2023
Box ID: 4229d286-c19f-46f9-b38b-036ff09528c7, Width: 19,64, Height: 20,14, Depth: 22,45, Weight: 10,76 Volume: 8908,93, Expiration Date: 14.04.2023

Pallet ID: 1b558712-fcdc-4856-91a7-8774d7a41eb1
Dimensions: 27,209993 x 30,659996 x 7,6299963
Volume: 36688,08
Weight: 61,8
Expiration Date: 15.04.2023
Number of Boxes: 3
Box ID: 562f9acd-0158-49d2-909d-7ea33fb839f9, Width: 19,34, Height: 21,62, Depth: 24,76, Weight: 11,12 Volume: 10377,94, Expiration Date: 17.04.2023
Box ID: 656bbf22-ecc9-4009-973e-e5506a6cb693, Width: 22,54, Height: 24,42, Depth: 20,84, Weight: 12,14 Volume: 11447,57, Expiration Date: 20.04.2023
Box ID: 6583311e-ad85-4f69-97d5-0efdb0d5d13f, Width: 20,87, Height: 25,54, Depth: 15,94, Weight: 8,54 Volume: 8497,18, Expiration Date: 15.04.2023

Pallet ID: 2d497685-2ab4-4633-8825-f85f23f600bc
Dimensions: 29,660004 x 2,0600033 x 28,080002
Volume: 32637,32
Weight: 67,11
Expiration Date: 11.04.2023
Number of Boxes: 4
Box ID: 6a7a2ae1-f846-46f7-87e2-19073ddca4bd, Width: 12,34, Height: 25,67, Depth: 14,56, Weight: 5,23 Volume: 4595,57, Expiration Date: 11.04.2023
Box ID: 6aed251c-ed8c-4532-bf3e-d172f55d04bb, Width: 22,69, Height: 21,54, Depth: 19,42, Weight: 11,78 Volume: 9462,57, Expiration Date: 12.04.2023
Box ID: 6ff29e8b-5db0-41be-ac2f-3829483be6a9, Width: 16,49, Height: 24,19, Depth: 18,94, Weight: 8,65 Volume: 7528,94, Expiration Date: 17.04.2023
Box ID: 70251422-53ba-40ed-97ed-5f1436381ff0, Width: 23,18, Height: 21,98, Depth: 18,32, Weight: 11,45 Volume: 9334,56, Expiration Date: 14.04.2023

Pallet ID: 3a3cbae2-1ecd-4fb9-b769-abc86e1ae467
Dimensions: 20,899998 x 18,06 x 35,799995
Volume: 42978,62
Weight: 59,89
Expiration Date: 15.04.2023
Number of Boxes: 3
Box ID: 79523117-4378-4e23-8499-4bf47ee8206a, Width: 23,14, Height: 25,23, Depth: 21,91, Weight: 11,34 Volume: 12811,58, Expiration Date: 19.04.2023
Box ID: 7da0fa7b-3489-4c50-8151-3a2f783cacf8, Width: 18,54, Height: 23,95, Depth: 24,07, Weight: 10,43 Volume: 10690,95, Expiration Date: 15.04.2023
Box ID: 82b6f527-c297-43a2-99d2-370df57538fd, Width: 15,23, Height: 20,87, Depth: 18,75, Weight: 8,12 Volume: 5963,24, Expiration Date: 18.04.2023

Pallet ID: 4cbc9105-e5d8-4ef1-a13e-4b4bf447708d
Dimensions: 51,339996 x 19,550001 x 5,289997
Volume: 37103,254
Weight: 59,32
Expiration Date: 12.04.2023
Number of Boxes: 3
Box ID: 86bb6d9c-36f0-450c-be14-fdfee21355ed, Width: 14,52, Height: 27,48, Depth: 23,76, Weight: 9,67 Volume: 9485,57, Expiration Date: 12.04.2023
Box ID: 8707ae67-5c15-4bde-9795-6fb0b129053e, Width: 17,34, Height: 22,12, Depth: 24,83, Weight: 9,42 Volume: 9534,22, Expiration Date: 20.04.2023
Box ID: 97131f3b-f52a-434c-b572-ad8614e3e484, Width: 19,72, Height: 27,38, Depth: 23,65, Weight: 10,23 Volume: 12773,91, Expiration Date: 20.04.2023

Pallet ID: 7193c2ce-322d-47bd-aa9e-ff1170bf086a
Dimensions: 45,310005 x 13,199999 x 22,44
Volume: 38276,656
Weight: 57,86
Expiration Date: 13.04.2023
Number of Boxes: 3
Box ID: 9e259d5e-819d-4a86-8d15-cd39ffcb24e0, Width: 18,95, Height: 23,45, Depth: 20,64, Weight: 9,54 Volume: 9137,53, Expiration Date: 19.04.2023
Box ID: a1850c0f-c738-455c-880e-61cf2c8c9993, Width: 21,49, Height: 20,98, Depth: 18,82, Weight: 10,87 Volume: 8496,13, Expiration Date: 13.04.2023
Box ID: a539662b-15f5-4ba6-baf7-a7424a2c7bf9, Width: 14,83, Height: 23,51, Depth: 20,63, Weight: 7,45 Volume: 7221,81, Expiration Date: 16.04.2023

Pallet ID: 945b6210-7e95-461e-bd23-c43472377197
Dimensions: 1,5699978 x 44,299995 x 53,68
Volume: 31463,043
Weight: 59,91
Expiration Date: 12.04.2023
Number of Boxes: 3
Box ID: a7bcc950-73ba-41f1-abb8-4397318f475f, Width: 20,45, Height: 28,34, Depth: 13,79, Weight: 7,67 Volume: 7996,72, Expiration Date: 12.04.2023
Box ID: ade9cebe-5936-4903-a8d8-365fae4a831b, Width: 21,39, Height: 23,87, Depth: 19,65, Weight: 11,26 Volume: 10053,82, Expiration Date: 14.04.2023
Box ID: b3f8955c-0bd5-49e1-ae94-ee5f39f6578b, Width: 21,12, Height: 26,03, Depth: 17,64, Weight: 10,98 Volume: 9679,01, Expiration Date: 19.04.2023

Pallet ID: b0d514a6-b5c4-4c47-a147-20f74e4de17d
Dimensions: 19,060001 x 16,300003 x 18,230001
Volume: 38637,81
Weight: 60,690002
Expiration Date: 15.04.2023
Number of Boxes: 3
Box ID: bbd74dfe-9425-4bc6-970f-fbb6bfebe94a, Width: 19,89, Height: 27,81, Depth: 19,03, Weight: 10,02 Volume: 10510,98, Expiration Date: 21.04.2023
Box ID: bbf44c3e-0ee2-4837-b903-3ec03d9daac8, Width: 22,14, Height: 26,29, Depth: 16,72, Weight: 10,56 Volume: 9735,59, Expiration Date: 15.04.2023
Box ID: be55f33d-9a37-4c5d-bc8c-9fc05ede9e78, Width: 20,95, Height: 28,11, Depth: 21,53, Weight: 10,11 Volume: 12727,58, Expiration Date: 20.04.2023

Pallet ID: b376912b-06aa-4b19-98c0-ce7ab7d9975a
Dimensions: 1,289999 x 45,239994 x 8,149999
Volume: 23643,64
Weight: 56,489998
Expiration Date: 13.04.2023
Number of Boxes: 3
Box ID: c3698b90-60a5-479e-991a-4d25ca057b4f, Width: 18,68, Height: 22,36, Depth: 19,56, Weight: 9,23 Volume: 8218,51, Expiration Date: 16.04.2023
Box ID: ccc2552f-9afd-459e-8e34-dc3856bfb6d8, Width: 17,29, Height: 19,58, Depth: 22,34, Weight: 9,58 Volume: 7598,14, Expiration Date: 13.04.2023
Box ID: cee2ffcd-d4b1-4fcf-9c11-0636bdd8daf6, Width: 16,74, Height: 28,34, Depth: 15,47, Weight: 7,68 Volume: 7351,36, Expiration Date: 15.04.2023

Pallet ID: b8b1bbc4-00ff-4650-b3a2-9630e49f9927
Dimensions: 50,6 x 7,83 x 28,130003
Volume: 39724,43
Weight: 59,44
Expiration Date: 11.04.2023
Number of Boxes: 3
Box ID: d9a37c2a-a23e-4e8d-bbae-7590a4ca3856, Width: 15,35, Height: 23,34, Depth: 20,98, Weight: 8,43 Volume: 7507,89, Expiration Date: 16.04.2023
Box ID: ddef61db-0365-41b4-8183-8bfa368dd1f0, Width: 21,25, Height: 26,67, Depth: 22,21, Weight: 10,78 Volume: 12584,93, Expiration Date: 17.04.2023
Box ID: e30f1f94-64f9-4475-aaf6-4fed914fae8e, Width: 22,87, Height: 21,45, Depth: 17,24, Weight: 10,23 Volume: 8486,56, Expiration Date: 11.04.2023

Pallet ID: c00d7242-cd50-4573-a0a0-a305e7670e51
Dimensions: 14,309998 x 12,189991 x 3,4200058
Volume: 40072,96
Weight: 69,17
Expiration Date: 12.04.2023
Number of Boxes: 4
Box ID: e323cb9d-3c16-43d9-b0c3-659ce624a054, Width: 15,43, Height: 24,35, Depth: 20,74, Weight: 6,89 Volume: 7800,26, Expiration Date: 12.04.2023
Box ID: e613b9c2-36e1-46d7-a5c9-820b4bdd9b37, Width: 23,47, Height: 23,09, Depth: 21,32, Weight: 12,34 Volume: 11522,97, Expiration Date: 18.04.2023
Box ID: e90d3eb1-353c-4db4-9b07-8134c3e0a00e, Width: 14,79, Height: 28,01, Depth: 21,43, Weight: 8,02 Volume: 8892,47, Expiration Date: 19.04.2023
Box ID: f2ad035b-41cd-4e02-8307-c983ace93715, Width: 23,71, Height: 25,45, Depth: 18,64, Weight: 11,92 Volume: 11260,68, Expiration Date: 19.04.2023

Pallet ID: d579645c-1ef5-4ccf-be11-b198ea763922
Dimensions: 18,400003 x 38,809998 x 21,469995
Volume: 47029,254
Weight: 60,14
Expiration Date: 13.04.2023
Number of Boxes: 3
Box ID: f9c8b640-2339-4fd0-a79d-a7aee5f0f58c, Width: 19,25, Height: 27,65, Depth: 20,52, Weight: 9,34 Volume: 10971,02, Expiration Date: 17.04.2023
Box ID: ff4dea59-ab3a-4049-91f4-eac3d95ab55d, Width: 16,98, Height: 26,18, Depth: 18,53, Weight: 9,12 Volume: 8206,14, Expiration Date: 13.04.2023
Box ID: ffff2dc4-216e-4488-b50b-6a3730c383d6, Width: 21,78, Height: 25,17, Depth: 22,87, Weight: 11,68 Volume: 12520,28, Expiration Date: 17.04.2023

Произошла ошибка: Sequence contains no elements

```
Читать логи вряд ли кому-то интересно, поэтому пробегусь по основным моментам: прошу обратить внимание на вывод данных о весе, объёме и сроке годности паллеты:

>Срок годности паллеты вычисляется из наименьшего срока годности коробки, вложенной в паллету. Вес паллеты вычисляется из суммы веса вложенных коробок + 30кг. Объем паллеты вычисляется как сумма объема всех находящихся на ней коробок и произведения ширины, высоты и глубины паллеты.

Если вдруг смутила ошибка, она говорит о том, что коробки закончились, а пустые паллеты остались. С точки зрения функционала кода всё корректно.

***
Выполним группировку и сортировку паллетов:

![image](https://github.com/user-attachments/assets/0dedbbb2-50f4-4487-878c-e2acc706c2e6)

Снова немножко логов:

```
Group Expiration Date: 11.04.2023
Pallet ID: 05f7e0e8-fcca-4ace-87e0-dac32793bb44
Dimensions: 15,340004 x 42,260002 x 30,760004
Volume: 45280,83
Weight: 57,27
Expiration Date: 11.04.2023
Number of Boxes: 3
Box ID: 0b627484-bdad-4d35-9809-fd1db6cdc4df, Width: 17,89, Height: 22,76, Depth: 20,34, Weight: 9,03 Volume: 8261,29, Expiration Date: 16.04.2023
Box ID: 0f1a8b3c-da1f-4e4e-9bce-e7af37958cf1, Width: 17,98, Height: 22,67, Depth: 25,45, Weight: 9,79 Volume: 10383,51, Expiration Date: 14.04.2023
Box ID: 123f14b6-c1b5-47ee-9df9-6d5a49f157ca, Width: 18,23, Height: 21,89, Depth: 16,78, Weight: 8,45 Volume: 6695,28, Expiration Date: 11.04.2023

Pallet ID: b8b1bbc4-00ff-4650-b3a2-9630e49f9927
Dimensions: 50,6 x 7,83 x 28,130003
Volume: 39724,43
Weight: 59,44
Expiration Date: 11.04.2023
Number of Boxes: 3
Box ID: d9a37c2a-a23e-4e8d-bbae-7590a4ca3856, Width: 15,35, Height: 23,34, Depth: 20,98, Weight: 8,43 Volume: 7507,89, Expiration Date: 16.04.2023
Box ID: ddef61db-0365-41b4-8183-8bfa368dd1f0, Width: 21,25, Height: 26,67, Depth: 22,21, Weight: 10,78 Volume: 12584,93, Expiration Date: 17.04.2023
Box ID: e30f1f94-64f9-4475-aaf6-4fed914fae8e, Width: 22,87, Height: 21,45, Depth: 17,24, Weight: 10,23 Volume: 8486,56, Expiration Date: 11.04.2023

Pallet ID: 2d497685-2ab4-4633-8825-f85f23f600bc
Dimensions: 29,660004 x 2,0600033 x 28,080002
Volume: 32637,32
Weight: 67,11
Expiration Date: 11.04.2023
Number of Boxes: 4
Box ID: 6a7a2ae1-f846-46f7-87e2-19073ddca4bd, Width: 12,34, Height: 25,67, Depth: 14,56, Weight: 5,23 Volume: 4595,57, Expiration Date: 11.04.2023
Box ID: 6aed251c-ed8c-4532-bf3e-d172f55d04bb, Width: 22,69, Height: 21,54, Depth: 19,42, Weight: 11,78 Volume: 9462,57, Expiration Date: 12.04.2023
Box ID: 6ff29e8b-5db0-41be-ac2f-3829483be6a9, Width: 16,49, Height: 24,19, Depth: 18,94, Weight: 8,65 Volume: 7528,94, Expiration Date: 17.04.2023
Box ID: 70251422-53ba-40ed-97ed-5f1436381ff0, Width: 23,18, Height: 21,98, Depth: 18,32, Weight: 11,45 Volume: 9334,56, Expiration Date: 14.04.2023

Group Expiration Date: 12.04.2023
Pallet ID: 4cbc9105-e5d8-4ef1-a13e-4b4bf447708d
Dimensions: 51,339996 x 19,550001 x 5,289997
Volume: 37103,254
Weight: 59,32
Expiration Date: 12.04.2023
Number of Boxes: 3
Box ID: 86bb6d9c-36f0-450c-be14-fdfee21355ed, Width: 14,52, Height: 27,48, Depth: 23,76, Weight: 9,67 Volume: 9485,57, Expiration Date: 12.04.2023
Box ID: 8707ae67-5c15-4bde-9795-6fb0b129053e, Width: 17,34, Height: 22,12, Depth: 24,83, Weight: 9,42 Volume: 9534,22, Expiration Date: 20.04.2023
Box ID: 97131f3b-f52a-434c-b572-ad8614e3e484, Width: 19,72, Height: 27,38, Depth: 23,65, Weight: 10,23 Volume: 12773,91, Expiration Date: 20.04.2023

Pallet ID: 945b6210-7e95-461e-bd23-c43472377197
Dimensions: 1,5699978 x 44,299995 x 53,68
Volume: 31463,043
Weight: 59,91
Expiration Date: 12.04.2023
Number of Boxes: 3
Box ID: a7bcc950-73ba-41f1-abb8-4397318f475f, Width: 20,45, Height: 28,34, Depth: 13,79, Weight: 7,67 Volume: 7996,72, Expiration Date: 12.04.2023
Box ID: ade9cebe-5936-4903-a8d8-365fae4a831b, Width: 21,39, Height: 23,87, Depth: 19,65, Weight: 11,26 Volume: 10053,82, Expiration Date: 14.04.2023
Box ID: b3f8955c-0bd5-49e1-ae94-ee5f39f6578b, Width: 21,12, Height: 26,03, Depth: 17,64, Weight: 10,98 Volume: 9679,01, Expiration Date: 19.04.2023

Pallet ID: c00d7242-cd50-4573-a0a0-a305e7670e51
Dimensions: 14,309998 x 12,189991 x 3,4200058
Volume: 40072,96
Weight: 69,17
Expiration Date: 12.04.2023
Number of Boxes: 4
Box ID: e323cb9d-3c16-43d9-b0c3-659ce624a054, Width: 15,43, Height: 24,35, Depth: 20,74, Weight: 6,89 Volume: 7800,26, Expiration Date: 12.04.2023
Box ID: e613b9c2-36e1-46d7-a5c9-820b4bdd9b37, Width: 23,47, Height: 23,09, Depth: 21,32, Weight: 12,34 Volume: 11522,97, Expiration Date: 18.04.2023
Box ID: e90d3eb1-353c-4db4-9b07-8134c3e0a00e, Width: 14,79, Height: 28,01, Depth: 21,43, Weight: 8,02 Volume: 8892,47, Expiration Date: 19.04.2023
Box ID: f2ad035b-41cd-4e02-8307-c983ace93715, Width: 23,71, Height: 25,45, Depth: 18,64, Weight: 11,92 Volume: 11260,68, Expiration Date: 19.04.2023

Group Expiration Date: 13.04.2023
Pallet ID: b376912b-06aa-4b19-98c0-ce7ab7d9975a
Dimensions: 1,289999 x 45,239994 x 8,149999
Volume: 23643,64
Weight: 56,489998
Expiration Date: 13.04.2023
Number of Boxes: 3
Box ID: c3698b90-60a5-479e-991a-4d25ca057b4f, Width: 18,68, Height: 22,36, Depth: 19,56, Weight: 9,23 Volume: 8218,51, Expiration Date: 16.04.2023
Box ID: ccc2552f-9afd-459e-8e34-dc3856bfb6d8, Width: 17,29, Height: 19,58, Depth: 22,34, Weight: 9,58 Volume: 7598,14, Expiration Date: 13.04.2023
Box ID: cee2ffcd-d4b1-4fcf-9c11-0636bdd8daf6, Width: 16,74, Height: 28,34, Depth: 15,47, Weight: 7,68 Volume: 7351,36, Expiration Date: 15.04.2023

Pallet ID: 7193c2ce-322d-47bd-aa9e-ff1170bf086a
Dimensions: 45,310005 x 13,199999 x 22,44
Volume: 38276,656
Weight: 57,86
Expiration Date: 13.04.2023
Number of Boxes: 3
Box ID: 9e259d5e-819d-4a86-8d15-cd39ffcb24e0, Width: 18,95, Height: 23,45, Depth: 20,64, Weight: 9,54 Volume: 9137,53, Expiration Date: 19.04.2023
Box ID: a1850c0f-c738-455c-880e-61cf2c8c9993, Width: 21,49, Height: 20,98, Depth: 18,82, Weight: 10,87 Volume: 8496,13, Expiration Date: 13.04.2023
Box ID: a539662b-15f5-4ba6-baf7-a7424a2c7bf9, Width: 14,83, Height: 23,51, Depth: 20,63, Weight: 7,45 Volume: 7221,81, Expiration Date: 16.04.2023

Pallet ID: 03c5406b-7505-498a-897d-ac0e84ddb3c8
Dimensions: 20,979998 x 14,259993 x 41,439995
Volume: 39643,945
Weight: 58,41
Expiration Date: 13.04.2023
Number of Boxes: 3
Box ID: 0a8951d6-53f2-46b7-bb41-5feeaa32fed1, Width: 17,73, Height: 24,84, Depth: 21,19, Weight: 9,89 Volume: 9286,89, Expiration Date: 13.04.2023
Box ID: 0ac26cac-e0af-451e-9f6e-7406deb16e02, Width: 16,14, Height: 24,95, Depth: 19,28, Weight: 7,87 Volume: 7795,69, Expiration Date: 20.04.2023
Box ID: 0b556a24-8fe4-405d-b602-755a07f45931, Width: 20,03, Height: 26,58, Depth: 19,12, Weight: 10,65 Volume: 10163,57, Expiration Date: 18.04.2023

Pallet ID: d579645c-1ef5-4ccf-be11-b198ea763922
Dimensions: 18,400003 x 38,809998 x 21,469995
Volume: 47029,254
Weight: 60,14
Expiration Date: 13.04.2023
Number of Boxes: 3
Box ID: f9c8b640-2339-4fd0-a79d-a7aee5f0f58c, Width: 19,25, Height: 27,65, Depth: 20,52, Weight: 9,34 Volume: 10971,02, Expiration Date: 17.04.2023
Box ID: ff4dea59-ab3a-4049-91f4-eac3d95ab55d, Width: 16,98, Height: 26,18, Depth: 18,53, Weight: 9,12 Volume: 8206,14, Expiration Date: 13.04.2023
Box ID: ffff2dc4-216e-4488-b50b-6a3730c383d6, Width: 21,78, Height: 25,17, Depth: 22,87, Weight: 11,68 Volume: 12520,28, Expiration Date: 17.04.2023

Group Expiration Date: 14.04.2023
Pallet ID: 1ad67e31-ebbe-4e6b-b080-d62ceb0d2cdf
Dimensions: 21,550003 x 10,320007 x 0,6399994
Volume: 39580,53
Weight: 70,87
Expiration Date: 14.04.2023
Number of Boxes: 4
Box ID: 1d443e37-ff1a-4a85-8e9b-8f067780e057, Width: 16,67, Height: 24,53, Depth: 23,12, Weight: 8,72 Volume: 9444,96, Expiration Date: 21.04.2023
Box ID: 1d69d3c1-0f41-4323-bb77-a5e582b4fe56, Width: 20,34, Height: 26,78, Depth: 19,74, Weight: 11,45 Volume: 10690,85, Expiration Date: 16.04.2023
Box ID: 257af4dc-d4a7-43be-810a-a3030dab24e4, Width: 18,06, Height: 25,64, Depth: 22,48, Weight: 9,94 Volume: 10393,46, Expiration Date: 18.04.2023
Box ID: 4229d286-c19f-46f9-b38b-036ff09528c7, Width: 19,64, Height: 20,14, Depth: 22,45, Weight: 10,76 Volume: 8908,93, Expiration Date: 14.04.2023

Group Expiration Date: 15.04.2023
Pallet ID: 3a3cbae2-1ecd-4fb9-b769-abc86e1ae467
Dimensions: 20,899998 x 18,06 x 35,799995
Volume: 42978,62
Weight: 59,89
Expiration Date: 15.04.2023
Number of Boxes: 3
Box ID: 79523117-4378-4e23-8499-4bf47ee8206a, Width: 23,14, Height: 25,23, Depth: 21,91, Weight: 11,34 Volume: 12811,58, Expiration Date: 19.04.2023
Box ID: 7da0fa7b-3489-4c50-8151-3a2f783cacf8, Width: 18,54, Height: 23,95, Depth: 24,07, Weight: 10,43 Volume: 10690,95, Expiration Date: 15.04.2023
Box ID: 82b6f527-c297-43a2-99d2-370df57538fd, Width: 15,23, Height: 20,87, Depth: 18,75, Weight: 8,12 Volume: 5963,24, Expiration Date: 18.04.2023

Pallet ID: b0d514a6-b5c4-4c47-a147-20f74e4de17d
Dimensions: 19,060001 x 16,300003 x 18,230001
Volume: 38637,81
Weight: 60,690002
Expiration Date: 15.04.2023
Number of Boxes: 3
Box ID: bbd74dfe-9425-4bc6-970f-fbb6bfebe94a, Width: 19,89, Height: 27,81, Depth: 19,03, Weight: 10,02 Volume: 10510,98, Expiration Date: 21.04.2023
Box ID: bbf44c3e-0ee2-4837-b903-3ec03d9daac8, Width: 22,14, Height: 26,29, Depth: 16,72, Weight: 10,56 Volume: 9735,59, Expiration Date: 15.04.2023
Box ID: be55f33d-9a37-4c5d-bc8c-9fc05ede9e78, Width: 20,95, Height: 28,11, Depth: 21,53, Weight: 10,11 Volume: 12727,58, Expiration Date: 20.04.2023

Pallet ID: 1b558712-fcdc-4856-91a7-8774d7a41eb1
Dimensions: 27,209993 x 30,659996 x 7,6299963
Volume: 36688,08
Weight: 61,8
Expiration Date: 15.04.2023
Number of Boxes: 3
Box ID: 562f9acd-0158-49d2-909d-7ea33fb839f9, Width: 19,34, Height: 21,62, Depth: 24,76, Weight: 11,12 Volume: 10377,94, Expiration Date: 17.04.2023
Box ID: 656bbf22-ecc9-4009-973e-e5506a6cb693, Width: 22,54, Height: 24,42, Depth: 20,84, Weight: 12,14 Volume: 11447,57, Expiration Date: 20.04.2023
Box ID: 6583311e-ad85-4f69-97d5-0efdb0d5d13f, Width: 20,87, Height: 25,54, Depth: 15,94, Weight: 8,54 Volume: 8497,18, Expiration Date: 15.04.2023
```
На этот раз обратим внимание на группы и порядок расположения паллетов в зависимости от их веса и срока годности:

>- Сгруппировать все паллеты по сроку годности, отсортировать по возрастанию срока годности, в каждой группе отсортировать паллеты по весу.
***

Что ж заключительный пункт:

![image](https://github.com/user-attachments/assets/7a977624-a22d-41a4-ac2e-b9aacb12abbb)

```
Pallet ID: 1b558712-fcdc-4856-91a7-8774d7a41eb1
Dimensions: 27,209993 x 30,659996 x 7,6299963
Volume: 36688,08
Weight: 61,8
Expiration Date: 15.04.2023
Number of Boxes: 3
Box ID: 562f9acd-0158-49d2-909d-7ea33fb839f9, Width: 19,34, Height: 21,62, Depth: 24,76, Weight: 11,12 Volume: 10377,94, Expiration Date: 17.04.2023
Box ID: 656bbf22-ecc9-4009-973e-e5506a6cb693, Width: 22,54, Height: 24,42, Depth: 20,84, Weight: 12,14 Volume: 11447,57, Expiration Date: 20.04.2023
Box ID: 6583311e-ad85-4f69-97d5-0efdb0d5d13f, Width: 20,87, Height: 25,54, Depth: 15,94, Weight: 8,54 Volume: 8497,18, Expiration Date: 15.04.2023

Pallet ID: b0d514a6-b5c4-4c47-a147-20f74e4de17d
Dimensions: 19,060001 x 16,300003 x 18,230001
Volume: 38637,81
Weight: 60,690002
Expiration Date: 15.04.2023
Number of Boxes: 3
Box ID: bbd74dfe-9425-4bc6-970f-fbb6bfebe94a, Width: 19,89, Height: 27,81, Depth: 19,03, Weight: 10,02 Volume: 10510,98, Expiration Date: 21.04.2023
Box ID: bbf44c3e-0ee2-4837-b903-3ec03d9daac8, Width: 22,14, Height: 26,29, Depth: 16,72, Weight: 10,56 Volume: 9735,59, Expiration Date: 15.04.2023
Box ID: be55f33d-9a37-4c5d-bc8c-9fc05ede9e78, Width: 20,95, Height: 28,11, Depth: 21,53, Weight: 10,11 Volume: 12727,58, Expiration Date: 20.04.2023

Pallet ID: 3a3cbae2-1ecd-4fb9-b769-abc86e1ae467
Dimensions: 20,899998 x 18,06 x 35,799995
Volume: 42978,62
Weight: 59,89
Expiration Date: 15.04.2023
Number of Boxes: 3
Box ID: 79523117-4378-4e23-8499-4bf47ee8206a, Width: 23,14, Height: 25,23, Depth: 21,91, Weight: 11,34 Volume: 12811,58, Expiration Date: 19.04.2023
Box ID: 7da0fa7b-3489-4c50-8151-3a2f783cacf8, Width: 18,54, Height: 23,95, Depth: 24,07, Weight: 10,43 Volume: 10690,95, Expiration Date: 15.04.2023
Box ID: 82b6f527-c297-43a2-99d2-370df57538fd, Width: 15,23, Height: 20,87, Depth: 18,75, Weight: 8,12 Volume: 5963,24, Expiration Date: 18.04.2023
```
***
На этом всё, можно со спокойной душой нажать кнопку "Выход"

![image](https://github.com/user-attachments/assets/bbfc7aee-ef60-49cf-9d9f-dd2e1ff4d097)

Буду рад любой обратной связи по проекту, понимаю, что не исключено наличие недочётов, возоможно и "костылей", но проект открыт для конструктивной критики!
Заранее спасибо!







