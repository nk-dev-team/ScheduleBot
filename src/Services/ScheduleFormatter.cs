using System.Text;

namespace Services;

public static class ScheduleFormatter
{
    public static string Format(List<Schedule> schedules)
    {
        if (!schedules.Any()) return "Немає розкладу.";

        var day = "";
        var className = "";
        var index = 1;
        var sb = new StringBuilder();
        foreach (var schedule in schedules)
        {
            if (className != schedule.Class)
            {
                className = schedule.Class;
                sb.AppendLine($"\n<b>Розклад {className}</b>");
                day = "";
            }

            if (day != schedule.Day)
            {
                day = schedule.Day;
                sb.AppendLine();
                sb.AppendLine($"{day}:");
                index = 1;
            }

            sb.AppendLine($"{index}. {schedule.Start} - {schedule.End}, {schedule.Subject}");
            index++;
        }

        return sb.ToString();
    }
}
