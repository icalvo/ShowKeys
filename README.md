# ShowKeys

A transparent overlay application that displays the last 15 keyboard combinations pressed.

## Features

- Displays a transparent overlay bar above the taskbar
- Shows the last 15 keyboard combinations pressed
- Modifier keys (Ctrl, Alt, Shift, Win) are only shown when combined with other keys
- Shift+letter combinations show the letter in uppercase, single letters in lowercase
- Special keys (Enter, Tab, Backspace, etc.) are displayed by their name

## Requirements

- .NET 8.0 or higher
- Windows operating system

## Usage

1. Launch the application
2. The overlay will appear above your taskbar
3. Press keyboard combinations to see them displayed in the overlay
4. The most recent combinations will appear on the left side
5. Only the last 15 combinations will be shown
6. The application can be closed from the task manager

## Technical Information

This application uses a global keyboard hook to capture key presses system-wide. It processes the key combinations and displays them in a transparent WPF window positioned above the taskbar.

Note: Because the application uses global keyboard hooks, it may require elevated privileges to run properly depending on your system configuration.
