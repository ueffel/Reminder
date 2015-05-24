# Reminder
Shows a message box with a predefined message at a predefined time to remind you.

![Remind message](https://raw.github.com/ueffel/reminder/master/screenshot2.png)

# Compile
```
$ csc -utf8output -o reminder.cs
```
or if csc not in PATH
```
$ c:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe -utf8output -o reminder.cs
```
or
```
$ c:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe -utf8output -o reminder.cs
```

# Usage

## Commandline
```
$ reminder 2m Message
```
to remind you of "Message" in two minutes. Supports Xh (hours), Xm (minutes), Xs (seconds) or any combination where X is a postive integer.

or
```
$ reminder 13:37 Message
```
to remind you of "Message" at 13:37. (If it is already past today then the next day)

## GUI
Just start reminder.exe

![Set up reminder](https://raw.github.com/ueffel/reminder/master/screenshot1.png)
