using System.Collections;
using System.Globalization;

namespace FizzleToDo
{
    public class Program
    {
        private static List<ToDo> todoList = new List<ToDo>();
        [Flags()]
        internal enum TaskChoices : sbyte { CreateTask = 1, ViewTasks, EditTask, DeleteTask, Exit }

        private static readonly int ListIndexOffset = 0x1;

        public static void Main(string[] args)
        {
            todoList = ToDo.LoadList();
            if (CheckListEmpty(todoList))
                PrePopulateList(todoList);
            ToDo.SaveList(todoList);

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
                    case 1:
                        todoList.Add(CreateTask());
                        ToDo.SaveList(todoList);
                        break;
                    case 2:
                        ViewTasks(todoList);
                        Console.Write("Press Any Key To Continue...");
                        Console.ReadKey(intercept: true);
                        break;
                    case 3:
                        if (CheckListEmpty(todoList))
                        {
                            Console.WriteLine($"No tasks were found in {ToDo.PATH}, Cannot edit a empty list");
                            Console.Write("Press any key to continue...");
                            Console.ReadKey(intercept: true);
                            break;
                        }
                        Console.Write($"{todoList.Count()} Tasks Found enter the task index to edit: ");
                        vInt = int.TryParse(Console.ReadLine(), out index);
                        do
                        {
                            EditTask(index, todoList);
                            ToDo.SaveList(todoList);
                        } while (!vInt);
                        break;
                    case 4:
                        if (CheckListEmpty(todoList))
                        {
                            Console.WriteLine("Cannot delete from a empty list");
                            Console.Write("Press any key to continue...");
                            Console.ReadKey();
                            break;
                        }
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

                            if (!vInt || index > todoList.Count() || index <= 0)
                            {
                                Console.WriteLine("Invalid Task Index");
                                break;
                            }
                            else
                            {
                                DeleteTask(todoList, index);
                                ToDo.SaveList(todoList);
                            }
                        } while (!vInt);
                        break;
                    case 5:
                        Console.WriteLine("See you soon!");
                        return;
                }
            } while (!valid);


            Console.Write("Press Any Key To Exit");
            Console.ReadKey(intercept: true);
        }
        private static bool CheckListEmpty(List<ToDo> todoList) => !todoList.Any();
        private static ToDo CreateTask(params string[] newMsg)
        {
            int index = 0;
            List<string> msg = new List<string>();
            if (newMsg.Length > 0)
                for (int i = 0; i < newMsg.Length; i++)
                    msg.Add(" " + newMsg[i]);

            else msg.Add(string.Empty);

            bool validDate = false, datePassed, validTime = false;
            DateTime date = new DateTime(), time = new DateTime();
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

            do
            {
                Console.Write($"Enter the new{msg[index]} hh:mm: ");
                string[] tokens = Console.ReadLine()!.Split(':');
                validTime = DateTime.TryParse($"{int.Parse(tokens[0])}:{int.Parse(tokens[1])}", enUS, DateTimeStyles.None, out time);
                if (!validTime)
                    Console.WriteLine("Please enter a valid time using hh:mm format");
            } while (!validTime);

            return new ToDo(taskName, DateTime.Now, new DateTime(date.Year, date.Month, date.Day, time.Hour, time.Minute, 0));
        }
        private static void ViewTasks(List<ToDo> todoList)
        {
            if (CheckListEmpty(todoList))
                System.Console.WriteLine($"There were no tasks found in {ToDo.PATH}");
            for (int i = 1; i <= todoList.Count; i++)
            {
                ToDo? task = todoList[i - ListIndexOffset];
                Console.WriteLine($"Task {i}:");
                Console.WriteLine($"\tTask Name: {task.TaskName}");
                Console.WriteLine($"\tTime Created: {task.CreationDate}");
                Console.WriteLine($"\tEnd Time: {task.EndDate}");
            }

        }
        private static ToDo EditTask(int index, List<ToDo> list)
        {
            if (index > list.Count || index < 1)
                throw new IndexOutOfRangeException("List out of index bounds");

            return list[index - ListIndexOffset] = CreateTask("New");
        }
        private static void DeleteTask(List<ToDo> list, int index) => list.RemoveAt(index - ListIndexOffset);
        private static int GetChoice()
        {
            Console.WriteLine("Would You Like To?");
            IList list = Enum.GetValues(typeof(TaskChoices));
            for (int i = ListIndexOffset; i <= list.Count; i++)
            {
                object? option = list[i - ListIndexOffset];
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
        private static List<ToDo> PrePopulateList(List<ToDo> list)
        {
            list.Add(new ToDo("Wash Dishes", DateTime.Now, DateTime.Now.AddDays(5)));
            list.Add(new ToDo("Walk Dog", DateTime.Now, DateTime.Now.AddHours(2)));
            list.Add(new ToDo("Feed dog", DateTime.Now, DateTime.Now.AddHours(2)));
            return list;
        }
    }
}