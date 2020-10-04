namespace TeisterMask.DataProcessor
{
    using System;
    using System.Globalization;
    using System.Linq;
    using Data;
    using Newtonsoft.Json;
    using Formatting = Newtonsoft.Json.Formatting;

    public class Serializer
    {
        public static string ExportProjectWithTheirTasks(TeisterMaskContext context)
        {
            throw new NotImplementedException();
        }

        public static string ExportMostBusiestEmployees(TeisterMaskContext context, DateTime date)
        {
            var employee = context.Employees
                     .ToArray()
                     .Where(x => x.EmployeesTasks.Any(y => y.Task.OpenDate >= date))
                     .Select(x => new
                     {
                         x.Username,
                         Tasks = x.EmployeesTasks
                                  .Where(y => y.Task.OpenDate >= date)
                                  .OrderByDescending(d => d.Task.DueDate)
                                  .ThenBy(n => n.Task.Name)
                                  .Select(t => new
                                  {
                                      TaskName = t.Task.Name,
                                      OpenDate = t.Task.OpenDate
                                                  .ToString("d", CultureInfo.InvariantCulture),
                                      DueDate = t.Task.DueDate
                                                  .ToString("d", CultureInfo.InvariantCulture),
                                      LabelType = t.Task.LabelType,
                                      ExecutionType =t.Task.ExecutionType
                                  })
                                  .ToArray()
                     })
                     .OrderByDescending(x => x.Tasks.Count())
                     .ThenBy(x => x.Username)
                     .Take(10)
                     .ToArray();

            return JsonConvert.SerializeObject(employee, Formatting.Indented);
        }
    }
}