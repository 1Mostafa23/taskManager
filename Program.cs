using System;
using System.Text.Json;
using System.IO;
using System.Collections.Generic;
using System.Data.Common;
public class Program
{
    public static void Main(string[] args)
    {
        List<Task> tasks = new List<Task>();
        int taskIdCounter = 1;


        bool exit = false;
        while (!exit)
        {
            Console.Clear();
            Console.WriteLine($"====Task Manager=====");
            Console.WriteLine($"1. Add Task");
            Console.WriteLine($"2. View Tasks");
            Console.WriteLine($"3. Save Tasks");
            Console.WriteLine($"4. load Tasks");
            Console.WriteLine($"5. remove Task");
            Console.WriteLine($"6. edit Task");
            Console.WriteLine($"7. search Task");
            Console.WriteLine($"8. filter by status");
            Console.WriteLine($"9. Exit");

            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    Console.Clear();
                    Console.WriteLine("enter task title: ");
                    string title = Console.ReadLine();

                    Console.WriteLine("enter task description: ");
                    string description = Console.ReadLine();

                    Task newTask = new Task(taskIdCounter++, title, description);
                    tasks.Add(newTask);

                    Console.WriteLine("Task added successfully!");
                    Console.ReadKey();


                    break;
                case "2":
                    Console.Clear();
                    Console.WriteLine("Current Tasks:");
                    if (tasks.Count == 0)
                    {
                        Console.WriteLine("No tasks available.");
                    }
                    else
                    
                    {
                        foreach (var task in tasks)
                        {
                            Console.WriteLine(task);
                            Console.WriteLine(new string('-', 40));
                        }
                    }
                    Console.ReadKey();
                    break;
                case "3":
                    SaveTasks(tasks);
                    Console.WriteLine("Tasks saved.");
                    Console.ReadKey();
                    break;
                case "4":
                    tasks = LoadTasks();
                    if (tasks.Count > 0)
                        taskIdCounter = tasks.Max(t => t.Id) + 1;
                    else
                        taskIdCounter = 1;

                    Console.WriteLine("Tasks loaded successfully.");
                    Console.ReadKey();
                    break;
                case "5":
                    Console.Clear();
                    Console.Write("Enter the ID of the task to remove: ");
                    try
                    {
                        int removeId = int.Parse(Console.ReadLine());

                        var taskToRemove = tasks.FirstOrDefault(t => t.Id == removeId);
                        if (taskToRemove != null)
                        {
                            tasks.Remove(taskToRemove);
                            Console.WriteLine("Task removed.");
                        }
                        else
                        {
                            Console.WriteLine("Task not found.");
                        }
                    }
                    catch (FormatException)
                    {
                        Console.WriteLine("Invalid input. Please enter a number.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Unexpected error: " + ex.Message);
                    }

                    Console.ReadKey();
                    break;

                // edit task 
                case "6":
                    Console.Clear();
                    Console.Write("Enter the ID of the task to edit: ");
                    if (int.TryParse(Console.ReadLine(), out int editId))
                    {
                        var taskToEdit = tasks.FirstOrDefault(t => t.Id == editId);
                        if (taskToEdit != null)
                        {
                            Console.WriteLine("Leave blank to keep current value.");

                            Console.Write($"New Title ({taskToEdit.Title}): ");
                            string newTitle = Console.ReadLine();
                            if (!string.IsNullOrWhiteSpace(newTitle))
                                taskToEdit.Title = newTitle;

                            Console.Write($"New Description ({taskToEdit.Description}): ");
                            string newDesc = Console.ReadLine();
                            if (!string.IsNullOrWhiteSpace(newDesc))
                                taskToEdit.Description = newDesc;

                            Console.Write($"Is Completed? (yes/no) ({(taskToEdit.IsCompleted ? "yes" : "no")}): ");
                            string doneInput = Console.ReadLine().ToLower();
                            if (doneInput == "yes")
                                taskToEdit.IsCompleted = true;
                            else if (doneInput == "no")
                                taskToEdit.IsCompleted = false;

                            Console.WriteLine("Task updated.");
                        }
                        else
                        {
                            Console.WriteLine("Task not found.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid ID format.");
                    }
                    Console.ReadKey();
                    break;
                case "7":
                    Console.Clear();
                    Console.Write("Enter keyword to search: ");
                    string keyword = Console.ReadLine().ToLower();

                    var matched = tasks
                        .Where(t => t.Title.ToLower().Contains(keyword) ||
                                    t.Description.ToLower().Contains(keyword))
                        .ToList();

                    if (matched.Count == 0)
                    {
                        Console.WriteLine("No matching tasks found.");
                    }
                    else
                    {
                        Console.WriteLine("Matching tasks:");
                        foreach (var task in matched)
                        {
                            bool inTitle = task.Title.ToLower().Contains(keyword);
                            bool inDesc = task.Description.ToLower().Contains(keyword);

                            Console.WriteLine(task);
                            Console.Write("→ Found in: ");
                            if (inTitle) Console.Write("Title ");
                            if (inDesc) Console.Write("Description");
                            Console.WriteLine();
                            Console.WriteLine(new string('-', 40));
                        }
                    }

                    Console.ReadKey();
                    break;

                case "8":
                    Console.Clear();
                    Console.WriteLine("Filter by status:");
                    Console.WriteLine("1. Completed");
                    Console.WriteLine("2. Not Completed");
                    string filterChoice = Console.ReadLine();
                    List<Task> filtered = new();
                    if (filterChoice == "1")
                    {
                        filtered = tasks.Where(t => t.IsCompleted).ToList();
                    }
                    else if (filterChoice == "2")
                    {
                        filtered = tasks.Where(t => !t.IsCompleted).ToList();
                    }
                    else
                    {
                        Console.WriteLine("Invalid choice.");
                        Console.ReadKey();
                        break;
                    }
                    if (filtered.Count == 0)
                    {
                        Console.WriteLine("No tasks found with the selected status.");
                    }
                    else
                    {
                        Console.WriteLine("Filtered Tasks:");
                        foreach (var task in filtered)
                        {
                            Console.WriteLine(task);
                            Console.WriteLine(new string('-', 40));
                        }
                    }

                    Console.ReadKey();
                    break;
                case "9":
                    exit = true;
                    Console.WriteLine("Exiting the Task Manager. Goodbye!");

                    break;


                default:
                    Console.WriteLine("Invalid option, please try again.");
                    Console.ReadKey();
                    break;
            }
            Console.WriteLine("going back to the main menu...");
        }
    }

    // json serialization and deserialization methods

    public static void SaveTasks(List<Task> tasks)
    {
        string json = JsonSerializer.Serialize(tasks, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText("tasks.json", json);
    }
    static List<Task> LoadTasks()
    {
        try
        {
            if (!File.Exists("tasks.json"))
                return new List<Task>();
            string json = File.ReadAllText("tasks.json");
            return JsonSerializer.Deserialize<List<Task>>(json) ?? new List<Task>();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading tasks: " + ex.Message);
            return new List<Task>();
        }
    }
}