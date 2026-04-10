using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using UralSibApp.Models;

namespace UralSibApp.Services
{
    public class DatabaseService
    {
        private readonly string _connectionString;

        public DatabaseService()
        {
            // Получаем настройки строки подключения
            var connStringSettings = ConfigurationManager.ConnectionStrings["UralSibConnection"];
            if (connStringSettings == null)
                throw new Exception("Строка подключения 'UralSibConnection' не найдена в App.config.");

            _connectionString = connStringSettings.ConnectionString;
        }

        // ========== ОТДЕЛЫ ==========
        public List<Department> GetDepartments()
        {
            var departments = new List<Department>();
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(@"
                    SELECT d.Id, d.Name, d.Floor,
                           (SELECT COUNT(*) FROM Teams WHERE DepartmentId = d.Id) AS TeamsCount
                    FROM Departments d", conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        departments.Add(new Department
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Floor = reader.GetInt32(2),
                            TeamsCount = reader.GetInt32(3)
                        });
                    }
                }
            }
            return departments;
        }

        public void AddDepartment(Department dept)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("INSERT INTO Departments (Name, Floor) VALUES (@Name, @Floor)", conn);
                cmd.Parameters.AddWithValue("@Name", dept.Name);
                cmd.Parameters.AddWithValue("@Floor", dept.Floor);
                cmd.ExecuteNonQuery();
            }
        }

        public void UpdateDepartment(Department dept)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("UPDATE Departments SET Name = @Name, Floor = @Floor WHERE Id = @Id", conn);
                cmd.Parameters.AddWithValue("@Id", dept.Id);
                cmd.Parameters.AddWithValue("@Name", dept.Name);
                cmd.Parameters.AddWithValue("@Floor", dept.Floor);
                cmd.ExecuteNonQuery();
            }
        }

        public void DeleteDepartment(int id)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("DELETE FROM Departments WHERE Id = @Id", conn);
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.ExecuteNonQuery();
            }
        }

        // ========== КОМАНДЫ ==========
        public List<Team> GetTeams()
        {
            var teams = new List<Team>();
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(@"
                    SELECT t.Id, t.Name, t.DepartmentId, d.Name AS DepartmentName, 
                           t.ManagerId, e.FullName AS ManagerName,
                           (SELECT COUNT(*) FROM Employees WHERE TeamId = t.Id) AS EmployeesCount
                    FROM Teams t
                    LEFT JOIN Departments d ON t.DepartmentId = d.Id
                    LEFT JOIN Employees e ON t.ManagerId = e.Id", conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        teams.Add(new Team
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            DepartmentId = reader.GetInt32(2),
                            DepartmentName = reader.GetString(3),
                            ManagerId = reader.IsDBNull(4) ? (int?)null : reader.GetInt32(4),
                            ManagerName = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                            EmployeesCount = reader.GetInt32(6)
                        });
                    }
                }
            }
            return teams;
        }

        public void AddTeam(Team team)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(
                    "INSERT INTO Teams (Name, DepartmentId, ManagerId) VALUES (@Name, @DeptId, @ManagerId)", conn);
                cmd.Parameters.AddWithValue("@Name", team.Name);
                cmd.Parameters.AddWithValue("@DeptId", team.DepartmentId);
                cmd.Parameters.AddWithValue("@ManagerId", team.ManagerId.HasValue ? (object)team.ManagerId.Value : DBNull.Value);
                cmd.ExecuteNonQuery();
            }
        }

        public void UpdateTeam(Team team)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(
                    "UPDATE Teams SET Name = @Name, DepartmentId = @DeptId, ManagerId = @ManagerId WHERE Id = @Id", conn);
                cmd.Parameters.AddWithValue("@Id", team.Id);
                cmd.Parameters.AddWithValue("@Name", team.Name);
                cmd.Parameters.AddWithValue("@DeptId", team.DepartmentId);
                cmd.Parameters.AddWithValue("@ManagerId", team.ManagerId.HasValue ? (object)team.ManagerId.Value : DBNull.Value);
                cmd.ExecuteNonQuery();
            }
        }

        public void DeleteTeam(int id)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("DELETE FROM Teams WHERE Id = @Id", conn);
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.ExecuteNonQuery();
            }
        }

        // ========== СОТРУДНИКИ ==========
        public List<Employee> GetEmployees()
        {
            var employees = new List<Employee>();
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(@"
                    SELECT e.Id, e.FullName, e.ContactInfo, e.Address, e.Position, e.TeamId, t.Name AS TeamName
                    FROM Employees e
                    LEFT JOIN Teams t ON e.TeamId = t.Id", conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        employees.Add(new Employee
                        {
                            Id = reader.GetInt32(0),
                            FullName = reader.GetString(1),
                            ContactInfo = reader.GetString(2),
                            Address = reader.GetString(3),
                            Position = reader.GetString(4),
                            TeamId = reader.GetInt32(5),
                            TeamName = reader.IsDBNull(6) ? string.Empty : reader.GetString(6)
                        });
                    }
                }
            }
            return employees;
        }

        public void AddEmployee(Employee emp)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(
                    @"INSERT INTO Employees (FullName, ContactInfo, Address, Position, TeamId) 
                      VALUES (@FullName, @Contact, @Address, @Position, @TeamId)", conn);
                cmd.Parameters.AddWithValue("@FullName", emp.FullName);
                cmd.Parameters.AddWithValue("@Contact", emp.ContactInfo);
                cmd.Parameters.AddWithValue("@Address", emp.Address);
                cmd.Parameters.AddWithValue("@Position", emp.Position);
                cmd.Parameters.AddWithValue("@TeamId", emp.TeamId);
                cmd.ExecuteNonQuery();
            }
        }

        public void UpdateEmployee(Employee emp)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(
                    @"UPDATE Employees 
                      SET FullName = @FullName, ContactInfo = @Contact, Address = @Address, 
                          Position = @Position, TeamId = @TeamId 
                      WHERE Id = @Id", conn);
                cmd.Parameters.AddWithValue("@Id", emp.Id);
                cmd.Parameters.AddWithValue("@FullName", emp.FullName);
                cmd.Parameters.AddWithValue("@Contact", emp.ContactInfo);
                cmd.Parameters.AddWithValue("@Address", emp.Address);
                cmd.Parameters.AddWithValue("@Position", emp.Position);
                cmd.Parameters.AddWithValue("@TeamId", emp.TeamId);
                cmd.ExecuteNonQuery();
            }
        }

        public void DeleteEmployee(int id)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("DELETE FROM Employees WHERE Id = @Id", conn);
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.ExecuteNonQuery();
            }
        }
    }
}