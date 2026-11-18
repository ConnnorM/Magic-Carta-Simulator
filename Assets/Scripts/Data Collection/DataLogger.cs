using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;

public static class DataLogger
{
    private const string RoundFileName = "Carta_Rounds.csv";
    // Column order here must match the order of fields in PlayerDataCollector.WriteRow
    private const string RoundsHeader =
        "session_id," +
        "player_id," +
        "player_name," +
        "round," +
        "target_card_id," +
        "target_card_name," +
        "target_card_visibility," +
        "found_target," +
        "time_to_correct," +
        "distance," +
        "first_move_delay," +
        "wrong_card_count," +
        "wrong_card_ids," +
        "input_type," +
        "editor";

    public const string GuessFileName = "Carta_Guesses.csv";
    public const string GuessesHeader =
        "session_id," +
        "player_id," +
        "player_name," +
        "round," +
        "target_card_id," +
        "guess_number," +
        "guessed_card_id," +
        "guessed_card_name," +
        "time_to_guess," +
        "cursor_pos_x," +
        "cursor_pos_y";

    public const string BoardFileName = "Carta_Boards.csv";
    public const string BoardHeader =
        "session_id," +
        "card_id," +
        "card_name," +
        "pos_x," +
        "pos_y," +
        "pos_deg," +
        "stack_order," +
        "target_order," +
        "board_w," +
        "board_h";
    

    private static string folder;

    // Short random ID per game, so rows from different games can be told apart
    public static string SessionId { get; private set; } = "none";

    public static void StartNewSession()
    {
        SessionId = System.Guid.NewGuid().ToString("N").Substring(0, 8);
    }

    // Always writes a dot as the decimal separator, whatever the computer's language settings
    public static string Num(double value)
    {
        return value.ToString("0.###", CultureInfo.InvariantCulture);
    }

    // Desktop/MagicCartaData, falling back to Unity's persistent folder if the Desktop isn't writable
    private static string GetFolder()
    {
        if (folder != null) return folder;
        try
        {
            string desktop = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
            string dir = Path.Combine(desktop, "MagicCartaData");
            Directory.CreateDirectory(dir);
            folder = dir;
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"DataLogger: Desktop not writable ({e.Message}), using persistentDataPath");
            folder = Application.persistentDataPath;
        }
        return folder;
    }

    // Quotes any field containing a comma, quote or newline, doubling inner quotes
    private static string Escape(string field)
    {
        if (field == null) return "";
        if (field.IndexOfAny(new[] { ',', '"', '\n', '\r' }) >= 0)
            return "\"" + field.Replace("\"", "\"\"") + "\"";
        return field;
    }

    public static void AppendRowRounds(string[] fields)
    {
        AppendRow(RoundFileName, RoundsHeader, fields);
    }

    public static void AppendRow(string fileName, string header, string[] fields)
    {
        string line = string.Join(",", System.Array.ConvertAll(fields, Escape));

        // Two attempts: if the first path fails, fall back to the persistent folder and retry
        for (int attempt = 0; attempt < 2; attempt++)
        {
            try
            {
                string path = Path.Combine(GetFolder(), fileName);
                if (!File.Exists(path)) File.AppendAllText(path, header + "\n", Encoding.UTF8);
                File.AppendAllText(path, line + "\n", Encoding.UTF8);
                return;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"DataLogger: write failed ({e.Message})");
                folder = Application.persistentDataPath;
            }
        }
    }
    
    // Writes several rows with a single file open
    public static void AppendRows(string fileName, string header, System.Collections.Generic.List<string[]> rows)
    {
        var sb = new StringBuilder();
        foreach (string[] fields in rows)
        {
            sb.Append(string.Join(",", System.Array.ConvertAll(fields, Escape))).Append('\n');
        }

        for (int attempt = 0; attempt < 2; attempt++)
        {
            try
            {
                string path = Path.Combine(GetFolder(), fileName);
                if (!File.Exists(path)) File.AppendAllText(path, header + "\n", Encoding.UTF8);
                File.AppendAllText(path, sb.ToString(), Encoding.UTF8);
                return;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"DataLogger: write failed ({e.Message})");
                folder = Application.persistentDataPath;
            }
        }
    }
}