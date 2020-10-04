namespace TeisterMask.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;

    using Data;
    using ProductShop.XMLHelper;
    using TeisterMask.DataProcessor.ImportDto;
    using TeisterMask.Data.Models;
    using System.Globalization;
    using TeisterMask.Data.Models.Enums;
    using Newtonsoft.Json;
    using System.Linq;
    using System.Runtime.CompilerServices;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedProject
            = "Successfully imported project - {0} with {1} tasks.";

        private const string SuccessfullyImportedEmployee
            = "Successfully imported employee - {0} with {1} tasks.";

        public static string ImportProjects(TeisterMaskContext context, string xmlString)
        {
            var rootAttributeName = "Projects";

            var projectsDTO = XMLConverter.Deserializer<ProjectDTO>(xmlString, rootAttributeName);

            var projects = new List<Project>();

            var result = string.Empty;

            foreach (var projectDTO in projectsDTO)
            {
                if (!IsValid(projectDTO))
                {
                    result += ErrorMessage + Environment.NewLine;
                    continue;
                }

                DateTime dateOpenProject;
                var validOpenDateProject = DateTime.TryParseExact(projectDTO.OpenDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateOpenProject);

                if (!validOpenDateProject)
                {
                    result += ErrorMessage + Environment.NewLine;
                    continue;
                }

                DateTime dateDueProject;
                var validDueDateProject = DateTime.TryParseExact(projectDTO.DueDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateDueProject);

                if (!validDueDateProject)
                {
                    result += ErrorMessage + Environment.NewLine;
                    continue;
                }

                var tasks = new List<Task>();

                foreach (var taskDTO in projectDTO.Tasks)
                {
                    if (!IsValid(taskDTO))
                    {
                        result += ErrorMessage + Environment.NewLine;
                        continue;
                    }

                    DateTime dateOpenTask;
                    var validOpenDateTask = DateTime.TryParseExact(projectDTO.OpenDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateOpenTask);

                    if (!validOpenDateTask)
                    {
                        result += ErrorMessage + Environment.NewLine;
                        continue;
                    }
                    DateTime dateDueTask;
                    var validDueDateTask = DateTime.TryParseExact(projectDTO.DueDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateDueTask);

                    if (!validDueDateTask)
                    {
                        result += ErrorMessage + Environment.NewLine;
                        continue;
                    }

                    if (dateOpenTask < dateOpenProject || dateDueTask > dateDueProject)
                    {
                        result += ErrorMessage + Environment.NewLine;
                        continue;
                    }

                    var task = new Task
                    {
                        Name = taskDTO.Name,
                        OpenDate = dateOpenTask,
                        DueDate = dateDueTask,
                        ExecutionType = (ExecutionType)taskDTO.ExecutionType,
                        LabelType = (LabelType)taskDTO.LabelType
                    };

                    tasks.Add(task);
                }

                projects.Add(new Project
                {
                    Name = projectDTO.Name,
                    OpenDate = dateOpenProject,
                    DueDate = dateDueProject,
                    Tasks = tasks

                });

                result += string.Format(SuccessfullyImportedProject, projectDTO.Name, projectDTO.Tasks.Count) + Environment.NewLine;
            };

            context.Projects.AddRange(projects);

            context.SaveChanges();

            return result.Trim();
        }

        public static string ImportEmployees(TeisterMaskContext context, string jsonString)
        {
            var result = string.Empty;

            var employeesDTO = JsonConvert.DeserializeObject<EmployeeDTO[]>(jsonString);

            var employees = new List<Employee>();

            foreach (var employeeDTO in employeesDTO)
            {
                if (!IsValid(employeeDTO))
                {
                    result += ErrorMessage + Environment.NewLine;
                    continue;
                }

                var taskIds = context.Tasks.Select(X => X.Id).ToList();

                foreach (var taskIdDTO in employeeDTO.Tasks)
                {
                    if (!taskIds.Contains(taskIdDTO))
                    {
                        result += ErrorMessage + Environment.NewLine;
                        continue;
                    }
                }

                employees.Add( new Employee 
                {
                    Username = employeeDTO.Username,
                    Email = employeeDTO.Email,
                    Phone = employeeDTO.Phone,
                    EmployeesTasks = employeeDTO.Tasks.Select(x => new EmployeeTask
                    {
                        TaskId = x
                    })
                    .ToHashSet()
                });


                result += string.Format(SuccessfullyImportedEmployee, employeeDTO.Username,employeeDTO.Tasks.Count) + Environment.NewLine;
            };

            context.Employees.AddRange(employees);

            context.SaveChanges();

            return result.Trim();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}