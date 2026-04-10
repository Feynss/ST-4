Console.WriteLine("=== Bug Workflow Demo ===\n");

Console.WriteLine("--- Сценарий 1: Обычный фикс ---");
var bug1 = new Bug("Странится авторизации крашится при пустом пароле");
bug1.Submit();
bug1.SendToFix();
bug1.FixDone();
bug1.VerifyOk();
Console.WriteLine(bug1 + "\n");

Console.WriteLine("--- Сценарий 2: Не баг ---");
var bug2 = new Bug("Цвет кнопки не соответствует макету");
bug2.Submit();
bug2.MarkNotABug();
Console.WriteLine(bug2 + "\n");

Console.WriteLine("--- Сценарий 3: Необходимы больше данных -> повторный рабор -> фикс ---");
var bug3 = new Bug("Приложение тормозит");
bug3.Submit();
bug3.NeedMoreInfo();
bug3.ReturnToTriage();
bug3.SendToFix();
bug3.FixDone();
bug3.VerifyFail();
bug3.ReturnToTriage();
bug3.SendToFix();
bug3.FixDone();
bug3.VerifyOk();
Console.WriteLine(bug3 + "\n");

Console.WriteLine("--- Scenario 4: Закрыть -> переоткрыть ---");
var bug4 = new Bug("Нулевая ссылка при старте программы");
bug4.Submit();
bug4.SendToFix();
bug4.FixDone();
bug4.VerifyOk();
bug4.Reopen();
bug4.ReturnToTriage();
bug4.SendToFix();
bug4.FixDone();
bug4.ProblemSolved();
Console.WriteLine(bug4 + "\n");

Console.WriteLine("=== Все сценарии пройдены успешно ===");
