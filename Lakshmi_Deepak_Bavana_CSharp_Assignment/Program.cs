using System.Text.Json;
using System.Drawing;
using System.Drawing.Imaging;

HttpClient client = new HttpClient();

string url = "https://rc-vault-fap-live-1.azurewebsites.net/api/gettimeentries?code=vO17RnE8vuzXzPJo5eaLLjXjmRW07law99QTD90zat9FfOQJKKUcgQ==";

string json = await client.GetStringAsync(url);

List<TimeEntry> entries = JsonSerializer.Deserialize<List<TimeEntry>>(json);

Dictionary<string, double> totals = new Dictionary<string, double>();

foreach (TimeEntry entry in entries)
{
    if (entry.EmployeeName == null) continue;
    if (entry.DeletedOn != null) continue;

    TimeSpan duration = entry.EndTimeUtc - entry.StarTimeUtc;
    double hours = duration.TotalHours;

    if (hours < 0) continue;

    if (totals.ContainsKey(entry.EmployeeName))
        totals[entry.EmployeeName] += hours;
    else
        totals[entry.EmployeeName] = hours;
}

var sorted = totals.OrderByDescending(x => x.Value).ToList();

string html = "<html><body>";
html += "<h1>Employees by total time worked</h1>";
html += "<table border='1' cellpadding='8'>";
html += "<tr><th>Name</th><th>Total time worked (hours)</th></tr>";

foreach (var item in sorted)
{
    string rowColor = "";
    if (item.Value < 100)
        rowColor = " style='background-color:red'";

    html += "<tr" + rowColor + "><td>" + item.Key + "</td><td>" + item.Value.ToString("F2") + "</td></tr>";
}

html += "</table></body></html>";

File.WriteAllText("employees.html", html);
Console.WriteLine("Created: " + Path.GetFullPath("employees.html"));

double grandTotal = 0;
foreach (var item in sorted)
    grandTotal += item.Value;

Color[] colors = { Color.Blue, Color.Orange, Color.Red, Color.Cyan, Color.Green,
                   Color.Gold, Color.Purple, Color.HotPink, Color.Brown, Color.Gray };

Bitmap bmp = new Bitmap(1000, 600);
Graphics g = Graphics.FromImage(bmp);
g.Clear(Color.White);

Rectangle pieArea = new Rectangle(30, 50, 500, 500);
Font font = new Font("Arial", 12);

float startAngle = 0;
int legendY = 80;
int i = 0;

foreach (var item in sorted)
{
    float sweep = (float)(item.Value / grandTotal * 360);
    Brush brush = new SolidBrush(colors[i]);

    g.FillPie(brush, pieArea, startAngle, sweep);
    startAngle += sweep;

    double percent = item.Value / grandTotal * 100;
    g.FillRectangle(brush, 580, legendY, 20, 20);
    g.DrawString(item.Key + " - " + percent.ToString("F1") + "%", font, Brushes.Black, 610, legendY);

    legendY += 35;
    i++;
}

bmp.Save("piechart.png", ImageFormat.Png);
Console.WriteLine("Created: " + Path.GetFullPath("piechart.png"));
class TimeEntry
{
    public string Id {  get; set; }
    public string EmployeeName { get; set; }
    public DateTime StarTimeUtc { get; set; }
    public DateTime EndTimeUtc { get; set; }
    public string EntryNotes { get; set; }
    public DateTime? DeletedOn { get; set; }
}

