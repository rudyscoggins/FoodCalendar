﻿using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;

namespace FoodCalendar
{
  class Program
  {


    const string _breakfastGoogleDoc = "15t8XA64LZcfq1lJBoKXk8lDftIdY-igZH4gUMdqISoo";
    const string _lunchGoogleDoc = "1wPRm231CqlDjiAs41eGOizoE-bEVysDZYGN73hsiZ8g";
    private const string _googleDriveClientId = "56482650599-6qtndved984b7u5lp2862ir3mc73n20l.apps.googleusercontent.com";
    private const string _googleDriveSecret = "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";

    static void Main(string[] args)
    {
      var breakfastFoods = GetItemsFromGoogleDoc(_breakfastGoogleDoc);

      var lunchFoods = GetItemsFromGoogleDoc(_lunchGoogleDoc);

      string outputLocation = "../../../../../data/";

      // Read the breakfast foods from file      
      if (breakfastFoods.Count == 0)
      {
        Console.WriteLine("No breakfast foods found.");
        return;
      }

      // Read the lunch foods from file      
      if (lunchFoods.Count == 0)
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
        Font foodFont = new Font("Arial", 10);
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
          int breakfastIndex = i % breakfastFoods.Count;
          int lunchIndex = i % lunchFoods.Count;

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


      // Save the calendar image as a JPEG file
      calendar.Save($"{outputLocation}food_calendar.jpg", ImageFormat.Jpeg);

      Console.WriteLine($"Calendar saved to {System.IO.Path.GetFullPath(outputLocation)}food_calendar.jpg.");
    }

    private static List<string> GetItemsFromGoogleDoc(string fileId)
    {
      // Set the scope to read-only access to Google Drive
      string[] scopes = { DriveService.Scope.DriveReadonly };

      // Set the credentials for the Google Drive API
      UserCredential credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
          new ClientSecrets
          {
            ClientId = _googleDriveClientId,
            ClientSecret = _googleDriveSecret
          },
          scopes,
          "user",
          CancellationToken.None).Result;

      // Create the Drive API client
      DriveService service = new DriveService(new BaseClientService.Initializer()
      {
        HttpClientInitializer = credential,
        ApplicationName = "Read Google Doc"
      });
            
      // Download the file content as a stream
      var request = service.Files.Export(fileId, "text/plain");
      var stream = new MemoryStream();
      request.Download(stream);

      // Convert the stream content to a list of items
      var reader = new StreamReader(stream);
      stream.Position = 0;
      var items = new List<string>();


      while (!reader.EndOfStream)
      {
        var line = reader.ReadLine();
        items.Add(line);
      }

      return items;
    }
  }
}