using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using UralSibApp.Models;
using UralSibApp.Services;

namespace UralSibApp.Views
{
    public partial class EmployeeView : UserControl
    {
        private readonly DatabaseService _db = new DatabaseService();
        private Employee _editingEmployee = null;

        public EmployeeView()
        {
            InitializeComponent();
            LoadData();
            ClearEditForm();
        }

        private void LoadData()
        {
            try
            {
                EmployeesGrid.ItemsSource = _db.GetEmployees();
                TeamComboBox.ItemsSource = _db.GetTeams();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearEditForm()
        {
            _editingEmployee = null;
            FullNameTextBox.Text = string.Empty;
            ContactTextBox.Text = string.Empty;
            AddressTextBox.Text = string.Empty;
            PositionTextBox.Text = string.Empty;
            TeamComboBox.SelectedIndex = -1;
            EditPanel.Visibility = Visibility.Collapsed;
        }

        private void ShowEditForm(Employee emp)
        {
            _editingEmployee = emp;
            if (emp != null)
            {
                FullNameTextBox.Text = emp.FullName;
                ContactTextBox.Text = emp.ContactInfo;
                AddressTextBox.Text = emp.Address;
                PositionTextBox.Text = emp.Position;
                TeamComboBox.SelectedItem = (TeamComboBox.ItemsSource as System.Collections.IEnumerable)
                    ?.OfType<Team>().FirstOrDefault(t => t.Id == emp.TeamId);
            }
            else
            {
                FullNameTextBox.Text = string.Empty;
                ContactTextBox.Text = string.Empty;
                AddressTextBox.Text = string.Empty;
                PositionTextBox.Text = string.Empty;
                TeamComboBox.SelectedIndex = -1;
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
            var selected = EmployeesGrid.SelectedItem as Employee;
            if (selected == null)
            {
                MessageBox.Show("Выберите сотрудника для редактирования.");
                return;
            }
            ShowEditForm(selected);
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var selected = EmployeesGrid.SelectedItem as Employee;
            if (selected == null)
            {
                MessageBox.Show("Выберите сотрудника для удаления.");
                return;
            }

            if (MessageBox.Show($"Удалить сотрудника '{selected.FullName}'?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    _db.DeleteEmployee(selected.Id);
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
            if (string.IsNullOrWhiteSpace(FullNameTextBox.Text))
            {
                MessageBox.Show("Введите ФИО сотрудника.");
                return;
            }

            if (TeamComboBox.SelectedItem is not Team selectedTeam)
            {
                MessageBox.Show("Выберите команду.");
                return;
            }

            try
            {
                if (_editingEmployee == null)
                {
                    var newEmp = new Employee
                    {
                        FullName = FullNameTextBox.Text.Trim(),
                        ContactInfo = ContactTextBox.Text.Trim(),
                        Address = AddressTextBox.Text.Trim(),
                        Position = PositionTextBox.Text.Trim(),
                        TeamId = selectedTeam.Id
                    };
                    _db.AddEmployee(newEmp);
                }
                else
                {
                    _editingEmployee.FullName = FullNameTextBox.Text.Trim();
                    _editingEmployee.ContactInfo = ContactTextBox.Text.Trim();
                    _editingEmployee.Address = AddressTextBox.Text.Trim();
                    _editingEmployee.Position = PositionTextBox.Text.Trim();
                    _editingEmployee.TeamId = selectedTeam.Id;
                    _db.UpdateEmployee(_editingEmployee);
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

        private void EmployeesGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Пусто
        }
    }
}