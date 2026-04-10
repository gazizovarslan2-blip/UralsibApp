using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using UralSibApp.Models;
using UralSibApp.Services;

namespace UralSibApp.Views
{
    public partial class TeamView : UserControl
    {
        private readonly DatabaseService _db = new DatabaseService();
        private Team _editingTeam = null;

        public TeamView()
        {
            InitializeComponent();
            LoadData();
            ClearEditForm();
        }

        private void LoadData()
        {
            try
            {
                TeamsGrid.ItemsSource = _db.GetTeams();
                DepartmentComboBox.ItemsSource = _db.GetDepartments();
                ManagerComboBox.ItemsSource = _db.GetEmployees();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearEditForm()
        {
            _editingTeam = null;
            NameTextBox.Text = string.Empty;
            DepartmentComboBox.SelectedIndex = -1;
            ManagerComboBox.SelectedIndex = -1;
            EditPanel.Visibility = Visibility.Collapsed;
        }

        private void ShowEditForm(Team team)
        {
            _editingTeam = team;
            if (team != null)
            {
                NameTextBox.Text = team.Name;
                DepartmentComboBox.SelectedItem = (DepartmentComboBox.ItemsSource as System.Collections.IEnumerable)
                    ?.OfType<Department>().FirstOrDefault(d => d.Id == team.DepartmentId);
                ManagerComboBox.SelectedItem = (ManagerComboBox.ItemsSource as System.Collections.IEnumerable)
                    ?.OfType<Employee>().FirstOrDefault(e => e.Id == team.ManagerId);
            }
            else
            {
                NameTextBox.Text = string.Empty;
                DepartmentComboBox.SelectedIndex = -1;
                ManagerComboBox.SelectedIndex = -1;
            }
            EditPanel.Visibility = Visibility.Visible;
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
            ClearEditForm();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            ShowEditForm(null);
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var selected = TeamsGrid.SelectedItem as Team;
            if (selected == null)
            {
                MessageBox.Show("Выберите команду для редактирования.");
                return;
            }
            ShowEditForm(selected);
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var selected = TeamsGrid.SelectedItem as Team;
            if (selected == null)
            {
                MessageBox.Show("Выберите команду для удаления.");
                return;
            }

            if (MessageBox.Show($"Удалить команду '{selected.Name}'?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    _db.DeleteTeam(selected.Id);
                    LoadData();
                    ClearEditForm();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка удаления: {ex.Message}", "Ошибка",
                                    MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                MessageBox.Show("Введите название команды.");
                return;
            }

            if (DepartmentComboBox.SelectedItem is not Department selectedDept)
            {
                MessageBox.Show("Выберите отдел.");
                return;
            }

            int? managerId = (ManagerComboBox.SelectedItem as Employee)?.Id;

            try
            {
                if (_editingTeam == null)
                {
                    var newTeam = new Team
                    {
                        Name = NameTextBox.Text.Trim(),
                        DepartmentId = selectedDept.Id,
                        ManagerId = managerId
                    };
                    _db.AddTeam(newTeam);
                }
                else
                {
                    _editingTeam.Name = NameTextBox.Text.Trim();
                    _editingTeam.DepartmentId = selectedDept.Id;
                    _editingTeam.ManagerId = managerId;
                    _db.UpdateTeam(_editingTeam);
                }

                LoadData();
                ClearEditForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            ClearEditForm();
        }

        private void TeamsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Пусто
        }
    }
}