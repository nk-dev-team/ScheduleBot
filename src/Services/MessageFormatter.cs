using System;
using System.Text;
using System.Text.Json;


namespace Services;

public static class MessageFormatter
{
    public static string FormatPrompt(string input)
    {
        return 
            $$$"""
                Твоя задача — створити список уроків у форматі JSON лише з тих даних які ввів користувач. 

                Формат відповіді:
                {{
                    "Schedules": [
                        {
                        "Day": день тижня з великої літери. Тобто один із варіантів: Понеділок, Вівторок, Середа, Четвер, П'ятниця, Субота, Неділя
                        "Class": клас учнів, до якого належить урок. Він позначається як число від 1 до 11 (тобто рік навчання) та літера українського алфавіту через дефіс.
                        "Start": час початку у форматі HH:MM
                        "End": час завершення у форматі HH:MM
                        "Subject": назва предмета з великої літери
                        }
                    ],
                    "Error": "Текст помилки"
                }}

                Головне завдання - розпізнати розклад навіть якщо там є помилки. Але якщо даних недостатньо заповнюй тільки Error і поясни чого саме тобі не вистачило.

                Виправляй помилки в словах, якщо є.

                Без пояснень, **без форматування Markdown**, просто **чистий JSON-масив**, починаючи з `[` і закінчуючи `]`.

                Ось промпт від користувача: {{{input}}}
            """;
    }

    public static string SendAnswer(List<Schedule> schedules)
    {   
        var result = new StringBuilder();
        foreach (var schedule in schedules){
            result.AppendLine($"Збережено: {schedule.Day}, {schedule.Class}, {schedule.Start} - {schedule.End}, {schedule.Subject}");
        }
        return result.ToString();
    }
}
