using Microsoft.Data.Sqlite;

namespace Services;

public class DatabaseService
{
    private const string ConnectionString = "Data Source=schedule.db";

    public void Save(string userId, List<Schedule> schedules)
    {
        using var conn = new SqliteConnection(ConnectionString);
        conn.Open();

        foreach (var schedule in schedules) {
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT OR REPLACE INTO Schedule (UserId, Day, Class, Start, End, Subject)
                                VALUES ($userId, $day, $class, $start, $end, $subject)";
            cmd.Parameters.AddWithValue("$userId", userId);
            cmd.Parameters.AddWithValue("$day", ConvertDayToInt(schedule.Day));
            cmd.Parameters.AddWithValue("$class", schedule.Class);
            cmd.Parameters.AddWithValue("$start", schedule.Start);
            cmd.Parameters.AddWithValue("$end", schedule.End);
            cmd.Parameters.AddWithValue("$subject", schedule.Subject);
            cmd.ExecuteNonQuery();
        }
    }

    public List<Schedule> GetScheduleLessons(string? schoolClass = null)
    {
        using var conn = new SqliteConnection(ConnectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = string.IsNullOrWhiteSpace(schoolClass)
            ? "SELECT Day, Class, Start, End, Subject FROM Schedule ORDER BY CAST(SUBSTR(Class, 1, INSTR(Class, '-') - 1) AS INTEGER), Day, Start"
            : "SELECT Day, Class, Start, End, Subject FROM Schedule WHERE Class = $class ORDER BY Day, Start";
        if (!string.IsNullOrWhiteSpace(schoolClass)) cmd.Parameters.AddWithValue("$class", schoolClass);

        using var reader = cmd.ExecuteReader();
        var list = new List<Schedule>();
        while (reader.Read())
        {
            list.Add(new Schedule
            {
                Day = ConvertIntToDay(reader.GetString(0)),
                Class = reader.GetString(1),
                Start = reader.GetString(2),
                End = reader.GetString(3),
                Subject = reader.GetString(4)
            });
        }

        return list;
    }

    public void Initialize()
    {
        using var conn = new SqliteConnection(ConnectionString);
        conn.Open();

        var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS Schedule (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                UserId TEXT NOT NULL,
                Day INTEGER NOT NULL,
                Class TEXT NOT NULL,
                Start TEXT NOT NULL,
                End TEXT NOT NULL,
                Subject TEXT NOT NULL,
                UNIQUE (UserId, Day, Class, Start)
            );
        ";
        cmd.ExecuteNonQuery();
    }

    private static int ConvertDayToInt(string day)
    {
        return day switch
        {
            "Понеділок" => 1,
            "Вівторок" => 2,
            "Середа" => 3,
            "Четвер" => 4,
            "П'ятниця" => 5,
            "Субота" => 6,
            "Неділя" => 7,
            _ => 0,
        };
    }

    private static string ConvertIntToDay(string type)
    {
        return type switch
        {
            "1" => "Понеділок",
            "2" => "Вівторок",
            "3" => "Середа",
            "4" => "Четвер",
            "5" => "П'ятниця",
            "6" => "Субота",
            "7" => "Неділя",
            _ => "",
        };
    }
}
