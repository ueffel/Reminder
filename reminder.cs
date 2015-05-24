using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

public class reminder
{
	private static DateTime remindTime = DateTime.Now;
	private static string message = "";
	private static Regex expr = new Regex(@"^(\d+h)?(\d+m)?(\d+s)?$");
	private static Form f;
	private static Label checkLabel;
	private static TextBox timeBox;
	private static TextBox messageBox;

	private static void startGUI()
	{
		if (f != null)
		{
			f.ShowDialog();
			return;
		}

		f = new Form();
		f.Text = "Reminder";
		f.Size = new Size(280, 180);
		f.Closed += new EventHandler(onClose_evt);

		Label timeLabel = new Label();
		timeLabel.Location = new Point(10, 13);
		timeLabel.Size = new Size(50, 20);
		timeLabel.Text = "When:";
		timeLabel.Parent = f;

		timeBox = new TextBox();
		timeBox.Location = new Point(60, 10);
		timeBox.Size = new Size(100, 20);
		timeBox.Text = "1m";
		timeBox.Name = "timeBox";
		timeBox.TextChanged += new EventHandler(checkLabel_Refresh);
		timeBox.Parent = f;

		checkLabel = new Label();
		checkLabel.Location = new Point(180, 12);
		checkLabel.Size = new Size(50, 20);
		checkLabel.Name = "checkLabel";
		checkLabel.Text = DateTime.Now.AddMinutes(1).ToString("HH.mm:ss");
		checkLabel.Parent = f;

		Label messageLabel = new Label();
		messageLabel.Location = new Point(10, 43);
		messageLabel.Size = new Size(50, 20);
		messageLabel.Text = "Message:";
		messageLabel.Parent = f;

		messageBox = new TextBox();
		messageBox.Location = new Point(60, 40);
		messageBox.Size = new Size(180, 50);
		// messageBox.Multiline = true;
		messageBox.Text = "Dinner is ready!";
		messageBox.Name = "messageBox";
		messageBox.Parent = f;

		Button okButton = new Button();
		okButton.Location = new Point(82, 70);
		okButton.Size = new Size(100, 50);
		okButton.Text = "Remind me!";
		okButton.Font = new Font(FontFamily.GenericSansSerif, 10);
		okButton.Click += new EventHandler(okClick_evt);
		okButton.Parent = f;

		System.Windows.Forms.Timer refresh = new System.Windows.Forms.Timer();
		refresh.Tick += new EventHandler(checkLabel_Refresh);
		refresh.Interval = 500;
		refresh.Start();

		f.ShowDialog();
	}

	private static void okClick_evt(object sender, EventArgs e)
	{
		message = messageBox.Text;
		setRemindTime(timeBox.Text);
		f.Hide();
	}

	private static void checkLabel_Refresh(object sender, EventArgs e)
	{
		setRemindTime(timeBox.Text);
		checkLabel.Text = remindTime.ToString("HH:mm:ss");
	}

	private static void onClose_evt(object sender, EventArgs e)
	{
		Environment.Exit(0);
	}

	private static void setRemindTime(string timeText)
	{
		if (expr.IsMatch(timeText))
		{
			int time = 0;
			Regex exp = new Regex(@"(\d+[smhd])");
			foreach(string token in exp.Split(timeText))
				time += getTime(token);
			remindTime = DateTime.Now.AddSeconds(time);
		}
		else if (Regex.IsMatch(timeText, @"^\d{1,2}:\d{1,2}(:\d{1,2})?$"))
		{
			try { remindTime = DateTime.Parse(timeText); }
			catch (Exception) { return; }
			if (remindTime.CompareTo(DateTime.Now) < 0)
				remindTime = remindTime.AddDays(1);
		}
		else
			remindTime = DateTime.Now;
	}

	private static int getTime(string timeString)
	{
		int time = 0;
		if (string.IsNullOrEmpty(timeString))
			return time;
		string timeIntString = timeString.Substring(0, timeString.Length-1);
		int timeInt = 0;
		try { timeInt = Convert.ToInt32(timeIntString); }
		catch (Exception) {}
		switch (timeString[timeString.Length-1])
		{
		case 's':
			time += timeInt;
			break;
		case 'm':
			time += timeInt * 60;
			break;
		case 'h':
			time += timeInt * 3600;
			break;
		}
		return time;
	}

	[DllImport("kernel32.dll", SetLastError=true)]
	private static extern int FreeConsole();

	public static void Main(string[] args)
	{
		FreeConsole();
		if (args.Length == 0)
			startGUI();

		for(int i = 0; i < args.Length; i++)
		{
			if (expr.IsMatch(args[i]))
			{
				int time = 0;
				Regex exp = new Regex(@"(\d+[smhd])");
				foreach(string token in exp.Split(args[i]))
					time += getTime(token);
				remindTime = remindTime.AddSeconds(time);
			}
			else if (args[i].Contains(":"))
			{
				remindTime = DateTime.Parse(args[i]);
				Console.WriteLine("{0}", remindTime);
				if (remindTime.CompareTo(DateTime.Now) < 0)
					remindTime = remindTime.AddDays(1);
			}
			else
				message += args[i];
		}

		if (remindTime.CompareTo(DateTime.Now) <= 0)
		{
			MessageBox.Show("Nope", "Reminder", MessageBoxButtons.OK,
				MessageBoxIcon.Exclamation);
			Main(new string[0]);
		}

		System.Threading.Timer t = new System.Threading.Timer(
			x =>
			{
				MessageBox.Show(remindTime.ToString("HH:mm:ss")
						+ " - " + message, "Reminder", MessageBoxButtons.OK,
					MessageBoxIcon.Information);
				Environment.Exit(0);
			}, null, remindTime.Subtract(DateTime.Now),
			Timeout.InfiniteTimeSpan);

		while (true)
			Thread.Sleep(500);
	}
}
