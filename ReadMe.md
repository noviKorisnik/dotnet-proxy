# dotnet-proxy
___
The goal of this project is to provide help when client app can't access some public rest web api because of cors restriction. It appears, at least in some cases, that such restriction doesn't apply when running server project, so this proxy has a place to jump in as a middle man.

The plan is to keep this simple as possible, so it will be extended just as some specific requirement may ask.

Initial run provides proxy on get requests for given url, no error handling and no fancy at all.

Here is how the goal has been met.
### create app
```
dotnet new webapi -o DotnetProxy
cd DotnetProxy
dotnet new gitignore
```
### weather forecast?
Initial template gives us Weather forecast something... no need for that now - delete wf class file, rename controller and it's class to **ProxyController**, remove references to wf in controller.
### service
New class **ProxyService**. To get result from external source. One using declaration is needed:
``` c#
using System.Net.Http;
```
... because we'll deal with Http messages, request and response. Here:
``` c#
public string Get(string url)
{
    using (HttpClient client = new HttpClient())
    {
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);

        HttpResponseMessage response = client.SendAsync(request).Result;

        return response.Content.ReadAsStringAsync().Result;
    }
}
```
### register dependency injection
Register service for dependency injection in **_ConfigureServices_** method of **Startup**:
``` c#
services.AddScoped<ProxyService>();
```
### setting up controller
Now service can be used in **ProxyController**, to declare it:
``` c#
private readonly ProxyService _service;
//...
public ProxyController(
    //...
    ProxyService service
    )
{
    //...
    _service = service;
}
```
... and then use:
``` c#
[HttpGet]
public object Get(string url)
{
    return Content(_service.Get(url), "application/json");
}
```
### cors
This project was introduced to help with missing cors response headers, it would be nice that it provides some to be better for that at least.

In **Startup** declare...
``` c#
private readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
```
Again at method **_ConfigureServices_**, add some cors lines:
``` c#
services.AddCors(options =>
{
    options.AddPolicy(
        name: MyAllowSpecificOrigins,
        builder =>
        {
            builder.WithOrigins("http://localhost:3000");
        });
});
```
Yes, that is because I intend to use my client apps from this address, otherwise it can be adapted to different needs.

And, for the end, to add use of cors to **_Configure_** method pipeline:
``` c#
app.UseCors(MyAllowSpecificOrigins);
```
Important notice here that it should be placed after **_UseRouting_** call and before **_UseAuthorization_** - that is pipeline of processing request from one middleware layer to another, some of them are maybe somehow dependent so specific order sequence should be followed (I haven't studied this whole process in detail yet, but I'm wise enough to accept fair recommendation).

And - that is all - ready to use!
___
... if this tends to grow, it will get it's snapshot branches, no worry...
___
