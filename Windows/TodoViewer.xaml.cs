﻿using Microsoft.EntityFrameworkCore;
using Notification.Wpf;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TaskSharp.Windows
{
    /// <summary>
    /// Interaction logic for TodoViewer.xaml
    /// </summary>
    public partial class TodoViewer : Window
    {
        private readonly NotesContext _context = new NotesContext();

        public TodoViewer()
        {
            InitializeComponent();
        }

        private void Refresh()
        {
            var uid = (int)Application.Current.Properties["uid"];
            int NoteId = (int)Application.Current.Properties["noteId"];
            var todos = _context.TodoLists.Where(x => x.UserId == uid && x.Id == NoteId).First();

            TodoName.Text = todos.Name;
            TodoTags.Text = todos.Tags;

            var todoDict = JsonSerializer.Deserialize<Dictionary<string, bool>>(todos.Todos);
            TodoElements.ItemsSource = todoDict;

            foreach (var done in todoDict.Values)
            {
                if (!done)
                {
                    todos.Done = false;
                    _context.SaveChanges();
                    return;
                }
            }

            todos.Done = true;          
            var notificationManager = new NotificationManager();
            notificationManager.Show(new NotificationContent
            {
                Title = todos.Name,
                Message = "To-do lista je obavljena!",
                Type = NotificationType.Information,
                CloseOnClick = true // closes message when message is clicked
            });

            _context.SaveChanges();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _context.Database.EnsureCreated();
            _context.Users.Load();
            _context.TodoLists.Load();
            Refresh();
        }

        private void TickClicked(object sender, MouseButtonEventArgs e)
        {
            var todoID = (string)((Image)sender).Tag;
            var uid = (int)Application.Current.Properties["uid"];
            int NoteId = (int)Application.Current.Properties["noteId"];

            var todos = _context.TodoLists.Where(x => x.UserId == uid && x.Id == NoteId).First();
            //todos.CheckUncheck(todoID);

            _context.SaveChanges();
            Refresh();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            _context.Dispose();
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var dashboard = new Dashboard();
            dashboard.Show();
        }
    }
}
