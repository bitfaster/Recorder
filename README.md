# Recorder
Record high level profiling data in ASP.NET Core applications at runtime. Profiles can then be visualized using speedscope.

# Usage

To use the recorder, register the middleware then request a `BlackBox` recorder at runtime to instrument different subsystems. The blackbox will store a configurable number of request profiles (default is 16).

### Register middleware

At startup, register the middleware:

```cs
// always run 
app.UseRequestRecorder();

// profile specific URL
app.UseWhen(context => context.Request.Path.StartsWithSegments("Foo") , appBuilder =>
{
    appBuilder.UseRequestRecorder();
});
```

### Instrument code

For example, to instrument output create an instrumented formatter and decorate the existing formatter classes.

```cs
public class InstrumentedOutputFormatter : IOutputFormatter
{
    private readonly IOutputFormatter inner;

    public InstrumentedOutputFormatter(IOutputFormatter inner) { this.inner = inner; }

    public bool CanWriteResult(OutputFormatterCanWriteContext context)
    {
        return inner.CanWriteResult(context);
    }

    public async Task WriteAsync(OutputFormatterWriteContext context)
    {
        // record time spent in this method via Capture
        var blackBox  = context.HttpContext.RequestServices.GetService<BlackBox>();
        using var c = blackBox.Capture("OutputFormatter");
        
        await inner.WriteAsync(context);
    }
}
```

```cs
builder.Services
    .AddMvcOptions(options => 
    {
        // wrap all the formatters with instrumentation
        List<IOutputFormatter> newFormatters = new List<IOutputFormatter>(options.OutputFormatters.Count);

        foreach (var f in options.OutputFormatters)
        {
            newFormatters.Add(new InstrumentedOutputFormatter(f));
        }

        options.OutputFormatters.Clear();

        foreach (var nf in newFormatters)
        {
            options.OutputFormatters.Add(nf);
        }
    });
```

### Profiling controllers

Add controllers to integrate into speedscope. The first controller is a simple redirector to make it easier to remember the URI. The second one serves up the .json file containing the profile data.

```cs
builder.Services.AddCors(options =>
{
    options.AddPolicy(ProfileDataController.CorsPolicyName,
    builder =>
    {
        builder
            .WithOrigins(@"https://www.speedscope.app")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

// ...

app.UseCors(ProfileDataController.CorsPolicyName);
```

```cs
[ApiController]
[Route("[controller]")]
public class ProfileController : ControllerBase
{
    public async Task<IActionResult> Get()
    {
        if (!BlackBox.HasHistory)
        { 
            return NotFound("No profiles have been recorded");
        }

        return Redirect($"https://speedscope.app#profileURL=https://{this.HttpContext.Request.Host}/ProfileData");
    }
}

[ApiController]
[Route("[controller]")]
public class ProfileDataController : ControllerBase
{
    public const string CorsPolicyName = "allowSpeedscope";

    [EnableCors(CorsPolicyName)]
    public async Task<IActionResult> Get()
    {
        var memoryStream = new MemoryStream();

        using (var speedscopeWriter = new SpeedscopeWriter(memoryStream))
        {
            speedscopeWriter.WritePreAmble();

            foreach (var request in BlackBox.History)
            {
                speedscopeWriter.WriteEvent(request);
            }

            speedscopeWriter.Flush();
        }
        memoryStream.Position = 0;

        var fileResult = File(memoryStream, "application/json", "profile.json");
        return fileResult;
    }
}
```
