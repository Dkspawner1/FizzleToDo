using System.Collections;
using System.Globalization;

namespace FizzleToDo
{
    public class Program
    {
        private static List<ToDo> todoList = new List<ToDo>();
        internal enum TaskChoices { CreateTask, ViewTasks, EditTask, DeleteTask, Exit }

        public static void Main(string[] args)
        {
            PopulateList(todoList);
            ToDo.SaveList(todoList);

            todoList = ToDo.LoadList();

            bool valid = false, vInt = false;
            int index = 0;
            do
            {
                int select = GetChoice();
                switch (select)
                {
                    default:
                        Console.WriteLine("Invalid Option");
                        valid = false;
                        break;
                    case 0:
                        todoList.Add(CreateTask());
                        ToDo.SaveList(todoList);
                        break;
                    case 1:
                        ViewTasks(todoList);
                        Console.Write("Press Any Key To Continue...");
                        Console.ReadKey(intercept: true);
                        break;
                    case 2:
                        Console.Write($"{todoList.Count()} Tasks Found enter the task index to edit: ");
                        vInt = int.TryParse(Console.ReadLine(), out index);
                        do
                        {
                            EditTask(index, todoList);
                            ToDo.SaveList(todoList);
                        } while (!vInt);
                        break;
                    case 3:
                        do
                        {
                            Console.Write($"{todoList.Count()} Tasks Found enter the task index or write all to remove: ");
                            var getIndex = Console.ReadLine();

                            if (getIndex!.Equals("all", StringComparison.InvariantCultureIgnoreCase))
                            {
                                todoList.Clear();
                                ToDo.SaveList(todoList);
                            }

                            vInt = int.TryParse(getIndex, out index);
                            if (!vInt || index > todoList.Count() - 1)
                            {
                                Console.WriteLine("Invalid Task Index");
                                break;
                            }
                            DeleteTask(todoList, index);
                            ToDo.SaveList(todoList);
                        } while (!vInt);
                        break;
                    case 4:
                        Console.WriteLine("See you soon!");
                        return;
                }
            } while (!valid);


            Console.Write("Press Any Key To Exit");
            Console.ReadKey(intercept: true);
        }
        private static ToDo CreateTask(params string[] newMsg)
        {
            int index = 0;
            List<string> msg = new List<string>();
            if (newMsg.Length > 0)
                for (int i = 0; i < newMsg.Length; i++)
                    msg.Add(" " + newMsg[i]);

            else msg.Add(string.Empty);

            bool validDate = false, datePassed;
            DateTime date = new DateTime();
            Console.Write($"Please Enter The{msg[index]} Task Name: ");
            string taskName = Console.ReadLine()!;

            CultureInfo enUS = new CultureInfo("en-US");
            do
            {
                Console.Write($"Please Enter The{msg[index]} End Date: mmddyyyy: ");
                string? endDate = Console.ReadLine();

                validDate = DateTime.TryParseExact(endDate, "MMddyyyy", enUS, DateTimeStyles.None, out date);
                datePassed = hasPassed(date);

                if (!validDate)
                    Console.WriteLine("Please enter a valid date in the format of mmddyyyy");
            } while (!validDate && !datePassed);

            Console.Write($"Enter the new{msg[index]} hh:mm or press enter to skip: ");
            string[] tokens = Console.ReadLine()!.Split(':');
            var time = DateTime.Parse($"{int.Parse(tokens[0])}:{int.Parse(tokens[1])}", enUS);

            return new ToDo(taskName, DateTime.Now, new DateTime(date.Year, date.Month, date.Day, time.Hour, time.Minute, 0));
        }
        private static void ViewTasks(List<ToDo> todoList)
        {
            for (int i = 0; i < todoList.Count; i++)
            {
                ToDo? task = todoList[i];
                Console.WriteLine($"Task {i}:");
                Console.WriteLine($"Task Name: {task.TaskName}");
                Console.WriteLine($"Time Created: {task.CreationDate}");
                Console.WriteLine($"End Time: {task.EndDate}");
            }

        }
        private static ToDo EditTask(int index, List<ToDo> list) => list[index] = CreateTask("New");
        private static void DeleteTask(List<ToDo> list, int index) => list.RemoveAt(index);
        private static int GetChoice()
        {
            Console.WriteLine("Would You Like To?");
            IList list = Enum.GetValues(typeof(TaskChoices));
            for (int i = 0; i < list.Count; i++)
            {
                object? option = list[i];
                Console.WriteLine($"({i}) {option}");
            }
            Console.Write("?: ");
            bool valid = int.TryParse(Console.ReadLine(), out int choice);
            if (valid)
                return choice;
            return -1;
        }
        private static bool hasPassed(DateTime dateToCheck) => (GetDateZeroTime(dateToCheck) < GetDateZeroTime(DateTime.Now));
        public static DateTime GetDateZeroTime(DateTime date) => new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
        private static List<ToDo> PopulateList(List<ToDo> list)
        {
            list.Add(new ToDo("Wash Dishes", DateTime.Now, DateTime.Now.AddDays(5)));
            list.Add(new ToDo("Walk Dog", DateTime.Now, DateTime.Now.AddHours(2)));
            list.Add(new ToDo("Feed dog", DateTime.Now, DateTime.Now.AddHours(2)));
            return list;
        }
        // This is a test
    }
}