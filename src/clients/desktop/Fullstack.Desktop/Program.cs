using Photino.NET;

var url = args.Length > 0 ? args[0] : "http://localhost:5000";

var window = new PhotinoWindow()
    .SetTitle("Fullstack Demo - Desktop")
    .SetUseOsDefaultSize(false)
    .SetSize(1280, 720)
    .Center()
    .SetResizable(true)
    .Load(new Uri(url));

window.WaitForClose();
