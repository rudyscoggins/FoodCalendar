using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace FoodCalendar
{
  class Program
  {
    static void Main(string[] args)
    {
      string dataLocation = "../../../../../data/";

      // Read the breakfast foods from file
      string[] breakfastFoods = File.ReadAllLines($"{dataLocation}breakfast.txt");
      if (breakfastFoods.Length == 0)
      {
        Console.WriteLine("No breakfast foods found.");
        return;
      }

      // Read the lunch foods from file
      string[] lunchFoods = File.ReadAllLines($"{dataLocation}lunch.txt");
      if (lunchFoods.Length == 0)
      {
        Console.WriteLine("No lunch foods found.");
        return;
      }

      // Create a calendar image
      int width = 700;
      int height = 1000;
      Bitmap calendar = new Bitmap(width, height);



      using (Graphics graphics = Graphics.FromImage(calendar))
      {

        graphics.Clear(Color.White);

        // Set up the fonts and formatting
        Font dateFont = new Font("Arial", 24);
        Font foodFont = new Font("Arial", 14);
        Font headerFont = new Font("Arial", 14, FontStyle.Bold);
        StringFormat format = new StringFormat();
        format.Alignment = StringAlignment.Center;
        format.LineAlignment = StringAlignment.Center;

        //DateTime today = DateTime.Today.AddMonths(1);
        DateTime today = DateTime.Today;
        graphics.DrawString(today.ToString("MMMM yyyy"), headerFont, Brushes.Black, new Rectangle(0, 0, 200, 100), format);

        // Draw the days of the week
        float cellWidth = (float)width / 7;
        float cellHeight = (float)height / 7;
        string[] daysOfWeek = { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };
        for (int i = 0; i < 7; i++)
        {
          RectangleF rect = new RectangleF(i * cellWidth, 0, cellWidth, cellHeight + 100 );
          graphics.DrawString(daysOfWeek[i], dateFont, Brushes.Black, rect, format);
        }

        // Draw the calendar cells        
        DateTime firstDayOfMonth = new DateTime(today.Year, today.Month, 1);
        int offset = (int)firstDayOfMonth.DayOfWeek;

        for (int i = 1; i <= DateTime.DaysInMonth(today.Year, today.Month); i++)
        {
          int breakfastIndex = i % breakfastFoods.Length;
          int lunchIndex = i % lunchFoods.Length;

          int row = (i + offset - 1) / 7 + 1;
          int col = (i + offset - 1) % 7;

          RectangleF dateRect = new RectangleF(col * cellWidth, row * cellHeight, cellWidth, cellHeight / 2);
          RectangleF breakfastRect = new RectangleF(col * cellWidth, row * cellHeight + cellHeight / 2, cellWidth, cellHeight / 4);
          RectangleF lunchRect = new RectangleF(col * cellWidth, row * cellHeight + cellHeight / 4 * 3, cellWidth, cellHeight / 4);

          // Draw the day cell with a black border
          graphics.DrawRectangle(Pens.Black, col * cellWidth, row * cellHeight, cellWidth, cellHeight);
          graphics.DrawString(i.ToString(), dateFont, Brushes.Black, dateRect, format);
          graphics.DrawString(breakfastFoods[breakfastIndex], foodFont, Brushes.Black, breakfastRect, format);
          graphics.DrawString(lunchFoods[lunchIndex], foodFont, Brushes.Black, lunchRect, format);
        }
      }











      //using (Graphics graphics = Graphics.FromImage(calendar))
      //{
      //  graphics.Clear(Color.White);

      //  graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

      //  // Set up the fonts and formatting
      //  Font dateFont = new Font("Arial", 24);
      //  Font foodFont = new Font("Arial", 14);
      //  StringFormat format = new StringFormat();
      //  format.Alignment = StringAlignment.Center;
      //  format.LineAlignment = StringAlignment.Center;

      //  // Draw the days of the week
      //  float cellWidth = (float)width / 7;
      //  float cellHeight = (float)height / 6;
      //  string[] daysOfWeek = { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };
      //  for (int i = 0; i < 7; i++)
      //  {
      //    RectangleF rect = new RectangleF(i * cellWidth, 0, cellWidth, cellHeight / 2);
      //    graphics.DrawString(daysOfWeek[i], dateFont, Brushes.Black, rect, format);
      //  }

      //  // Get the date of the first day of the month
      //  DateTime today = DateTime.Today;
      //  DateTime firstDayOfMonth = new DateTime(today.Year, today.Month, 1);

      //  // Determine the index of the first day of the month in the calendar grid
      //  int firstDayIndex = ((int)firstDayOfMonth.DayOfWeek + 6) % 7;

      //  // Draw the calendar cells
      //  for (int i = 1; i <= DateTime.DaysInMonth(today.Year, today.Month); i++)
      //  {
      //    int breakfastIndex = (i - 1) % breakfastFoods.Length;
      //    int lunchIndex = (i - 1) % lunchFoods.Length;

      //    int row = (i + firstDayIndex - 1) / 7 + 1;
      //    int col = (i + firstDayIndex - 2) % 7;

      //    RectangleF dateRect = new RectangleF(col * cellWidth, row * cellHeight, cellWidth, cellHeight / 2);
      //    RectangleF breakfastRect = new RectangleF(col * cellWidth, row * cellHeight + cellHeight / 2, cellWidth, cellHeight / 4);
      //    RectangleF lunchRect = new RectangleF(col * cellWidth, row * cellHeight + cellHeight / 4 * 3, cellWidth, cellHeight / 4);

      //    // Draw the day cell with a black border
      //    graphics.DrawRectangle(Pens.Black, col * cellWidth, row * cellHeight, cellWidth, cellHeight);
      //    graphics.DrawString(i.ToString(), dateFont, Brushes.Black, dateRect, format);         

      //    graphics.DrawString(breakfastFoods[breakfastIndex], foodFont, Brushes.Black, breakfastRect, format);
      //    graphics.DrawString(lunchFoods[lunchIndex], foodFont, Brushes.Black, lunchRect, format);
      //  }
      //}

      // Save the calendar image as a JPEG file
      calendar.Save($"{dataLocation}food_calendar.jpg", ImageFormat.Jpeg);

      Console.WriteLine("Calendar saved to food_calendar.jpg.");
    }
  }
}