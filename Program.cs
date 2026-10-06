var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

string filePath = Path.Combine(AppContext.BaseDirectory, "shared.txt");

// Make sure the file exists
if (!File.Exists(filePath))
{
    File.WriteAllText(filePath, "");
}


// =========================
// WEBSITE
// =========================

app.MapGet("/", () => Results.Content("""
<!DOCTYPE html>
<html>
<head>

    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">

    <title>Shared Text</title>

    <style>

        * {
            box-sizing: border-box;
        }

        body {
            margin: 0;
            min-height: 100vh;

            background: #08080c;
            color: white;

            font-family: Arial, sans-serif;

            display: flex;
            justify-content: center;
            align-items: center;

            padding: 20px;
        }

        .container {
            width: 100%;
            max-width: 800px;

            background: #15151d;

            padding: 30px;

            border-radius: 20px;

            border: 1px solid #292934;
        }

        h1 {
            text-align: center;
            margin-top: 0;
        }

        p {
            text-align: center;
            color: #999;
        }

        textarea {
            width: 100%;
            height: 350px;

            margin-top: 20px;

            padding: 20px;

            background: #0b0b10;
            color: white;

            border: 1px solid #30303c;
            border-radius: 12px;

            font-size: 18px;
            font-family: Arial, sans-serif;

            resize: vertical;

            outline: none;
        }

        textarea:focus {
            border-color: #7067ff;
        }

        button {
            width: 100%;

            margin-top: 15px;

            padding: 16px;

            border: none;
            border-radius: 12px;

            background: #7067ff;
            color: white;

            font-size: 17px;
            font-weight: bold;

            cursor: pointer;
        }

        button:hover {
            background: #827aff;
        }

        #status {
            text-align: center;

            margin-top: 15px;

            color: #7067ff;
        }

    </style>

</head>

<body>

<div class="container">

    <h1>📋 Shared Text</h1>

    <p>
        Write on your phone, then copy it from your PC.
    </p>

    <textarea
        id="textBox"
        placeholder="Write your text here..."
    ></textarea>

    <button onclick="copyText()">
        Copy Text
    </button>

    <div id="status"></div>

</div>


<script>

const textBox = document.getElementById("textBox");
const status = document.getElementById("status");


// Load saved text when the page opens
async function loadText()
{
    const response = await fetch("/text");

    const text = await response.text();

    textBox.value = text;
}


// Save text to the PC
async function saveText()
{
    await fetch("/text",
    {
        method: "POST",

        headers:
        {
            "Content-Type": "text/plain"
        },

        body: textBox.value
    });

    status.textContent = "✓ Saved";
}


// Save whenever you type
textBox.addEventListener("input", saveText);


// Copy text
async function copyText()
{
    try
    {
        await navigator.clipboard.writeText(textBox.value);

        status.textContent = "✓ Copied!";
    }
    catch
    {
        textBox.select();

        document.execCommand("copy");

        status.textContent = "✓ Copied!";
    }
}


// Load saved text
loadText();

</script>

</body>
</html>
""", "text/html"));


// =========================
// GET SAVED TEXT
// =========================

app.MapGet("/text", () =>
{
    string text = File.ReadAllText(filePath);

    return Results.Text(text);
});


// =========================
// SAVE TEXT
// =========================

app.MapPost("/text", async (HttpRequest request) =>
{
    using var reader = new StreamReader(request.Body);

    string text = await reader.ReadToEndAsync();

    File.WriteAllText(filePath, text);

    return Results.Ok();
});


// =========================
// START SERVER
// =========================

app.Run();
