using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;

public static class DataLogger
{
    private const string FileName = "CartaData.csv";
    // Column order here must match the order of fields in PlayerDataCollector.WriteRow
    private const string Header =
        "session_id," +
        "player_id," +
        "round," +
        "target_card_id," +
        "target_card_name," +
        "found_target," +
        "time_to_correct," +
        "distance," +
        "editor";

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

    public static void AppendRow(string[] fields)
    {
        string line = string.Join(",", System.Array.ConvertAll(fields, Escape));

        // Two attempts: if the first path fails, fall back to the persistent folder and retry
        for (int attempt = 0; attempt < 2; attempt++)
        {
            try
            {
                string path = Path.Combine(GetFolder(), FileName);
                if (!File.Exists(path)) File.AppendAllText(path, Header + "\n", Encoding.UTF8);
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
}