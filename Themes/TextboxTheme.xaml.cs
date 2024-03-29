﻿using System.Windows;
using System.Windows.Controls;
using TaskSharp.Classes;

namespace TaskSharp.Themes
{
    public partial class TextboxTheme : ResourceDictionary
    {
        private readonly NotesContext _context = new();

        public delegate void NotesHandler(List<Note> x);
        public static event NotesHandler calledNote;
        public static void CallNoteRefresher(List<Note> x)
        {
            calledNote?.Invoke(x);
        }

        public delegate void EventsHandler(List<Event> x, List<Event> y);
        public static event EventsHandler calledEvent;
        public static void CallEventRefresher(List<Event> x, List<Event> y)
        {
            calledEvent?.Invoke(x, y);
        }

        public delegate void RemindersHandler(List<Reminder> x, List<Reminder> y);
        public static event RemindersHandler calledReminder;
        public static void CallReminderRefresher(List<Reminder> x, List<Reminder> y)
        {
            calledReminder?.Invoke(x, y);
        }

        public delegate void TodosHandler(List<TodoList> x, List<TodoList> y);
        public static event TodosHandler calledTodo;
        public static void CallTodoRefresher(List<TodoList> x, List<TodoList> y)
        {
            calledTodo?.Invoke(x, y);
        }

        private void SearchNotes(object sender, TextChangedEventArgs e)
        {
            int noteType = (int)Application.Current.Properties["noteType"];
            var uid = (int)Application.Current.Properties["uid"];
            string text = (sender as TextBox).Text.ToLower();

            switch (noteType)
            {
                case 0: // note
                    var notes = _context.Notes
                        .Where(x => x.UserId == uid && (x.Name.ToLower().Contains(text) || x.Tags.ToLower().Contains(text)))
                        .OrderByDescending(x => x.Pinned)
                        .ThenByDescending(x => x.CreationDate)
                        .ToList();

                    CallNoteRefresher(notes);
                    break;

                case 1: // event
                    var upcomingEvents = _context.Events
                        .Where(x => x.UserId == uid && x.EndDate >= DateTime.Today && (x.Name.ToLower().Contains(text) || x.Tags.ToLower().Contains(text)))
                        .OrderByDescending(x => x.Pinned)
                        .ThenBy(x => x.EndDate)
                        .ToList();
                    var expiredEvents = _context.Events
                        .Where(x => x.UserId == uid && x.EndDate < DateTime.Today && (x.Name.ToLower().Contains(text) || x.Tags.ToLower().Contains(text)))
                        .OrderByDescending(x => x.EndDate)
                        .ToList();

                    CallEventRefresher(upcomingEvents, expiredEvents);
                    break;

                case 2: // reminder
                    var upcomingReminders = _context.Reminders
                        .Where(x => x.UserId == uid && x.DueDate >= DateTime.Today && (x.Name.ToLower().Contains(text) || x.Tags.ToLower().Contains(text)))
                        .OrderByDescending(x => x.Pinned)
                        .ThenBy(x => x.DueDate)
                        .ThenByDescending(x => x.Priority)
                        .ToList();
                    var expiredReminders = _context.Reminders
                        .Where(x => x.UserId == uid && x.DueDate < DateTime.Today && (x.Name.ToLower().Contains(text) || x.Tags.ToLower().Contains(text)))
                        .OrderByDescending(x => x.DueDate)
                        .ToList();

                    CallReminderRefresher(upcomingReminders, expiredReminders);
                    break;

                case 3: // to-do list
                    var undoneTodos = _context.TodoLists
                        .Where(x => x.UserId == uid && x.Done == false && (x.Name.ToLower().Contains(text) || x.Tags.ToLower().Contains(text)))
                        .OrderByDescending(x => x.Pinned)
                        .ToList();
                    var doneTodos = _context.TodoLists
                        .Where(x => x.UserId == uid && x.Done == true && (x.Name.ToLower().Contains(text) || x.Tags.ToLower().Contains(text)))
                        .ToList();

                    CallTodoRefresher(undoneTodos, doneTodos);
                    break;
            }
        }
    }
}
