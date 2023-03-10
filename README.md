# Recorder
Record high level profiling data in ASP.NET Core applications at runtime. Profiles can then be visualized using [speedscope](https://github.com/jlfwong/speedscope).

![image](https://user-images.githubusercontent.com/12851828/222244380-f5b1138f-ba71-4c77-831b-59cc556ff539.png)


# Usage

To use the recorder, register the middleware then request a `BlackBox` recorder at runtime to instrument different subsystems. The blackbox will store a configurable number of request profiles (default is 16).

###

Register types

```cs
// use default profile naming (root stack frame will have a name like 'GET relative/url?queryparams')
builder.Services.AddRequestRecording();

// for custom naming, implement INomenclator
builder.Services.AddRequestRecording<MyNomenclator>();
```

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
        using var frame = context.HttpContext.RequestServices.RecordStackFrame("OutputFormatter");
        
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

### Profiling controller

Add a controller to integrate with speedscope easily at runtime by sending HTTP GET to https://localhost/profile. This controller will first redirect the caller to speedscope with a parameterized profile URL. When speedscope requests the profile data, detect it via the origin header and return the profile.

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
    public const string CorsPolicyName = "allowSpeedscope";

    [EnableCors(CorsPolicyName)]
    public async Task<IActionResult> Get()
    {
        if (this.HttpContext.Request.Headers.TryGetValue("Origin", out var origin) && origin[0] == "https://www.speedscope.app")
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

        if (!BlackBox.HasHistory)
        {
            return NotFound("No profiles have been recorded");
        }

        return Redirect($"https://speedscope.app#profileURL=https://{this.HttpContext.Request.Host}/Profile");
    }
}
```
