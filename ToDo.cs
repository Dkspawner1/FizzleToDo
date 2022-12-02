using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FizzleToDo
{
    public partial class ToDo
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        private static readonly string PATH = "todo.json";

        // Properties / Variables
        public string TaskName { get; set; } = string.Empty;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull | JsonIgnoreCondition.WhenWritingDefault)]
        public DateTime? CreationDate { get; set; } = default;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull | JsonIgnoreCondition.WhenWritingDefault)]
        public DateTime? EndDate { get; set; } = default;

        // For Json Deserialize
        public ToDo()
        { }
        // Using Date
        public ToDo(string taskName, DateTime creationDate, DateTime endDate)
        {
            TaskName = taskName;
            CreationDate = creationDate;
            EndDate = endDate;
        }
        private static void CreateFile() => File.Create(PATH);
        public static void SaveList(List<ToDo> list)
        {
            if (!File.Exists(PATH))
                CreateFile();
            var serialized = JsonSerializer.Serialize<List<ToDo>>(list);
            File.WriteAllText(PATH, serialized);
        }
        public static List<ToDo> LoadList()
        {
            if (!File.Exists(PATH))
                CreateFile();

            var contents = File.ReadAllText(PATH);
            var deserialized = JsonSerializer.Deserialize<List<ToDo>>(contents);
            if (deserialized == null)
                throw new Exception("List is null please populate list");
            return deserialized;
        }
    }
}