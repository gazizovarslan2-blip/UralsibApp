using System;
using System.Windows;
using System.Windows.Controls;
using UralSibApp.Models;
using UralSibApp.Services;

namespace UralSibApp.Views
{
    public partial class DepartmentView : UserControl
    {
        private readonly DatabaseService _db = new DatabaseService();
        private Department _editingDepartment = null;

        public DepartmentView()
        {
            InitializeComponent();
            LoadData();
            ClearEditForm();
        }

        private void LoadData()
        {
            try
            {
                DepartmentsGrid.ItemsSource = _db.GetDepartments();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки отделов: {ex.Message}", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearEditForm()
        {
            _editingDepartment = null;
            NameTextBox.Text = string.Empty;
            FloorTextBox.Text = string.Empty;
            EditPanel.Visibility = Visibility.Collapsed;
        }

        private void ShowEditForm(Department dept)
        {
            _editingDepartment = dept;
            if (dept != null)
            {
                NameTextBox.Text = dept.Name;
                FloorTextBox.Text = dept.Floor.ToString();
            }
            else
            {
                NameTextBox.Text = string.Empty;
                FloorTextBox.Text = string.Empty;
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
            var selected = DepartmentsGrid.SelectedItem as Department;
            if (selected == null)
            {
                MessageBox.Show("Выберите отдел для редактирования.");
                return;
            }
            ShowEditForm(selected);
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var selected = DepartmentsGrid.SelectedItem as Department;
            if (selected == null)
            {
                MessageBox.Show("Выберите отдел для удаления.");
                return;
            }

            if (MessageBox.Show($"Удалить отдел '{selected.Name}'?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    _db.DeleteDepartment(selected.Id);
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
                MessageBox.Show("Введите название отдела.");
                return;
            }

            if (!int.TryParse(FloorTextBox.Text, out int floor))
            {
                MessageBox.Show("Введите корректный этаж (целое число).");
                return;
            }

            try
            {
                if (_editingDepartment == null)
                {
                    var newDept = new Department { Name = NameTextBox.Text.Trim(), Floor = floor };
                    _db.AddDepartment(newDept);
                }
                else
                {
                    _editingDepartment.Name = NameTextBox.Text.Trim();
                    _editingDepartment.Floor = floor;
                    _db.UpdateDepartment(_editingDepartment);
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

        private void DepartmentsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Можно оставить пустым
        }
    }
}